using Newtonsoft.Json;

namespace DataVault.Entities.Dv
{
    public class LookupItem
    {
        [JsonProperty("CODE")]
        public string Code { get; set; }

        [JsonProperty("NAME")]
        public string Name { get; set; }
    }
}