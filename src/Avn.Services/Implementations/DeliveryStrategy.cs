namespace Avn.Services.Implementations;

public interface IDeliveryStrategy
{
    Task<IActionResponse<byte[]>> ExecuteAsync(int dropId, int count, CancellationToken cancellationToken);
}

public interface IDeliveryFactory
{
    IDeliveryStrategy GetInstance(DeliveryType type);
}
public class DeliveryFactory : IDeliveryFactory
{
    private readonly ITokensService _tokensService;
    public DeliveryFactory(ITokensService tokensService) => _tokensService = tokensService;

    public IDeliveryStrategy GetInstance(DeliveryType type)
           => type switch
           {
               DeliveryType.Link => new LinkStrategy(_tokensService),
               DeliveryType.QR => new QrStrategy(),
               _ => new NullObjectStrategy(),
           };
}

public class LinkStrategy : IDeliveryStrategy
{
    private readonly ITokensService _tokensService;
    public LinkStrategy(ITokensService tokensService)
    {
        _tokensService = tokensService;
    }
    public async Task<IActionResponse<byte[]>> ExecuteAsync(int dropId, int count, CancellationToken cancellationToken)
    {
        var tokens = Enumerable.Range(0, count).Select(row => new CreateTokenDto
        {
            DropId = dropId
        }).ToList();

        var tokenResult = await _tokensService.AddRangeAsync(tokens, cancellationToken);
        if (!tokenResult.IsSuccess)
            return new ActionResponse<byte[]>(tokenResult.StatusCode, tokenResult.Message);

        byte[] bytes = null;
        using var ms = new MemoryStream();
        using TextWriter tw = new StreamWriter(ms);
        foreach (var item in tokenResult.Data)
            tw.Write(item);
        tw.Flush();
        ms.Position = 0;
        bytes = ms.ToArray();

        return new ActionResponse<byte[]>()
        {
            IsSuccess = true,
            Data = bytes
        };
    }
}
public class QrStrategy : IDeliveryStrategy
{
    public async Task<IActionResponse<byte[]>> ExecuteAsync(int dropId, int count, CancellationToken cancellationToken)
      => new ActionResponse<byte[]>() { IsSuccess = true };
}
public class NullObjectStrategy : IDeliveryStrategy
{
    public async Task<IActionResponse<byte[]>> ExecuteAsync(int dropId, int count, CancellationToken cancellationToken)
           => new ActionResponse<byte[]>() { IsSuccess = true };
}
