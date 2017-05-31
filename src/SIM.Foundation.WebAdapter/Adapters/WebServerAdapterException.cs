namespace SIM.Adapters
{
  using System;
  using System.Net.Http;
  using JetBrains.Annotations;

  public class WebServerAdapterException : Exception
  {
    [CanBeNull]
    public HttpResponseMessage Response { get; }

    public WebServerAdapterException([NotNull] Exception ex, [CanBeNull] string message = null, [CanBeNull] HttpResponseMessage response = null)
      : base($"Failed to perform an operation with IIS. {message}".TrimEnd(" .".ToCharArray()), ex)
    {
      Response = response;
    }

    public WebServerAdapterException([CanBeNull] string message = null, [CanBeNull] HttpResponseMessage response = null)
      : base($"Failed to perform an operation with IIS. {message}".TrimEnd(" .".ToCharArray()))
    {
      Response = response;
    }
  }
}