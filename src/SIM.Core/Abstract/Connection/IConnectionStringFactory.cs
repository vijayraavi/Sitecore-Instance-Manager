namespace SIM.Abstract.Connection
{
  using JetBrains.Annotations;

  public interface IConnectionStringFactory
  {
    [CanBeNull]
    IConnectionString TryParse([NotNull] string connectionString);

    [NotNull]
    IConnectionString Parse([NotNull] string connectionString);
  }
}