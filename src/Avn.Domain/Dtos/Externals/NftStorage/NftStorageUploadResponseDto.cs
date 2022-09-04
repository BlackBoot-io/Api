namespace Avn.Domain.Dtos.Externals.NftStorage;

public record NftStorageUploadResponseDto(bool ok, NftStorageUploadResponseValue value, NftStorageUploadResponseError error);

public record NftStorageUploadResponseValue(string Cid);
public record struct NftStorageUploadResponseError(string Name, string Message);
