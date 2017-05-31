namespace SIM.Adapters
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Threading.Tasks;
  using JetBrains.Annotations;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using SIM.Base.FileSystem;
  using SIM.Base.Services;

  [TestClass]
  public class WebServerAdapter_Tests
  {
    private const string DefaultEnvWebServerPath = @"C:\Sitecore\etc\sim2\env\default\WebServer.txt";

    [NotNull]
    private WebServerAdapter Adapter { get; } = new WebServerAdapter(new WebServerConnectionString(File.ReadAllText(DefaultEnvWebServerPath)));

    public WebServerAdapter_Tests()
    {
      ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
    }

    [TestMethod]
    public async Task DeleteWebSite_MissingWebSite()
    {
      try
      {
        await Adapter.DeleteWebSite(GetRandomWebSiteName());
      }
      catch (AggregateException aex)
      {
        var iex = aex.InnerException;
        Assert.IsNotNull(iex);

        throw iex;
      }
    }

    [TestMethod]
    public async Task GetWebSiteFilePath_MissingWebSite()
    {
      var websiteName = GetRandomWebSiteName();

      try
      {
        await Adapter.GetWebSiteRootDirectoryPath(websiteName);
      }
      catch (WebSiteDoesNotExistException ex)
      {
        Assert.AreEqual(websiteName, ex.WebSiteName);
        Assert.AreEqual($"Failed to perform an operation with IIS. The requested '{websiteName}' website does not exist", ex.Message);

        return;
      }

      Assert.Fail();
    }

    [TestMethod]
    public async Task Deploy_Check_Delete_Check()
    {
      try
      {
        var websiteName = GetRandomWebSiteName();
        var websiteRootDirectoryPath = new FilePath("C:\\inetpub\\wwwroot");

        int count;
        try
        {
          await Adapter.CreateWebSite(websiteName, websiteRootDirectoryPath);

          var exists = await Adapter.WebSiteExists(websiteName);
          Assert.AreEqual(true, exists);

          var filePath = await Adapter.GetWebSiteRootDirectoryPath(websiteName);
          Assert.IsNotNull(filePath);
          Assert.AreEqual(websiteRootDirectoryPath.FullName, filePath.FullName);

          var websites = await Adapter.GetWebSites();
          Assert.AreEqual(true, websites.Contains(websiteName));

          count = websites.Count;
          Assert.AreEqual(true, count >= 1);
        }
        finally
        {
          await Adapter.DeleteWebSite(websiteName);
        }

        var existsAfter = await Adapter.WebSiteExists(websiteName);
        Assert.AreEqual(false, existsAfter);

        var websitesAfter = await Adapter.GetWebSites();
        Assert.IsNotNull(websitesAfter);

        var countAfter = websitesAfter.Count;
        Assert.AreEqual(count - 1, countAfter);
      }
      catch (AggregateException aex)
      {
        var iex = aex.InnerException;
        Assert.IsNotNull(iex);

        throw iex;
      }
    }

    [NotNull]
    private static string GetRandomWebSiteName()
    {
      return Guid.NewGuid().ToString("N");
    }
  }
}
