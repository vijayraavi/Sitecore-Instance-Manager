namespace SIM.UnitTests
{
  using Xunit;

  public class Program_Tests
  {              
    [Fact]
    public void ParseCommandLineArgs()
    {
      var line = "\"C:\\tmp\\sim.exe\" some !\" different \"\"?? params [] {} Eee!";
      var path = "C:\\tmp\\sim.exe";
      var args = AppRunner.ParseCommandLineArgs(line, path);

      Assert.Equal("some !\" different \"\"?? params [] {} Eee!", args);
    }

    [Fact]
    public void FromCommandLine()
    {
      var args = "some !\" different \"\"?? params [] {} Eee!";
      var name = AppRunner.GetCommandName(args);
      var data = AppRunner.GetCommandData(args, name);
      Assert.Equal("some", name);
      Assert.Equal("!\" different \"\"?? params [] {} Eee!", data);
    }
  } 
}
