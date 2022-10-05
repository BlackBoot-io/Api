#nullable disable
using Avn.Shared.Extentions;
using Microsoft.AspNetCore.Mvc;
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
    #region Implicit Operators
    public static implicit operator ActionResponse(OkResult result) => new(ActionResponseStatusCode.Success);

    public static implicit operator ActionResponse(BadRequestResult result) => new(ActionResponseStatusCode.BadRequest);

    public static implicit operator ActionResponse(BadRequestObjectResult result)
    {
        var message = result.Value?.ToString();
        if (result.Value is SerializableError errors)
        {
            var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
            message = string.Join(" | ", errorMessages);
        }
        return new(ActionResponseStatusCode.BadRequest, message);
    }

    public static implicit operator ActionResponse(ContentResult result) => new(ActionResponseStatusCode.Success, result.Content);
    public static implicit operator ActionResponse(NotFoundResult result) => new(ActionResponseStatusCode.NotFound);

    #endregion

   
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


    #region Implicit Operators
    public static implicit operator ActionResponse<TData>(TData data) => new(ActionResponseStatusCode.Success, data);
    public static implicit operator ActionResponse<TData>(OkResult result) => new(ActionResponseStatusCode.Success);
    public static implicit operator ActionResponse<TData>(OkObjectResult result) => new(ActionResponseStatusCode.Success, (TData)result.Value);
    public static implicit operator ActionResponse<TData>(BadRequestResult result) => new(ActionResponseStatusCode.BadRequest);
    public static implicit operator ActionResponse<TData>(BadRequestObjectResult result)
    {
        var message = result.Value?.ToString();
        if (result.Value is SerializableError errors)
        {
            var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
            message = string.Join(" | ", errorMessages);
        }
        return new(ActionResponseStatusCode.BadRequest, message);
    }
    public static implicit operator ActionResponse<TData>(ContentResult result) => new(ActionResponseStatusCode.Success, result.Content);
    public static implicit operator ActionResponse<TData>(NotFoundResult result) => new(ActionResponseStatusCode.NotFound);
    public static implicit operator ActionResponse<TData>(NotFoundObjectResult result) => new(ActionResponseStatusCode.NotFound, (TData)result.Value);

    #endregion
}