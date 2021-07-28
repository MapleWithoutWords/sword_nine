using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Http
{
    /// <summary>
    /// httpcontext扩展类
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// 根据key获取
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetKey(this HttpContext context, string key = "UserId")
        {
            var ret = context.User.FindFirst(key)?.Value;
            return ret;
        }

        /// <summary>
        /// 获取用户id
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetUserId(this HttpContext context)
        {
            var ret = context.GetKey("UserId");
            return ret ?? Guid.Empty.ToString();
        }

        /// <summary>
        /// 获取用户名
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetUserName(this HttpContext context)
        {
            var ret = context.GetKey("UserName");
            return ret ?? "";
        }

        /// <summary>
        /// 获取用户账号
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetAccount(this HttpContext context)
        {
            var ret = context.GetKey("Account");
            return ret ?? "";
        }

    }
}
