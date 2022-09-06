using Avn.Domain.Dtos.Externals.NftStorage;
using Avn.Services.External.Interfaces;

namespace Avn.Api.Controllers;

public class NftStorageController : BaseController
{
    private readonly INftStorageAdapter _nftStorageAdapter;

    public NftStorageController(INftStorageAdapter nftStorageAdapter) => _nftStorageAdapter = nftStorageAdapter;



    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> UploadNftAsync(UploadRequestDto item, CancellationToken cancellationToken)
    => Ok(await _nftStorageAdapter.UploadAsync(item, cancellationToken));
}