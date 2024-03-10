# Модуль доступа к данным DAL (Data Access Layer)

Модуль организует доступ к БД на уровне репозитория.

```sh
# для доступа к IRepository
dotnet add AspNet.Module.Dal.EfCore.Abstractions

# для регистрации модуля в Host
dotnet add AspNet.Module.Dal.EfCore
```

## Инициализация DbContext

```cs
using AspNet.Module.Dal.EfCore;

public class AppDbContext : BaseDbContext
{
}
```

## Регистрация модуля в Program

Добавляем в Host проект nuget пакет `AspNet.Module.Dal.EfCore`.

```cs
using AspNet.Module.Dal.EfCore;

var builder = AspNetWebApplication.CreateBuilder(args);
builder.RegisterModule(new EfCoreModule<AppDbContext>(() => Clock.Now));
```

> В конструкторе модуля EfCoreModule можно указать функцию получения текущего времени. Если не указан, то берем
> DateTime.UtcNow.

## Дополнительная конфигурация

В переменных окружения можно указать след параметры

* DbContext
    * DetailedErrors - показывать детали ошибок с параметрами

```json
{
  "DbContext": {
    "DetailedErrors": false
  }
}
```