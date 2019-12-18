using Newtonsoft.Json;

namespace DataVault.Entities.Audit
{
    public class AuditInfo
    {
        public AuditInfo()
        {
            AppId = User = Fingerprint = Ip = "UNKNOWN";
        }

        [JsonProperty("MD_APP_ID")]
        public string AppId { get; set; }

        [JsonProperty("MD_USER_ID")]
        public string User { get; set; }

        [JsonProperty("MD_FINGERPRINT")]
        public string Fingerprint { get; set; }

        [JsonProperty("MD_IP")]
        public string Ip { get; set; }
    }
}