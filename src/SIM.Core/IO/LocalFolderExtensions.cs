namespace SIM.IO
{
  using JetBrains.Annotations;
  using SIM.Abstract.IO;

  public static class LocalFolderExtensions
  {
    [NotNull]
    public static IFile GetChildFile(this IFolder folder, string fileName)
    {
      return new LocalFile(folder.FileSystem, folder.FileSystem.Path.Combine(folder.FullName, fileName));
    }
  }
}