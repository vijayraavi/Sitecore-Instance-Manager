namespace SIM
{
  using System;
  using Sitecore.Diagnostics.Base.Exceptions;
  using SIM.Base.Services;
  using Xunit;

  public class WebServerConnectionString_Tests
  {
    [Fact]
    public void Create_Valid_HttpsHostName()
    {
      var connect = new WebServerConnectionString("https://some.web.server:55539/?username=usr&password=Some:Password123&token=Some-Token123");

      Assert.Equal("https://some.web.server:55539", connect.Url);
      Assert.Equal("Some-Token123", connect.Token);
      Assert.Equal("usr", connect.UserName);
      Assert.Equal("Some:Password123", connect.Password);
    }

    [Fact]
    public void Create_Valid_HttpAddress()
    {
      var connect = new WebServerConnectionString("http://127.0.0.1:55539?username=usr&password=Some:Password123&token=Some-Token123");

      Assert.Equal("http://127.0.0.1:55539", connect.Url);
      Assert.Equal("Some-Token123", connect.Token);
    }

    [Fact]
    public void Create_Invalid_NoUserName()
    {
      Assert.Throws<ArgumentNullOrEmptyException>(() => new WebServerConnectionString("https://some.web.server:55539/?password=Some:Password123&token=Some-Token123"));
    }

    [Fact]
    public void Create_Invalid_NoPassword()
    {
      Assert.Throws<ArgumentNullOrEmptyException>(() => new WebServerConnectionString("https://some.web.server:55539/?username=usr&token=Some-Token123"));
    }

    [Fact]
    public void Create_Invalid_NoToken()
    {
      Assert.Throws<ArgumentNullOrEmptyException>(() => new WebServerConnectionString("https://some.web.server:55539/?username=usr&password=Some:Password123"));
    }

    [Fact]
    public void Create_Invalid_NoTokenValue()
    {
      Assert.Throws<ArgumentNullOrEmptyException>(() => new WebServerConnectionString("https://some.web.server:55539/?username=usr&password=Some:Password123&token="));
    }

    [Fact]
    public void Create_Invalid_WrongSchema()
    {
      Assert.Throws<ArgumentException>(() => new WebServerConnectionString("ftp://127.0.0.1:55539/?username=usr&password=Some:Password123&token=Some-Token123"));
    }
    
    [Fact]
    public void Create_Invalid_NoSchema()
    {
      Assert.Throws<UriFormatException>(() => new WebServerConnectionString("://127.0.0.1:55539/?username=usr&password=Some:Password123&token=Some-Token123"));
    }

    [Fact]
    public void Create_Invalid_NoDelimeter0()
    {
      Assert.Throws<ArgumentException>(() => new WebServerConnectionString("http127.0.0.1:55539/?username=usr&password=Some:Password123&token=Some-Token123"));
    }

    [Fact]
    public void Create_Invalid_NoDelimeter1()
    {
      Assert.Throws<UriFormatException>(() => new WebServerConnectionString("http//127.0.0.1:55539/?username=usr&password=Some:Password123&token=Some-Token123"));
    }

    [Fact]
    public void Create_Invalid_NoDelimeter2()
    {
      Assert.Throws<UriFormatException>(() => new WebServerConnectionString("http:/127.0.0.1:55539/?username=usr&password=Some:Password123&token=Some-Token123"));
    }

    [Fact]
    public void Create_Invalid_NoDelimeter3()
    {
      Assert.Throws<UriFormatException>(() => new WebServerConnectionString("http:127.0.0.1:55539/?username=usr&password=Some:Password123&token=Some-Token123"));
    }

    [Fact]
    public void Create_Invalid_NoPort()
    {
      Assert.Throws<ArgumentException>(() => new WebServerConnectionString("https://some.web.server/?username=usr&password=Some:Password123&token=Some-Token123"));
    }
  }
}
