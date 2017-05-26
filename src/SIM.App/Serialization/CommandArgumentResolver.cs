namespace SIM.Serialization
{
  using System.Reflection;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Serialization;
  using SIM.Base;

  public class CommandArgumentResolver : DefaultContractResolver
  {
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
      var property = base.CreateProperty(member, memberSerialization);
      var command = member.GetCustomAttribute<CommandArgumentAttribute>();
      if (command == null)
      {
        return null;
      }

      var name = command.Name;
      if (string.IsNullOrEmpty(name))
      {
        return property;
      }

      property.PropertyName = name;
      return property;
    }
  }
}
