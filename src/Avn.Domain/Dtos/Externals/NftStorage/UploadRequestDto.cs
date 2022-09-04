namespace Avn.Domain.Dtos.Externals.NftStorage;

public record UploadRequestDto(string Name, string Description, byte[] Image, object MetaData);
