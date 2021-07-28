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
using System.Linq;
using Dapper;
using System.Text;
using NoRain.Common;
using Sword.Nine.DAL.Bussiness.DataBase;

namespace Sword.Nine.Dao
{
    /// <summary>
    /// 类别表服务实现类
    /// </summary>
    public partial class SnClassDalImpl : BaseReposition<SnClassDto>, ISnClassDal
    {
        public SnClassDalImpl(ILoggerFactory loggerFactory, IDbFactory dbFactory) : base(loggerFactory, dbFactory)
        {
        }
        /// <summary>
        /// 根据url查询类别信息
        /// </summary>
        /// <param name="urlParams"></param>
        /// <returns></returns>
        public override async Task<(List<SnClassDto>, PageDTO)> GetUrlParamAsync(List<UrlParams> urlParams)
        {
            List<string> list = GetTableName();
            IDictionary<string, object> pairs = null;
            //获取查询条件
            var condition = urlParams.GetCondition(list, ref pairs, "a");
            var query = Db.Queryable<SnClass, SnClassDirectory>((a, b) => new object[] {
                 JoinType.Left,a.ClassDirectoryId==b.Id
            }).Where(condition.ConditionalModels).Select((a, b) => new SnClassDto
            {
                Id = SqlFunc.GetSelfAndAutoFill(a.Id),
                DirectoryName = b.Name,
                DirectoryCode = b.Code,
            }).OrderBy(condition.OrderCondition);

            var result = await ToPageListAsync(query, condition);
            return (result, condition.Page);
        }

        public override Task<SnClassDto> GetByIdAsync(string id, int? status)
        {
            var query = Db.Queryable<SnClass, SnClassDirectory>((a, b) => new object[] {
                 JoinType.Left,a.ClassDirectoryId==b.Id
            }).Where((a, b) => a.Id == id && a.IsDeleted == false);

            if (status.HasValue)
            {
                query = query.Where((a, b) => a.Status == status.Value);
            }

            return query.Select((a, b) => new SnClassDto
            {
                Id = SqlFunc.GetSelfAndAutoFill(a.Id),
                DirectoryName = b.Name,
                DirectoryCode = b.Code,
            }).SingleAsync();
        }

        /// <summary>
        /// 根据数据源id获取类别信息
        /// </summary>
        /// <param name="dsId"></param>
        /// <returns></returns>
        public async Task<List<SnClassDto>> GetByDataSourceIdAsync(string dsId)
        {
            List<UrlParams> urlParams = new List<UrlParams>()
            {
                new UrlParams{  Key="dataSourceId", Values=new List<string>{ dsId} }
            };
            var ret = await GetUrlParamAsync(urlParams);
            return ret.Item1;
        }

        public async Task<int> PublishAsync(List<string> classIds)
        {
            var allAttributeDatas = await Db.Queryable<SnClass, SnClassAttribute>((a, b) => new object[] {
                JoinType.Left,a.Id==b.ClassId
            }).Where((a, b) => a.IsDeleted == false && b.IsDeleted == false && classIds.Contains(a.Id)).Select((a, b) => new SnClassAttributeListDto
            {
                Id = SqlFunc.GetSelfAndAutoFill(b.Id),
                ClassCode = a.Code,
                ClassName = a.Name,
                ClassTableName = a.TableName
            }).ToListAsync();

            var dataSource = await Db.Queryable<SnDataSource, SnClass>((a, b) => new object[] {
                JoinType.Inner,a.Id==b.DataSourceId
            }).Where((a, b) => a.IsDeleted == false && b.IsDeleted == false && classIds.Contains(b.Id))
            .Select((a, b) => new SnDataSource
            {
                Id = SqlFunc.GetSelfAndAutoFill(a.Id)
            }).FirstAsync();

            var dbType = (DataBaseTypeEnum)dataSource.Type;


            var instance = DataBaseTableFactory.GetInstance(dbType);
            await Db.Ado.ExecuteCommandAsync(instance.GetCreateDataBaseSql(dataSource));

            var connStr = instance.GetDataBaseConnectionStr(dataSource);
            using var db = dbFactory.CreateDbConnection(connStr.Item1, connStr.Item2);
            var execSql = new StringBuilder(64);
            execSql.AppendLine();

            foreach (var classId in allAttributeDatas.GroupBy(e => e.ClassId).Select(e => e.Key))
            {
                var attributeList = allAttributeDatas.Where(e => e.ClassId == classId).ToList();
                if (attributeList.Count < 1)
                {
                    continue;
                }
                var tableName = attributeList[0].ClassTableName;
                var createTableSql = instance.GetCreateTableSql(tableName, attributeList);
                execSql.AppendLine(createTableSql);
            }
            return await db.ExecuteAsync(execSql.ToString());
        }

    }
}
