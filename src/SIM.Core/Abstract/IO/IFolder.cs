namespace SIM.Abstract.IO
{                     
  public interface IFolder : IFileSystemEntry
  {
    bool TryCreate();

    void Create();

    bool Exists { get; }
  }
}