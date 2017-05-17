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
    private static JsonSerializerSettings JsonSettings { get; } =
      new JsonSerializerSettings
      {
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.Indented,
        ContractResolver = new CommandArgumentResolver()
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

        return ProcessVerb(verb.Key);
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
    private AppOutput ProcessVerb(Type type)
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
    private static AppOutput ProcessCommand(ICommand command)
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
    internal static string Serialize(object output)
    {
      return JsonConvert.SerializeObject(output, JsonSettings) ?? "";
    }

    internal static object Deserialize(Type type, string commandData)
    {
      return JsonConvert.DeserializeObject(commandData, type, JsonSettings);
    }
  }
}
