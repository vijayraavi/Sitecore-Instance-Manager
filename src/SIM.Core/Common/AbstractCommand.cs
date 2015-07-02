namespace SIM.Common
{
  using System;
  using System.Diagnostics;
  using JetBrains.Annotations;

  public abstract class AbstractCommand<TResult> : AbstractCommand
  {
    [NotNull]
    public new ICommandResult<TResult> Execute()
    {
      return (ICommandResult<TResult>)base.Execute();
    }

    protected sealed override ICommandResult CreateResult()
    {
      return new CommandResult<TResult>();
    }

    protected sealed override void DoExecute(ICommandResult result)
    {
      DoExecute((ICommandResult<TResult>)result);
    }

    protected abstract void DoExecute([NotNull] ICommandResult<TResult> result);
  }

  public abstract class AbstractCommand : ICommand
  {
    [NotNull]
    public ICommandResult Execute()
    {
      var result = CreateResult();
      var timer = new Stopwatch();
      timer.Start();
      try
      {
        try
        {
          DoExecute(result);
        }
        finally
        {
          timer.Stop();
        }

        result.Success = result.Success ?? true;
      }
      catch (MessageException ex)
      {
        result.Message = ex.Message;
        result.Success = false;
      }
      catch (Exception ex)
      {
        // TODO: Replace with new logging engine; Log.Error(ex, $"{GetType().Name} command has failed with unhandled exception");
        result.Success = false;
        result.Error = new CustomException(ex);
      }

      result.Elapsed = timer.Elapsed;

      return result;
    }

    [NotNull]
    protected virtual ICommandResult CreateResult()
    {
      return new CommandResult();
    }

    protected abstract void DoExecute([NotNull] ICommandResult result);
  }
}