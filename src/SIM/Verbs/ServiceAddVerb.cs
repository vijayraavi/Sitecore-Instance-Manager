namespace SIM.Verbs
{
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Abstract.Connection;
  using SIM.Abstract.Services;
  using SIM.Commands;
  using SIM.Services;

  public class ServiceAddVerb : ServiceAddCommand
  {
    [UsedImplicitly]
    public ServiceAddVerb() 
      : this(Default.ConnectionStringFactory, Default.ServiceStore)
    {
    }                                         

    private ServiceAddVerb(IConnectionStringFactory connectionStringFactory, IServiceStore serviceStore) 
      : base(connectionStringFactory, serviceStore)
    { 
    }

    [Option('t', "type", Required = true)]
    public override ServiceType? ServiceType { get; set; }

    [Option('n', "name", Required = true)]
    public override string ServiceName { get; set; }
                     
    [Option('c', "connectionString", Required = true)]
    public override string ConnectionString { get; set; }
  }
}