using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoRain.Reposition;
using Microsoft.Extensions.Logging;
using NoRain.Reposition.BaseReposition;
using Sword.Nine.Domain;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
namespace Sword.Nine.Dao
{
    /// <summary>
    /// 规则表服务实现类
    /// </summary>
    public partial interface ISnRuleDal : IBaseReposition<SnRule>
    {

        /// <summary>
        /// 获取某个数据源下的所有类别属性规则
        /// </summary>
        /// <param name="dsId"></param>
        /// <returns></returns>
        Task<List<SnRuleDto>> GetByDataSourceIdAsync(string dsId);

        /// <summary>
        /// 获取某个数据源下的所有类别属性规则
        /// </summary>
        /// <param name="dsId"></param>
        /// <returns></returns>
        Task<List<SnRuleDto>> GetByClassIdAsync(string classId);

        /// <summary>
        /// 获取所有类别属性规则
        /// </summary>
        /// <param name="attrId">属性id</param>
        /// <returns></returns>
        Task<List<SnRuleDto>> GetByAttributeIdAsync(string attrId);
    }
}
