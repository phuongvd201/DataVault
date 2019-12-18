using System;
using System.Data.Common;

using DataVault.Common.Extensions;

namespace DataVault.Internal
{
    internal class DataCallback
    {
        public Type Type { get; set; }

        public Delegate Callback { get; set; }

        public DbParameter Parameter { get; set; }

        public void Invoke()
        {
            var value = Parameter.Value;
            if (value == DBNull.Value)
                value = Type.Default();

            Callback.DynamicInvoke(value);
        }
    }
}