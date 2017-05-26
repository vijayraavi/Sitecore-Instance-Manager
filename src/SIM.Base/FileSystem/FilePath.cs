namespace SIM.Base.FileSystem
{
  using JetBrains.Annotations;

  public sealed class FilePath
  {
    public FilePath([NotNull] string fullname)
    {
      FullName = fullname;
    }

    [NotNull]
    public string FullName { get; }

    [NotNull]
    public static implicit operator string([NotNull] FilePath filePath)
    {
      return filePath.FullName;
    }
  }
}
