namespace AspNet.Module.Dal.EfCore.Clock;

/// <summary>
///     Часы для БД
/// </summary>
public static class DatabaseClock
{
    private static Func<DateTime> _customClockFunc = () => DateTime.UtcNow;

    /// <summary>
    ///     Текущее время
    /// </summary>
    public static DateTime UtcNow => _customClockFunc();

    /// <summary>
    ///     Кастомные часы
    /// </summary>
    public static void CustomClock(Func<DateTime> customClockFunc) => _customClockFunc = customClockFunc;
}