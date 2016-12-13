namespace SIM.IO
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using JetBrains.Annotations;
  using SIM.Abstract.IO;

  public class MockFileSystem : Dictionary<string, IFileSystemEntry>, IMockFileSystem
  {
    public IPathHelper Path { get; } = new SystemPathHelper();

    public IFolder ParseFolder(string path)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));
                
      return new MockFolder(this, path);
    }

    public IFile ParseFile(string path)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));

      return new MockFile(this, path) { Exists = false };
    }

    public IFile ParseFile(string path, string contents)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));
      Assert.ArgumentNotNullOrEmpty(contents, nameof(contents));

      var file = new MockFile(this, path, contents) { Exists = true };  

      return file;
    }

    public IFile ParseFile(string path, Stream stream)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));
      Assert.ArgumentNotNull(stream, nameof(stream));

      var file = new MockFile(this, path, stream) { Exists = true };   

      return file;
    }

    public IFile ParseFile(string path, Func<Stream> openRead)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));
      Assert.ArgumentNotNull(openRead, nameof(openRead));

      var file = new MockFile(this, path, openRead) { Exists = true };   

      return file;
    }

    public IFile ParseFile(string path, Func<Stream> openRead, Func<Stream> openWrite)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));
      Assert.ArgumentNotNull(openRead, nameof(openRead));
      Assert.ArgumentNotNull(openWrite, nameof(openWrite));

      var file = new MockFile(this, path, openRead, openWrite) { Exists = true };   

      return file;
    }

    public T Add<T>(string path, T file) where T : IFileSystemEntry
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));
      Assert.ArgumentNotNull(file, nameof(file));

      base.Add(Path.GetFullPath(path), file);

      return file;
    }

    public bool Contains(string path)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));

      return ContainsKey(Path.GetFullPath(path));
    }
  }
}