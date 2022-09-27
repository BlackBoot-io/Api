using Avn.Shared.Core.Data;

namespace Avn.Data.Seeds;

public class SubscriptionSeed : IDataSeedProvider
{
    public int Order => 1;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
