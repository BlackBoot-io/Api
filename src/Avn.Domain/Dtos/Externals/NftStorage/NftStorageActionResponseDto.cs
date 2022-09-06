using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Avn.Services.External")]
namespace Avn.Domain.Dtos.Externals.NftStorage;


internal record struct NftStorageActionResponse<TValue>(bool Ok, TValue Value, NftStorageUploadResponseError Error);
internal record struct NftStorageActionResponse(bool Ok, NftStorageUploadResponseValue Value, NftStorageUploadResponseError Error);
internal record struct NftStorageUploadResponseValue(string CId);
internal record struct NftStorageUploadResponseError(string Name, string Message);
