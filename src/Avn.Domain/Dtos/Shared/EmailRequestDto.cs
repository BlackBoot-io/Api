namespace Avn.Domain.Dtos;

public record EmailRequestDto(VerificationType Template,
    string Receiver,
    string Subject,
    string Content);
