namespace SIM.Common
{
  using System;
  using Xunit;

  public class AbstractCommandTests
  {
    [Fact]
    public void TestCommandThrowsException()
    {
      var sut = new CommandThrowsException<AccessViolationException>();

      // act
      var result = sut.Execute();

      // assert
      Assert.NotNull(result);
      Assert.Equal(false, result.Success);

      Assert.NotNull(result.Error);
      Assert.Equal(typeof(AccessViolationException).FullName, result.Error.ClassName);
      Assert.Equal(null, result.Error.InnerException);
      Assert.NotEqual(null, result.Error.Data);
      Assert.Equal(0, result.Error.Data.Count);
      Assert.Equal(true, result.Elapsed.Ticks > 0);
    }

    private class CommandThrowsException<T> : AbstractCommand where T : Exception, new()
    {
      protected override void DoExecute(ICommandResult result)
      {
        throw new T();
      }
    }

    [Fact]
    public void TestCommandDoesNothing()
    {
      var sut = new CommandDoesNothing();

      // act
      var result = sut.Execute();

      // assert
      Assert.Equal(true, result.Success);
      Assert.Equal(null, result.Error);
      Assert.Equal(true, result.Elapsed.Ticks > 0);
    }

    private class CommandDoesNothing : AbstractCommand
    {
      protected override void DoExecute(ICommandResult result)
      {                      
      }
    }

    [Fact]
    public void TestCommandFails()
    {
      var sut = new CommandFails();

      // act
      var result = sut.Execute();

      // assert
      Assert.Equal(false, result.Success);
      Assert.Equal(null, result.Error);
      Assert.Equal(true, result.Elapsed.Ticks > 0);
    }
            
    private class CommandFails : AbstractCommand
    {
      protected override void DoExecute(ICommandResult result)
      {
        result.Success = false;
      }
    }
  }
}