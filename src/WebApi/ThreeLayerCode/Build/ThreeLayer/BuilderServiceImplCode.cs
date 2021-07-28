using Sword.Nine.Rule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeLayerCode
{
    public class BuilderServiceImplCode : IBuilderCode
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
            //获取该表内所有的引用属性
            var refAttrList = tb.AttrDatas.Where(e => e.ValueType == ValueTypeEnum.Reference);

            fileContentSb.AppendLine($"namespace {config.NameSpace}.Service");
            fileContentSb.AppendLine("{");
            fileContentSb.AppendLine($"    /// <summary>");
            fileContentSb.AppendLine($"    /// {desc}服务实现类");
            fileContentSb.AppendLine($"    /// </summary>");
            fileContentSb.AppendLine($"    public partial class {_tableName}ServiceImpl: BaseServiceImpl<{_tableName }Entity>,I{_tableName }Service");
            fileContentSb.AppendLine("    {");
            fileContentSb.AppendLine("        /// <sumary>");
            fileContentSb.AppendLine($"       /// {desc}数据层");
            fileContentSb.AppendLine("        /// </sumary>");
            fileContentSb.AppendLine($"        public I{_tableName}Dal DalImpl {{get;set;}}");

            //处理引用dal层及赋值
            StringBuilder dalSb = new StringBuilder();
            StringBuilder assignmentSb = new StringBuilder();
            HandleReferenceDal(fileContentSb, refAttrList, dalSb, assignmentSb);

            //不更新的字段
            var noUpdateCodeString = GetNotUpdateCode(tb);

            fileContentSb.AppendLine("        /// <sumary>");
            fileContentSb.AppendLine($"       /// {desc}构造函数");
            fileContentSb.AppendLine("        /// </sumary>");
            fileContentSb.AppendLine("        /// <param name=\"loggerFactory\">日志</param>");
            fileContentSb.AppendLine("        /// <param name=\"baseReposition\">基类数据访问层</param>");
            fileContentSb.AppendLine($"        public {_tableName }ServiceImpl(ILoggerFactory loggerFactory, IBaseReposition<{_tableName}Entity> baseReposition,I{_tableName}Dal dalImpl{dalSb},IHttpContextAccessor conotext) : base(loggerFactory,baseReposition,conotext)");
            fileContentSb.AppendLine("        {");
            fileContentSb.AppendLine("            this.DalImpl=dalImpl;");
            fileContentSb.AppendLine(assignmentSb.ToString());
            fileContentSb.AppendLine(noUpdateCodeString);
            fileContentSb.AppendLine("        }");

            //验证数据代码
            var validDataCodeString = GetValidCode(tb);
            fileContentSb.AppendLine(validDataCodeString);

            fileContentSb.AppendLine("    }");
            fileContentSb.AppendLine("}");
            return fileContentSb.ToString();
        }

        private static void HandleReferenceDal(StringBuilder fileContentSb, IEnumerable<AttributeDto> refAttrList, StringBuilder dalSb, StringBuilder assignmentSb)
        {
            foreach (var item in refAttrList)
            {
                fileContentSb.AppendLine("        /// <sumary>");
                fileContentSb.AppendLine($"       /// {item.ReferenceClassName}数据层");
                fileContentSb.AppendLine("        /// </sumary>");
                fileContentSb.AppendLine($"        public I{item.ReferenceClassCode}Dal {item.ReferenceClassCode}Dal {{get;set;}}");

                dalSb.Append($", I{item.ReferenceClassCode}Dal {item.ReferenceClassCode}Dal");
                assignmentSb.AppendLine($"            this.{item.ReferenceClassCode}Dal={item.ReferenceClassCode}Dal;");
            }
        }

        /// <summary>
        /// 获取验证数据代码
        /// </summary>
        /// <param name="tb"></param>
        /// <returns></returns>
        private string GetValidCode(TableDto tb)
        {

            var refAttrList = tb.AttrDatas.Where(e => e.ValueType == ValueTypeEnum.Reference);
            var onlyAttrList = tb.AttrDatas.Where(e => e.RuleDatas.Any(x => x.RuleType == 2));
            var validDataSb = new StringBuilder();

            validDataSb.AppendLine($"        public override async Task<APIResult> ValidDataAsync({tb.Code}Entity entity)");
            validDataSb.AppendLine("        {");
            foreach (var item in refAttrList)
            {
                validDataSb.AppendLine($"            if (await {item.ReferenceClassCode}Dal.CountAsync(e => e.IsDeleted == false && entity.{item.Code} == e.Id) < 1)");
                validDataSb.AppendLine($"            {{");
                validDataSb.AppendLine($"                return APIResult.ErrorResult(\"{item.Name}非法\");");
                validDataSb.AppendLine($"            }}");
            }

            string onlyCodeString = string.Join("||", onlyAttrList.Select(item => $"entity.{item.Code}==e.{item.Code}"));
            if (string.IsNullOrEmpty(onlyCodeString)==false)
            {
                onlyCodeString = $" && ({onlyCodeString})";
                validDataSb.AppendLine($"            if (await DalImpl.CountAsync(e => e.IsDeleted == false && entity.Id != e.Id{onlyCodeString} ) > 0)");
                validDataSb.AppendLine($"            {{");
                validDataSb.AppendLine($"                return APIResult.ErrorResult(\"{string.Join('或', onlyAttrList.Select(e => e.Code))}，非法\");");
                validDataSb.AppendLine($"            }}");
            }
            validDataSb.AppendLine($"            return await base.ValidDataAsync(entity);");


            validDataSb.AppendLine("        }");
            return validDataSb.ToString();
        }
        /// <summary>
        /// 获取不更新字段代码
        /// </summary>
        /// <param name="tb"></param>
        /// <returns></returns>
        private string GetNotUpdateCode(TableDto tb)
        {
            var notUpdateAttrList = tb.AttrDatas.Where(e => e.RuleDatas.Any(x => x.RuleType == 3));
            return $"            UpdateIgnorPros = new List<string> {{ {string.Join(",", notUpdateAttrList.Select(e => $"\"{e.Code}\""))} }};";
        }

        public void Generator(string saveRootDire, DataSourceDto dataSourceDto)
        {

            var ngroColumns = new List<string> { "Id", "Version", "LastUpdateTime", "LastUpdateUser", "IsDeleted", "CreateTime", "CreateUser", "Status", "TenantId" };
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dataSourceDto.DirPathName, "Configs/ServiceImplConfig.json");
            var config = configPath.GetModel<ConfigModel>();
            if (config.Reference == null)
            {
                config.Reference = new List<string>();
            }
            config.Reference.Add($"{dataSourceDto.NameSpace}.Domain");
            config.Reference.Add($"{dataSourceDto.NameSpace}.DAL");
            config.NameSpace = dataSourceDto.NameSpace;


            string saveDir = Path.Combine(saveRootDire, $"{dataSourceDto.NameSpace}.Service");
            foreach (var item in config.FileOutputPath.Split('/'))
            {
                saveDir = Path.Combine(saveDir, item);
                if (!System.IO.Directory.Exists(saveDir))
                {
                    System.IO.Directory.CreateDirectory(saveDir);
                }
            }


            dataSourceDto.DeleteNoExistsTable(saveDir, "", "ServiceImpl");
            foreach (var item in dataSourceDto.TableList)
            {

                var fileName = $"{item.Code }ServiceImpl.cs";
                var newDir = Path.Combine(saveDir, item.DirectoryCode);
                newDir.CreateDirectory();
                var filePath = Path.Combine(newDir, fileName);
                if (File.Exists(filePath) == false)
                {
                    var content = Build(item, config, ngroColumns);
                    File.WriteAllText(filePath, content);
                }
            }

            Console.WriteLine("生成 ServiceImpl 层成功！！！");
        }
    }
}
