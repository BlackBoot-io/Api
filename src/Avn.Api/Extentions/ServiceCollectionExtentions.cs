#nullable disable

namespace Avn.Api.Extentions;

public static class ServiceCollectionExtentions
{
    public static void AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDBContext>((IServiceProvider serviceProvider, DbContextOptionsBuilder options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("ApplicationDbContext"));
        });
    }

}