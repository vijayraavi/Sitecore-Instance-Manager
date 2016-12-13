namespace SIM.Verbs
{
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Abstract.Connection;
  using SIM.Abstract.Services;
  using SIM.Commands;
  using SIM.Services;

  public class MongoAddVerb : ServiceAddCommand
  {
    [UsedImplicitly]
    public MongoAddVerb()
      : this(Default.ConnectionStringFactory, Default.ServiceStore)
    {
    }

    private MongoAddVerb(IConnectionStringFactory connectionStringFactory, IServiceStore serviceStore)
      : base(connectionStringFactory, serviceStore)
    {
    }

    public override ServiceType? ServiceType => Services.ServiceType.MongoDB;

    [Option('n', "name", Required = true)]
    public override string ServiceName { get; set; }

    [Option('c', "connectionString", Required = true)]
    public override string ConnectionString { get; set; }
  }
}