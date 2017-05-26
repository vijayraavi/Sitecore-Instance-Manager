namespace SIM.UnitTests
{
  using JetBrains.Annotations;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using SIM.Base;

  [TestClass]
  public class App_Tests
  {
    [TestMethod]
    public void ParseCommandLineArgs()
    {
      var line = "\"C:\\tmp\\sim.exe\" some !\" different \"\"?? params [] {} Eee!";
      var path = "C:\\tmp\\sim.exe";
      var args = "some !\" different \"\"?? params [] {} Eee!";
      
      Assert.AreEqual(args, AppBuilder.ParseCommandLineArgs(line, path));
    }

    [TestMethod]
    public void FromCommandLine()
    {
      var line = "\"C:\\tmp\\sim.exe\" some !\" different \"\"?? params [] {} Eee!";
      var path = "C:\\tmp\\sim.exe";

      var app = AppBuilder.FromCommandLine(line, path);
      Assert.AreEqual("some", app.CommandName);
      Assert.AreEqual("!\" different \"\"?? params [] {} Eee!", app.CommandData);
    }

    [TestMethod]
    public void Process_MissingCommand()
    {
      var sut = new App("test123", "");

      var output = sut.Process();
      Assert.IsNotNull(output);
      Assert.AreEqual(false, output.Success);
      Assert.AreEqual(-2, output.ReturnCode);
      Assert.AreEqual("Cannot find command 'test123'. Run 'sim info' to get list of all supported commands.", output.Error);
    }

    [TestMethod]
    public void Process_HelpCommand()
    {
      var sut = new App("help", "");

      var output = sut.Process();
      Assert.IsNotNull(output);
      Assert.AreEqual(true, output.Success);
      Assert.AreEqual(0, output.ReturnCode);
      Assert.AreEqual(null, output.Error);

      var result = output.Result;
      Assert.IsNotNull(result);
      Assert.AreEqual("SIM.exe is a command-line version of Sitecore Instance Manager 2.0 (SIM2), read more on https://github.com/sitecore/sitecore-instance-manager. The list of commands see in \'Data\' array, the arguments are passed in JSON5 format. For example: C:\\> sim help {\'command\': \"help\"}", result.Message);

      var data = result.Data;
      Assert.IsNotNull(data);
      Assert.AreEqual("help - Provides information about app or particular command", data[0]);
      Assert.AreEqual(1, data.Length);
    }

    [TestMethod]
    public void Deserialize_Json()
    {
      var command = (DeserializeClass)App.Deserialize(typeof(DeserializeClass), "{'test123': 'myprop123'}");
      Assert.IsNotNull(command);
      Assert.AreEqual("myprop123", command.MyProp);
    }

    [TestMethod]
    public void Serialize()
    {
      var obj = new
      {
        Test123 = "myprop123",
        Something = null as object,
        Inner = new
        {
          Name = "name123"
        }
      };

      var json = App.Serialize(obj);
      Assert.IsNotNull(json);
      Assert.AreEqual(@"{
  'Test123': 'myprop123',
  'Inner': {
    'Name': 'name123'
  }
}".Replace('\'', '"'), json);
    }

    public class DeserializeClass 
    {
      [CommandArgument("test123", "description123")]
      public string MyProp { get; [UsedImplicitly] set; }
    }
  }
}
