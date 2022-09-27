using Avn.Shared.Core.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Avn.Data;


public static class ServiceCollectionExtentions
{
    public static void RegisterDatabaseSeed(this IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionExtentions).Assembly;
        var applicationServices = assembly.GetTypes().Where(x => x.IsClass && x.IsPublic && !x.IsAbstract && typeof(IDataSeedProvider).IsAssignableFrom(x)).ToList();
        applicationServices.ForEach(row =>
        {
            services.AddScoped(typeof(IDataSeedProvider), row);
        });
    }
}
