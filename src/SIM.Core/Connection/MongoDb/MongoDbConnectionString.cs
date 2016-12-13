namespace SIM.Connection.MongoDb
{
  using JetBrains.Annotations;
  using SIM.Abstract.Connection;

  public class MongoDbConnectionString : IConnectionString
  {
    public MongoDbConnectionString([NotNull] string value)
    {
      Assert.ArgumentNotNull(value, nameof(value));
      Assert.ArgumentCondition(value == string.Empty || value.StartsWith("mongodb://"), nameof(value), "The connection string value doesn't start with mongodb://");

      Value = value;
    }

    public string Value { get; }
  }
}
