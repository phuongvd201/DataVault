namespace DataVault.Query
{
    public enum QueryOperator
    {
        Equal = 1,
        LessThan = 2,
        LessThanOrEqual = 3,
        GreaterThan = 4,
        GreaterThanOrEqual = 5,
        Like = 8,
        In = 16,
        Null = 32,
        NotEqual = 64
    }
}