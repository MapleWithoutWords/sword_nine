using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoRain.Reposition;
using Microsoft.Extensions.Logging;
using NoRain.Reposition.BaseReposition;
using Sword.Nine.Domain;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
using SqlSugar;

namespace Sword.Nine.Dao
{
    /// <summary>
    /// 规则表服务实现类
    /// </summary>
    public partial class SnRuleDalImpl : BaseReposition<SnRule>, ISnRuleDal
    {
        public SnRuleDalImpl(ILoggerFactory loggerFactory, IDbFactory dbFactory) : base(loggerFactory, dbFactory)
        {
        }

        /// <summary>
        /// 获取某个数据源下的所有类别属性规则
        /// </summary>
        /// <param name="dsId"></param>
        /// <returns></returns>
        public async Task<List<SnRuleDto>> GetByDataSourceIdAsync(string dsId)
        {
            return await Db.Queryable<SnDataSource, SnClass, SnRuleClassAttributeSetting, SnRule>((a, b, c, d) => new object[]
             {
                JoinType.Inner,a.Id==b.DataSourceId,
                JoinType.Inner,b.Id==c.ClassId,
                JoinType.Inner,c.RuleId==d.Id,
             }).Where((a, b, c, d) => a.IsDeleted == false &&
                                         b.IsDeleted == false &&
                                         c.IsDeleted == false &&
                                         d.IsDeleted == false &&
                                         a.Id == dsId)
             .Select((a, b, c, d) => new SnRuleDto
             {
                 Id = SqlFunc.GetSelfAndAutoFill(d.Id),
                 Value = c.Value,
                 ClassAttributeId = c.ClassAttributeId,
                 ClassId = c.ClassId
             })
             .ToListAsync();
        }
        /// <summary>
        /// 获取某个数据源下的所有类别属性规则
        /// </summary>
        /// <param name="dsId"></param>
        /// <returns></returns>
        public async Task<List<SnRuleDto>> GetByClassIdAsync(string classId)
        {
            return await Db.Queryable<SnClass, SnRuleClassAttributeSetting, SnRule>((a, b, c) => new object[]
             {
                JoinType.Inner,a.Id==b.ClassId,
                JoinType.Inner,b.RuleId==c.Id,
             }).Where((a, b, c) => a.IsDeleted == false &&
                                   b.IsDeleted == false &&
                                   c.IsDeleted == false &&
                                   a.Id == classId)
             .Select((a, b, c) => new SnRuleDto
             {
                 Id = SqlFunc.GetSelfAndAutoFill(c.Id),
                 Value = b.Value,
                 ClassAttributeId = b.ClassAttributeId,
                 ClassId = b.ClassId
             })
             .ToListAsync();
        }

        /// <summary>
        /// 获取所有类别属性规则
        /// </summary>
        /// <param name="attrId">属性id</param>
        /// <returns></returns>
        public async Task<List<SnRuleDto>> GetByAttributeIdAsync(string attrId)
        {
            return await Db.Queryable<SnRuleClassAttributeSetting, SnRule>((a, b) => new object[]
             {
                JoinType.Inner,a.RuleId==b.Id,
             }).Where((a, b) => a.IsDeleted == false &&
                                   b.IsDeleted == false &&
                                   a.ClassAttributeId == attrId)
             .Select((a, b) => new SnRuleDto
             {
                 Id = SqlFunc.GetSelfAndAutoFill(b.Id),
                 Value = a.Value,
                 ClassAttributeId = a.ClassAttributeId,
                 ClassId = a.ClassId
             })
             .ToListAsync();
        }
    }
}
