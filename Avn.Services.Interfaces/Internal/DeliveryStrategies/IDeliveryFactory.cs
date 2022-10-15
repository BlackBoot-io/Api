
using Avn.Shared.Core;

namespace Avn.Services.Interfaces.DeliveryStrategies;

public interface IDeliveryFactory : IScopedDependency
{
    IDeliveryStrategy GetInstance(DeliveryType type);
}
