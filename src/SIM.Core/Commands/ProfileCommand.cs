namespace SIM.Commands
{
  using System;
  using System.Data;
  using JetBrains.Annotations;
  using SIM.Abstract.IO;
  using SIM.Common;
  using SIM.Extensions;
  using SIM.Profiles;

  public class ProfileCommand : AbstractCommand<IProfile>
  {
    [NotNull]
    private readonly IProfileProvider ProfileProvider;

    public ProfileCommand([NotNull] IProfileProvider profileProvider)
    {
      Assert.ArgumentNotNull(profileProvider, nameof(profileProvider));

      ProfileProvider = profileProvider;
    }

    public IFolder DefaultLocation { get; [UsedImplicitly] set; }

    public IFile LicenseFile { get; [UsedImplicitly] set; }

    protected override void DoExecute(ICommandResult<IProfile> result)
    {
      Ensure.IsNotNull(result, nameof(result));

      var profile = ProfileProvider.TryRead();
      var isValid = profile != null;
      profile = profile ?? new Profile();
      
      var changes = 0;     
      var defaultLocation = DefaultLocation;
      if (defaultLocation != null)
      {
        profile.DefaultLocation = defaultLocation;
        changes += 1;
      }

      var license = LicenseFile;
      if (license != null)
      {
        profile.License = license;
        changes += 1;
      }                    

      if (changes > 0 || !isValid)
      {
        ProfileProvider.Save(profile);
      }

      try
      {
        result.Data = profile.IsNotNull(nameof(profile));
      }
      catch (Exception ex)
      {
        throw new DataException("Profile file is corrupted", ex);
      }
    }
  }
}