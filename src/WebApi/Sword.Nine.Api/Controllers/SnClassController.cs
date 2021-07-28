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
using System.ComponentModel.DataAnnotations;

namespace Sword.Nine.Api.Controllers
{
    /// <summary>
    /// 类别表
    /// </summary>
    [Route("api/snclass")]
    [ApiController]
    public partial class SnClassController : BaseApiController<ISnClassService, SnClassDto>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service">类别表服务</param>
        /// <param name="loggerFactory">日志服务</param>>
        public SnClassController(ISnClassService service, ILoggerFactory loggerFactory) : base(service, loggerFactory)
        {
        }

        /// <summary>
        /// 根据rul查询数据
        /// </summary>
        /// <returns></returns>
        [ProducesDefaultResponseType(typeof(APIResult<List<SnClassDto>>))]
        public override async Task<IActionResult> GetByUrl()
        {
            return await base.GetByUrl();
        }

        /// <summary>
        /// 获取目录类别树
        /// </summary>
        /// <returns></returns>
        [HttpGet("get_directory_class")]
        public async Task<IActionResult> GetDirectoryClassAsync()
        {
            var urlparam = this.Request.Query.GetUrlParams();
            var ret = await InstanceService.GetDirectoryClassAsync(urlparam);
            return this.Json(ret);
        }
        [HttpGet("publish")]
        public async Task<IActionResult> Publish([Required] string classId)
        {
            var ret = await InstanceService.PublishAsync(new List<string> { classId });
            return this.JsonApiResultData(ret);
        }

        /// <summary>
        /// 获取类别信息及属性信息
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpGet("getinfo/{classId}")]
        public async Task<IActionResult> GetInfo(string classId)
        {
            var ret = await InstanceService.GetInfoAndAttributeAsync(classId);
            return this.Json(ret);
        }
    }
}
