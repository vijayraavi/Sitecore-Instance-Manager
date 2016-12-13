namespace SIM.IO
{
  using SIM.Abstract.IO;

  public abstract class MockFileSystemEntry : IFileSystemEntry
  {
    protected MockFileSystemEntry(IMockFileSystem fileSystem, string fullPath)
    {
      SIM.Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));
      SIM.Assert.ArgumentNotNullOrEmpty(fullPath, nameof(fullPath));

      FileSystem = fileSystem;
      MockFileSystem = fileSystem;
      FullName = fullPath;
    }

    public IFileSystem FileSystem { get; }

    protected IMockFileSystem MockFileSystem { get; }

    public string FullName { get; }
  }
}