namespace DataVault
{
    internal class DataVaultResult
    {
        /// <summary>
        /// ERROR: MySQL Error Code
        /// SUCCESS:  0
        /// </summary>
        public DataVaultBusinessErrorCode StatusCode { get; internal set; }

        /// <summary>
        /// ERROR: NULL or Error Message
        /// E.g. {"MSG": "Error Message Here"}
        /// SUCCESS: Data result
        /// </summary>
        public string JsonResult { get; internal set; }

        public string CommandDetails { get; set; }
    }
}