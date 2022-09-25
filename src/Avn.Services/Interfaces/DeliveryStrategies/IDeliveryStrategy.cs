namespace Avn.Services.Interfaces.DeliveryStrategies;

public interface IDeliveryStrategy
{
    Task<IActionResponse<byte[]>> ExecuteAsync(int dropId, int count, CancellationToken cancellationToken);
}
