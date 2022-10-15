namespace Avn.Services.External.Dtos;

public record UploadRequestDto(string Name, string Description, byte[] Image, object Properties);
