using ExpenseTracker.Application.Requests.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ExpenseTracker.Filters;

public class UserRequestFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var arg in context.ActionArguments)
        {
            if (arg.Value is UserRequest userRequest)
            {
                var user = context.HttpContext.User;
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (Guid.TryParse(userId, out var result))
                {
                    var modifiedRequest = userRequest with
                    {
                        UserId = result
                    };

                    context.ActionArguments[arg.Key] = modifiedRequest;
                }
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No logic needed for after execution
    }
}
