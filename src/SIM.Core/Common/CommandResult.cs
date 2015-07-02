namespace SIM.Common
{
  using System;

  public class CommandResult : ICommandResult
  {
    public bool? Success { get; set; }

    public string Message { get; set; }

    public TimeSpan Elapsed { get; set; }

    public CustomException Error { get; set; }
  }

  public class CommandResult<TResult> : ICommandResult<TResult>
  {
    private TResult _Data;

    public bool? Success { get; set; }

    public string Message { get; set; }

    public CustomException Error { get; set; }

    public TimeSpan Elapsed { get; set; }
    
    public TResult Data
    {
      get
      {
        return _Data;
      }

      set
      {
        Success = true;
        _Data = value;
      }
    }
  }
}