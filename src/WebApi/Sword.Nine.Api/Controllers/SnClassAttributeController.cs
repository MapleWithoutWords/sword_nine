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

namespace Sword.Nine.Api.Controllers
{
    /// <summary>
    /// 类别属性
    /// </summary>
    [Route("api/snclassattribute")]
    [ApiController]
    public partial class SnClassAttributeController: BaseApiController<ISnClassAttributeService, SnClassAttributeListDto>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service">类别属性服务</param>
        /// <param name="loggerFactory">日志服务</param>>
        public SnClassAttributeController(ISnClassAttributeService service,ILoggerFactory loggerFactory) : base(service,loggerFactory)
        {
        }

        /// <summary>
        /// 保存类别属性，包括新增、修改、删除等
        /// </summary>
        /// <param name="dtolist"></param>
        /// <returns></returns>
        [HttpPost("save")]
        public async Task<IActionResult> SaveAsync([FromBody]List<SnClassAttributeListDto> dtolist)
        {
            if (dtolist==null|| dtolist.Count<1)
            {
                return this.JsonApiResultData("");
            }
            var ret = await InstanceService.SaveAsync(dtolist);
            return this.Json(ret);
        }
    }
}
