using System;

using DataVault.Common.Extensions;

using Newtonsoft.Json;

namespace DataVault.Common.Enumeration
{
    public class EnumerationConverter<TEnum> : EnumerationConverter<TEnum, string>
        where TEnum : Enumeration<TEnum, string>
    {
    }

    public class EnumerationConverter<TEnum, TValue> : JsonConverter<TEnum>
        where TEnum : Enumeration<TEnum, TValue>
        where TValue : IEquatable<TValue>, IComparable<TValue>
    {
        public override bool CanRead => true;

        public override bool CanWrite => true;

        public override TEnum ReadJson(JsonReader reader, Type objectType, TEnum existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader == null || string.IsNullOrWhiteSpace(reader.Value?.ToString()))
            {
                return null;
            }

            return GetFromCode((string)reader.Value);

            TEnum GetFromCode(string code)
            {
                try
                {
                    return Enumeration<TEnum, TValue>.Get(code);
                }
                catch (Exception ex)
                {
                    var validValues = Enumeration<TEnum, TValue>.List.ConvertArray(x => x.Code).JoinNotEmpty(", ");

                    //throw new InvalidLookupException(typeof(TEnum), code, Enumeration<TEnum, TValue>.List.ConvertArray(x => x.Code), ex);
                    var message = $"Invalid lookup value. Lookup type: {typeof(TEnum).Name.ToUnderscoreCase().ToUpper()}. Value: {code}. Expected: {validValues}.";
                    throw new JsonSerializationException(message, ex);
                }
            }
        }

        public override void WriteJson(JsonWriter writer, TEnum value, JsonSerializer serializer)
        {
            if (value is null)
                writer.WriteNull();
            else
                writer.WriteValue(value.Code);
        }
    }
}