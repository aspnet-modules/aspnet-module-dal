namespace AspNet.Module.Dal.EfCore.Audit.Models;

/// <summary>
///     Аудит объектов в БД
/// </summary>
public class AuditEntry
{
    /// <summary>
    ///     Дата аудита
    /// </summary>
    public DateTime AuditedAt { get; init; }

    /// <summary>
    ///     Первичный ключ объекта
    /// </summary>
    public string EntityId { get; init; } = null!;

    /// <summary>
    ///     Операция над объектом
    /// </summary>
    public string EntityOperation { get; init; } = null!;

    /// <summary>
    ///     Название таблицы
    /// </summary>
    public string EntityType { get; init; } = null!;

    /// <summary>
    ///     Детальное сообщение
    /// </summary>
    public string Message { get; init; } = null!;

    /// <summary>
    ///     IP адрес пользователя, кто инициировал изменение
    /// </summary>
    public string RemoteIpAddress { get; init; } = null!;

    /// <summary>
    ///     Ид пользователя, кто инициировал изменение
    /// </summary>
    public string UserId { get; init; } = null!;

    /// <summary>
    ///     фИО пользователя, кто инициировал изменение
    /// </summary>
    public string UserName { get; init; } = null!;
}