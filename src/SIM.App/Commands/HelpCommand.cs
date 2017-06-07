namespace SIM.Commands
{
  using System;
  using System.Linq;
  using System.Reflection;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using SIM.Base;

  public class HelpCommand : ICommand
  {
    [UsedImplicitly]
    public HelpCommand()
    {
    }

    [CanBeNull]
    internal App App { get; set; }

    [CanBeNull]
    [CommandArgument("command", "Name of command to get detailed info about")]
    public string CommandName { get; set; }

    public CommandResult Execute()
    {
      Assert.ArgumentNotNull(App);

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
            .Select(x => 
              new
              {
                Name = x.Name,
                Command = x.GetCustomAttribute<CommandArgumentAttribute>()
              })
            .Where(x => x.Command != null)
            .Select(x => $"{x.Command.Name ?? x.Name} - {x.Command.Description}")
            .ToArray<object>()
        };
      }

      if (!string.IsNullOrWhiteSpace(CommandName))
      {
        throw new ArgumentException($"Cannot find command '{CommandName}'. Run '{App.ExecutableName} help' to get list of all supported commands.");
      }

      return new Success
      {
        Message = $"{App.Information.TrimEnd(". ".ToCharArray())}. The list of commands see in 'Data' array, the arguments are passed in JSON5 format. For example: C:\\> {App.ExecutableName} help {{'command': \"help\"}}",
        Data = App.Verbs
          .Select(x => $"{x.Value[0]} - {x.Value[1]}" as object)
          .ToArray()
      };
    }
  }
}
