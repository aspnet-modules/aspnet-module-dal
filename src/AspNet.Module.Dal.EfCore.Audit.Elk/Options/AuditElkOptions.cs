using System.ComponentModel.DataAnnotations;

namespace AspNet.Module.Dal.EfCore.Audit.Elk.Options;

/// <summary>
///     Найтроки ELK для аудита
/// </summary>
public class AuditElkOptions
{
    /// <summary>
    ///     Индекс аудита
    /// </summary>
    [Required]
    public string Index { get; internal set; } = null!;

    /// <summary>
    ///     Адрес сервера. В основном с портом 9200
    /// </summary>
    [Required]
    public Uri Url { get; internal set; } = null!;
}