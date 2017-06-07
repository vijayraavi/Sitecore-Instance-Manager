namespace SIM.UnitTests.Commands
{
  using System;                                           
  using SIM.Commands;
  using Xunit;

  public class HelpCommand_Tests
  {
    [Fact]
    public void Process_CommandName_Empty()
    {
      var commandName = "";
      var sut = new HelpCommand
      {
        CommandName = commandName,
        App = new TestApp(commandName)
      };

      var result = sut.Execute();
      Assert.NotNull(result);
      
      Assert.Equal(
        "App info. The list of commands see in 'Data' array, the arguments are passed in JSON5 format. For example: C:\\> app_exe help {'command': \"help\"}",
        result.Message);

      var data = result.Data;
      Assert.NotNull(data);

      Assert.Equal(
        "help - help_descr", 
        data[0]);

      Assert.Equal(1, data.Length);
    }

    [Fact]
    public void Process_CommandName_Help()
    {
      var commandName = "help";
      var sut = new HelpCommand()
      {
        CommandName = commandName,
        App = new TestApp(commandName)
      };

      var result = sut.Execute();
      Assert.NotNull(result);
      
      Assert.Equal(
        "Command 'help'. help_descr. Arguments:",
        result.Message);

      var data = result.Data;

      Assert.NotNull(data);
      Assert.Equal("command - Name of command to get detailed info about", data[0]);
      Assert.Equal(1, data.Length);
    }
  }
}
