namespace SIM.Verbs
{
  using JetBrains.Annotations;
  using SIM.Abstract.IO;
  using SIM.Commands;
  using SIM.Profiles;

  public class ProfileVerb : ProfileCommand
  {
    [UsedImplicitly]
    public ProfileVerb() : base(Default.ProfileProvider)
    {
    }

    public ProfileVerb([NotNull] IFileSystem fileSystem) : base(new ProfileProvider(fileSystem))
    {
      Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));
    }                                        
  }
}