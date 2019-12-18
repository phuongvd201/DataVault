namespace DataVault.Entities.Audit
{
    public interface IAuditContext
    {
        AuditInfo AuditInfo { get; set; }
    }
}