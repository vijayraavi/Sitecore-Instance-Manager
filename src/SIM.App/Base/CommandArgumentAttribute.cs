namespace SIM.Base
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// Defines command argument name, description and instructs the <see cref="T:Newtonsoft.Json.JsonSerializer" /> to always serialize the member with the specified name.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class CommandArgumentAttribute : Attribute
  {
    [NotNull]
    public string Name { get; }

    [NotNull]
    public string Description { get; }

    public CommandArgumentAttribute([NotNull] string name, [NotNull] string description)
    {
      Name = name;
      Description = description;
    }
  }
}