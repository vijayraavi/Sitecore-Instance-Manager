namespace SIM.Connection.MongoDb
{
  using SIM.Abstract.Connection;

  public class MongoDbConnectionStringParser : IConnectionStringParser
  {
    public IConnectionString TryParse(string connectionString)
    {
      if (connectionString == null)
      {
        return null;
      }

      return new MongoDbConnectionString(connectionString);
    }
  }
}