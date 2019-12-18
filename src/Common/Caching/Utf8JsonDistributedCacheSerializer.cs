using System.Text;

using Newtonsoft.Json;

namespace DataVault.Common.Caching
{
    public class Utf8JsonDistributedCacheSerializer : IDistributedCacheSerializer
    {
        public byte[] Serialize<T>(T obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }

        public T Deserialize<T>(byte[] bytes)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        }
    }
}