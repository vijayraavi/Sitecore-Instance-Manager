namespace SIM
{
  using JetBrains.Annotations;
  using SIM.Abstract.Connection;
  using SIM.Abstract.IO;
  using SIM.Abstract.Services;
  using SIM.IO;
  using SIM.Profiles;
  using SIM.Serialization;
  using SIM.Stores;

  public static class Default
  {
    [NotNull]
    public static readonly IFileSystem FileSystem 
      = LocalFileSystem.Default;

    [NotNull]
    public static readonly Serializer Serializer
      = new Serializer(FileSystem);

    [NotNull]
    public static readonly IConnectionStringFactory ConnectionStringFactory
      = Connection.ConnectionStringFactory.Default;

    [NotNull]
    public static readonly IServiceStore ServiceStore
      = new ServiceStore(FileSystem, Serializer);

    [NotNull]
    public static readonly IProfileProvider ProfileProvider
      = new ProfileProvider(FileSystem);
  }
}