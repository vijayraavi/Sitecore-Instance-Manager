using System;

namespace SIM.Common
{
  using JetBrains.Annotations;

  public interface ICommandResult
  {
    [CanBeNull]
    bool? Success { get; set; }

    [CanBeNull]
    string Message { get; set; }

    TimeSpan Elapsed { get; set; }

    [CanBeNull]
    CustomException Error { get; set; }
  }

  public interface ICommandResult<T> : ICommandResult
  {                                        
    [CanBeNull]
    T Data { get; set; }
  }
}