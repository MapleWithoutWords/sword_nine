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
    /// 规则表
    /// </summary>
    [Route("api/snrule")]
    [ApiController]
    public partial class SnRuleController: BaseApiController<ISnRuleService, SnRule>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service">规则表服务</param>
        /// <param name="loggerFactory">日志服务</param>>
        public SnRuleController(ISnRuleService service,ILoggerFactory loggerFactory) : base(service,loggerFactory)
        {
        }

        /// <summary>
        /// 获取所有类别属性规则
        /// </summary>
        /// <param name="attrId">属性id</param>
        /// <returns></returns>
        [HttpGet("getbyattrid")]
        [ProducesResponseType(typeof(APIResult<List<SnRuleDto>>),200)]
        public async Task<IActionResult> GetByAttributeIdAsync(string attrId)
        { 
            var ret = await InstanceService.GetByAttributeIdAsync(attrId);
            return this.JsonApiResultData(ret);
        }
        /// <summary>
        /// 保存规则，包括新增、修改、删除等
        /// </summary>
        /// <param name="dtolist"></param>
        /// <returns></returns>
        [HttpPost("save")]
        public async Task<IActionResult> SaveAsync([FromBody] List<SnRule> dtolist)
        {
            if (dtolist == null || dtolist.Count < 1)
            {
                return this.JsonApiResultData("");
            }
            var ret = await InstanceService.SaveAsync(dtolist);
            return this.Json(ret);
        }
    }
}
