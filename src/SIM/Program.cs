namespace SIM
{
  using System;
  using System.Diagnostics;

  public static class Program
  {
    public static int Main()
    {
      var commandLine = Environment.CommandLine;
      var fileName = Process.GetCurrentProcess().MainModule.FileName;

      var app = AppBuilder.FromCommandLine(commandLine, fileName);

      return app.Start();
    }
  }
}
