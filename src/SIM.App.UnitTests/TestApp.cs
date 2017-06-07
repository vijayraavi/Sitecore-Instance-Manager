namespace SIM.UnitTests
{
  using System;
  using System.Collections.Generic;
  using JetBrains.Annotations;
  using SIM.Commands;

  public class TestApp : App
  {
    public TestApp([NotNull] string commandName, [NotNull] string commandData = "") 
      : base(commandName, commandData)
    {
    }

    protected internal override IReadOnlyDictionary<Type, string[]> Verbs { get; } =
      new Dictionary<Type, string[]>
      {
        { typeof(HelpCommand), new[] { "help", "help_descr" } }
      };

    public override string Information { get; } = "App info";

    public override string ExecutableName { get; } = "app_exe";
  }
}