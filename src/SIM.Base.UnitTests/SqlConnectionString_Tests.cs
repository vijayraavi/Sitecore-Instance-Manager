namespace SIM
{
  using System;
  using SIM.Base.Services;
  using Xunit;

  public class SqlConnectionString_Tests
  {
    [Fact]
    public void Create_Valid()
    {
      var connect = new SqlConnectionString("Data Source=.\\SQL2016; User ID=sa; Password=12345");

      Assert.Equal("Data Source=.\\SQL2016; User ID=sa; Password=12345", connect.Value);
      Assert.Equal(".\\SQL2016", connect.Builder.DataSource);
      Assert.Equal("sa", connect.Builder.UserID);
      Assert.Equal("12345", connect.Builder.Password);
    }

    [Fact]
    public void Create_Invalid()
    {
      Assert.Throws<ArgumentException>(() => new SqlConnectionString("Data Sour=.\\SQL2016"));
    }
  }
}