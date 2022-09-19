namespace Avn.Domain.Enums;

public enum TemplateType : byte
{
    EmailVerification = 2,
    ForgetPassword = 3,
    CreateDrop = 4,
    DropRejected = 5,
    ConfirmDrop = 6
}