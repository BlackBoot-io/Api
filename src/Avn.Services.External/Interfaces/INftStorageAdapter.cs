using Avn.Services.External.Dtos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Avn.Services.External.Interfaces;


/// <summary>
/// Ineteract With IPFS Through NftStorage
/// </summary>
public interface INftStorageAdapter : ITransientDependency
{
    /// <summary>
    /// Get All Uploaded File In IPFS
    /// </summary>
    /// <param name="endDate">results created before provided timestamp </param>
    /// <param name="limit">number of result to return</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<IEnumerable<GetResponseDto>>> GetAllAsync(DateTime endDate, int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get Specific Uploaded File In IPFS
    /// </summary>
    /// <param name="contentId">Content Id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<GetResponseDto>> GetAsync(string contentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Upload File In IPFS
    /// </summary>
    /// <param name="item">The Model</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns></returns>
    Task<IActionResponse<UploadResponseDto>> UploadAsync(UploadRequestDto item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete Uploaded File in IPFS
    /// </summary>
    /// <param name="contentId"> contentId </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse> DeleteAsync(string contentId, CancellationToken cancellationToken = default);
}
