namespace SIM.Verbs
{
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Abstract.Connection;
  using SIM.Abstract.Services;
  using SIM.Commands;
  using SIM.Services;

  public class SolrAddVerb : ServiceAddCommand
  {
    [UsedImplicitly]
    public SolrAddVerb()
      : this(Default.ConnectionStringFactory, Default.ServiceStore)
    {
    }

    private SolrAddVerb(IConnectionStringFactory connectionStringFactory, IServiceStore serviceStore)
      : base(connectionStringFactory, serviceStore)
    {
    }

    public override ServiceType? ServiceType => Services.ServiceType.Solr;

    [Option('n', "name", Required = true)]
    public override string ServiceName { get; set; }

    [Option('c', "connectionString", Required = true)]
    public override string ConnectionString { get; set; }
  }
}