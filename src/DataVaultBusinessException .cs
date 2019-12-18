using System.Data.Common;

namespace DataVault
{
    public class DataVaultBusinessException : DbException
    {
        public DataVaultBusinessErrorCode DataVaultBusinessErrorCode { get; set; }

        public string CommandLog { get; set; }

        public DataVaultBusinessException()
        {
        }

        public DataVaultBusinessException(string message) : base(message)
        {
        }

        public DataVaultBusinessException(DataVaultBusinessErrorCode businessErrorCode, string message)
            : base(message)
        {
            DataVaultBusinessErrorCode = businessErrorCode;
        }

        public DataVaultBusinessException(DataVaultBusinessErrorCode businessErrorCode, string message, string commandLog)
            : base(message)
        {
            DataVaultBusinessErrorCode = businessErrorCode;
            CommandLog = commandLog;
        }
    }
}