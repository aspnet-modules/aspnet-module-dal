# AspNet.Module.Dal

Data access layer packages built on top of Entity Framework Core.

## Installation

```sh
# abstractions for repositories and base EF Core contracts
dotnet add package AspNet.Module.Dal.EfCore.Abstractions

# runtime module registration in the host
dotnet add package AspNet.Module.Dal.EfCore
```

## DbContext Initialization

```cs
using AspNet.Module.Dal.EfCore;

public class AppDbContext : BaseDbContext
{
}
```

## Module Registration

```cs
using AspNet.Module.Dal.EfCore;

var builder = AspNetWebApplication.CreateBuilder(args);
builder.RegisterModule(new EfCoreModule<AppDbContext>(() => Clock.Now));
```

You can optionally pass a function that returns the current time to `EfCoreModule`. If not specified, `DateTime.UtcNow` is used.

## Additional Configuration

You can configure database options through environment variables or configuration files.

```json
{
  "DbContext": {
    "DetailedErrors": false
  }
}
```

`DetailedErrors` enables EF Core detailed error messages including parameter values.

## Source Code

- Repository: [github.com/aspnet-modules/aspnet-module-dal](https://github.com/aspnet-modules/aspnet-module-dal)
