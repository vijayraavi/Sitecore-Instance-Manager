namespace SIM.Base
{
  using JetBrains.Annotations;

  /// <summary>
  /// CommandResult class is made abstract to prevent its usage instead of Success class. 
  /// The main aim is to highlight that result can only be successfull and exceptions must 
  /// be used to deliver unsuccessful, negative result.
  /// </summary>
  public abstract class CommandResult
  {
    /// <summary>
    /// The message that either provides an answer or comments the data provided separately.
    /// </summary>
    [NotNull]
    public string Message { get; set; } = "";

    /// <summary>
    /// Placeholder for output data when it is more complicated than a string.
    /// </summary>
    [CanBeNull]
    public object[] Data { get; set; }
  }
}