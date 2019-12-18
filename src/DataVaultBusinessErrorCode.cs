namespace DataVault
{
    public enum DataVaultBusinessErrorCode
    {
        Failed = -1,
        Success = 0,
        EmailAlreadyExists = 101,
        InvalidPrimaryKey = 102,
        InvalidSatellitePayloadFormat = 103,
        InvalidSatelliteData = 104,
        InvalidDateTimeData = 105,
        PayloadExceedFieldMaximumLength = 106,
        BusinessKeyIsRequired = 107,
        NumberOfFkFieldsNotMatchGranularityOfLink = 108,
        NumberOfBkFieldsNotMatchRequiredBkOfHub = 109,
        InvalidTableName = 110,
        InvalidInputValue = 111,
    }
}