namespace SIM.IO
{
  using System;
  using System.IO;
  using JetBrains.Annotations;
  using SIM.Abstract.IO;
  using SIM.Extensions;

  public class LocalFolder : LocalFileSystemEntry, IFolder, IEquatable<LocalFolder>
  {
    public LocalFolder([NotNull] IFileSystem fileSystem, [NotNull] string path) : base(fileSystem, path)
    {
      DirectoryInfo = new DirectoryInfo(FullName);
    }

    [NotNull]
    public DirectoryInfo DirectoryInfo { get; }

    public bool Exists => DirectoryInfo.Exists;

    public bool TryCreate()
    {
      if (!DirectoryInfo.Exists)
      {
        return false;
      }

      DirectoryInfo.Create();

      return true;
    }

    public bool Equals(LocalFolder other)
    {
      return this.Equals(other, x => x?.DirectoryInfo.FullName);
    }

    public override bool Equals(object obj)
    {
      return this.Equals(obj, x => x?.DirectoryInfo.FullName);
    }

    public override int GetHashCode()
    {
      return this.DirectoryInfo.GetHashCode();
    }
  }
}