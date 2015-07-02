namespace SIM.Serialization
{
  using System.IO;
  using JetBrains.Annotations;
  using Newtonsoft.Json;
  using SIM.Abstract.IO;
  using SIM.Extensions;
  using SIM.Serialization.Connection.SqlServer;
  using SIM.Serialization.IO;

  public class Serializer : ISerializer
  {
    public Serializer([NotNull] IFileSystem fileSystem)
    {
      Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));

      FileSystem = fileSystem;  
      InnerSerializer = GetSerializer(fileSystem);
    }

    [NotNull]
    public IFileSystem FileSystem { get; }

    [NotNull]
    public JsonSerializer InnerSerializer { get; }

    public string Serialize(object obj)
    {
      return JsonConvert.SerializeObject(obj, GetConverters(FileSystem)).IsNotNull();
    }

    public void Serialize(TextWriter writer, object obj)
    {
      Assert.ArgumentNotNull(writer, nameof(writer));

      InnerSerializer.Serialize(writer, obj);
    }
                   
    [NotNull]
    public T Deserialize<T>(StreamReader reader)
    {
      Assert.ArgumentNotNull(reader, nameof(reader));

      return (T)InnerSerializer.Deserialize(reader, typeof(T)).IsNotNull();
    }

    public T DeserializeObject<T>(string text)
    {
      return JsonConvert.DeserializeObject<T>(text, GetConverters(FileSystem));
    }                                        

    [NotNull]
    private static JsonConverter[] GetConverters([NotNull] IFileSystem fileSystem)
    {
      Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));

      return new JsonConverter[]
      {
        new FileConverter(fileSystem),
        new FolderConverter(fileSystem),
        new SqlServerConnectionStringConverter(), 
      };
    }

    [NotNull]
    private static JsonSerializer GetSerializer([NotNull] IFileSystem fileSystem)
    {
      Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));

      var serializer = new JsonSerializer
      {
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.Indented
      };

      var converters = serializer.Converters;
      Assert.IsNotNull(converters, nameof(converters));

      foreach (var converter in GetConverters(fileSystem))
      {
        converters.Add(converter);
      }

      return serializer;
    }
  }
}
