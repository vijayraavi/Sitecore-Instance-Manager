namespace SIM
{
  using System;
  using JetBrains.Annotations;

  public static class AppBuilder
  {
    [NotNull]
    public static App FromCommandLine([NotNull] string commandLine, [NotNull] string executableFilePath)
    {
      var args = ParseCommandLineArgs(commandLine, executableFilePath);

      var commandName = args.Substring(0, Math.Max((int)0, (int)args.IndexOf(' ')));
      var commandData = args.Substring(Math.Min((int)args.Length, commandName.Length + 1));

      return new App(commandName, commandData);
    }

    internal static string ParseCommandLineArgs([NotNull] string commandLine, [NotNull] string executableFilePath)
    {
      return commandLine.Substring(Math.Min(commandLine.Length, $"\"{executableFilePath}\" ".Length));
    }
  }
}