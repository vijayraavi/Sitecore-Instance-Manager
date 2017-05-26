namespace SIM.Base.Services
{
  using JetBrains.Annotations;

  public sealed class SqlConnectionString : ConnectionString
  {
    public SqlConnectionString([NotNull] string value)
      : base(value)
    {
    }
  }
}