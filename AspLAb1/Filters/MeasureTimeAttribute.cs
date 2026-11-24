using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspLAb1.Filters;

/// <summary>
/// Action filter attribute measuring action execution duration in milliseconds.
/// </summary>
public class MeasureTimeAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        LogActionFilter.AppendOrder(context.HttpContext, $"MeasureTime({Order}):Executing");
        context.HttpContext.Items[typeof(MeasureTimeAttribute)] = Stopwatch.StartNew();
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        LogActionFilter.AppendOrder(context.HttpContext, $"MeasureTime({Order}):Executed");

        if (!context.HttpContext.Response.HasStarted)
        {
            if (context.HttpContext.Items[typeof(MeasureTimeAttribute)] is Stopwatch sw)
            {
                sw.Stop();
                context.HttpContext.Response.Headers["X-Action-Ms"] = sw.ElapsedMilliseconds.ToString();
            }

            if (context.HttpContext.Items[LogActionFilter.FilterOrderItemKey] is List<string> trace)
            {
                context.HttpContext.Response.Headers["X-Filter-Order"] = string.Join(" -> ", trace);
            }
        }
    }
}
