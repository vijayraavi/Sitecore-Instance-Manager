namespace SIM.Stores
{
  using SIM.Abstract.Services;
  using SIM.Connection.MongoDb;
  using SIM.Connection.SqlServer;
  using SIM.Extensions;
  using SIM.IO;
  using SIM.Serialization;
  using SIM.Services;
  using Xunit;

  public class ServiceStoreTests
  {
    [Fact]
    public void Test()
    {
      var fs = new MockFileSystem();
      var file = fs.ParseFile("C:\\1.json");

      var sut = new ServiceStore(file, new Serializer(fs));

      sut.Add(new Service { Name = "local", Type = ServiceType.SqlServer, ConnectionString = new SqlServerConnectionString("")});
      sut.Add(new Service { Name = "local", Type = ServiceType.MongoDB, ConnectionString = new MongoDbConnectionString("")});
               
      Assert.True(sut.Exists("local", ServiceType.SqlServer));
      Assert.True(sut.Exists("local", ServiceType.MongoDB));
      Assert.False(sut.Exists("local", ServiceType.Solr));
    }
  }
}
