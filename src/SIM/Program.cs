namespace SIM
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using JetBrains.Annotations;
  using SIM.Commands;

  internal static class Program
  {
    internal static int Main()
    {
      var commandLine = Environment.CommandLine;
      var fileName = Process.GetCurrentProcess().MainModule.FileName;
      var runner = new SimRunner();
      
      return runner.Start(commandLine, fileName);
    }
    
    internal class SimRunner : AppRunner
    {
      protected override App CreateApp(string commandName, string commandData) 
        => new SimApp(commandName, commandData);

      internal class SimApp : ConsoleApp
      {
        public SimApp([NotNull] string commandName, [NotNull] string commandData)
          : base(commandName, commandData)
        {
        }

        protected override IReadOnlyDictionary<Type, string[]> Verbs { get; } =
          new Dictionary<Type, string[]>
          {
            { typeof(HelpCommand), new[] { "help", "Provides information about app or particular command" } }
          };

        public override string Information { get; } 
          = "SIM.exe is a command-line version of Sitecore Instance Manager 2.0 (SIM2), " +
          "read more on https://github.com/sitecore/sitecore-instance-manager.";

        public override string ExecutableName { get; } 
          = "sim";
      }
    }
  }
}
