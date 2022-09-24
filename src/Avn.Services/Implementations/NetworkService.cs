using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avn.Services.Implementations;

public class NetworkService : INetworkService
{
    public Task<IActionResponse<List<Network>>> GetAllAvailableAsync()
    {
        throw new NotImplementedException();
    }
}

