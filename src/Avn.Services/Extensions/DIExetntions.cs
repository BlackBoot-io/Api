using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Avn.Services.Extensions;

internal static class DIExetntions
{
    /// <summary>
    ///  Register All Services that Inplemetes IScopedDependency
    ///  Interface Name Must Contain Class Name
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    public static void RegisterScopedServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var applicationServices = assembly.GetTypes().Where(x => x.IsClass && x.IsPublic && !x.IsAbstract && typeof(IScopedDependency).IsAssignableFrom(x)).ToList();
            applicationServices.ForEach(row =>
            {
                var typeInterface = row.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(x => x.Name.Contains(row.Name));
                if (typeInterface != null)
                    services.AddScoped(typeInterface, row);
                else
                    services.AddScoped(row);
            });
        }
    }

    /// <summary>
    ///  Register All Services that Inplemetes ITransientDependency
    ///  Interface Name Must Contain Class Name
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    public static void RegisterTransientServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var applicationServices = assembly.GetTypes().Where(x => x.IsClass && x.IsPublic && !x.IsAbstract && typeof(ITransientDependency).IsAssignableFrom(x)).ToList();
            applicationServices.ForEach(row =>
            {
                var typeInterface = row.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(x => x.Name.Contains(row.Name));
                if (typeInterface != null)
                    services.AddTransient(typeInterface, row);
                else
                    services.AddTransient(row);
            });
        }
    }

    /// <summary>
    ///  Register All Services that Inplemetes ISingletonDependency
    ///  Interface Name Must Contain Class Name
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    public static void RegisterSingletonServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var applicationServices = assembly.GetTypes().Where(x => x.IsClass && x.IsPublic && !x.IsAbstract && typeof(ISingletonDependency).IsAssignableFrom(x)).ToList();
            applicationServices.ForEach(row =>
            {
                var typeInterface = row.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(x => x.Name.Contains(row.Name));
                if (typeInterface != null)
                    services.AddSingleton(typeInterface, row);
                else
                    services.AddSingleton(row);
            });
        }
    }
}