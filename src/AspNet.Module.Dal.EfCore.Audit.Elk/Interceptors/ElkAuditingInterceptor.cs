using System.Diagnostics.CodeAnalysis;
using AspNet.Module.Dal.EfCore.Audit.Elk.Services;
using AspNet.Module.Dal.EfCore.Audit.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AspNet.Module.Dal.EfCore.Audit.Elk.Interceptors;

[SuppressMessage("ReSharper", "TemplateIsNotCompileTimeConstantProblem")]
public class ElkAuditingInterceptor : BaseAuditingInterceptor
{
    public ElkAuditingInterceptor(ILogger<ElkAuditingInterceptor> logger, IHttpContextAccessor httpContextAccessor,
        ElkAuditService auditService) : base(logger, httpContextAccessor)
    {
        AuditService = auditService;
    }

    private ElkAuditService AuditService { get; }

    protected override void CommitAudit(SaveChangesAudit audit) => AuditService.AddAudit(audit);

    protected override Task CommitAuditAsync(SaveChangesAudit audit, CancellationToken ct) =>
        AuditService.AddAuditAsync(audit, ct);
}