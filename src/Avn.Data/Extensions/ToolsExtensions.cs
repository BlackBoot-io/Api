using System.Reflection;

namespace Avn.Data.Extensions;

public static class ToolsExtensions
{
    /// <summary>
    /// Register all entities into dbContext dynamically
    /// </summary>
    /// <typeparam name="EntityType">Entity Class</typeparam>
    public static void RegisterAllEntities<EntityType>(this ModelBuilder modelBuilder, params Assembly[] assemblies)
    {
        var types = assemblies.SelectMany(a => a.GetExportedTypes())
                              .Where(c => c.IsClass &&
                                          c.IsPublic &&
                                          !c.IsAbstract &&
                                          typeof(EntityType)
                              .IsAssignableFrom(c));

        foreach (var type in types)
            modelBuilder.Entity(type);
    }
}
