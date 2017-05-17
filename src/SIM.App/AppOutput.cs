namespace SIM
{
  using JetBrains.Annotations;
  using SIM.Base;
  
  internal sealed class AppOutput
  {
    public AppOutput()
    {
      ReturnCode = -1;
    }

    public bool Success { get; set; }

    [CanBeNull]
    public string Error { get; set; }

    [CanBeNull]
    public CommandResult Result { get; set; }

    public int ReturnCode { get; set; }
  }
}
