using System.Reflection;

using Humanizer;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DataVault.Common.Json
{
    public class UnderscoreNamingResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            property.PropertyName = property.PropertyName.Underscore();

            return property;
        }
    }
}