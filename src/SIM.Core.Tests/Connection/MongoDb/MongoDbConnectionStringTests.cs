namespace SIM.Connection.MongoDb
{
  using System;                       
  using Xunit;

  public class MongoDbConnectionStringTests
  {
    [Fact]
    public void MongoDbConnectionString_New_Empty()
    {
      // arrange & act
      var connectionString = new MongoDbConnectionString("");

      // assert
      Assert.Equal("", connectionString.Value);
    }

    [Fact]
    public void MongoDbConnectionString_New_Base()
    {
      // arrange & act
      var value = "mongodb://localhost:27017";
      var connectionString = new MongoDbConnectionString(value);

      // assert
      Assert.Equal(value, connectionString.Value);
    }

    [Fact]
    public void MongoDbConnectionString_New_Full()
    {
      // arrange & act
      var value = "mongodb://localhost:27017/sitecore_analytics";
      var connectionString = new MongoDbConnectionString(value);

      // assert
      Assert.Equal(value, connectionString.Value);
    }

    [Fact]
    public void MongoDbConnectionString_Invalid()
    {
      // arrange
      var value = "some text";
      
      // act & assert
      Assert.Throws<ArgumentException>(() => new MongoDbConnectionString(value));
    }
  }
}