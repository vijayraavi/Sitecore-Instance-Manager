namespace SIM.Adapters
{
  using JetBrains.Annotations;

  public sealed class WebSiteDoesNotExistException : WebServerAdapterException
  {
    [NotNull]
    public string WebSiteName { get; }

    public WebSiteDoesNotExistException([NotNull] string websiteName)
      : base($"The requested '{websiteName}' website does not exist")
    {
      WebSiteName = websiteName;
    }
  }
}