using System;
using System.Globalization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DataVault.Common.Json
{
    /// <summary>
    /// Custom converter for Json.Net to handle multiple datetime string format.
    /// </summary>
    public class DateTimeJsonConverter : DateTimeConverterBase
    {
        public static string[] DefaultInputFormats =
        {
            "yyyyMMdd", // API datetime query format.
            "yyyy/MM/dd",
            "dd/MM/yyyy",
            "dd-MM-yyyy",
            "yyyyMMddHHmmss",
            "yyyy-MM-dd HH:mm:ss.FFFFFF", // MYSQL JSON string datetime format
        };

        public static string DefaultOutputFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
        public static bool DefaultEvaluateEmptyStringAsNull = true;

        private readonly string[] _inputFormats = DefaultInputFormats;
        private readonly string _outputFormat = DefaultOutputFormat;
        private readonly bool _evaluateEmptyStringAsNull = DefaultEvaluateEmptyStringAsNull;

        public DateTimeJsonConverter()
        {
        }

        public DateTimeJsonConverter(string[] inputFormats, string outputFormat, bool evaluateEmptyStringAsNull = true)
        {
            if (inputFormats != null) _inputFormats = inputFormats;
            if (outputFormat != null) _outputFormat = outputFormat;
            _evaluateEmptyStringAsNull = evaluateEmptyStringAsNull;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string value = reader.Value != null ? reader.Value.ToString() : null;
            try
            {
                // The following line grants Nullable DateTime support. We will return (DateTime?)null if the Json property is null.
                if (string.IsNullOrEmpty(value) && Nullable.GetUnderlyingType(objectType) != null)
                {
                    // If EvaluateEmptyStringAsNull is true an empty string will be treated as null, 
                    // otherwise we'll let DateTime.ParseExact will throw an exception in a couple lines.
                    if (value == null || _evaluateEmptyStringAsNull) return null;
                }

                return DateTime.ParseExact(value, _inputFormats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
            catch (Exception)
            {
                throw new NotSupportedException($"Input value '{value}' is not parseable using the following supported formats: {string.Join(",", _inputFormats)}");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString(_outputFormat));
        }
    }
}