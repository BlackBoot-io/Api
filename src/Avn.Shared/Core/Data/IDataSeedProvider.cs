namespace Avn.Shared.Core.Data;

public interface IDataSeedProvider
{
    int Order { get; }
    Task SeedAsync(CancellationToken cancellationToken = default);
}
