namespace DataVault.Common.Optional
{
    public interface IOptional
    {
        bool HasValue { get; }

        object RawValue { get; } // must NOT throw InvalidOperationException
    }
}