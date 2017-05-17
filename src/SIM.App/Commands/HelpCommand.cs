namespace SIM.Commands
{
  using System;
  using System.Linq;
  using System.Reflection;
  using JetBrains.Annotations;
  using SIM.Base;

  public class HelpCommand : ICommand
  {
    [CanBeNull]
    [CommandArgument("command", "Name of command to get detailed info about")]
    public string CommandName { get; set; }

    public CommandResult Execute()
    {
      //var verb = App.Verbs.FirstOrDefault(x => string.Equals(x.Value[0], CommandName, StringComparison.OrdinalIgnoreCase));
      foreach (var verb in App.Verbs)
      {
        if (!string.Equals(verb.Value[0], CommandName, StringComparison.OrdinalIgnoreCase))
        {
          continue;
        }

        return new Success
        {
          Message = $"Command '{verb.Value[0]}'. {verb.Value[1]}. Arguments:",
          Data = verb.Key
            .GetProperties()
            .Select(x => x.GetCustomAttribute<CommandArgumentAttribute>())
            .Where(x => x != null)
            .Select(x => $"{x.Name} - {x.Description}")
            .ToArray<object>()
        };
      }

      if (!string.IsNullOrWhiteSpace(CommandName))
      {
        throw new ArgumentException($"Cannot find command '{CommandName}'. Run 'sim help' to get list of all supported commands.");
      }

      return new Success
      {
        Message = "SIM.exe is a command-line version of Sitecore Instance Manager 2.0 (SIM2), read more on https://github.com/sitecore/sitecore-instance-manager. The list of commands see in 'Data' array, the arguments are passed in JSON5 format. For example: C:\\> sim help {'command': \"help\"}",
        Data = App.Verbs
          .Select(x => $"{x.Value[0]} - {x.Value[1]}" as object)
          .ToArray()
      };
    }
  }
}
