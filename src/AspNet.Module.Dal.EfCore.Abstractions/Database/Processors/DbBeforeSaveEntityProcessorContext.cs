namespace AspNet.Module.Dal.EfCore.Database.Processors;

/// <summary>
///     Контекст процессора перед сохранением в БД
/// </summary>
public class DbBeforeSaveEntityProcessorContext
{
    /// <summary>
    ///     Создание объекта
    /// </summary>
    public DbBeforeSaveEntityProcessorContext()
    {
        Data = new Dictionary<string, object>();
    }

    /// <summary>
    ///     Параметры
    /// </summary>
    public IDictionary<string, object> Data { get; }
}