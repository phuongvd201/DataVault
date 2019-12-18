using System;

using DataVault.Common.Extensions;
using DataVault.Entities.Dv;

namespace DataVault.Entities
{
    public class InvalidLookupException : Exception
    {
        public Type LookupType { get; set; }

        public string Value { get; set; }

        public InvalidLookupException(Type entityType, string value, LookupItem[] validValues)
            : this(entityType, value, validValues, null)
        {
        }

        public InvalidLookupException(Type entityType, string value, LookupItem[] validValues, Exception innerException)
            : base($"Invalid lookup value. Lookup type: {entityType.Name.ToUnderscoreCase().ToUpper()}. Value: {value}. Expected: {validValues.ConvertArray(x => x.Code).JoinNotEmpty(", ")}.", innerException)
        {
            LookupType = entityType;
            Value = value;
        }
    }
}