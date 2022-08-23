using Avn.Services.External.Implementations;
using System.Threading;
using System.Threading.Tasks;

namespace Avn.Services.External.Interfaces;


public class NftStorageAdaptar : INftStorageAdaptar
{
    public Task<object> Upload(object item, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }
}
