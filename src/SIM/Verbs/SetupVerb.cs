namespace SIM.Verbs
{
  using System;
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Abstract.IO;
  using SIM.Commands;
  using SIM.Profiles;

  public class SetupVerb : ProfileCommand
  {
    [UsedImplicitly]
    public SetupVerb() 
      : this(Default.FileSystem)
    { 
    }

    public SetupVerb([NotNull] IFileSystem fileSystem) : base(new ProfileProvider(fileSystem))
    {
      Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));

      FileSystem = fileSystem;
    }

    [NotNull]
    public IFileSystem FileSystem { get; }

    [NotNull]
    [UsedImplicitly]
    [Option('d', "defaultLocation", Required = true)]
    public string Location
    {
      get
      {
        throw new NotSupportedException();
      }
                        
      set
      {
        Assert.ArgumentNotNullOrEmpty(value, nameof(value));

        // TODO: Customize CommandLineParser to do that automagically via overriding and annotating DefaultLocation
        this.DefaultLocation = FileSystem.ParseFolder(value);
      }
    }

    [NotNull]
    [UsedImplicitly]
    [Option('l', "license", Required = true)]
    public string License
    {
      get
      {                                       
        throw new NotSupportedException();
      }

      set
      {
        Assert.ArgumentNotNullOrEmpty(value, nameof(value));

        base.LicenseFile = FileSystem.ParseFile(value);
      }
    }
  }
}