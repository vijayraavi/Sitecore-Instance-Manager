namespace SIM.UnitTests.Commands
{
  using System;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using SIM.Commands;

  [TestClass]
  public class HelpCommand_Tests
  {
    [TestMethod]
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
        Assert.AreEqual("Cannot find command \'test123\'. Run \'sim help\' to get list of all supported commands.", ex.Message);

        return;
      }

      Assert.Fail();
    }

    [TestMethod]
    public void Process_CommandName_Empty()
    {
      var sut = new HelpCommand
      {
        CommandName = ""
      };

      var result = sut.Execute();
      Assert.IsNotNull(result);
      
      Assert.AreEqual(
        "SIM.exe is a command-line version of Sitecore Instance Manager 2.0 (SIM2), read more on https://github.com/sitecore/sitecore-instance-manager. The list of commands see in 'Data' array, the arguments are passed in JSON5 format. For example: C:\\> sim help {'command': \"help\"}",
        result.Message);

      var data = result.Data;
      Assert.IsNotNull(data);

      Assert.AreEqual(
        "help - Provides information about app or particular command", 
        data[0]);

      Assert.AreEqual(1, data.Length);
    }

    [TestMethod]
    public void Process_CommandName_Help()
    {
      var sut = new HelpCommand
      {
        CommandName = "help"
      };

      var result = sut.Execute();
      Assert.IsNotNull(result);
      
      Assert.AreEqual(
        "Command 'help'. Provides information about app or particular command. Arguments:",
        result.Message);

      var data = result.Data;

      Assert.IsNotNull(data);
      Assert.AreEqual("command - Name of command to get detailed info about", data[0]);
      Assert.AreEqual(1, data.Length);
    }
  }
}
