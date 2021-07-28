using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sword.Nine.Rule
{
    public class AttributeDto
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }
        public int Length { get; set; }
        /// <summary>
        /// 属性名称备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 值类型
        /// </summary>
        public ValueTypeEnum ValueType { get; set; }

        /// <summary>
        /// 引用实体类名称
        /// </summary>
        public string ReferenceClassName { get; set; }
        ///<summary>
        ///<para>引用类别code</para>
        ///</summary>
        ///</summary>
        public virtual string ReferenceClassCode { get; set; }
        /// <summary>
        /// 引用实体类表名称
        /// </summary>
        public string ReferenceClassTableName { get; set; }

        public string ClassName { get; set; }
        public string ClassTableName { get; set; }
        /// <summary>
        /// 规则
        /// </summary>
        public List<RuleDto> RuleDatas { get; set; } = new List<RuleDto>();
    }

    public class RuleDto
    {
        public int RuleType { get; set; }
        /// <summary>
        /// 数据
        /// Nullable:【null】
        /// NoSpecDefaultValue=1,
        /// Regex：存放正则表达式【String】
        /// Uniqueness=存放与当前属性组合成唯一值的其它属性【List<AttributeDto>】
        /// Reference=存放其它类别的属性【AttributeDto】
        /// </summary>
        public object Data { get; set; }

    }



    public class TableDto
    {
        public string DirectoryCode { get; set; }
        /// <summary>
        /// 中文名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 实体类名称
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 数据库表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 实体备注
        /// </summary>
        public string Remark { get; set; }
        public List<AttributeDto> AttrDatas { get; set; } = new List<AttributeDto>();

        ///// <summary>
        ///// 规则
        ///// </summary>
        //public List<RuleDto> RuleDatas { get; set; } = new List<RuleDto>();

    }


    public class DataSourceDto
    {
        /// <summary>
        /// 项目名称，项目前缀
        /// </summary>
        public string NameSpace { get; set; }
        public string ProjectName { get; set; }
        /// <summary>
        /// 目录下
        /// </summary>
        public string DirPathName { get; set; } = "";

        /// <summary>
        /// 表
        /// </summary>
        public List<TableDto> TableList { get; set; } = new List<TableDto>();

        /// <summary>
        /// 删除不存在的表
        /// </summary>
        /// <param name="dataSourceDto"></param>
        /// <param name="fileList"></param>
        public void DeleteNoExistsTable(string saveDir,string leftSuffix, string rightSuffix)
        {

            foreach (var item in this.TableList.GroupBy(e => e.DirectoryCode).Where(e=>string.IsNullOrEmpty(e.Key)==false).Select(e => e.Key))
            {
                var newDir = Path.Combine(saveDir, item);
                if (Directory.Exists(newDir)==false)
                {
                    continue;
                }
                var fileList = Directory.GetFiles(newDir);
                var dirTab = this.TableList.Where(e => e.DirectoryCode == item);

                foreach (var file in fileList)
                {
                    var fileName = Path.GetFileName(file);
                    if (dirTab.Any(e => $"{leftSuffix}{e.Code}{rightSuffix}.cs" == fileName))
                    {
                        continue;
                    }
                    File.Delete(file);
                }
            }
        }
    }
}

