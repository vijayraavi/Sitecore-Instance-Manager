namespace SIM.Commands
{
  using System;
  using SIM.Abstract.Connection;
  using SIM.Abstract.Services;
  using SIM.Common;
  using SIM.Connection.SqlServer;
  using SIM.Services;

  public class ServiceAddCommand : AbstractCommand
  {
    private IConnectionStringFactory ConnectionStringFactory { get; }

    private IServiceStore ServiceStore { get; }

    public ServiceAddCommand(IConnectionStringFactory connectionStringFactory, IServiceStore serviceStore)
    {
      ConnectionStringFactory = connectionStringFactory;
      ServiceStore = serviceStore;
    }

    public virtual ServiceType? ServiceType { get; set; }

    public virtual string ServiceName { get; set; }

    public virtual string ConnectionString { get; set; }

    protected override void DoExecute(ICommandResult result)
    {
      Assert.ArgumentNotNull(ServiceName, nameof(ServiceName));
      Assert.ArgumentNotNull(ServiceType, nameof(ServiceType));
      Assert.ArgumentNotNull(ConnectionString, nameof(ConnectionString));
     
      var connectionString = new SqlServerConnectionString(ConnectionString);

      if (ServiceStore.Exists(ServiceName))
      {
        result.Success = false;
        result.Message = $"Service with the '{ServiceName}' already exists";

        return;
      }

      try
      {
        var service = new Service
        {
          Type = ServiceType,
          ConnectionString = connectionString,
          Name = ServiceName
        };

        ServiceStore.Add(service);
      }
      catch (Exception ex)
      {
        result.Error = new CustomException(ex);
        result.Message = $"Failed to add the '{ServiceName}' service";

        return;
      }

      result.Success = true;
    }
  }
}