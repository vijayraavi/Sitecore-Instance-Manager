namespace SIM.UnitTests
{
  using JetBrains.Annotations;
  using SIM.Base;
  using Xunit;

  public class App_Tests
  {                  
    [Fact]
    public void Process_MissingCommand()
    {
      var sut = new App("test123", "");

      var output = sut.Process();
      Assert.NotNull(output);
      Assert.Equal(false, output.Success);
      Assert.Equal(-2, output.ReturnCode);
      Assert.Equal("Cannot find command 'test123'. Run 'sim info' to get list of all supported commands.", output.Error);
    }

    [Fact]
    public void Process_HelpCommand()
    {
      var sut = new App("help", "");

      var output = sut.Process();
      Assert.NotNull(output);
      Assert.Equal(true, output.Success);
      Assert.Equal(0, output.ReturnCode);
      Assert.Equal(null, output.Error);

      var result = output.Result;
      Assert.NotNull(result);
      Assert.Equal("SIM.exe is a command-line version of Sitecore Instance Manager 2.0 (SIM2), read more on https://github.com/sitecore/sitecore-instance-manager. The list of commands see in \'Data\' array, the arguments are passed in JSON5 format. For example: C:\\> sim help {\'command\': \"help\"}", result.Message);

      var data = result.Data;
      Assert.NotNull(data);
      Assert.Equal("help - Provides information about app or particular command", data[0]);
      Assert.Equal(1, data.Length);
    }

    [Fact]
    public void Deserialize_Json()
    {
      var command = (DeserializeClass)App.Deserialize(typeof(DeserializeClass), "{\"test123\": \"myprop123\", \"ignore\": \"123\", \"inner\": { \"name\": \"name123\"}}");
      Assert.NotNull(command);
      Assert.Equal("myprop123", command.MyProp);
      Assert.Equal(null, command.Ignore);
      Assert.Equal("name123", command.Inner?.Name);
    }

    [Fact]
    public void Deserialize_Json5_Reduced()
    {
      var command = (DeserializeClass)App.Deserialize(typeof(DeserializeClass), "test123: 'myprop123', /*no ignore*/ inner: { name: 123, } //");
      Assert.NotNull(command);
      Assert.Equal("myprop123", command.MyProp);
      Assert.Equal(null, command.Ignore);
      Assert.Equal("123", command.Inner?.Name);
    }

    [Fact]
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
      Assert.NotNull(json);
      Assert.Equal("{\r\n  'Test123': 'myprop123',\r\n  'Inner': {\r\n    'Name': 'name123'\r\n  }\r\n}".Replace('\'', '"'), json);
    }

    public class DeserializeClass 
    {
      [CommandArgument("test123", "desc_test123")]
      public string MyProp { get; [UsedImplicitly] set; }

      public string Ignore { get; [UsedImplicitly] set; }

      [CommandArgument("desc_inner")]
      public InnerTestClass Inner { get; set; }

      public class InnerTestClass
      {
        [CommandArgument("desc_name")]
        public string Name { get; set; }
      }
    }
  }
}
