namespace SIM.Adapters
{
  using System;
  using System.Collections.Generic;
  using System.Net;
  using System.Net.Http;
  using System.Threading.Tasks;
  using JetBrains.Annotations;
  using Newtonsoft.Json;
  using Sitecore.Diagnostics.Base;
  using SIM.Base.FileSystem;
  using SIM.Base.Services;
  using System.Linq;
  using System.Net.Http.Headers;
  using System.Text;

  public sealed class WebServerAdapter
  {
    [NotNull]
    private WebServerConnectionString ConnectionString { get; }

    [NotNull]
    private HttpClient Client { get; }

    public WebServerAdapter([NotNull] WebServerConnectionString connectionString)
    {
      var client = new HttpClient(new HttpClientHandler
      {
        Credentials = new NetworkCredential(connectionString.UserName, connectionString.Password)
      });

      var headers = client.DefaultRequestHeaders;
      Assert.IsNotNull(headers);

      // save token to default headers
      headers.Add("Access-Token", $"Bearer {connectionString.Token}");
      headers.Accept
        .Add(new MediaTypeWithQualityHeaderValue("application/json"));

      ConnectionString = connectionString;
      Client = client;
    }

    /// <summary>
    /// Creates a website with given name and root directory with http websitename:*:80 binding.
    /// </summary>
    [NotNull]
    public async Task CreateWebSite([NotNull] string websiteName, [NotNull] FilePath websiteRootDirectoryPath)
    {
      var siteObject = new
      {
        name = websiteName,
        physical_path = websiteRootDirectoryPath.FullName,
        bindings = new[]
        {
          new
          {
            port = 80,
            ip_address = "*",
            hostname = websiteName,
            protocol = "http"
          }
        }
      };

      HttpResponseMessage response;
      try
      {
        response = await PostAsync("/api/webserver/websites", siteObject);
        Assert.IsNotNull(response);
      }
      catch (Exception ex)
      {
        throw new WebServerAdapterException(ex, "Failed to create a website");
      }

      if (response.StatusCode != HttpStatusCode.Created)
      {
        throw new WebServerAdapterException("Failed to create a website", response);
      }
    }

    /// <summary>
    /// Deletes website with given name if exists, otherwise no exception.
    /// </summary>
    [NotNull]
    public async Task DeleteWebSite([NotNull] string websiteName)
    {
      try
      {
        var website = await DoGetWebsite(websiteName);
        if (website == null)
        {
          return;
        }

        var websiteId = website.Id;
        Assert.IsNotNullOrEmpty(websiteId);

        await DoDeleteWebsiteById(websiteId);
      }
      catch (Exception ex)
      {
        throw new WebServerAdapterException(ex, $"Failed to delete website: {websiteName}");
      }
    }

    [NotNull]
    private async Task DoDeleteWebsiteById([NotNull] string websiteId)
    {
      var res = await DeleteAsync($"/api/webserver/websites/{websiteId}");

      if (res.StatusCode != HttpStatusCode.NoContent)
      {
        throw new WebServerAdapterException($"Failed to delete website with id: {websiteId}", res);
      }
    }

    [NotNull]
    public async Task<FilePath> GetWebSiteRootDirectoryPath([NotNull] string websiteName)
    {
      WebsiteInfo website;
      try
      {
        website = await DoGetWebsite(websiteName);
      }
      catch (Exception ex)
      {
        throw new WebServerAdapterException(ex, $"Failed to get website root directory path: {websiteName}");
      }

      if (website == null)
      {
        throw new WebSiteDoesNotExistException(websiteName);
      }

      return website.RootDirectoryPath;
    }

    [NotNull]
    [ItemCanBeNull]
    private async Task<WebsiteInfo> DoGetWebsite([NotNull] string websiteName)
    {
      var websites = await DoGetWebsites();

      return websites.FirstOrDefault(x => string.Equals(x.Name, websiteName, StringComparison.OrdinalIgnoreCase));
    }

    [NotNull]
    [ItemNotNull]
    private async Task<WebsiteInfo[]> DoGetWebsites()
    {
      var res = await GetAsync("/api/webserver/websites?fields=name,physical_path");
      Assert.IsNotNull(res);

      if (res.StatusCode != HttpStatusCode.OK)
      {
        throw new WebServerAdapterException("Failed to list websites", res);
      }

      var json = await res.Content.ReadAsStringAsync();
      var response = JsonConvert.DeserializeObject<ListWebsitesResponse>(json);
      Assert.IsNotNull(response);

      var websites = response.Websites;
      Assert.IsNotNull(websites);

      return websites;
    }

    [NotNull]
    public async Task<bool> WebSiteExists([NotNull] string websiteName)
    {
      try
      {
        var websites = await GetWebSites();
        var exists = websites.Any(x => string.Equals(x, websiteName, StringComparison.OrdinalIgnoreCase));

        return exists;
      }
      catch (Exception ex)
      {
        throw new WebServerAdapterException(ex);
      }
    }

    [NotNull]
    [ItemNotNull]
    public async Task<IReadOnlyList<string>> GetWebSites()
    {
      try
      {
        var websites = await DoGetWebsites();
        var names = websites
          .Select(x => x.Name)
          .Where(x => !string.IsNullOrEmpty(x))
          .ToArray();

        return names;
      }
      catch (Exception ex)
      {
        throw new WebServerAdapterException(ex);
      }
    }

    [NotNull]
    [ItemNotNull]
    private async Task<HttpResponseMessage> GetAsync([NotNull] string url)
    {
      // ReSharper disable once PossibleNullReferenceException
      var response = await Client.GetAsync(GetAbsoluteUrl(url));
      Assert.IsNotNull(response);

      return response;
    }

    [NotNull]
    [ItemNotNull]
    private async Task<HttpResponseMessage> PostAsync([NotNull] string url, [NotNull] object obj)
    {
      var content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

      // ReSharper disable once PossibleNullReferenceException
      var response = await Client.PostAsync(GetAbsoluteUrl(url), content);
      Assert.IsNotNull(response);

      return response;
    }

    [NotNull]
    [ItemNotNull]
    private async Task<HttpResponseMessage> DeleteAsync([NotNull] string url)
    {
      // ReSharper disable once PossibleNullReferenceException
      var response = await Client.DeleteAsync(GetAbsoluteUrl(url));
      Assert.IsNotNull(response);

      return response;
    }

    [NotNull]
    private string GetAbsoluteUrl([NotNull] string url)
    {
      return $"{ConnectionString.Url}{url}";
    }
  }

  public sealed class ListWebsitesResponse
  {
    [CanBeNull]
    public WebsiteInfo[] Websites { get; set; }
  }

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
