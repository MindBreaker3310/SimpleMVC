using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using SimpleMVC.Controllers;

namespace SimpleMVC.Filters
{
    public class MyCustomExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<MyCustomExceptionFilter> _logger;

        public MyCustomExceptionFilter(ILogger<MyCustomExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            //記錄錯誤
            _logger.LogError("出事了!阿伯!");

            //建立新的View
            ViewResult result = new ViewResult()
            {
                ViewName = "/Views/Shared/CustomErrorPage.cshtml",
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = context.Exception,
                }
            };
            result.ViewData["Controller"] = context.RouteData.Values["Controller"].ToString();
            result.ViewData["Action"] = context.RouteData.Values["Action"].ToString();

            context.Result = result;
            context.ExceptionHandled = true;//表示錯誤已接住

        }
    }
}

