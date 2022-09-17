#nullable disable
using Avn.Shared.Extentions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Avn.Api.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class IdentityMapperFilterAttribute : ActionFilterAttribute, IAsyncActionFilter
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            var ActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            bool skipAuthorize = ActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                                                            .Any(a => a.GetType().Equals(typeof(AllowAnonymousAttribute)));
            if (skipAuthorize)
            {
                await base.OnActionExecutionAsync(context, next);
                return;
            }
            if (context.HttpContext.User.Claims.Any())
            {
                var userId = context.HttpContext?.User?.Identity?.GetUserIdAsGuid();
                if (userId is not null)
                    context.ActionArguments["userId"] = userId;
                else
                {
                    context.HttpContext.Response.StatusCode = 401;
                    context.Result = new JsonResult(new ActionResponse<object>
                    {
                        StatusCode = ActionResponseStatusCode.UnAuthorized,
                        IsSuccess = false,
                        Message = "UnAuthorized Access. Invalid Token!"
                    });
                }
            }
            else
            {
                #region Not Existing User Claims
                context.HttpContext.Response.StatusCode = 403;
                context.Result = new JsonResult(new ActionResponse<object>
                {
                    StatusCode = ActionResponseStatusCode.Forbidden,
                    IsSuccess = false,
                    Message = "UnAuthorized Access To Api.",
                });
                #endregion
            }
        }
        catch (Exception e)
        {
            context.HttpContext.Response.StatusCode = 500;
            context.Result = new JsonResult(new ActionResponse<object>
            {
                IsSuccess = false,
                StatusCode = ActionResponseStatusCode.ServerError,
                Message = $"Internall error in authorize filter.{Environment.NewLine}{e.Message}"
            });
            await base.OnActionExecutionAsync(context, next);
        }
        await base.OnActionExecutionAsync(context, next);
    }
}