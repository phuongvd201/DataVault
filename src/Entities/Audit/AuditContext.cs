namespace DataVault.Entities.Audit
{
    public class AuditContext : IAuditContext
    {
        public AuditInfo AuditInfo { get; set; } = new AuditInfo();
    }
}