using CompanyEmployees.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CompanyEmployees.ActionFilters;

public class ValidationFilterAttribute:IActionFilter
{
    private readonly ILogger<ValidationFilterAttribute> _logger;

    public ValidationFilterAttribute(ILogger<ValidationFilterAttribute> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var action = context.RouteData.Values["action"];
        var controller = context.RouteData.Values["controller"];
        var param = context.ActionArguments.SingleOrDefault(x => x.ToString().Contains("Dto")).Value;
        if (param == null)
        {
            _logger.LogError($"( Object sent from the client is null , Controller == {controller} , Action == {action}");
            context.Result =
                new BadRequestObjectResult(
                    $"(Object sent from the client is null , Controller == {controller} , Action == {action})");
            return;
        } 
        if (!context.ModelState.IsValid)
        {
            _logger.LogError($"(Invalid model state for the {param})");
            context.Result=new UnprocessableEntityObjectResult(context.ModelState);
            return;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        
    }
}