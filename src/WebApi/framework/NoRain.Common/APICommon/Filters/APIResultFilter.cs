using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAM.Common.APICommon.Filters
{
   public class APIResultFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is BadRequestObjectResult)
            {
                BadRequestObjectResult result = (BadRequestObjectResult)context.Result;
                var validationDetails = (ValidationProblemDetails)result.Value;
                StringBuilder sb = new StringBuilder();
                foreach (var item in validationDetails.Errors)
                {
                    foreach (var val in item.Value)
                    {
                        sb.AppendLine(val);
                    }
                }
                context.Result = ActionResultExtensions.JsonErrorData(null, sb.ToString());
                
                return;
            }
        }
    }
}
