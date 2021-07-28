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
    /// 规则类别属性配置表
    /// </summary>
    [Route("api/snruleclassattributesetting")]
    [ApiController]
    public partial class SnRuleClassAttributeSettingController: BaseApiController<ISnRuleClassAttributeSettingService, SnRuleClassAttributeSetting>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service">规则类别属性配置表服务</param>
        /// <param name="loggerFactory">日志服务</param>>
        public SnRuleClassAttributeSettingController(ISnRuleClassAttributeSettingService service,ILoggerFactory loggerFactory) : base(service,loggerFactory)
        {
        }

        /// <summary>
        /// 更新或新增规则配置
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        [HttpPost("update_or_insert")]
        public async Task<IActionResult> UpdateOrInsertRuleSetting(List<RuleAttributeSettingDto> dtos)
        {
            var ret = await InstanceService.UpdateOrInsertRuleSettingAsync(dtos);
            return this.Json(ret);
        }

        /// <summary>
        /// 根据类别id获取属性规则配置
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpGet("query_attr_rule_setting")]
        public async Task<IActionResult> QueryAttrRuleSetting(string classId)
        {
            var ret = await InstanceService.QueryAttrRuleSettingAsync(classId);
            return this.Json(ret);
        }
    }
}
