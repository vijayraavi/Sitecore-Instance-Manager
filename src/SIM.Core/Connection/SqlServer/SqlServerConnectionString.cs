namespace SIM.Connection.SqlServer
{
  using System.Data.SqlClient;
  using JetBrains.Annotations;
  using SIM.Abstract.Connection;

  public class SqlServerConnectionString : IConnectionString
  {
    public SqlServerConnectionString([NotNull] string connectionString)
    {
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      Value = connectionString;
      Builder = new SqlConnectionStringBuilder(connectionString);
    }

    public SqlConnectionStringBuilder Builder { get; }

    public string Value { get; }
  }
}
