using Avn.Domain.Dtos.Externals.NftStorage;
using System.Threading;
using System.Threading.Tasks;

namespace Avn.Services.External.Interfaces;

public interface INftStorageAdapter : ITransientDependency
{
    Task<IActionResponse<UploadResponseDto>> Upload(UploadRequestDto item, CancellationToken cancellationToken = default);
}
