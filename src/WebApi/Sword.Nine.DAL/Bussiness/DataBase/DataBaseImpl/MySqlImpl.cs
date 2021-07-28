using Sword.Nine.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sword.Nine.DAL.Bussiness.DataBase.DataBaseImpl
{
    public class MySqlImpl : DataBaseTableFactory
    {
        public override string GetCreateDataBaseSql(SnDataSource dataSource)
        {
            return $@"CREATE DATABASE /*!32312 IF NOT EXISTS*/`{dataSource.DatabaseName}` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;";
        }

        public override string GetCreateTableSql(string tableName, List<SnClassAttributeListDto> list)
        {
            var createUpdateTableSql = new StringBuilder(64);
            createUpdateTableSql.AppendLine($@"drop table if EXISTS `{tableName}`;");
            createUpdateTableSql.AppendLine($@"create table `{tableName}`(");
            foreach (var attItem in list)
            {
                var ret = GetDataTypeSql(attItem);
                createUpdateTableSql.AppendLine($@" `{attItem.ColumnName}` {ret.Item1} {(attItem.IsNullable ? "not null" : "null")} {ret.Item2} comment '{attItem.Remark ?? attItem.Name}', ");

            }
            var primaryAttr = list.FirstOrDefault(e => e.IsPrimary);
            if (primaryAttr != null)
            {
                createUpdateTableSql.AppendLine($@"   primary key({primaryAttr.ColumnName}));");
            }
            else
            {
                createUpdateTableSql.AppendLine($@"  );");
            }
            return createUpdateTableSql.ToString();
        }

        public override (string, string) GetDataBaseConnectionStr(SnDataSource dataSource)
        {
            var provide = "mysql";
            var constr = $"Server={dataSource.Host};Port={dataSource.Port};database={dataSource.DatabaseName};uid={dataSource.UserName};pwd={dataSource.Password};SslMode=None;Pooling=true;Max Pool Size=200;";
            return (constr, provide);
        }

        public override string GetIfTableExistsSql(string dbName, string tableName)
        {
            //表是否存在SQL语句
            return $@" select *                              
                       from information_schema.TABLES
                       where `table_name`='{dbName}' and table_schema='{tableName}';";
        }

        public override string GetColumnSql(List<SnClassAttributeListDto> insertList, List<SnClassAttributeListDto> updateList)
        {
            var createUpdateTableSql = new StringBuilder();
            foreach (var attItem in insertList)
            {
                var ret = GetDataTypeSql(attItem);

                createUpdateTableSql.AppendLine($@"alter table {attItem.ClassTableName} add column `{attItem.ColumnName}` {ret.Item1} {(attItem.IsNullable ? "not null" : "null")} {ret.Item2} comment '{attItem.Remark ?? attItem.Name}'; ");
            }
            foreach (var attItem in updateList)
            {
                var ret = GetDataTypeSql(attItem);

                createUpdateTableSql.AppendLine($@"alter table {attItem.ClassTableName} add column `{attItem.ColumnName}` {ret.Item1} {(attItem.IsNullable ? "not null" : "null")} {ret.Item2} comment '{attItem.Remark ?? attItem.Name}'; ");
            }
            return createUpdateTableSql.ToString();
        }

        /// <summary>
        /// 获取数据类型及默认值
        /// </summary>
        /// <param name="attItem"></param>
        /// <returns></returns>
        private (string, string) GetDataTypeSql(SnClassAttributeListDto attItem)
        {

            string typeSql;
            string defaultSql = "";
            if (attItem.ValueType == 0)
            {
                typeSql = $@"varchar({attItem.Length})";
                if (attItem.DefaultValue.IsNoNullAndNoEmpty())
                {
                    defaultSql = $@" default '{attItem.DefaultValue}'";
                }
            }
            else if (attItem.ValueType == 1)
            {
                typeSql = $@"int";
                if (attItem.DefaultValue.TryPartInt(out int val))
                {
                    defaultSql = $@" default {val}";
                }
            }
            else if (attItem.ValueType == 2)
            {
                typeSql = $@"float";
                if (attItem.DefaultValue.TryPartFloat(out float val))
                {
                    defaultSql = $@" default {val}";
                }
            }
            else if (attItem.ValueType == 3)
            {
                typeSql = $@"datetime";
                if (attItem.DefaultValue.TryPartDateTime(out DateTime val))
                {
                    defaultSql = $@" default '{val}'";
                }
            }
            else if (attItem.ValueType == 4)
            {
                typeSql = $@" varchar(36) ";
            }
            else if (attItem.ValueType == 5)
            {
                typeSql = $@" bigint ";
            }
            else if (attItem.ValueType == 6)
            {
                typeSql = $@" double ";
            }
            else if (attItem.ValueType == 7)
            {
                typeSql = $@" bit ";
            }
            else
            {
                throw new ApplicationException("未知的值类型");
            }
            return (typeSql, defaultSql);
        }
    }
}
