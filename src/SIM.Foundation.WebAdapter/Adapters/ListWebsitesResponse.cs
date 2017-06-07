namespace SIM.Adapters
{
  using JetBrains.Annotations;

  public sealed class ListWebsitesResponse
  {
    [CanBeNull]
    public WebsiteInfo[] Websites { get; set; }
  }
}