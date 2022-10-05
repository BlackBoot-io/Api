#nullable disable
using Avn.Shared.Extentions;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Avn.Shared.Core;

public interface IActionResponse
{
    public bool IsSuccess { get; set; }
    public ActionResponseStatusCode StatusCode { get; set; }
    public string Message { get; set; }
}
public interface IActionResponse<TData> : IActionResponse
{
    public TData Data { get; set; }
}


public class ActionResponse : IActionResponse
{

    public ActionResponse()
    {
        IsSuccess = true;
        StatusCode = ActionResponseStatusCode.Success;
        Message = StatusCode.GetDisplayName();
    }
    public ActionResponse(ActionResponseStatusCode statusCode)
    {
        IsSuccess = statusCode switch
        {
            ActionResponseStatusCode.Success => true,
            _ => false
        };
        StatusCode = statusCode;
        Message = statusCode.GetDisplayName();
    }
    public ActionResponse(ActionResponseStatusCode statusCode, string message) : this(statusCode)
    {
        Message = message;
    }


    public bool IsSuccess { get; set; }
    public ActionResponseStatusCode StatusCode { get; set; }
    public string Message { get; set; }
  
}
public class ActionResponse<TData> : ActionResponse, IActionResponse<TData>
{
    public TData Data { get; set; }

    public ActionResponse() => Data = default;
    public ActionResponse(ActionResponseStatusCode statusCode) : base(statusCode) => Data = default;
    public ActionResponse(ActionResponseStatusCode statusCode, string message) : base(statusCode, message) => Data = default;
    public ActionResponse(ActionResponseStatusCode statusCode, TData data) : base(statusCode) => Data = data;
    public ActionResponse(TData data) => Data = data;
    public ActionResponse(ActionResponseStatusCode statusCode, TData data, string message) : base(statusCode, message) => Data = data;
}