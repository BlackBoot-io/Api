using Avn.Shared.Extentions;
using System.ComponentModel.DataAnnotations;

namespace Avn.Shared.Core.Data;

public class SaveChangesResult
{
    public SaveChangesResult()
    {
        IsSuccess = true;
        ResultType = SaveChangesResultType.Success;
        Message = ResultType.GetDisplayName();
    }
    public SaveChangesResult(SaveChangesResultType resultType)
    {
        IsSuccess = resultType switch
        {
            SaveChangesResultType.Success => true,
            _ => false
        };
        ResultType = resultType;
        Message = resultType.GetDisplayName();
    }
    public SaveChangesResult(SaveChangesResultType resultType, string message) : this(resultType)
    {
        Message = message;
    }

    public bool IsSuccess { get; set; }
    public SaveChangesResultType ResultType { get; set; }
    public string Message { get; set; }
}

public enum SaveChangesResultType : byte
{
    [Display(Name = "Success")]
    Success = 1,
    [Display(Name = "UpdateException")]
    UpdateException = 2,
    [Display(Name = "UpdateConcurrencyException")]
    UpdateConcurrencyException = 3,
}
