using Sword.Nine.Rule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeLayerCode
{
    public class BuilderApiCode : IBuilderCode
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

            fileContentSb.AppendLine($"");
            fileContentSb.AppendLine($"namespace {config.NameSpace}.Api.Controllers");
            fileContentSb.AppendLine("{");
            fileContentSb.AppendLine($"    /// <summary>");
            fileContentSb.AppendLine($"    /// {desc}");
            fileContentSb.AppendLine($"    /// </summary>");
            fileContentSb.AppendLine($"    [Route(\"api/{tb.Code.ToLower()}\")]");
            fileContentSb.AppendLine($"    [ApiController]");
            fileContentSb.AppendLine($"    public partial class {tb.Code}Controller: BaseApiController<I{tb.Code}Service, {tb.Code}Entity>");
            fileContentSb.AppendLine("    {");
            fileContentSb.AppendLine("        /// <summary>");
            fileContentSb.AppendLine("        /// 构造函数");
            fileContentSb.AppendLine("        /// </summary>");
            fileContentSb.AppendLine($"        /// <param name=\"service\">{desc}服务</param>");
            fileContentSb.AppendLine($"        /// <param name=\"loggerFactory\">日志服务</param>>");
            fileContentSb.AppendLine($"        public {tb.Code}Controller(I{tb.Code}Service service,ILoggerFactory loggerFactory) : base(service,loggerFactory)");
            fileContentSb.AppendLine("        {");
            fileContentSb.AppendLine("        }");
            fileContentSb.AppendLine("    }");
            fileContentSb.AppendLine("}");
            return fileContentSb.ToString();
        }

        public void Generator(string saveRootDire, DataSourceDto dataSourceDto)
        {
            var ngroColumns = new List<string> { "Id", "Version", "LastUpdateTime", "LastUpdateUser", "IsDeleted", "CreateTime", "CreateUser", "Status" };
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dataSourceDto.DirPathName, "Configs/ApiConfig.json");
            var config = configPath.GetModel<ConfigModel>();
            if (config.Reference==null)
            {
                config.Reference = new List<string>();
            }
            config.Reference.Add($"{dataSourceDto.NameSpace}.Service");
            config.Reference.Add($"{dataSourceDto.NameSpace}.Domain");
            config.NameSpace = dataSourceDto.NameSpace;


            string saveDir = Path.Combine(saveRootDire,$"{dataSourceDto.NameSpace}.WebApi");
            foreach (var item in config.FileOutputPath.Split('/'))
            {
                saveDir = Path.Combine(saveDir, item);
                if (!System.IO.Directory.Exists(saveDir))
                {
                    System.IO.Directory.CreateDirectory(saveDir);
                }
            }


            dataSourceDto.DeleteNoExistsTable(saveDir,"", "Controller");
            foreach (var item in dataSourceDto.TableList)
            {

                var fileName = item.Code + "Controller.cs";
                var newDir = Path.Combine(saveDir, item.DirectoryCode);
                newDir.CreateDirectory();
                var filePath = Path.Combine(newDir, fileName);
                if (File.Exists(filePath) == false)
                {
                    var content = Build(item, config, ngroColumns);
                    File.WriteAllText(filePath, content);
                }
            }
            Console.WriteLine("生成 Api 层成功！！！");
        }
    }
}
