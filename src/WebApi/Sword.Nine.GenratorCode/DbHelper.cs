using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sword.Nine.GenratorCode
{
    public class DbHelper
    {
        private StringBuilder template;

        public DbHelper()
        {
            // var sss=Host.ResolveAssemblyReference("$(ProjectDir)");
        }
        // string connectionString = "Server=server=192.168.66.179;database=wiz_frdb;uid=root;pwd=123456;SslMode=None;Pooling=true;Max Pool Size=200;";
        string sql = @"SELECT 
                                    b.table_comment TableDescription
                                    ,a.TABLE_NAME TableName
                                    ,a.COLUMN_NAME ColumnName
                                    ,a.IS_NULLABLE IsNull
                                    ,a.DATA_TYPE ColumnType
                                    ,a.ORDINAL_POSITION SeqNo
                                    ,IFNULL(a.CHARACTER_MAXIMUM_LENGTH,'') Length
                                    ,a.COLUMN_DEFAULT DefaultValue
                                    ,IFNULL(a.NUMERIC_SCALE,'') FloatCount
                                    ,a.COLUMN_COMMENT ColumnDescription
                                    ,a.COLUMN_KEY PrimaryKey

                                    FROM information_schema.columns a 
                                    INNER JOIN INFORMATION_SCHEMA.TABLES b ON a.TABLE_NAME=b.TABLE_NAME
                                    WHERE a.table_schema = '{0}' AND b.table_schema = '{0}'  {1}  {2}
                                    ORDER BY a.TABLE_NAME,a.COLUMN_NAME";

        public List<DbTable> GetTableInfo(List<string> filterTable, ConfigModel config = null)
        {
            List<DbTable> result = new List<DbTable>();
            string tableString = string.Empty;
            if (!string.IsNullOrEmpty(config.Table) && config.Table != "*" && config.Table.Split(',').Length > 0)
            {
                tableString = string.Join(",", config.Table.Split(',').ToList().Select(c => string.Format("'{0}'", c)));
                tableString = $" AND a.TABLE_NAME IN({tableString})";
            }
            sql = string.Format(sql, config.DataBase, NotExcludeTable(filterTable), tableString);

            using (MySqlConnection dbConn = new MySqlConnection(config.ConnectionString))
            {
                var dbColumns = dbConn.Query<Columns>(sql).ToList();
                if (dbColumns.Count() > 0)
                {
                    foreach (IGrouping<string, Columns> item in dbColumns.GroupBy(s => s.TableName))
                    {
                        var tableName = DbTable._ToFirstUpper(item.Key);//去掉下划线后第一个字母大写;// item.Key.ToString();
                        var desc = item.FirstOrDefault().TableDescription;
                        var items = item.ToList();
                        DbTable tb = new DbTable() { TableName = tableName, TableDesc = desc, Columns = new List<Columns>() };
                        if (items.Count > 0)
                        {
                            foreach (var column in items)
                            {
                                column.ColumnName = DbTable._ToFirstUpper(column.ColumnName);//去掉下划线后第一个字母大写
                            }
                            tb.Columns.AddRange(items);
                        }
                        result.Add(tb);
                    }
                }
                dbConn.Close();
                return result;
            }
        }
        public ConfigModel GetModels(string jsonPath)
        {
            ConfigModel model = null;
            //读取json文件  
            using (StreamReader sr = new StreamReader(jsonPath, System.Text.Encoding.Default))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Converters.Add(new JavaScriptDateTimeConverter());
                serializer.NullValueHandling = NullValueHandling.Ignore;
                //构建Json.net的读取流  
                JsonReader reader = new JsonTextReader(sr);
                //对读取出的Json.net的reader流进行反序列化，并装载到模型中  
                model = serializer.Deserialize<ConfigModel>(reader);
            }
            return model;
        }
        /// <summary>
        ///不包括定义的表
        /// </summary>      
        /// <param name="filterTable"></param>
        /// <returns></returns>
        static string NotExcludeTable(List<string> filterTable)
        {
            var where = string.Empty;
            if (filterTable != null && filterTable.Count > 0)
            {
                foreach (var prefix in filterTable)
                {
                    where += $" and a.table_name not like '{prefix}%' ";

                }
            }
            return where;
        }
    }
    /// <summary>
    /// 表类
    /// </summary>
    public class Columns
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 表说明
        /// </summary>
        public string TableDescription { get; set; }
        /// <summary>
        /// 字段序号
        /// </summary>
        public string SeqNo { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 是否自增
        /// </summary>
        public string IsIncreate { get; set; }
        /// <summary>
        /// 是否关键字段
        /// </summary>
        public string PrimaryKey { get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
        public string ColumnType { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public string Length { get; set; }
        /// <summary>
        /// 字段的精度
        /// </summary>
        public string FloatCount { get; set; }
        /// <summary>
        /// 是否允许为空
        /// </summary>
        public string IsNull { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 字段描述
        /// </summary>
        public string ColumnDescription { get; set; }
        /// <summary>
        /// _表名
        /// </summary>
        public string _表名
        {
            get
            {
                var str = TableName;
                var newName = string.Empty;
                if (str.IndexOf('_') > -1)
                {
                    var list = str.Split('_');
                    foreach (var name in list)
                    {
                        newName += ToFirstUpper(name);
                    }
                }
                return !string.IsNullOrEmpty(newName) ? newName : TableName;
            }
        }
        /// <summary>
        /// _列名
        /// </summary>
        public string _列名
        {
            get
            {
                var str = ColumnName;
                var newName = string.Empty;
                if (str.IndexOf('_') > -1)
                {
                    var list = str.Split('_');
                    foreach (var name in list)
                    {
                        newName += ToFirstUpper(name);
                    }
                }
                return !string.IsNullOrEmpty(newName) ? newName : ColumnName;
            }
        }

        /// <summary>
        /// 字符串第一个字母大写
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public string ToFirstUpper(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (str.Length > 1)
                {
                    return char.ToUpper(str[0]) + str.Substring(1);
                }
                return char.ToUpper(str[0]).ToString();
            }
            return str;
        }

    }
    /// <summary>
    /// 数据类型映射
    /// </summary>
    public class DbTypeMap
    {
        public static string GetColumnAttribute(Columns column, string tbName = "")
        {
            var noRequiries = new List<string> { "Path", "ApplicationKey", "TopId", "Path", "ParentId", "Certificate" };
            StringBuilder sb = new StringBuilder();
            var str = string.Empty;
            var csDataType = MapToCsharpType(column.ColumnType);
            var columnDesc = "";
            if (!string.IsNullOrEmpty(column.ColumnDescription))
            {
                var nameList = column.ColumnDescription.Replace("\r\n", "┃").Split('┃');
                if (nameList.Length > 0)
                {
                    var n = 0;
                    foreach (var name in nameList)
                    {
                        if (n == 0)
                        {
                            columnDesc += " ///<para>" + name.Trim() + "</para>\n";
                        }
                        else
                        {
                            columnDesc += " ///<para>" + name.Trim() + "</para>\n";
                        }
                        n++;
                    }
                    columnDesc = columnDesc.TrimEnd('\n');
                }
            }
            else
            {
                columnDesc = "///" + column._列名;
            }
            sb.AppendLine("           ///<summary>");
            sb.AppendLine("           " + columnDesc);
            sb.AppendLine("            ///</summary>");
            if (column.ColumnName.ToLower() == "id")
            {
                sb.AppendLine("            [Key]");
                sb.AppendLine("            [Required(ErrorMessage =\"" + column.ColumnName + "不能为null,可以为空字符串''\",AllowEmptyStrings =true)]");
                sb.AppendLine("            [StringLength(" + column.Length + ",ErrorMessage = \"" + column.ColumnName + "最长不能超" + column.Length + "个字符!\")]");
                sb.AppendLine("            [SugarColumn(ColumnName =\"" + column.Length + "\",IsPrimaryKey =true)]");
                sb.AppendLine("            public virtual " + csDataType + " " + column.ColumnName + " { get; set; }=\"\";");
            }
            else if (column.ColumnType != null && (column.ColumnType.ToLower() == "varchar" || column.ColumnType.ToLower() == "char"))
            {
                if (noRequiries.Contains(column.ColumnName))
                {

                    sb.AppendLine("            [SugarColumn(ColumnName =\"" + column.ColumnName + "\")]");
                    sb.AppendLine("             [StringLength(" + column.Length + ",ErrorMessage = \"" + column.ColumnDescription.Replace("\r\n", "") + " 最长不能超 " + column.Length + " 个字符!\")]");
                    sb.AppendLine("             public virtual " + csDataType + " " + column._列名 + " { get; set; }");

                }
                else if (column.ColumnName == "TenantId")
                {

                    sb.AppendLine("            [StringLength(" + column.Length + ",ErrorMessage = \"" + column.ColumnDescription.Replace("\r\n", "") + " 最长不能超 " + column.Length + " 个字符!\")]");
                    sb.AppendLine("            [SugarColumn(ColumnName =\"" + column.ColumnName + "\")]");
                    sb.AppendLine("            [NoNull(ErrorMessage =\"" + column.ColumnName + "不能为空\")]");
                    sb.AppendLine("            public virtual " + csDataType + " " + column._列名 + " { get; set; }=Guid.Empty.ToString();");
                }
                else
                {
                    sb.AppendLine("            [StringLength(" + column.Length + ",ErrorMessage = \"" + column.ColumnDescription.Replace("\r\n", "") + " 最长不能超 " + column.Length + " 个字符!\")]");
                    sb.AppendLine("            [SugarColumn(ColumnName =\"" + column.ColumnName + "\")]");
                    sb.AppendLine("            [NoNull(ErrorMessage =\"" + column.ColumnName + "不能为空\")]");
                    sb.AppendLine("            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText=\"\")]");
                    sb.AppendLine("            public virtual " + csDataType + " " + column._列名 + " { get; set; }");
                }
            }
            else
            {
                sb.AppendLine("            [SugarColumn(ColumnName =\"" + column.ColumnName + "\")]");
                sb.AppendLine("            [NoNull(ErrorMessage =\"" + column.ColumnName + "不能为空\")]");
                sb.AppendLine("            public virtual " + csDataType + " " + column._列名 + " { get; set; }");
            }
            return str = sb.ToString();
        }
        public static string MapToCsharpType(string dbType)
        {
            string sysType = "string";
            switch (dbType)
            {
                case "bigint":
                    sysType = "long";
                    break;
                case "smallint":
                    sysType = "short";
                    break;
                case "int":
                    sysType = "int";
                    break;
                case "uniqueidentifier":
                    sysType = "Guid";
                    break;
                case "smalldatetime":
                case "datetime":
                case "datetime2":
                case "date":
                case "time":
                case "timestamp":
                    sysType = "DateTime";
                    break;
                case "float":
                case "double":
                    sysType = "double";
                    break;
                case "real":
                    sysType = "float";
                    break;
                case "numeric":
                case "smallmoney":
                case "decimal":
                case "money":
                    sysType = "decimal";
                    break;
                case "tinyint":
                    sysType = "byte";
                    break;
                case "bit":
                    sysType = "bool";
                    break;
                case "image":
                case "binary":
                case "varbinary":
                    sysType = "byte[]";
                    break;
                case "geography":
                    sysType = "Microsoft.SqlServer.Types.SqlGeography";
                    break;
                case "geometry":
                    sysType = "Microsoft.SqlServer.Types.SqlGeometry";
                    break;
            }
            return sysType;
        }
    }
    /// <summary>
    /// 数据库表
    /// </summary>
    public class DbTable
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public string TableDesc { get; set; }
        /// <summary>
        /// 所有列
        /// </summary>
        public List<Columns> Columns { get; set; }
        /// <summary>
        /// _TableName
        /// </summary>
        public string _TableName
        {
            get
            {
                var str = TableName;
                var newName = string.Empty;
                if (str.IndexOf('_') > -1)
                {
                    var list = str.Split('_');
                    foreach (var name in list)
                    {
                        newName += ToFirstUpper(name);
                    }
                }
                return !string.IsNullOrEmpty(newName) ? newName : TableName;
            }
        }
        /// <summary>
        /// 字符串第一个字母大写
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string ToFirstUpper(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (str.Length > 1)
                {
                    return char.ToUpper(str[0]) + str.Substring(1);
                }
                return char.ToUpper(str[0]).ToString();
            }
            return str;
        }
        /// <summary>
        /// _字符串第一个字母大写
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string _ToFirstUpper(string str)
        {
            var newName = string.Empty;
            if (str.IndexOf('_') > -1)
            {
                var list = str.Split('_');
                foreach (var name in list)
                {
                    newName += ToFirstUpper(name) + "_";
                }
            }
            return !string.IsNullOrEmpty(newName) ? newName.TrimEnd('_') : ToFirstUpper(str);
        }
    }
    public class ConfigModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DataBase { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NameSpace { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> Reference { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Table { get; set; }
        public bool IsNotExistsDelete { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsCreateDto { get; set; }
        /// <summary>
        /// entity文件输出目录名称
        /// </summary>
        public string FileOutputPath { get; set; }

    }
}
