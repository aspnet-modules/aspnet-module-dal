using Microsoft.Extensions.DependencyInjection;

namespace AspNet.Module.Dal.EfCore;

/// <summary>
///     Настройки EfCore -> Factory
/// </summary>
public class FactoryEfCoreConfig : EfCoreConfig
{
    /// <summary>
    ///     Область жизни регистрированной фабрики
    /// </summary>
    public override ServiceLifetime ServiceLifetime { get; init; } =
        ServiceLifetime.Singleton;
}