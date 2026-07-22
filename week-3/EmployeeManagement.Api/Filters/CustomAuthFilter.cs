using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeeManagement.Api.Filters;

public sealed class CustomAuthFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorization))
        {
            context.Result = new BadRequestObjectResult("Invalid request - No Auth token");
            return;
        }

        if (!authorization.Any(value => value?.Contains("Bearer", StringComparison.OrdinalIgnoreCase) is true))
        {
            context.Result = new BadRequestObjectResult("Invalid request - Token present but Bearer unavailable");
        }
    }
}
