namespace SIM.Abstract.Services
{
  using JetBrains.Annotations;

  public interface IServiceStore
  {
    void Add([NotNull] Service service);

    void Remove([NotNull] string name);

    bool Exists([NotNull] string name);
  }
}
