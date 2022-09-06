#nullable disable
using Avn.Shared.Extentions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Avn.Api.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class IdentityMapperFilter : ActionFilterAttribute, IAsyncActionFilter
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
    {
        try
        {
            var ActionDescriptor = filterContext.ActionDescriptor as ControllerActionDescriptor;
            bool skipAuthorize = ActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                                                            .Any(a => a.GetType().Equals(typeof(AllowAnonymousAttribute)));
            if (skipAuthorize)
            {
                await base.OnActionExecutionAsync(filterContext, next);
                return;
            }
            if (filterContext.HttpContext.User.Claims.Any())
            {
                var userId = filterContext.HttpContext?.User?.Identity?.GetUserIdAsGuid();
                if (userId is not null)
                    filterContext.ActionArguments["userId"] = userId;
                else
                {
                    filterContext.HttpContext.Response.StatusCode = 401;
                    filterContext.Result = new JsonResult(new ActionResponse<object>
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
                filterContext.HttpContext.Response.StatusCode = 403;
                filterContext.Result = new JsonResult(new ActionResponse<object>
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
            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.Result = new JsonResult(new ActionResponse<object>
            {
                IsSuccess = false,
                StatusCode = ActionResponseStatusCode.ServerError,
                Message = $"Internall error in authorize filter.{Environment.NewLine}{e.Message}"
            });
            await base.OnActionExecutionAsync(filterContext, next);
        }
        await base.OnActionExecutionAsync(filterContext, next);
    }
}