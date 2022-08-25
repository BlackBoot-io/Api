namespace Avn.Domain.Dtos;

public record EmailRequestDto(EmailTemplate Template, string Receiver, string Subject, string Content);
