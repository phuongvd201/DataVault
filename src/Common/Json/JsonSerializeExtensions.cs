using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DataVault.Common.Extensions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataVault.Common.Json
{
    public static class JsonSerializeExtensions
    {
        public static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new DateTimeJsonConverter(),
            },
        };

        public static string ToJsonString(this object obj)
        {
            return ToJsonString(obj, DefaultJsonSerializerSettings);
        }

        public static string ToJsonString(this object obj, JsonSerializerSettings settings)
        {
            return obj != null
                ? JsonConvert.SerializeObject(obj, settings ?? DefaultJsonSerializerSettings)
                : default;
        }

        public static T FromJsonString<T>(this string value)
        {
            return FromJsonString<T>(value, DefaultJsonSerializerSettings);
        }

        public static async Task<T> ParseFromJsonPathAsync<T>(this Task<string> value, string jsonPath)
        {
            var json = await value.ConfigureAwait(false);

            return ParseFromJsonPath<T>(json, jsonPath);
        }

        public static T ParseFromJsonPath<T>(this string value, string jsonPath)
        {
            var jObject = JObject.Parse(value);

            var jToken = jObject.SelectToken(jsonPath);
            if (jToken.HasValues)
            {
                return jToken.ToObject<T>();
            }

            return default;
        }

        public static T FromJsonString<T>(this string value, JsonSerializerSettings settings)
        {
            return value != null
                ? JsonConvert.DeserializeObject<T>(value, settings ?? DefaultJsonSerializerSettings)
                : default;
        }

        public static bool ValidJson(this string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return false;
            }

            value = value.Trim();
            if ((!value.StartsWith("{", StringComparison.Ordinal) || !value.EndsWith("}", StringComparison.Ordinal))
                && (!value.StartsWith("[", StringComparison.Ordinal) || !value.EndsWith("]", StringComparison.Ordinal)))
            {
                return false;
            }

            try
            {
                JToken.Parse(value);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
            catch (Exception) //some other exception
            {
                return false;
            }
        }

        public static bool HasJsonValue(this string value)
        {
            try
            {
                var jToken = JToken.Parse(value);

                return jToken != null && jToken.HasValues;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsNullOrEmpty(this JToken token)
        {
            return token == null ||
                   token.Type == JTokenType.Array && !token.HasValues ||
                   token.Type == JTokenType.Object && !token.HasValues ||
                   token.Type == JTokenType.String && token.ToString() == string.Empty ||
                   token.Type == JTokenType.Null;
        }
    }
}