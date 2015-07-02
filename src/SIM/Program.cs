namespace SIM
{
  using System;
  using System.Linq;
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Common;
  using SIM.Verbs;

  public static class Program
  {
    public static void Main([NotNull] string[] args)
    {
      CoreApp.InitializeLogging();

      CoreApp.LogMainInfo();

      Analytics.Start();                  

      var parser = new Parser(options =>
      {
        options.MutuallyExclusive = true;
        options.HelpWriter = Console.Error;
      });

      var verbs = new VerbsPalette();
      parser.ParseArguments(args, verbs, (str, obj) =>
      {
        var verb = obj as ICommand;
        if (verb == null)
        {
          Console.WriteLine("Note, commands provide output when work is done i.e. WITHOUT any progress indication.");

          Environment.Exit(Parser.DefaultExitCodeFail);
          return;
        }

        var commandResult = verb.Execute();
        Assert.IsNotNull(commandResult, nameof(ICommandResult));

        Default.Serializer.Serialize(Console.Out, commandResult);
      });
    }
  }
}