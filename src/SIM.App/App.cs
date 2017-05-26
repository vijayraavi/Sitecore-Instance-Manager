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

  public sealed class App
  {
    [NotNull]
    internal static IReadOnlyDictionary<Type, string[]> Verbs { get; } =
      new Dictionary<Type, string[]>
      {
        { typeof(HelpCommand), new[] { "help", "Provides information about app or particular command" } }
      };

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
    internal string CommandName { get; }

    [NotNull]
    internal string CommandData { get; }

    public App([NotNull] string commandName, [NotNull] string commandData)
    {
      CommandName = commandName;
      CommandData = commandData;
    }

    public int Start()
    {
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

        return ProcessVerb(type);
      }

      var error = new AppOutput
      {
        Error = $"Cannot find command '{firstWord}'. Run 'sim info' to get list of all supported commands.",
        Success = false,
        ReturnCode = -2
      };

      return error;
    }

    internal static int ProcessOutput([NotNull] AppOutput output)
    {
      Console.WriteLine(Serialize(output));

      return output.ReturnCode;
    }

    [NotNull]
    private AppOutput ProcessVerb([NotNull] Type type)
    {
      var commandData = CommandData;
      if (commandData == "")
      {
        commandData = "{}";
      }

      var command = (ICommand)Deserialize(type, commandData);
      Assert.IsNotNull(command);

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
