using Avn.Services.Interfaces.DeliveryStrategies;

namespace Avn.Services.Implementations.DeliveryStrategies;

public class DeliveryFactory : IDeliveryFactory
{
    private readonly Lazy<ITokensService> _tokensService;
    public DeliveryFactory(Lazy<ITokensService> tokensService) => _tokensService = tokensService;

    public IDeliveryStrategy GetInstance(DeliveryType type)
           => type switch
           {
               DeliveryType.Link => new LinkStrategy(_tokensService.Value),
               DeliveryType.QR => new QrStrategy(),
               _ => new NullObjectStrategy(),
           };
}
