namespace SIM.Profiles
{
  using System.IO;
  using SIM.Extensions;
  using SIM.IO;
  using SIM.Serialization;
  using Xunit;

  public class ProfileProviderTests
  {
    [Fact]
    public void ProfileProvider_Read_MissingFile()
    {       
      // arrange                                                              
      var fs = new MockFileSystem();                                          
      var file = fs.ParseFile("C:\\1", () => { throw new FileNotFoundException("Could not find file \'C:\\1\'."); });

      var sut = new ProfileProvider(file);

      // act & assert
      Assert.Throws<FileNotFoundException>(
        () => sut.Read());          
    }

    [Fact]
    public void ProfileProvider_Read_DefaultLocation()
    {
      // arrange  
      var profile = new
      {
        DefaultLocation = "D:\\Some\\Folder"
      };

      var fs = new MockFileSystem();
      var serializer = new Serializer(fs);
      var json = serializer.Serialize(profile).IsNotNull();
      var file = fs.ParseFile("C:\\1", json);

      var sut = new ProfileProvider(file);

      // act
      var actual = sut.Read();

      // assert
      Assert.StrictEqual("D:\\Some\\Folder", actual.DefaultLocation?.FullName);
      Assert.StrictEqual(null, actual.License);
    }

    [Fact]
    public void ProfileProvider_Read_LicenseFile()
    {
      // arrange                
      var profile = new
      {
        License = "D:\\Some\\File.lic"
      };

      var fs = new MockFileSystem();
      var serializer = new Serializer(fs);
      var json = serializer.Serialize(profile).IsNotNull();
      var file = fs.ParseFile("C:\\1", json);

      var sut = new ProfileProvider(file);

      // act
      var actual = sut.Read();

      // assert
      Assert.StrictEqual("D:\\Some\\File.lic", actual.License?.FullName);
      Assert.StrictEqual(null, actual.DefaultLocation);
    }

    [Fact]
    public void ProfileProvider_Read()
    { 
      var fs = new MockFileSystem();
      var serializer = new Serializer(fs);

      // arrange
      var profile = new
      {
        DefaultLocation = "D:\\Some\\Folder",
        License = "D:\\Some\\File.lic"
      };

      var json = serializer.Serialize(profile).IsNotNull();
      var file = fs.ParseFile("C:\\1", json);

      var sut = new ProfileProvider(file);

      // act
      var actual = sut.Read();

      // assert
      Assert.StrictEqual("D:\\Some\\Folder", actual.DefaultLocation?.FullName);
      Assert.StrictEqual("D:\\Some\\File.lic", actual.License?.FullName);
    }

    [Fact]
    public void ProfileProvider_Save()
    {
      // arrange
      var fs = new MockFileSystem();
      var file = fs.ParseFile("C:\\1");
      var sut = new ProfileProvider(file);
      var profile = new Profile
      {
        DefaultLocation = fs.ParseFolder("D:\\Some\\Folder"),
        License = fs.ParseFile("D:\\Some\\File.lic")
      };

      // act
      sut.Save(profile);

      // assert              
      var text = new StreamReader(file.OpenRead()).ActAndDispose(x => x.ReadToEnd());
      SIM.Assert.IsNotNull(text, nameof(text));

      var actual = new Serializer(fs).DeserializeObject<Profile>(text);

      Assert.Equal("D:\\Some\\Folder", actual.DefaultLocation?.FullName);
      Assert.Equal("D:\\Some\\File.lic", actual.License?.FullName);
    }
  }    
}