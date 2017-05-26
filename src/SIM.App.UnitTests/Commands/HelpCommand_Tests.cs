namespace SIM.UnitTests.Commands
{
  using System;                                           
  using SIM.Commands;
  using Xunit;

  public class HelpCommand_Tests
  {
    [Fact]
    public void Process_CommandName_Missing()
    {
      var sut = new HelpCommand
      {
        CommandName = "test123"
      };

      try
      {
        sut.Execute();
      }
      catch (ArgumentException ex)
      {
        Assert.Equal("Cannot find command \'test123\'. Run \'sim help\' to get list of all supported commands.", ex.Message);

        return;
      }

      throw new NotSupportedException();
    }

    [Fact]
    public void Process_CommandName_Empty()
    {
      var sut = new HelpCommand
      {
        CommandName = ""
      };

      var result = sut.Execute();
      Assert.NotNull(result);
      
      Assert.Equal(
        "SIM.exe is a command-line version of Sitecore Instance Manager 2.0 (SIM2), read more on https://github.com/sitecore/sitecore-instance-manager. The list of commands see in 'Data' array, the arguments are passed in JSON5 format. For example: C:\\> sim help {'command': \"help\"}",
        result.Message);

      var data = result.Data;
      Assert.NotNull(data);

      Assert.Equal(
        "help - Provides information about app or particular command", 
        data[0]);

      Assert.Equal(1, data.Length);
    }

    [Fact]
    public void Process_CommandName_Help()
    {
      var sut = new HelpCommand
      {
        CommandName = "help"
      };

      var result = sut.Execute();
      Assert.NotNull(result);
      
      Assert.Equal(
        "Command 'help'. Provides information about app or particular command. Arguments:",
        result.Message);

      var data = result.Data;

      Assert.NotNull(data);
      Assert.Equal("command - Name of command to get detailed info about", data[0]);
      Assert.Equal(1, data.Length);
    }
  }
}
