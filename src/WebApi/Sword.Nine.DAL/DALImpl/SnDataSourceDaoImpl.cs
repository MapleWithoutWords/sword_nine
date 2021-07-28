using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoRain.Reposition;
using Microsoft.Extensions.Logging;
using NoRain.Reposition.BaseReposition;
using Sword.Nine.Domain;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
using DapperExtensions;
using Dapper;
using System.Reflection;
using NoRain.Common;

namespace Sword.Nine.Dao
{
    /// <summary>
    /// 数据源表服务实现类
    /// </summary>
    public partial class SnDataSourceDalImpl : BaseReposition<SnDataSource>, ISnDataSourceDal
    {
        public SnDataSourceDalImpl(ILoggerFactory loggerFactory, IDbFactory dbFactory) : base(loggerFactory, dbFactory)
        {
        }
        public override async Task<(List<SnDataSource>, PageDTO)> GetUrlParamAsync(List<UrlParams> urlParams)
        {
            List<string> list = GetTableName();
            IDictionary<string, object> pairs = null;
            //获取查询条件
            var condition = urlParams.GetCondition(list, ref pairs);
            var query = DbContext.AsQueryable();
            //其它参数查询
            if (urlParams.TryGetString("keyword", out string keyword))
            {
                query = query.Where(e => e.Name.Contains(keyword) || e.Code.Contains(keyword));
            }
            query = query.Where(condition.ConditionalModels).OrderBy(condition.OrderCondition);
            var result = await ToPageListAsync(query, condition);
            return (result, condition.Page);
        }
    }
}
