using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sword.Nine.Domain;
using NoRain.Service;
using System.ComponentModel.DataAnnotations.Schema;
using Common.APICommon;
using NoRain.Common;
namespace Sword.Nine.Service
{
    /// <summary>
    /// 规则类别属性配置表服务实现类
    /// </summary>
    public partial interface ISnRuleClassAttributeSettingService : IBaseService<SnRuleClassAttributeSetting>,IInjectFac
    {
        /// <summary>
        /// 更新或新增规则配置
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        Task<APIResult> UpdateOrInsertRuleSettingAsync(List<RuleAttributeSettingDto> dtos);

        /// <summary>
        /// 根据类别id获取属性规则配置
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        Task<APIResult> QueryAttrRuleSettingAsync(string classId);
    }
}
