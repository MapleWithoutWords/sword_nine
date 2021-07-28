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
    /// 规则表服务实现类
    /// </summary>
    public partial interface ISnRuleService : IBaseService<SnRule>,IInjectFac
    {
        /// <summary>
        /// 获取所有类别属性规则
        /// </summary>
        /// <param name="attrId">属性id</param>
        /// <returns></returns>
        Task<List<SnRuleDto>> GetByAttributeIdAsync(string attrId);

        /// <summary>
        /// 保存类别属性
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<APIResult> SaveAsync(List<SnRule> entities);
    }
}
