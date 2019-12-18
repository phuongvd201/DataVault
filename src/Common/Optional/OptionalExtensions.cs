namespace DataVault.Common.Optional
{
    public static class OptionalExtensions
    {
        public static T GetValueOrDefault<T>(this Optional<T> value)
        {
            return value.HasValue ? value.Value : default;
        }

        public static Optional<T> ToOptional<T>(this T value)

        {
            return new Optional<T>(value);
        }
    }
}