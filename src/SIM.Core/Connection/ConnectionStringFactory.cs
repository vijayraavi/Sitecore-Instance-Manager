namespace SIM.Connection
{
  using System;
  using JetBrains.Annotations;
  using SIM.Abstract.Connection;
  using SIM.Connection.MongoDb;
  using SIM.Connection.SqlServer;

  public class ConnectionStringFactory : IConnectionStringFactory
  {
    public static readonly IConnectionStringFactory Default = new ConnectionStringFactory();

    [NotNull]
    private readonly IConnectionStringParser[] _Parsers =
    {
      new SqlServerConnectionStringParser(),
      new MongoDbConnectionStringParser(), 
    };

    public IConnectionString TryParse(string connectionString)
    {
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      foreach (var parser in _Parsers)
      {
        var value = parser.TryParse(connectionString);
        if (value != null)
        {
          return value;
        }
      }

      return null;
    }

    public IConnectionString Parse(string connectionString)
    {
      var result = TryParse(connectionString);
      if (result != null)
      {
        return result;
      }

      throw new ArgumentException($"The connection string is not recognized: {connectionString}");
    }
  }
}
