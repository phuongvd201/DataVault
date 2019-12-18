using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using DataVault.Common.Extensions;

namespace DataVault.Common.Enumeration
{
    public abstract class Enumeration<TEnum> : Enumeration<TEnum, string>, IEnumeration
        where TEnum : Enumeration<TEnum, string>
    {
        protected Enumeration(string code, string name) :
            base(code, name)
        {
        }
    }

    public abstract class Enumeration<TEnum, TValue> :
        IEquatable<Enumeration<TEnum, TValue>>,
        IComparable<Enumeration<TEnum, TValue>>
        where TEnum : Enumeration<TEnum, TValue>
        where TValue : IEquatable<TValue>, IComparable<TValue>
    {
        static readonly Lazy<Dictionary<string, TEnum>> _fromCode =
            new Lazy<Dictionary<string, TEnum>>(() => GetAllOptions().ToDictionary(item => item.Code));

        static readonly Lazy<Dictionary<string, TEnum>> _fromCodeIgnoreCase =
            new Lazy<Dictionary<string, TEnum>>(() => GetAllOptions().ToDictionary(item => item.Code, StringComparer.OrdinalIgnoreCase));

        private static IEnumerable<TEnum> GetAllOptions()
        {
            Type baseType = typeof(TEnum);
            IEnumerable<Type> enumTypes = Assembly.GetAssembly(baseType).GetTypes().Where(t => baseType.IsAssignableFrom(t));

            List<TEnum> options = new List<TEnum>();
            foreach (Type enumType in enumTypes)
            {
                List<TEnum> typeEnumOptions = enumType.GetFieldsOfType<TEnum>();
                options.AddRange(typeEnumOptions);
            }

            return options.OrderBy(t => t.Code).ToList();
        }

        public static TEnum[] List => _fromCode.Value.Values.ToArray();

        public string Code { get; }

        public TValue Name { get; }

        protected Enumeration(string code, TValue name)
        {
            Code = code;
            Name = name;
        }

        public static TEnum GetOrNull(string code, bool ignoreCase = false)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            return FromCode(ignoreCase ? _fromCodeIgnoreCase.Value : _fromCode.Value);

            TEnum FromCode(Dictionary<string, TEnum> dictionary)
            {
                if (!dictionary.TryGetValue(code, out var result))
                {
                    return null;
                }

                return result;
            }
        }

        public static TEnum Get(string code, bool ignoreCase = false)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException($"Error when convert value {code} to {typeof(TEnum)}");
            }

            return FromCode(ignoreCase ? _fromCodeIgnoreCase.Value : _fromCode.Value);

            TEnum FromCode(Dictionary<string, TEnum> dictionary)
            {
                if (!dictionary.TryGetValue(code, out var result))
                {
                    throw new ArgumentException($"Error when convert value '{code}' to {typeof(TEnum).Name}");
                }

                return result;
            }
        }

        public override string ToString()
        {
            return Code;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() =>
            Code.GetHashCode();

        public override bool Equals(object obj) =>
            (obj is Enumeration<TEnum, TValue> other) && Equals(other);

        public virtual bool Equals(Enumeration<TEnum, TValue> other)
        {
            // check if same instance
            if (ReferenceEquals(this, other))
                return true;

            // it's not same instance so 
            // check if it's not null and is same value
            if (other is null)
                return false;

            return Code.Equals(other.Code);
        }

        public static bool operator ==(Enumeration<TEnum, TValue> left, Enumeration<TEnum, TValue> right)
        {
            // Handle null on left side
            if (left is null)
                return right is null; // null == null = true

            // Equals handles null on right side
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Enumeration<TEnum, TValue> left, Enumeration<TEnum, TValue> right) =>
            !(left == right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual int CompareTo(Enumeration<TEnum, TValue> other) =>
            string.Compare(Code, other.Code, StringComparison.Ordinal);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Enumeration<TEnum, TValue> left, Enumeration<TEnum, TValue> right) =>
            left.CompareTo(right) < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Enumeration<TEnum, TValue> left, Enumeration<TEnum, TValue> right) =>
            left.CompareTo(right) <= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Enumeration<TEnum, TValue> left, Enumeration<TEnum, TValue> right) =>
            left.CompareTo(right) > 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Enumeration<TEnum, TValue> left, Enumeration<TEnum, TValue> right) =>
            left.CompareTo(right) >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator string(Enumeration<TEnum, TValue> enumeration) =>
            enumeration.Code;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Enumeration<TEnum, TValue>(string code) =>
            Get(code);
    }
}