namespace Avn.Services.External.Dtos;

public record EmailRequestDto(
    string Receiver,
    string Subject = "",
    string Content = "",
    byte[] File = null);

public record EmailResponseDto(bool IsSucess, string Message);