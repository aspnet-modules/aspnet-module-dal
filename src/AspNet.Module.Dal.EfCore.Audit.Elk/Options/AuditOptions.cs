using System.ComponentModel.DataAnnotations;

namespace AspNet.Module.Dal.EfCore.Audit.Elk.Options;

/// <summary>
///     Настройки аудита
/// </summary>
public class AuditOptions
{
    /// <summary>
    ///     Настройки Elk
    /// </summary>
    [Required]
    public AuditElkOptions Elk { get; internal set; } = null!;

    /// <summary>
    ///     Флаг включения аудита
    /// </summary>
    public bool Enabled { get; internal set; } = true;

    /// <summary>
    ///     Название секции конфигурации
    /// </summary>
    public static string Key => "Audit";
}