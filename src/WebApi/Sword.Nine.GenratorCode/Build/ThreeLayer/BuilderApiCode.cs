using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sword.Nine.GenratorCode.Build.ThreeLayer
{
    public class BuilderApiCode : IBuilderCode
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

            fileContentSb.AppendLine($"");
            fileContentSb.AppendLine($"namespace {config.NameSpace}.Api.Controllers");
            fileContentSb.AppendLine("{");
            fileContentSb.AppendLine($"    /// <summary>");
            fileContentSb.AppendLine($"    /// {desc}");
            fileContentSb.AppendLine($"    /// </summary>");
            fileContentSb.AppendLine($"    [Route(\"api/{tb._TableName.ToLower()}\")]");
            fileContentSb.AppendLine($"    [ApiController]");
            fileContentSb.AppendLine($"    public partial class {tb._TableName}Controller: BaseApiController<I{tb._TableName}Service, {tb._TableName}>");
            fileContentSb.AppendLine("    {");
            fileContentSb.AppendLine("        /// <summary>");
            fileContentSb.AppendLine("        /// 构造函数");
            fileContentSb.AppendLine("        /// </summary>");
            fileContentSb.AppendLine($"        /// <param name=\"service\">{desc}服务</param>");
            fileContentSb.AppendLine($"        /// <param name=\"loggerFactory\">日志服务</param>>");
            fileContentSb.AppendLine($"        public {tb._TableName}Controller(I{tb._TableName}Service service,ILoggerFactory loggerFactory) : base(service,loggerFactory)");
            fileContentSb.AppendLine("        {");
            fileContentSb.AppendLine("        }");
            fileContentSb.AppendLine("    }");
            fileContentSb.AppendLine("}");
            return fileContentSb.ToString();
        }

        public void Generator(string saveRootDire,List<DbTable> tableList)
        {
            DbHelper Db = new DbHelper();
            var config = Db.GetModels("./Configs/ApiConfig.json");
            //var tableList = Db.GetTableInfo(new List<string>(), config);
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

            if (config.IsNotExistsDelete)
            {
                var fileList = Directory.GetFiles(saveDir);
                foreach (var item in fileList)
                {
                    var fileName = Path.GetFileName(item);
                    if (tableList.Any(e => $"{e._TableName}Controller.cs" == fileName))
                    {
                        continue;
                    }
                    File.Delete(item);
                }
            }
            foreach (var item in tableList)
            {
                var fileName = item._TableName + "Controller.cs";
                var filePath = Path.Combine(saveDir, fileName);
                if (File.Exists(filePath)==false)
                {
                    var content = Build(item, config, ngroColumns);
                    File.WriteAllText(filePath, content);
                }
            }
            Console.WriteLine("生成 Api 层成功！！！");
        }
    }
}
