namespace AspNet.Module.Dal.EfCore.Audit.Models;

public class SaveChangesAudit
{
    public Guid AuditId { get; set; }
    public DateTime EndTime { get; set; }

    public ICollection<AuditEntry> Entries { get; } = new List<AuditEntry>();
    public string? ErrorMessage { get; set; }
    public DateTime StartTime { get; set; }
    public bool Succeeded { get; set; }
}