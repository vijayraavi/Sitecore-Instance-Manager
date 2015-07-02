namespace SIM.IO
{
  using System;
  using SIM.Abstract.IO;
  using SIM.Extensions;

  public class MockFolder : MockFileSystemEntry, IFolder, IEquatable<MockFolder>
  {
    public MockFolder(IFileSystem fileSystem, string fullPath) : base(fileSystem, fullPath)
    {
    }

    public bool Exists { get; private set; }

    public bool TryCreate()
    {
      var existedBefore = Exists;

      Exists = true;

      return !existedBefore;
    }

    public bool Equals(MockFolder other)
    {
      return this.Equals(other, x => x?.FullName);
    }

    public override bool Equals(object obj)
    {
      return this.Equals(obj, x => x?.FullName);
    }

    public override int GetHashCode()
    {
      return this.FullName.GetHashCode();
    }
  }
}