namespace SIM.Extensions
{
  using System.IO;
  using JetBrains.Annotations;

  public  static class StreamExtensions
  {
    public static StreamReader OpenReader([NotNull] this Stream stream)
    {
      Assert.ArgumentNotNull(stream, nameof(stream));

      return new StreamReader(stream);
    }
  }
}
