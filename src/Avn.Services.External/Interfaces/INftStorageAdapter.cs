using Avn.Domain.Dtos.Externals.NftStorage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Avn.Services.External.Interfaces;

public interface INftStorageAdapter : ITransientDependency
{
    Task<IActionResponse<IEnumerable<UploadResponseDto>>> GetAllAsync(DateTime startDate, int limit, CancellationToken cancellationToken = default);
    Task<IActionResponse<UploadResponseDto>> GetAsync(string cid, CancellationToken cancellationToken = default);
    Task<IActionResponse<UploadResponseDto>> UploadAsync(UploadRequestDto item, CancellationToken cancellationToken = default);
    Task<IActionResponse> DeleteAsync(string cid, CancellationToken cancellationToken = default);
}
