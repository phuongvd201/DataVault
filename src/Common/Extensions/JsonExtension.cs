using System.Collections.Generic;
using System.Linq;

using DataVault.Common.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataVault.Common.Extensions
{
    public static class JsonExtension
    {
        public static string GetByKey(this string jsonData, string key)
        {
            JArray array = JArray.Parse(jsonData);
            foreach (JObject content in array.Children<JObject>())
            {
                var jPropertyList = content.Properties().Select(x => new KeyValuePair<string, string>(x.Name, x.Value.ToString()));
                var jdata = jPropertyList.Where(t => t.Value.Equals(key));
                if (jdata.Count() > 0)
                {
                    return JToken.Parse(jPropertyList.Select(t => t.Value).ToArray().ElementAt(1)).First().ToString();
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Get list of json names of properties wich have value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<string> GetJsonFieldNames<T>(T obj) where T: class
        {
            var jsonNames = obj == null
                ? new List<string>()
                : JsonConvert.SerializeObject(obj).DeserializeAndFlatten().Keys.ToList();

            return jsonNames;
        }
    }
}