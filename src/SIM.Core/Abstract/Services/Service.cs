namespace SIM.Abstract.Services
{
  using JetBrains.Annotations;        
  using SIM.Abstract.Connection;
  using SIM.Services;
                         
  public class Service
  {
    [NotNull]
    public string Name { get; set; }

    [NotNull]
    public ServiceType? Type { get; set; }

    [NotNull]
    public IConnectionString ConnectionString { get; set; }
  }
}