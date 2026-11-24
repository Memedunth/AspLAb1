using Microsoft.AspNetCore.Mvc.Filters;

namespace AspLAb1.Filters;

/// <summary>
/// Action filter logging action entry/exit and recording filter execution order.
/// </summary>
public class LogActionFilter : IActionFilter, IOrderedFilter
{
    internal const string FilterOrderItemKey = "FilterOrder";

    private readonly ILogger<LogActionFilter> _log;

    public LogActionFilter(ILogger<LogActionFilter> log) => _log = log;

    public int Order => 1;

    public void OnActionExecuting(ActionExecutingContext c)
    {
        AppendOrder(c.HttpContext, $"LogAction({Order}):Executing");
        _log.LogInformation("START [Order={Order}] {Action}({Args})",
            Order,
            c.ActionDescriptor.DisplayName,
            string.Join(", ", c.ActionArguments.Select(a => $"{a.Key}={a.Value}")));
    }

    public void OnActionExecuted(ActionExecutedContext c)
    {
        AppendOrder(c.HttpContext, $"LogAction({Order}):Executed");
        _log.LogInformation("KONIEC [Order={Order}] {Action}", Order, c.ActionDescriptor.DisplayName);

        if (!c.HttpContext.Response.HasStarted &&
            c.HttpContext.Items[FilterOrderItemKey] is List<string> trace)
        {
            c.HttpContext.Response.Headers["X-Filter-Order"] = string.Join(" -> ", trace);
        }
    }

    internal static void AppendOrder(HttpContext httpContext, string entry)
    {
        if (httpContext.Items[FilterOrderItemKey] is not List<string> trace)
        {
            trace = new List<string>();
            httpContext.Items[FilterOrderItemKey] = trace;
        }
        trace.Add(entry);
    }
}
