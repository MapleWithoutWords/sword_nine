using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SAM.Common.APICommon.Filters
{
    public class APIExceptionFilter : IAsyncExceptionFilter
    {
        ILoggerFactory loggerFactory;
        ILogger logger;
        public APIExceptionFilter(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
            logger = loggerFactory.CreateLogger(this.GetType());
        }
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            logger.LogError(context.Exception.ToString());
            context.Result = ActionResultExtensions.JsonErrorData(null, context.Exception.Message);
            return;
        }
    }
}
