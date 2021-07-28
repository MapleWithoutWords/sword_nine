using Sword.Nine.Rule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeLayerCode
{
    public class BuilderDomainCode : IBuilderCode
    {
        public string Build(TableDto tb, ConfigModel config, List<string> ngroColumns)
        {
            var tableName = tb.TableName;
            var _tableName = tb.Code;//去掉下划线的表名
            var desc = (string.IsNullOrEmpty(tb.Remark) ? tb.Name : tb.Remark);
            var fileContentSb = new StringBuilder();
            foreach (var refer in config.Reference)
            {
                fileContentSb.AppendLine("using " + refer + ";");
            }

            fileContentSb.AppendLine($"namespace {config.NameSpace}.Domain");
            fileContentSb.AppendLine("{");
            fileContentSb.AppendLine($"    /// <summary>");
            fileContentSb.AppendLine($"    /// 实体类：{desc}");
            fileContentSb.AppendLine($"    /// </summary>");
            fileContentSb.AppendLine($"    [Table(\"{ tableName}\")]");
            fileContentSb.AppendLine($"    [SugarTable(\"{tableName}\")]");
            fileContentSb.AppendLine($"    public partial class {_tableName}Entity: BaseEntity");
            fileContentSb.AppendLine("    {");
            foreach (var column in tb.AttrDatas)
            {
                if (ngroColumns.Any(e => e.Equals(column.Code, StringComparison.CurrentCultureIgnoreCase)))
                {
                    continue;
                }
                var text = DbTypeMap.GetColumnAttribute(column);
                fileContentSb.AppendLine($"       {text}");
            }
            fileContentSb.AppendLine("    }");
            fileContentSb.AppendLine("}");
            return fileContentSb.ToString();
        }

        public void Generator(string saveRootDire, DataSourceDto dataSourceDto)
        {
            var ngroColumns = new List<string> { "Id", "Version", "LastUpdateTime", "LastUpdateUser", "IsDeleted", "CreateTime", "CreateUser", "Status" };
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,dataSourceDto.DirPathName, "Configs/DomainConfig.json");
            var config = configPath.GetModel<ConfigModel>();
            if (config.Reference == null)
            {
                config.Reference = new List<string>();
            }
            config.NameSpace = dataSourceDto.NameSpace;


            string saveDir = Path.Combine(saveRootDire, $"{dataSourceDto.NameSpace}.Domain");
            foreach (var item in config.FileOutputPath.Split('/'))
            {
                saveDir = Path.Combine(saveDir, item);
                if (!System.IO.Directory.Exists(saveDir))
                {
                    System.IO.Directory.CreateDirectory(saveDir);
                }
            }


            dataSourceDto.DeleteNoExistsTable(saveDir, "", "Entity");
            foreach (var item in dataSourceDto.TableList)
            {

                var fileName = item.Code + "Entity.cs";
                var newDir = Path.Combine(saveDir, item.DirectoryCode);
                newDir.CreateDirectory();
                var filePath = Path.Combine(newDir, fileName);
                var content = Build(item, config, ngroColumns);
                File.WriteAllText(filePath, content);
            }

            Console.WriteLine("生成 Domain 层成功！！！");
        }
    }
}
