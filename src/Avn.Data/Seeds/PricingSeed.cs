

using Avn.Data.UnitofWork;
using Avn.Shared.Core.Data;

namespace Avn.Data.Seeds;

public class PricingSeed : IDataSeedProvider
{
    private readonly IAppUnitOfWork _uow;
    public int Order => 2;

    public PricingSeed(IAppUnitOfWork uow)
        => _uow = uow;



    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {

        if (await _uow.PricingRepo.Queryable().AnyAsync(x => x.IsFree, cancellationToken))
            return;

        var network = await _uow.NetworkRepo.Queryable().FirstOrDefaultAsync(x => x.IsDefault, cancellationToken);
        if (network is null)
            return;

        var model = new Pricing
        {
            Name = "Basic",
            UsdtAmount = 0,
            DiscountForYearlySubscription = 0,
            RequestsPerSecond = 10,
            RequestsPerDay = 1000,
            TokenPerDay = 100,
            IsActive = true,
            IsFree = true,
            NetworkInPricings = new List<NetworkInPricing>()
            {
                new()
                {
                    NetworkId=network.Id
                }
            }
        };
        await _uow.PricingRepo.AddAsync(model);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
