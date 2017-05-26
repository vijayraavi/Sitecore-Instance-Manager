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
      var args = "some !\" different \"\"?? params [] {} Eee!";

      Assert.Equal(args, Program.ParseCommandLineArgs(line, path));
    }

    [Fact]
    public void FromCommandLine()
    {
      var line = "\"C:\\tmp\\sim.exe\" some !\" different \"\"?? params [] {} Eee!";
      var path = "C:\\tmp\\sim.exe";

      var app = Program.CreateApp(line, path);
      Assert.Equal("some", app.CommandName);
      Assert.Equal("!\" different \"\"?? params [] {} Eee!", app.CommandData);
    }
  }
}
