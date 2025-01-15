using System.Security.Claims;
using AspNet.Module.Dal.EfCore.Audit.Models;
using AspNet.Module.Dal.EfCore.Clock;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace AspNet.Module.Dal.EfCore.Audit;

public abstract class BaseAuditingInterceptor(
    ILogger<BaseAuditingInterceptor> logger,
    IHttpContextAccessor httpContextAccessor)
    : ISaveChangesInterceptor
{
    private SaveChangesAudit? _audit;

    private bool CanAudit => HttpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

    private IHttpContextAccessor HttpContextAccessor { get; } = httpContextAccessor;
    private ILogger<BaseAuditingInterceptor> Logger { get; } = logger;

    public void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        if (_audit != null)
        {
            _audit.Succeeded = false;
            _audit.EndTime = DatabaseClock.UtcNow;
            _audit.ErrorMessage = eventData.Exception.Message;
        }

        Logger.LogDebug(eventData.Exception, eventData.Exception.Message);
    }

    public Task SaveChangesFailedAsync(DbContextErrorEventData eventData,
        CancellationToken cancellationToken)
    {
        if (_audit != null)
        {
            _audit.Succeeded = false;
            _audit.EndTime = DatabaseClock.UtcNow;
            _audit.ErrorMessage = eventData.Exception.Message;
        }

        Logger.LogDebug(eventData.Exception, eventData.Exception.Message);
        return Task.CompletedTask;
    }

    public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        if (_audit != null)
        {
            _audit.Succeeded = true;
            _audit.EndTime = DatabaseClock.UtcNow;
            CommitAudit(_audit);
        }

        return result;
    }

    public async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken)
    {
        if (_audit != null)
        {
            _audit.Succeeded = true;
            _audit.EndTime = DatabaseClock.UtcNow;
            await CommitAuditAsync(_audit, cancellationToken);
        }

        return result;
    }

    public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (CanAudit && eventData.Context != null && HttpContextAccessor.HttpContext != null)
        {
            _audit = CreateAudit(eventData.Context, HttpContextAccessor.HttpContext);
        }

        return result;
    }

    public async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken)
    {
        if (CanAudit && eventData.Context != null && HttpContextAccessor.HttpContext != null)
        {
            _audit = CreateAudit(eventData.Context, HttpContextAccessor.HttpContext);
        }

        return await Task.FromResult(result);
    }

    /// <summary>
    ///     Зафиксировать изменения
    /// </summary>
    protected abstract void CommitAudit(SaveChangesAudit audit);

    /// <summary>
    ///     Зафиксировать изменения
    /// </summary>
    protected abstract Task CommitAuditAsync(SaveChangesAudit audit, CancellationToken ct);

    private static SaveChangesAudit CreateAudit(DbContext context, HttpContext httpContext)
    {
        var now = DatabaseClock.UtcNow;
        var audit = new SaveChangesAudit { AuditId = Guid.NewGuid(), StartTime = now };
        var userId = GetUserId(httpContext.User);
        var userName = httpContext.User.Identity?.Name ?? string.Empty;
        var remoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

        foreach (var entry in context.ChangeTracker.Entries()
                     .Where(x => x.State != EntityState.Unchanged && x.State != EntityState.Detached))
        {
            var auditMessage = entry.State switch
            {
                EntityState.Deleted => $"Deleting {entry.Metadata.DisplayName()}",
                EntityState.Modified => $"Updating {entry.Metadata.DisplayName()}",
                EntityState.Added => $"Inserting {entry.Metadata.DisplayName()}",
                _ => string.Empty
            };

            audit.Entries.Add(new AuditEntry
            {
                EntityType = entry.Entity.GetType().Name,
                EntityId = GetEntityId(entry),
                EntityOperation = entry.State.ToString(),
                UserId = userId,
                UserName = userName,
                RemoteIpAddress = remoteIpAddress,
                Message = auditMessage,
                AuditedAt = now
            });
        }

        return audit;
    }

    private static string GetEntityId(EntityEntry entry) =>
        entry.Properties.FirstOrDefault(x => x.Metadata.IsPrimaryKey())?.CurrentValue?.ToString()
        ?? string.Empty;

    private static string GetUserId(ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Sid) ??
        user.FindFirstValue(ClaimTypes.NameIdentifier) ??
        user.FindFirstValue("sub") ??
        user.FindFirstValue("username") ??
        string.Empty;
}