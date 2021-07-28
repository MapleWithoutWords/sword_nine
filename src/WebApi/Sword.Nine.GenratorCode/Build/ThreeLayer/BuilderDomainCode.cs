using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sword.Nine.GenratorCode.Build.ThreeLayer
{
    public class BuilderDomainCode : IBuilderCode
    {
        public string Build(DbTable tb, ConfigModel config, List<string> ngroColumns)
        {
            var tableName = tb.TableName;
            var _tableName = tb._TableName;//去掉下划线的表名
            var desc = (string.IsNullOrEmpty(tb.TableDesc) ? tb._TableName : tb.TableDesc);
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
            fileContentSb.AppendLine($"    public partial class {tb._TableName}: BaseEntity");
            fileContentSb.AppendLine("    {");
            foreach (var column in tb.Columns)
            {
                if (ngroColumns.Contains(column._列名))
                {
                    continue;
                }
                var text = DbTypeMap.GetColumnAttribute(column, tb._TableName);
                fileContentSb.AppendLine($"       {text}");
            }
            fileContentSb.AppendLine("    }");
            fileContentSb.AppendLine("}");
            return fileContentSb.ToString();
        }

        public void Generator(string saveRootDire, List<DbTable> tableList)
        {
            DbHelper Db = new DbHelper();
            var config = Db.GetModels("./Configs/DomainConfig.json");
            string saveDir = saveRootDire;
            foreach (var item in config.FileOutputPath.Split('/'))
            {
                saveDir = Path.Combine(saveDir, item);
                if (!System.IO.Directory.Exists(saveDir))
                {
                    System.IO.Directory.CreateDirectory(saveDir);
                }
            }
            var ngroColumns = new List<string> { "Id", "Version", "LastUpdateTime", "LastUpdateUser", "IsDeleted", "CreateTime", "CreateUser", "Status", "TenantId" };

            var fileList = Directory.GetFiles(saveDir);
            foreach (var item in fileList)
            {
                var fileName = Path.GetFileName(item);
                if (tableList.Any(e => $"{e._TableName}.cs" == fileName))
                {
                    continue;
                }
                File.Delete(item);
            }
            foreach (var item in tableList)
            {
                var fileName = item._TableName + ".cs";
                var filePath = Path.Combine(saveDir, fileName);
                var content = Build(item, config, ngroColumns);
                File.WriteAllText(filePath, content);
            }
            Console.WriteLine("生成 Domain 层成功！！！");
        }
    }
}
