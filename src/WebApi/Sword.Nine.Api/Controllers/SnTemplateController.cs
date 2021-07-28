using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoRain.Reposition;
using Microsoft.Extensions.Logging;
using NoRain.Reposition.BaseReposition;
using Sword.Nine.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoRain.Api;
using Sword.Nine.Service;
using NoRain.Common;
using System.IO;
using System.IO.Compression;

namespace Sword.Nine.Api.Controllers
{
    /// <summary>
    /// 模板表
    /// </summary>
    [Route("api/sntemplate")]
    [ApiController]
    public partial class SnTemplateController : BaseApiController<ISnTemplateService, SnTemplate>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service">模板表服务</param>
        /// <param name="loggerFactory">日志服务</param>>
        public SnTemplateController(ISnTemplateService service, ILoggerFactory loggerFactory) : base(service, loggerFactory)
        {
        }

        /// <summary>
        /// 上传模板文件夹
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost("upload_template")]
        public async Task<IActionResult> UploadTemplate()
        {
            var fileList = Request.Form.Files;
            if (fileList.Count < 0)
            {
                return this.JsonErrorData("请上传压缩文件");
            }
            var file = fileList[0];
            var code =DateTime.Now.ToString("yyyyMMddHHmmss")+Guid.NewGuid().ToString("");
            var saveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StaticFiles/CodeTemplate", code);
            file.SaveToDirePathAsync(file.FileName, saveDir);
            var zipFilePath = Path.Combine(saveDir, file.FileName);
            ZipFile.ExtractToDirectory(zipFilePath, saveDir);
            return this.JsonApiResultData($@"StaticFiles/CodeTemplate/{code}");
        }
    }
}
