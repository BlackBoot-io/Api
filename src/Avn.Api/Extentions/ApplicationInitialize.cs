using Avn.Data.UnitofWork;
using Avn.Shared.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Avn.Api.Extentions;

public class ApplicationInitialize : BackgroundService
{
    private readonly IServiceScope _serviceScope;
    public ApplicationInitialize(IServiceScopeFactory serviceScope)
            => _serviceScope = serviceScope.CreateScope();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        var uow = _serviceScope.ServiceProvider.GetRequiredService<IAppUnitOfWork>();
        if (await uow.Database.EnsureCreatedAsync() && uow.Database.IsRelational())
            await uow.Database.MigrateAsync();


        var seedServices = _serviceScope.ServiceProvider.GetServices<IDataSeedProvider>();

        foreach (var seed in seedServices.OrderBy(x => x.Order))
        {
            await seed.SeedAsync(stoppingToken);
        }
    }
}
