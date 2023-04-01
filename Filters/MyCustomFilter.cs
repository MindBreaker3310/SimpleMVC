using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace SimpleMVC.Filters
{
    public class MyCustomFilter : Attribute, IActionFilter, IResultFilter
    {
        //Action執行前
        public void OnActionExecuting(ActionExecutingContext context)
        {
            DoLogging("1. OnActionExecuting", context.RouteData, context.HttpContext);
            if (DateTime.Now.Minute % 2 == 0)
            {
                context.HttpContext.Response.WriteAsync("<br />1. No entry when minutes are even!! <br />");
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                //不會進入OnActionExecuted，會直接到OnResultExecuting
            }
        }
        //執行後(若執行前回應錯誤代碼，將不會進入)
        public void OnActionExecuted(ActionExecutedContext context)
        {
            DoLogging("2. OnActionExecuted", context.RouteData, context.HttpContext);
        }

        //回傳結果前
        public void OnResultExecuting(ResultExecutingContext context)
        {
            DoLogging("3. OnResultExecuting", context.RouteData, context.HttpContext);
        }

        //回傳結果後
        public void OnResultExecuted(ResultExecutedContext context)
        {
            DoLogging("4. OnResultExecuted", context.RouteData, context.HttpContext);
        }



        public void DoLogging(string FunctionName, RouteData routeData, HttpContext httpContext)
        {
            var controller = routeData.Values["controller"].ToString();
            var action = routeData.Values["action"].ToString();

            httpContext.Response.WriteAsync($"<br /> function:{FunctionName}, controller:{controller}, action:{action} <br />");
        }
    }
}

