using Avn.Services.Interfaces.DeliveryStrategies;

namespace Avn.Services.Implementations.DeliveryStrategies;

public class LinkStrategy : IDeliveryStrategy
{
    private readonly ITokensService _tokensService;
    public LinkStrategy(ITokensService tokensService) => _tokensService = tokensService;
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

        return new ActionResponse<byte[]>(bytes);
    }
}
