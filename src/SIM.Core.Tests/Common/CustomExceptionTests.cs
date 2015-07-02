using System;

namespace SIM.Common
{
  using Xunit;

  public class CustomExceptionTests
  {
    [Fact]
    public void TestCustomException()
    {
      SomeMethod(123);
    }

    private void SomeMethod(int someValue)
    {
      Assert.Equal(123, someValue);

      var stackTrace = new[]
      {
        "at SIM.Common.CustomExceptionTests.SomeMethod(Int32 someValue)"
      };

      CustomException sut;
      try
      {
        try
        {
          throw new InnerTestException();          
        }
        catch (Exception iex)
        {          
          throw new OuterTestException(iex);
        }
      }
      catch (Exception ex)
      {
        sut = new CustomException(ex);
      }

      // assert
      Assert.Equal(typeof(OuterTestException).FullName, sut.ClassName);
      Assert.Equal("Test Message", sut.Message);
      Assert.Equal(stackTrace, sut.StackTrace);
      Assert.NotNull(sut.InnerException);
      Assert.Equal(typeof(InnerTestException).FullName, sut.InnerException.ClassName);
      Assert.Equal("Test Inner Message", sut.InnerException.Message);
      Assert.Equal(stackTrace, sut.InnerException.StackTrace);       
    }

    private class OuterTestException : Exception
    {
      public OuterTestException(Exception innerException = null) : base("Test Message", innerException)
      {
      }
    }

    private class InnerTestException : Exception
    {
      public InnerTestException() : base("Test Inner Message")
      {
      }
    }
  }
}
