using System;

namespace DataVault
{
    public class DataUpdateException : Exception
    {
        public DataUpdateException()
        {
        }

        public DataUpdateException(string message)
            : base(message)
        {
        }

        public DataUpdateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}