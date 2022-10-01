#nullable disable
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace Avn.Api.Filters;

public class ApiResultAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {

        if (context.Result is OkObjectResult redirectOkObjectResult && redirectOkObjectResult.Value is ActionResponse<string> redirectResult && redirectResult.StatusCode == ActionResponseStatusCode.Redirect)
            context.Result = new RedirectResult(redirectResult.Data);

        else if (context.Result is OkObjectResult okObjectResult && okObjectResult.Value is ActionResponse result)
            context.Result = new JsonResult(result)
            {
                StatusCode = (int)result.StatusCode
            };

        else if (context.Result is OkResult okResult)
            context.Result = new JsonResult(new ActionResponse())
            {
                StatusCode = okResult.StatusCode
            };
        else if (context.Result is ObjectResult badRequestObjectResult && badRequestObjectResult.StatusCode == 400)
        {
            string message = null;
            switch (badRequestObjectResult.Value)
            {
                case ValidationProblemDetails validationProblemDetails:
                    var errorMessages = validationProblemDetails.Errors.Select(p => new { p.Key, p.Value }).Distinct();
                    message = JsonSerializer.Serialize(errorMessages);
                    break;
                case SerializableError errors:
                    var errorMessages2 = errors.Select(p => new { p.Key, p.Value }).Distinct();
                    message = JsonSerializer.Serialize(errorMessages2);
                    break;
                case var value when value != null && value is not ProblemDetails:
                    message = badRequestObjectResult.Value.ToString();
                    break;
            }

            ActionResponse apiResult = new(ActionResponseStatusCode.BadRequest, message);
            context.Result = new JsonResult(apiResult) { StatusCode = badRequestObjectResult.StatusCode };
        }
        else if (context.Result is ObjectResult notFoundObjectResult && notFoundObjectResult.StatusCode == 404)
        {
            string message = null;
            if (notFoundObjectResult.Value != null && notFoundObjectResult.Value is not ProblemDetails)
                message = notFoundObjectResult.Value.ToString();

            ActionResponse apiResult = new(ActionResponseStatusCode.NotFound, message);
            context.Result = new JsonResult(apiResult) { StatusCode = notFoundObjectResult.StatusCode };
        }
        else if (context.Result is ContentResult contentResult)
        {
            ActionResponse apiResult = new(ActionResponseStatusCode.Success, contentResult.Content);
            context.Result = new JsonResult(apiResult) { StatusCode = contentResult.StatusCode };
        }
        else if (context.Result is ObjectResult objectResult && objectResult.StatusCode == null && objectResult.Value is not ActionResponse)
        {
            var apiResult = new ActionResponse<object>(objectResult.Value);
            context.Result = new JsonResult(apiResult) { StatusCode = objectResult.StatusCode };
        }

        base.OnResultExecuting(context);
    }
}
