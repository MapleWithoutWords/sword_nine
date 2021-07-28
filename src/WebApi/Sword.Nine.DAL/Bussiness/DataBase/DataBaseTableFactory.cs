using Sword.Nine.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sword.Nine.DAL.Bussiness.DataBase
{
    public abstract class DataBaseTableFactory
    {
        public static DataBaseTableFactory GetInstance(DataBaseTypeEnum dbType)
        {
            var impl = typeof(DataBaseTableFactory).Assembly.GetTypes().FirstOrDefault(e => typeof(DataBaseTableFactory).IsAssignableFrom(e) && e.IsAbstract == false && e.Name == $"{dbType.ToString()}Impl");
            if (impl==null)
            {
                throw new ApplicationException($"不支持发布的数据库类型:{dbType}");
            }
            return (DataBaseTableFactory)Activator.CreateInstance(impl);
        }
        /// <summary>
        /// 获取创建数据库连接字符串
        /// </summary>
        /// <returns></returns>
        public abstract (string, string) GetDataBaseConnectionStr(SnDataSource dataSource);
        /// <summary>
        /// 获取判断表是否存在sql
        /// </summary>
        /// <returns></returns>
        public abstract string GetIfTableExistsSql(string dbName,string tableName);
        /// <summary>
        /// 获取创建数据库sql
        /// </summary>
        /// <returns></returns>
        public abstract string GetCreateDataBaseSql(SnDataSource dataSource);
        /// <summary>
        /// 获取创建表sql
        /// </summary>
        /// <returns></returns>
        public abstract string GetCreateTableSql(string tableName, List<SnClassAttributeListDto> list);
        /// <summary>
        /// 获取添加或更新表字段sql
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public abstract string GetColumnSql(List<SnClassAttributeListDto> insertList, List<SnClassAttributeListDto> updateList);
    }
}
