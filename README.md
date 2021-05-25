# Prism.Extensions.ServiceCollection

This is a library for using containers other than `Microsoft.Exntesions.DependencyInjection` without being affected by the container settings.

add `Container.RegisterServices(IServiceCollection)` method.

## how to use

### [DryIoc](https://www.nuget.org/packages/DryIoc/) or [Prism.DryIoc](https://www.nuget.org/packages/Prism.DryIoc/)

install [DryIoc.Microsoft.DependencyInjection.Extension](https://www.nuget.org/packages/DryIoc.Microsoft.DependencyInjection.Extension/)

#### Prism.DryIoc

```cs
using DryIoc.Microsoft.DependencyInjection.Extension
// ...
Registry.GetContainer().RegisterServices(v =>
{
    v.AddTransient<IA>(v => new A1());
});
// ...
```

#### DryIoc

```cs
using DryIoc.Microsoft.DependencyInjection.Extension
// ...
Container.RegisterServices(v =>
{
    v.AddTransient<IA>(v => new A1());
});
// ...
```
