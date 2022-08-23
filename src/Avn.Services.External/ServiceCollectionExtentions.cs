using Avn.Shared.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Avn.Services;

public static class ServiceCollectionExtentions
{
    public static void RegisterApplicatioinExternalServices(this IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionExtentions).Assembly;
        services.RegisterScopedServices(assembly);
        services.RegisterSingletonServices(assembly);
        services.RegisterTransientServices(assembly);
    }
}

