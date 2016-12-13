namespace SIM.Abstract.IO
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using JetBrains.Annotations;

  public interface IMockFileSystem : IFileSystem, IDictionary<string, IFileSystemEntry>
  {         
    [NotNull]
    IFile ParseFile([NotNull] string path, [NotNull] string contents);

    [NotNull]
    IFile ParseFile([NotNull] string path, [NotNull] Stream stream);

    [NotNull]
    IFile ParseFile([NotNull] string path, [NotNull] Func<Stream> openRead);

    [NotNull]
    IFile ParseFile([NotNull] string path, [NotNull] Func<Stream> openRead, [NotNull] Func<Stream> openWrite);

    [NotNull]
    T Add<T>([NotNull] string path, [NotNull] T file) where T : IFileSystemEntry;

    bool Contains([NotNull] string path);
  }
}