﻿using Avn.Services.Interfaces.DeliveryStrategies;

namespace Avn.Services.Implementations.DeliveryStrategies;
public class NullObjectStrategy : IDeliveryStrategy
{
    public async Task<IActionResponse<byte[]>> ExecuteAsync(int dropId, int count, CancellationToken cancellationToken)
           => new ActionResponse<byte[]>();
}
