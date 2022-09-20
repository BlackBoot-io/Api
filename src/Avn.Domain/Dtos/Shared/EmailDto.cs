namespace Avn.Domain.Dtos;

public record EmailRequestDto(TemplateType Template,
    string Receiver,
    string Subject = "",
    string Content = "",
    byte[] File = null);

public record EmailResponseDto(bool IsSucess, string Message);