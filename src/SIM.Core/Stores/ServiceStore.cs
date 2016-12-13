namespace SIM.Stores
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using JetBrains.Annotations;
  using SIM.Abstract.IO;
  using SIM.Abstract.Services;
  using SIM.Extensions;
  using SIM.Serialization;
  using SIM.Services;

  public class ServiceStore : IServiceStore
  {
    [NotNull]
    private readonly object _SyncRoot = new object();

    [NotNull]
    protected IFile File { get; }

    [NotNull]
    protected ISerializer Serializer { get; }

    [NotNull]
    private Dictionary<ServiceType, Dictionary<string, Service>> Dictionary { get; }

    // main ctor
    public ServiceStore([NotNull] IFile file, [NotNull] ISerializer serializer)
    {
      Assert.ArgumentNotNull(file, nameof(file));
      Assert.ArgumentNotNull(serializer, nameof(serializer));

      var dictionary = file.Exists ? ReadFile(file, serializer) : new Dictionary<ServiceType, Dictionary<string, Service>>();

      File = file;
      Dictionary = dictionary;
      Serializer = serializer;
    }

    public ServiceStore([NotNull] IFileSystem fileSystem, [NotNull] Serializer serializer)
      : this(fileSystem.ParseFile(Environment.ExpandEnvironmentVariables("%APPDATA%\\SIM2\\Services.json")), serializer)
    {
      Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));
      Assert.ArgumentNotNull(serializer, nameof(serializer));
    }

    [NotNull]
    protected object InnerStore => Dictionary;

    public void Add(Service service)
    {
      Assert.ArgumentNotNull(service, nameof(service));

      lock (_SyncRoot)
      {
        Dictionary.GetOrCreate(service.Type.Value).Add(service.Name, service);

        Commit();
      }
    }

    public bool Exists(string name, ServiceType type)
    {
      return Dictionary.GetOrCreate(type).ContainsKey(name);
    }

    public void Remove(string name, ServiceType type)
    {
      lock (_SyncRoot)
      {
        Dictionary.TryGetValue(type)?.Remove(name);

        Commit();
      }
    }

    private void Commit()
    {
      using (var writer = new StreamWriter(File.OpenWrite()))
      {
        Serializer.Serialize(writer, InnerStore);
      }
    }

    [NotNull]
    private static Dictionary<ServiceType, Dictionary<string, Service>> ReadFile([NotNull] IFile file, [NotNull] ISerializer serializer)
    {
      Assert.ArgumentNotNull(file, nameof(file));
      Assert.ArgumentNotNull(serializer, nameof(serializer));

      using (var reader = new StreamReader(file.OpenRead()))
      {
        return serializer.Deserialize<Dictionary<ServiceType, Dictionary<string, Service>>>(reader).IsNotNull();
      }
    }
  }
}