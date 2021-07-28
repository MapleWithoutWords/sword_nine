using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoRain.Reposition;
using Microsoft.Extensions.Logging;
using NoRain.Reposition.BaseReposition;
using Sword.Nine.Domain;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
using NoRain.Common;
using SqlSugar;

namespace Sword.Nine.Dao
{
    /// <summary>
    /// 类别属性服务实现类
    /// </summary>
    public partial class SnClassAttributeDalImpl : BaseReposition<SnClassAttributeListDto>, ISnClassAttributeDal
    {
        /// <summary>
        /// {desc}构造函数
        /// </summary>
        /// <param name="loggerFactory">日志</param>
        /// <param name="dbFactory">数据库工厂</param>
        public SnClassAttributeDalImpl(ILoggerFactory loggerFactory, IDbFactory dbFactory) : base(loggerFactory, dbFactory)
        {
        }
        public override async Task<(List<SnClassAttributeListDto>, PageDTO)> GetUrlParamAsync(List<UrlParams> urlParams)
        {
            List<string> list = GetTableName();
            IDictionary<string, object> pairs = null;
            //获取查询条件
            var condition = urlParams.GetCondition(list, ref pairs,"a");

            var query = Db.Queryable<SnClassAttribute, SnClass, SnClass>((a, b, c) => new object[] {
                 JoinType.Inner,a.ClassId==b.Id,
                 JoinType.Left,a.ReferenceId==c.Id,
            }).Where(condition.ConditionalModels).Select((a, b, c) => new SnClassAttributeListDto
            {
                Id = SqlFunc.GetSelfAndAutoFill(a.Id),
                ClassCode = b.Code,
                ClassName = b.Name,
                ClassTableName = b.TableName,
                ReferenceClassCode = c.Code,
                ReferenceClassName = c.Name,
                ReferenceClassTableName = c.TableName,

            }).OrderBy(condition.OrderCondition);


            var result = await ToPageListAsync(query, condition);
            return (result, condition.Page);
        }

        /// <summary>
        /// 根据数据源id获取属性
        /// </summary>
        /// <param name="dsId"></param>
        /// <returns></returns>
        public async Task<List<SnClassAttributeListDto>> GetByDataSourceIdAsync(string dsId)
        {
            List<UrlParams> urlParams = new List<UrlParams>()
            {
                 new UrlParams{ Key="dataSourceId", Values=new List<string>{ dsId} }
            };
            var ret = await GetUrlParamAsync(urlParams);
            return ret.Item1;
        }
    }
}
