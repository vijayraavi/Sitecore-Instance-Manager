namespace SIM
{
  using System;
  using JetBrains.Annotations;

  public abstract class ConsoleApp : App
  {
    protected ConsoleApp([NotNull] string commandName, [NotNull] string commandData)
      : base(commandName, commandData)
    {
    }

    protected override void WriteOutput(string json)
    {
      Console.WriteLine(json);
    }
  }
}