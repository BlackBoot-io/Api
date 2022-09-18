using Avn.Services.External;
using Avn.Shared.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Avn.Services;

internal class Lazier<T> : Lazy<T> where T : class
{
    public Lazier(IServiceProvider provider)
        : base(() => provider.GetRequiredService<T>())
    {
    }
}

public static class ServiceCollectionExtentions
{
    public static void RegisterApplicatioinServices(this IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionExtentions).Assembly;
        services.RegisterScopedServices(assembly);
        services.RegisterSingletonServices(assembly);
        services.RegisterTransientServices(assembly);
        services.AddTransient(typeof(Lazy<>), typeof(Lazier<>));


        services.RegisterApplicatioinExternalServices();
    }
}