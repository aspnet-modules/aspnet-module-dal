using AspNet.Module.Dal.EfCore.Audit.Elk.Options;
using AspNet.Module.Dal.EfCore.Audit.Models;
using AspNet.Module.Dal.EfCore.Clock;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;

namespace AspNet.Module.Dal.EfCore.Audit.Elk.Services;

public class ElkAuditService
{
    private IElasticClient? _elasticClient;

    public ElkAuditService(IOptions<AuditOptions> options, ILogger<ElkAuditService> logger)
    {
        Logger = logger;
        Options = options.Value;
    }

    private IElasticClient ElasticClient => _elasticClient ??= CreateElasticClient();
    private ILogger<ElkAuditService> Logger { get; }

    private AuditOptions Options { get; }

    public void AddAudit(SaveChangesAudit audit)
    {
        var bulkResponse = ElasticClient.Bulk(b =>
        {
            b = b.IndexMany(audit.Entries.ToList());
            b = b.TypeQueryString(nameof(AuditEntry));
            return b;
        });
        if (!bulkResponse.IsValid)
        {
            Logger.LogDebug(bulkResponse.OriginalException, bulkResponse.DebugInformation);
        }
    }

    public async Task AddAuditAsync(SaveChangesAudit audit, CancellationToken ct)
    {
        var bulkResponse = await ElasticClient.BulkAsync(b =>
        {
            b = b.IndexMany(audit.Entries.ToList());
            b = b.TypeQueryString(nameof(AuditEntry));
            return b;
        }, ct);
        if (!bulkResponse.IsValid)
        {
            Logger.LogDebug(bulkResponse.OriginalException, bulkResponse.DebugInformation);
        }
    }

    private IElasticClient CreateElasticClient()
    {
        var index = $"{Options.Elk.Index}-{DatabaseClock.UtcNow.Year}";
        var settings = new ConnectionSettings(Options.Elk.Url).DefaultIndex(index);
        var elasticClient = new ElasticClient(settings);
        elasticClient.Map<AuditEntry>(a => a
            .AutoMap()
            .Properties(ps =>
                ps.Text(x => x.Name(y => y.EntityId))
                    .Text(x => x.Name(y => y.EntityType))
                    .Text(x => x.Name(y => y.EntityOperation))
                    .Text(x => x.Name(y => y.UserId))
                    .Text(x => x.Name(y => y.UserName))
                    .Text(x => x.Name(y => y.RemoteIpAddress))
                    .Text(x => x.Name(y => y.Message))
                    .Date(x => x.Name(y => y.AuditedAt))));
        return elasticClient;
    }
}