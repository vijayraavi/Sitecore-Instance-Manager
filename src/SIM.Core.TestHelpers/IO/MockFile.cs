namespace SIM.IO
{
  using System;
  using System.IO;
  using JetBrains.Annotations;
  using SIM.Abstract.IO;
  using SIM.Extensions;

  public class MockFile : MockFileSystemEntry, IFile, IEquatable<MockFile>
  {
    private bool _Exists;

    public MockFile(IMockFileSystem fileSystem, string path, [NotNull] string contents) 
      : base(fileSystem, path)
    {
      Assert.ArgumentNotNullOrEmpty(contents, nameof(contents));

      Stream = GetStream(contents);
      Folder = fileSystem.ParseFolder(fileSystem.Path.GetDirectoryName(FullName));
    }

    public MockFile([NotNull] IMockFileSystem fileSystem, [NotNull] string path, [NotNull] Stream stream) 
      : base(fileSystem, path)
    {
      Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));
      Assert.ArgumentNotNull(stream, nameof(stream));

      Stream = stream;
      Folder = fileSystem.ParseFolder(fileSystem.Path.GetDirectoryName(FullName));
    }

    public MockFile([NotNull] IMockFileSystem fileSystem, string path, Func<Stream> openRead = null, Func<Stream> openWrite = null)
      : base(fileSystem, path)
    {
      Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));

      OpenReadFunc = openRead;
      OpenWriteFunc = openWrite;
      Folder = fileSystem.ParseFolder(fileSystem.Path.GetDirectoryName(FullName));
    }

    public Func<Stream> OpenReadFunc { get; }

    public Func<Stream> OpenWriteFunc { get; }

    protected Stream Stream { get; set; }

    public IFolder Folder { get; }

    public bool Exists
    {
      get
      {
        return _Exists;
      }

      set
      {
        if (!MockFileSystem.Contains(FullName))
        {
          MockFileSystem.Add(FullName, this);
        }

        _Exists = value;
      }
    }

    public Stream OpenRead()
    {
      if (!Exists)
      {
        throw new FileNotFoundException();
      }

      if (OpenReadFunc != null)
      {
        return OpenReadFunc().IsNotNull();
      }

      if (Stream == null)
      {
        throw new IOException("File does not have a stream assigned");
      }           
      
      Stream.Seek(0, SeekOrigin.Begin);

      return Stream;
    }

    public Stream OpenWrite()
    {
      Exists = true;

      return Stream ?? OpenWriteFunc?.Invoke() ?? (Stream = new KeepAliveMemoryStream());
    }

    private static MemoryStream GetStream(string text)
    {               
      var stream = new KeepAliveMemoryStream();
      var writer = new StreamWriter(stream);
      writer.Write(text);
      writer.Flush();
      stream.Position = 0;

      return stream;
    }

    public bool Equals(MockFile other)
    {
      return this.Equals(other, x => x?.FullName);
    }

    public override bool Equals(object obj)
    {
      return this.Equals(obj, x => x?.FullName);
    }

    public override int GetHashCode()
    {
      return FullName.GetHashCode();
    }

    public class KeepAliveMemoryStream : MemoryStream
    {
      protected override void Dispose(bool disposing)
      {
        // keep stream open        
      }

      public override void Close()
      {        
      }

      public void DoDispose()
      {
        base.Close();
      }
    }
  }
}