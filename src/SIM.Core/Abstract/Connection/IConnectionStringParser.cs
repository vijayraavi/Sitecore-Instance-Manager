namespace SIM.Abstract.Connection
{
  using JetBrains.Annotations;

  public interface IConnectionStringParser
  {
    [CanBeNull]
    IConnectionString TryParse(string connectionString);
  }
}