using System.IO;

namespace SIM.Serialization
{
  using JetBrains.Annotations;

  public interface ISerializer
  {
    [CanBeNull]
    T Deserialize<T>([NotNull] StreamReader reader);

    [CanBeNull]
    T DeserializeObject<T>([CanBeNull] string text);

    [NotNull]
    string Serialize([CanBeNull] object obj);

    void Serialize([NotNull] TextWriter writer, [CanBeNull] object obj);     
  }
}