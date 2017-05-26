namespace SIM.Base.Services
{
  using JetBrains.Annotations;

  public abstract class ConnectionString
  {
    protected ConnectionString([NotNull] string value)
    {
      Value = value;
    }

    [NotNull]
    public string Value { get; }

    [NotNull]
    public static implicit operator string([NotNull] ConnectionString connectionString)
    {
      return connectionString.Value;
    }
  }
}
