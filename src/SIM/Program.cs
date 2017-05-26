namespace SIM
{
  using JetBrains.Annotations;
  using System;
  using System.Diagnostics;

  internal static class Program
  {
    internal static int Main()
    {
      var commandLine = Environment.CommandLine;
      var fileName = Process.GetCurrentProcess().MainModule.FileName;

      var app = CreateApp(commandLine, fileName);

      return app.Start();
    }

    [NotNull]
    internal static App CreateApp([NotNull] string commandLine, [NotNull] string executableFilePath)
    {
      var args = ParseCommandLineArgs(commandLine, executableFilePath);

      var commandName = args.Substring(0, Math.Max(0, args.IndexOf(' ')));
      var commandData = args.Substring(Math.Min(args.Length, commandName.Length + 1));

      return new ConsoleApp(commandName, commandData);
    }

    [NotNull]
    internal static string ParseCommandLineArgs([NotNull] string commandLine, [NotNull] string executableFilePath)
    {
      return commandLine.Substring(Math.Min(commandLine.Length, $"\"{executableFilePath}\" ".Length));
    }

    private class ConsoleApp : App
    {                               
      internal ConsoleApp(string commandName, string commandData)
        : base(commandName, commandData)
      {                                 
      }

      protected override void WriteOutput([NotNull] string json)
      {
        Console.WriteLine(json);
      }
    }
  }
}
