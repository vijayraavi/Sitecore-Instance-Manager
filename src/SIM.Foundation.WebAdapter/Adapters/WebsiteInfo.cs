namespace SIM.Adapters
{
  using System;
  using JetBrains.Annotations;
  using Newtonsoft.Json;
  using SIM.Base.FileSystem;

  public class WebsiteInfo
  {
    [CanBeNull]
    public string Id { get; set; }

    [CanBeNull]
    public string Name { get; set; }

    [CanBeNull]
    [JsonProperty("physical_path")]
    public string PhysicalPath { private get; set; }

    public FilePath RootDirectoryPath => new FilePath(PhysicalPath ?? throw new InvalidOperationException($"{nameof(PhysicalPath)} is not available"));
  }
}