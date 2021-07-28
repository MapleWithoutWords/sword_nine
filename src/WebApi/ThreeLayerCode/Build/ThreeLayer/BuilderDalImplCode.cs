using Sword.Nine.Rule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeLayerCode
{
    public class BuilderDalImplCode : IBuilderCode
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

            fileContentSb.AppendLine($"namespace {config.NameSpace}.DAL");
            fileContentSb.AppendLine("{");
            fileContentSb.AppendLine($"    /// <summary>");
            fileContentSb.AppendLine($"    /// {desc}服务实现类");
            fileContentSb.AppendLine($"    /// </summary>");
            fileContentSb.AppendLine($"    public partial class {_tableName}DalImpl: BaseReposition<{_tableName }Entity>,I{_tableName }Dal");
            fileContentSb.AppendLine("    {");
            fileContentSb.AppendLine("        /// <summary>");
            fileContentSb.AppendLine("        /// {desc}构造函数");
            fileContentSb.AppendLine("        /// </summary>");
            fileContentSb.AppendLine("        /// <param name=\"loggerFactory\">日志</param>");
            fileContentSb.AppendLine("        /// <param name=\"dbFactory\">数据库工厂</param>");
            fileContentSb.AppendLine($"        public {_tableName}DalImpl(ILoggerFactory loggerFactory, IDbFactory dbFactory) : base(loggerFactory, dbFactory)");
            fileContentSb.AppendLine("        {");
            fileContentSb.AppendLine("        }");
            fileContentSb.AppendLine("    }");
            fileContentSb.AppendLine("}");
            return fileContentSb.ToString();
        }

        public void Generator(string saveRootDire, DataSourceDto dataSourceDto)
        {
            var ngroColumns = new List<string> { "Id", "Version", "LastUpdateTime", "LastUpdateUser", "IsDeleted", "CreateTime", "CreateUser", "Status", "TenantId" };
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dataSourceDto.DirPathName, "Configs/DaoImplConfig.json");
            var config = configPath.GetModel<ConfigModel>();
            if (config.Reference == null)
            {
                config.Reference = new List<string>();
            }
            config.Reference.Add($"{dataSourceDto.NameSpace}.Domain");
            config.NameSpace = dataSourceDto.NameSpace;


            string saveDir = Path.Combine(saveRootDire, $"{dataSourceDto.NameSpace}.DAL");
            foreach (var item in config.FileOutputPath.Split('/'))
            {
                saveDir = Path.Combine(saveDir, item);
                if (!System.IO.Directory.Exists(saveDir))
                {
                    System.IO.Directory.CreateDirectory(saveDir);
                }
            }


            dataSourceDto.DeleteNoExistsTable(saveDir,"", "DalImpl");
            foreach (var item in dataSourceDto.TableList)
            {

                var fileName = item.Code + "DalImpl.cs";
                var newDir = Path.Combine(saveDir, item.DirectoryCode);
                newDir.CreateDirectory();
                var filePath = Path.Combine(newDir, fileName);
                if (File.Exists(filePath) == false)
                {
                    var content = Build(item, config, ngroColumns);
                    File.WriteAllText(filePath, content);
                }
            }
            Console.WriteLine("生成DaoImpl层成功！！！");
        }
    }
}
