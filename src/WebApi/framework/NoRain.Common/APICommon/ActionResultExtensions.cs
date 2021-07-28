using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NoRain.Common;
using SAM.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.AspNetCore.Mvc
{

    public static class ActionResultExtensions
    {
        /// <summary>
        /// 返回成功的数据：APIResult
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="data"></param>
        /// <param name="page"></param>
        /// <param name="msg">提示消息</param>
        /// <returns></returns>
        public static IActionResult JsonApiResultData(this ControllerBase controller, object data, PageDTO page = null, string msg = "操作成功")
        {
            APIResult res = new APIResult
            {
                Code = 0,
                Data = data,
                Page = page,
                Msg = msg,
            };
            return new JsonResult(res);
        }
        /// <summary>
        /// 返回错误的信息：APIResult
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static IActionResult JsonErrorData(this ControllerBase controller, string errorMsg)
        {
            APIResult res = new APIResult
            {
                Code = 1,
                Msg = errorMsg
            };
            return new JsonResult(res);
        }
        /// <summary>
        /// 返回json数据
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IActionResult Json(this ControllerBase controller, object data)
        {
            return new JsonResult(data);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static IActionResult DownLoadFile(this ControllerBase controller, string filePath)
        {
            var fs = System.IO.File.OpenRead(filePath);
            var providerList = new FileExtensionContentTypeProvider();
            var contentType = providerList.Mappings[Path.GetExtension(filePath)];
            return controller.File(fs, contentType, Path.GetFileName(filePath));
        }


        /// <summary>
        /// 返回json数据
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IActionResult JsonNet(this ControllerBase controller, object data)
        {
            
            return new ContentResult() {  Content= JsonConvert.SerializeObject(new APIResult { Data= data }, new IsoDateTimeConverter() { DateTimeFormat = "yyyy/MM/dd HH:mm:ss" }).Replace("0001/01/01 00:00:00","").Replace("1900/01/01 00:00:00", ""), ContentType="application/json"};
        }
    }
}
