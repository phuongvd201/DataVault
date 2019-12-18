using System;

using Newtonsoft.Json;

namespace DataVault.Common.Optional
{
    // used internally to make serialization more convenient, do NOT change this, do NOT implement this yourself

    [JsonConverter(typeof(OptionalConverter))]
    public struct Optional<T> : IEquatable<Optional<T>>, IEquatable<T>, IOptional
    {
        public bool HasValue { get; }

        //public T Value => HasValue ? _val : throw new InvalidOperationException("Value is not set.");

        public T Value
        {
            set => _val = value;
            get => _val;
        }

        object IOptional.RawValue => _val;

        //private readonly T _val;
        private T _val;

        public Optional(T value)
        {
            _val = value;
            HasValue = true;
        }

        public static Optional<T> Undefined => new Optional<T>();

        public override string ToString()
        {
            return $"{(HasValue ? Value?.ToString() : "<undefined>")}";
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case T t:
                    return Equals(t);
                case Optional<T> opt:
                    return Equals(opt);
                default:
                    return false;
            }
        }

        public bool Equals(Optional<T> e)
        {
            if (!HasValue && !e.HasValue)
                return true;

            return HasValue == e.HasValue && Value.Equals(e.Value);
        }

        public bool Equals(T other)
        {
            return HasValue && Value.Equals(other);
        }

        public override int GetHashCode()
            => HasValue ? Value.GetHashCode() : 0;

        public static implicit operator Optional<T>(T val)
            => new Optional<T>(val);

        public static explicit operator T(Optional<T> opt)
            => opt.Value;

        public static bool operator ==(Optional<T> opt1, Optional<T> opt2)
            => opt1.Equals(opt2);

        public static bool operator !=(Optional<T> opt1, Optional<T> opt2)
            => !opt1.Equals(opt2);

        public static bool operator ==(Optional<T> opt, T t)
            => opt.Equals(t);

        public static bool operator !=(Optional<T> opt, T t)
            => !opt.Equals(t);

        public TOut IfPresent<TOut>(Func<T, TOut> innerProperty, TOut otherwise)
        {
            return HasValue ? innerProperty(Value) : otherwise;
        }

        public TOut IfPresent<TOut>(Func<T, TOut> innerProperty)
        {
            return HasValue && Value != null ? innerProperty(Value) : default;
        }
    }
}