namespace Avn.Domain.Enums;

public enum TemplateType : byte
{
    EmailVerification = 1,    
    ForgetPassword = 2,
    CreateDrop = 3,
    DropRejected = 4,
    ConfirmDrop = 5
}