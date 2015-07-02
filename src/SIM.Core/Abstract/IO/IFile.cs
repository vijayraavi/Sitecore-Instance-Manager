namespace SIM.Abstract.IO
{
  using System.IO;
  using JetBrains.Annotations;

  public interface IFile : IFileSystemEntry
  { 
    [NotNull]
    Stream OpenRead();

    [NotNull]
    Stream OpenWrite();

    [NotNull]
    IFolder Folder { get; }

    bool Exists { get; }
  }
}
