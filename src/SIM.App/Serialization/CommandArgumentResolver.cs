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
      var name = member.GetCustomAttribute<CommandArgumentAttribute>()?.Name;
      if (!string.IsNullOrEmpty(name))
      {
        property.PropertyName = name;
      }

      return property;
    }
  }
}
