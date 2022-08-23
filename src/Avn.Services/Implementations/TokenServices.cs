using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avn.Services.Interfaces;

public class TokenServices : ITokenServices
{
    public Task<IActionResponse<object>> GetAsync(object id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    public Task<IActionResponse<object>> AddAsync(object eventId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    public Task<IActionResponse<object>> ConnectWalletAsync(object item, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }
    public Task<IActionResponse<object>> MintAsync(object item, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }
    public Task<IActionResponse<object>> BurnAsync(object item, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }
}
