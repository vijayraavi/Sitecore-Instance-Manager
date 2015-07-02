namespace SIM.Connection.SqlServer
{
  using SIM.Abstract.Connection;

  public class SqlServerConnectionStringParser : IConnectionStringParser
  {
    public IConnectionString TryParse(string connectionString)
    {
      if (connectionString == null)
      {
        return null;
      }

      try
      {
        return new SqlServerConnectionString(connectionString);
      }
      catch
      {
        return null;
      }       
    }
  }
}