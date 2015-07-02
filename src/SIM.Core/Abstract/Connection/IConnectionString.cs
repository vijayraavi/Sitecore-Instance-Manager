namespace SIM.Abstract.Connection
{
  using JetBrains.Annotations;

  public interface IConnectionString
  {
    [NotNull]
    string Value { get; }
  }
}
