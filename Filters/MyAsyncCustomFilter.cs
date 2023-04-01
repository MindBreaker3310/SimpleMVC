using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace SimpleMVC.Filters
{
    public class MyAsyncCustomFilter : Attribute, IAsyncActionFilter, IAsyncResultFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            DoLogging("1. OnActionExecutionAsync", context.RouteData, context.HttpContext);
            if (DateTime.Now.Minute % 2 == 0)
            {
                await context.HttpContext.Response.WriteAsync("<br />1. No entry when minutes are even!! (Async Filter) <br />");
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                //不會進入OnActionExecuted，會直接到OnResultExecuting
            }

            var resultContext = await next();

            DoLogging("2. OnActionExecutedAsync", context.RouteData, context.HttpContext);


        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            DoLogging("3. OnResultExecutingAsync", context.RouteData, context.HttpContext);

            var resultContext = await next();

            DoLogging("4. OnResultExecutedAsync", context.RouteData, context.HttpContext);

        }

        public void DoLogging(string FunctionName, RouteData routeData, HttpContext httpContext)
        {
            var controller = routeData.Values["controller"].ToString();
            var action = routeData.Values["action"].ToString();

            httpContext.Response.WriteAsync($"<br />async function:{FunctionName}, controller:{controller}, action:{action} <br />");
        }
    }
}

