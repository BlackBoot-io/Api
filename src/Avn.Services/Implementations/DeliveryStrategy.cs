namespace Avn.Services.Implementations;

public interface IDeliveryStrategy
{
    Task<IActionResponse<byte[]>> Execute();
}

//switch (drop.DeliveryType)
//{
//    case DeliveryType.Link:
//        var tokens = Enumerable.Repeat(drop, drop.Count).Select(row => new CreateTokenDto
//        {
//            DropId = drop.Id
//        }).ToList();

//        var tokenResult = await _tokensService.Value.AddRangeAsync(tokens, cancellationToken);
//        if (!tokenResult.IsSuccess)
//            return new ActionResponse<bool>(tokenResult.StatusCode, tokenResult.Message);

//        break;
//    case DeliveryType.QR:
//        //generate a url for claim and generate Qr from it
//        //then send this qr to users
//        break;
//    default:
//        break;
//}


public static class DeliveryFactory
{
    public static IDeliveryStrategy GetInstance(DeliveryType type)
           => type switch
           {
               DeliveryType.Link => new LinkStrategy(),
               DeliveryType.QR => new QrStrategy(),
               _ => new NullObjectStrategy(),
           };
}
public class NullObjectStrategy : IDeliveryStrategy
{
    
}
public class LinkStrategy : IDeliveryStrategy
{
    
}

public class QrStrategy : IDeliveryStrategy
{
    
}