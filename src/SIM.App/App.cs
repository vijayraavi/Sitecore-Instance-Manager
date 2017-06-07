namespace SIM
{
  using System;
  using System.Collections.Generic;
  using JetBrains.Annotations;
  using Newtonsoft.Json;
  using Sitecore.Diagnostics.Base;
  using SIM.Base;
  using SIM.Commands;
  using SIM.Serialization;
  using System.Diagnostics;
  using System.Linq;

  /// <summary>
  ///   The instance of App class repre
  /// </summary>
  public abstract class App
  {
    [NotNull]
    protected internal abstract IReadOnlyDictionary<Type, string[]> Verbs { get; }

    [NotNull]
    private static JsonSerializerSettings SerializerSettings { get; } =
      new JsonSerializerSettings
      {
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.Indented,
      };

    [NotNull]
    private static JsonSerializerSettings DeserializerSettings { get; } =
      new JsonSerializerSettings
      {
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.Indented,
        ContractResolver = new CommandArgumentResolver(),
      };

    [NotNull]
    public abstract string Information { get; }

    [NotNull]
    public abstract string ExecutableName { get; }

    protected internal App([NotNull] string commandName, [NotNull] string commandData)
    {
      CommandName = commandName;
      CommandData = commandData;
    }

    [NotNull]
    internal string CommandName { get; }

    [NotNull]
    internal string CommandData { get; }

    /// <summary>
    ///   Start work of application with given parameters.
    /// </summary>
    /// <returns>The return code.</returns>
    public int Start()
    {
      if (!Verbs.ContainsKey(typeof(HelpCommand)) && !Verbs.Keys.Any(x => x.GetType().IsAssignableFrom(typeof(HelpCommand))))
      {
        throw new InvalidOperationException($"The {typeof(HelpCommand).Name} command is missing in {nameof(Verbs)}");
      }

      return ProcessOutput(Process());
    }

    [NotNull]
    internal AppOutput Process()
    {
      var firstWord = CommandName;
      if (firstWord == "" || firstWord == "-?" || firstWord == "/?" || firstWord == "info" || firstWord == "-info" || firstWord == "--info" || firstWord == "--help" || firstWord == "--info")
      {
        firstWord = "help";
      }

      foreach (var verb in Verbs)
      {
        if (!string.Equals(verb.Value[0], firstWord, StringComparison.OrdinalIgnoreCase))
        {
          continue;
        }

        var type = verb.Key;
        Assert.IsNotNull(type);

        return ProcessVerb(type, CommandData);
      }

      var error = new AppOutput
      {
        Error = $"Cannot find command '{firstWord}'. Run '{ExecutableName} info' to get list of all supported commands.",
        Success = false,
        ReturnCode = -2
      };

      return error;
    }

    internal int ProcessOutput([NotNull] AppOutput output)
    {
      var json = Serialize(output);

      WriteOutput(json);

      return output.ReturnCode;
    }

    protected virtual void WriteOutput([NotNull] string json)
    {
      Debug.WriteLine(json);
    }

    [NotNull]
    private AppOutput ProcessVerb([NotNull] Type type, [NotNull] string commandData)
    {
      if (commandData == "")
      {
        commandData = "{}";
      }

      var command = (ICommand)Deserialize(type, commandData);
      Assert.IsNotNull(command);

      if (command is HelpCommand help)
      {
        help.App = this;
      }

      return ProcessCommand(command);
    }

    [NotNull]
    private static AppOutput ProcessCommand([NotNull] ICommand command)
    {
      try
      {
        var result = command.Execute();
        Assert.IsNotNull(result);

        return new AppOutput
        {
          Success = true,
          Result = result,
          ReturnCode = 0
        };
      }
      catch (Exception ex)
      {
        return new AppOutput
        {
          Error = $"Command failed with unhandled exception. Message: {ex.Message}"
        };
      }
    }

    [NotNull]
    internal static string Serialize([NotNull] object output)
    {
      return JsonConvert.SerializeObject(output, SerializerSettings) ?? "";
    }

    internal static object Deserialize([NotNull] Type type, [NotNull] string commandData)
    {
      commandData = commandData.Trim(" \r\n".ToCharArray());
      if (commandData.StartsWith("{") && commandData.EndsWith("}"))
      {
        commandData = commandData.Substring(1, commandData.Length - 2);
      }

      return JsonConvert.DeserializeObject($"{{{commandData}\r\n}}", type, DeserializerSettings);
    }
  }
}
