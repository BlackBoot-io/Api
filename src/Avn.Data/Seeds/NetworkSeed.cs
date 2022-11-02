using Avn.Data.UnitofWork;
using Avn.Shared.Core.Data;

namespace Avn.Data.Seeds;

public class NetworkSeed : IDataSeedProvider
{
    private readonly IAppUnitOfWork _uow;
    public int Order => 1;

    public NetworkSeed(IAppUnitOfWork uow)
        => _uow = uow;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {

        if (await _uow.NetworkRepo.AnyAsync(x => x.IsDefault, cancellationToken))
            return;

        _uow.NetworkRepo.Add(new()
        {
            Name = "Default",
            Type = Domain.Enums.NetworkType.TestNet,
            GasFee = 0,
            Wages = 0,
            SmartContractAddress = String.Empty,
            IsActive = true,
            IsDefault = true,

        });
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
