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
using Sword.Nine.Domain.Dto.DataSource.directory;

namespace Sword.Nine.Api.Controllers
{
    /// <summary>
    /// 类别目录
    /// </summary>
    [Route("api/snclassdirectory")]
    [ApiController]
    public partial class SnClassDirectoryController : BaseApiController<ISnClassDirectoryService, SnClassDirectory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service">类别目录服务</param>
        /// <param name="loggerFactory">日志服务</param>>
        public SnClassDirectoryController(ISnClassDirectoryService service, ILoggerFactory loggerFactory) : base(service, loggerFactory)
        {
        }

        /// <summary>
        /// 获取目录树
        /// </summary>
        /// <returns></returns>
        [HttpGet("get_directory_tree")]
        [ProducesResponseType(typeof(APIResult<List<DirectoryClassTreeDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDirectoryTreeAsync()
        {
            var urlParams = this.Request.Query.GetUrlParams();
            var ret = await InstanceService.GetDirectoryTreeAsync(urlParams);
            return this.Json(ret);
        }

        /// <summary>
        /// 保存绘图
        /// </summary>
        /// <param name="directoryId">目录id</param>
        /// <param name="cellDtos"></param>
        /// <returns></returns>
        [HttpPost("save_drawing")]
        public async Task<IActionResult> SaveDrawingAsync(DrawingSaveDto dto)
        {
            var ret = await InstanceService.SaveDrawingAsync(dto);
            return this.Json(ret);
        }
    }
}
