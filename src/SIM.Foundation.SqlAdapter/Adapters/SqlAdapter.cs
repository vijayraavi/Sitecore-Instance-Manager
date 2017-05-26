namespace SIM.Adapters
{
  using System;
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using JetBrains.Annotations;
  using Microsoft.SqlServer.Dac;
  using Sitecore.Diagnostics.Base;
  using SIM.Base.FileSystem;
  using SIM.Base.Services;

  public sealed class SqlAdapter
  {
    public SqlAdapter([NotNull] SqlConnectionString connectionString)
    {
      ConnectionString = connectionString;
    }

    [NotNull]
    public SqlConnectionString ConnectionString { get; }

    public void DeployDatabase([NotNull] string databaseName, [NotNull] FilePath sourceFilePath)
    {
      try
      {
        var package = DacPackage.Load(sourceFilePath);
        var options = new DacDeployOptions();
        var services = new DacServices(ConnectionString);

        services.Deploy(package, databaseName, true, options);
      }
      catch (Exception ex)
      {
        throw new SqlAdapterException(ex);
      }
    }

    public void DeleteDatabase([NotNull] string name)
    {
      try
      {
        using (var connection = new SqlConnection(ConnectionString))
        {
          connection.Open();
          var sql = $"ALTER DATABASE [{name}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; \r\nDROP DATABASE [{name}]";
          var command = new SqlCommand
          {
            CommandText = sql,
            Connection = connection,
            CommandTimeout = int.MaxValue
          };

          command.ExecuteNonQuery();
        }
      }
      catch (SqlException ex)
      {
        if (ex.Class == 14 && ex.State == 5 && ex.Message.Contains("because it does not exist"))
        {
          return;
        }

        throw new SqlAdapterException(ex);
      }
    }

    public bool DatabaseExists([NotNull] string databaseName)
    {
      try
      {
        using (var connection = new SqlConnection(ConnectionString))
        {
          connection.Open();
          var sql = $"SELECT COUNT(*) FROM sys.Databases WHERE [Name] = '{databaseName}'";
          var command = new SqlCommand
          {
            CommandText = sql,
            Connection = connection,
            CommandTimeout = int.MaxValue
          };

          var count = (int)command.ExecuteScalar();
          Assert.IsTrue(count <= 1, $"There is a problem with {nameof(SqlAdapter)}. {nameof(DatabaseExists)} function detected more than 1 databases with '{databaseName}' name which is definitely wrong.");

          return count == 1;
        }
      }
      catch (Exception ex)
      {
        throw new SqlAdapterException(ex);
      }
    }

    [NotNull]
    public IReadOnlyList<string> GetDatabases()
    {
      try
      {
        using (var connection = new SqlConnection(ConnectionString))
        {
          connection.Open();
          var sql = "SELECT [Name] FROM sys.Databases";
          var command = new SqlCommand
          {
            CommandText = sql,
            Connection = connection,
            CommandTimeout = int.MaxValue
          };

          var list = new List<string>();
          using (var reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              list.Add(reader.GetString(0));
            }
          }

          return list;
        }
      }
      catch (Exception ex)
      {
        throw new SqlAdapterException(ex);
      }
    }

    public FilePath GetDatabaseFilePath(string databaseName)
    {
      try
      {
        using (var connection = new SqlConnection(ConnectionString))
        {
          connection.Open();
          var sql = $"SELECT TOP 1 [m].[Physical_Name] " +
            $"FROM [sys].[master_files] [m] " +
            $"INNER JOIN [sys].[Databases] [d] ON [d].[database_id] = [m].[database_id] " +
            $"WHERE [m].[type_desc] = 'ROWS' AND [d].[Name] = '{databaseName}'";

          var command = new SqlCommand
          {
            CommandText = sql,
            Connection = connection,
            CommandTimeout = int.MaxValue
          };

          var filePath = (string)command.ExecuteScalar();
          if (string.IsNullOrWhiteSpace(filePath))
          {
            throw new DatabaseDoesNotExistException(databaseName);
          }

          return new FilePath(filePath);
        }
      }
      catch (SqlAdapterException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new SqlAdapterException(ex);
      }
    }
  }
}
