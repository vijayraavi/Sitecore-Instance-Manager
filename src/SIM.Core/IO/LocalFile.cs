namespace SIM.IO
{
  using System;
  using System.IO;
  using JetBrains.Annotations;
  using Newtonsoft.Json;
  using SIM.Abstract.IO;
  using SIM.Extensions;

  public class LocalFile : LocalFileSystemEntry, IFile, IEquatable<LocalFile>
  {
    public LocalFile([NotNull] string path) : this(LocalFileSystem.Default, path)
    { 
      FileInfo = new FileInfo(FullName);
    }

    public LocalFile([NotNull] IFileSystem fileSystem, [NotNull] string path) : base(fileSystem, path)
    {
      FileInfo = new FileInfo(FullName);
      Folder = new LocalFolder(fileSystem, FullName);
    }

    [NotNull]
    [JsonIgnore]
    public FileInfo FileInfo { get; }
                 
    [JsonIgnore]
    public IFolder Folder { get; }

    public bool Exists => FileInfo.Exists;

    public Stream OpenRead()
    {
      return FileInfo.OpenRead();
    }

    public Stream OpenWrite()
    {
      return FileInfo.OpenWrite();
    }

    public bool Equals(LocalFile other)
    {
      return this.Equals(other, x => x?.FileInfo.FullName);
    }

    public override bool Equals(object obj)
    {
      return this.Equals(obj, x => x?.FileInfo.FullName);
    }

    public override int GetHashCode()
    {
      return this.FileInfo.GetHashCode();
    }
  }
}