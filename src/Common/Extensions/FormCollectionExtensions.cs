using System;
using System.Linq;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

namespace DataVault.Common.Extensions
{
    public static class FormCollectionExtensions
    {
        public static T As<T>(this FormCollection responseData) where T : class
        {
            var instance = Activator.CreateInstance<T>();

            var properties = typeof(T).GetProperties().Where(x => Attribute.IsDefined(x, typeof(JsonPropertyAttribute)));

            foreach (var propertyInfo in properties)
            {
                var jsonPropertyAttributes = propertyInfo.GetCustomAttributes(typeof(JsonPropertyAttribute), false);

                var jsonPropertyName = ((JsonPropertyAttribute)jsonPropertyAttributes.FirstOrDefault())?.PropertyName;

                var value = responseData.ContainsKey(jsonPropertyName) ? responseData[jsonPropertyName].ToString() : string.Empty;

                propertyInfo.SetValue(instance, value);
            }

            return instance;
        }
    }
}