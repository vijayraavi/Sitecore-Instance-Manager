namespace SIM.Commands
{
  using NSubstitute;
  using SIM.Abstract.IO;
  using SIM.Extensions;
  using SIM.Profiles;
  using Xunit;

  public class ProfileCommandTests
  {
    [Fact]
    public void Profile_Read_Null()
    {
      var profile = (IProfile)null;

      // arrange
      var provider = Substitute.For<IProfileProvider>().IsNotNull();
      provider.TryRead().Returns(profile);

      var sut = new ProfileCommand(provider);

      // act
      var result = sut.Execute();

      // assert
      Assert.Equal(true, result.Success);
      Assert.NotNull(result.Data);

      provider.Received().TryRead();
      provider.DidNotReceive().Read();
      provider.Received().Save(result.Data);
    }

    [Fact]
    public void Profile_Read_Empty()
    {
      var profile = new Profile();

      // arrange
      var provider = Substitute.For<IProfileProvider>().IsNotNull();
      provider.TryRead().Returns(profile);

      var sut = new ProfileCommand(provider);

      // act
      var result = sut.Execute();
                                
      // assert
      Assert.Equal(true, result.Success);
      Assert.Equal(profile, result.Data);

      provider.Received().TryRead();
      provider.DidNotReceive().Read();
      provider.DidNotReceive().Save(profile);
    }

    [Fact]
    public void Profile_Read_Full()
    {
      var folder = Substitute.For<IFolder>().IsNotNull();
      var file = Substitute.For<IFile>().IsNotNull();
      var profile = new Profile { DefaultLocation = folder, License = file };

      // arrange
      var provider = Substitute.For<IProfileProvider>().IsNotNull();
      provider.TryRead().Returns(profile);

      var sut = new ProfileCommand(provider);
      
      // act
      var result = sut.Execute();  

      // assert
      Assert.Equal(true, result.Success);
      Assert.Equal(profile, result.Data);

      provider.Received().TryRead();
      provider.DidNotReceive().Read();
      provider.DidNotReceive().Save(profile);
    }

    [Fact]
    public void Profile_Execute_Empty_Full()
    {
      var profile = new Profile();
      var folder = Substitute.For<IFolder>().IsNotNull();
      var file = Substitute.For<IFile>().IsNotNull();

      // arrange
      var provider = Substitute.For<IProfileProvider>();
      provider.TryRead().Returns(profile);

      var sut = new ProfileCommand(provider)
      {
        DefaultLocation = folder,
        LicenseFile = file
      };

      var result = sut.Execute();

      // assert
      Assert.Equal(true, result.Success);
      Assert.Equal(profile, result.Data);
      Assert.Equal(folder, result.Data?.DefaultLocation);
      Assert.Equal(file, result.Data?.License);

      provider.Received().TryRead();
      provider.DidNotReceive().Read();
      provider.Received().Save(profile);
    }
  }
}