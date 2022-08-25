#nullable disable

using Avn.Data.Repository;
using Avn.Data.UnitofWork;

namespace Avn.Api.Extentions;

public static class ServiceCollectionExtentions
{
    public static void AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
        services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
        services.AddDbContext<ApplicationDbContext>((IServiceProvider serviceProvider, DbContextOptionsBuilder options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("ApplicationDbContext"));
        });
    }

}