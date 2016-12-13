namespace SIM.Abstract.Services
{
  using JetBrains.Annotations;
  using SIM.Services;

  public interface IServiceStore
  {
    void Add([NotNull] Service service);

    void Remove([NotNull] string name, ServiceType type);

    bool Exists([NotNull] string name, ServiceType type);
  }
}
