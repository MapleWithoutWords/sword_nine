
using NoRain.Reposition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using DapperExtensions.Mapper;
using System.Data;

namespace NoRain.Reposition
{
    /// <summary>
    /// Dapper中表映射问题
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DapperTableNameMapper<T> : PluralizedAutoClassMapper<T> where T : class
    {

        /// <summary>
        /// 缓存所有表映射
        /// </summary>
        static Dictionary<string, string> tableNameMapper = new Dictionary<string, string>();
        /// <summary>
        /// 重写父类方法
        /// </summary>
        /// <param name="tableName"></param>
        public override void Table(string tableName)
        {
            if (tableNameMapper == null || tableNameMapper.Count < 1)
            {
                var typeList = Assembly.Load(DbSetting.SugarSearchAssemblyName).GetTypes().Where(e => e.IsAbstract == false && typeof(BaseEntity).IsAssignableFrom(e) && e.GetCustomAttribute<TableAttribute>() != null);
                foreach (var item in typeList)
                {
                    var dbTableName = item.GetCustomAttribute<TableAttribute>().Name;
                    tableNameMapper[item.Name] = dbTableName;
                }
            }
            if (tableNameMapper.ContainsKey(tableName))
            {
                TableName = tableNameMapper[tableName];
            }

            //base.Table(TableName);
        }
    }
}
