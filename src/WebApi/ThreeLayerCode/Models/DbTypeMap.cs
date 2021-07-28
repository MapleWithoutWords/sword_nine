using Sword.Nine.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeLayerCode
{
    /// <summary>
    /// 数据类型映射
    /// </summary>
    public class DbTypeMap
    {
        public static string GetColumnAttribute(AttributeDto column)
        {
            var noRequiries = new List<string> { "Path", "ApplicationKey", "TopId", "Path", "ParentId", "Certificate" };
            StringBuilder sb = new StringBuilder();
            var str = string.Empty;
            var csDataType = MapToCsharpType(column.ValueType);
            var columnDesc = "";

            var NullableRule = column.RuleDatas.FirstOrDefault(e => e.RuleType == 0);
            var RegexRule = column.RuleDatas.FirstOrDefault(e => e.RuleType == 1 );
            var RegexRuleStr = $"";
            if (RegexRule != null)
            {
                RegexRuleStr += $"            [RegularExpression(@\"{RegexRule.Data}\",ErrorMessage =\"{column.Name}不符合规则！\")]";
            }
            var NullableRuleStr = "            [NoNull(ErrorMessage =\"" + column.Name + "不能为空\")]";
            if (NullableRule != null)
            {
                NullableRuleStr = "            [Required(ErrorMessage =\"" + column.Name + "不能为空\")]";
            }

            if (!string.IsNullOrEmpty(column.Remark))
            {
                var nameList = column.Remark.Replace("\r\n", "┃").Split('┃');
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
                columnDesc = "///" + column.Remark;
            }
            sb.AppendLine("           ///<summary>");
            sb.AppendLine("           " + columnDesc);
            sb.AppendLine("            ///</summary>");
            if (column.ColumnName.ToLower() == "id")
            {
                sb.AppendLine("            [Key]");
                sb.AppendLine("            [Required(ErrorMessage =\"" + column.ColumnName + "不能为null,可以为空字符串''\",AllowEmptyStrings =true)]");
                sb.AppendLine("            [StringLength(" + column.Length + ",ErrorMessage = \"" + column.Name + "最长不能超" + column.Length + "个字符!\")]");
                sb.AppendLine("            [SugarColumn(ColumnName =\"" + column.Length + "\",IsPrimaryKey =true)]");
                sb.AppendLine("            public virtual " + csDataType + " " + column.Code + " { get; set; }=\"\";");
            }
            else if ((column.ValueType == ValueTypeEnum.String))
            {
                if (noRequiries.Contains(column.ColumnName))
                {

                    sb.AppendLine("            [SugarColumn(ColumnName =\"" + column.ColumnName + "\")]");
                    sb.AppendLine("             [StringLength(" + column.Length + ",ErrorMessage = \"" + column.Remark.Replace("\r\n", "") + " 最长不能超 " + column.Length + " 个字符!\")]");
                    sb.AppendLine(RegexRuleStr);
                    sb.AppendLine("             public virtual " + csDataType + " " + column.Code + " { get; set; }");

                }
                else
                {
                    sb.AppendLine(NullableRuleStr);
                    sb.AppendLine(RegexRuleStr);
                    sb.AppendLine("            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText=\"\")]");
                    sb.AppendLine("            [StringLength(" + column.Length + ",ErrorMessage = \"" + column.Remark.Replace("\r\n", "") + " 最长不能超 " + column.Length + " 个字符!\")]");
                    sb.AppendLine("            [SugarColumn(ColumnName =\"" + column.ColumnName + "\")]");
                    sb.AppendLine("            public virtual " + csDataType + " " + column.Code + " { get; set; }");
                }
            }
            else
            {
                sb.AppendLine(NullableRuleStr);
                sb.AppendLine(RegexRuleStr);
                sb.AppendLine("            [SugarColumn(ColumnName =\"" + column.ColumnName + "\")]");
                sb.AppendLine("            public virtual " + csDataType + " " + column.Code + " { get; set; }");
            }
            return str = sb.ToString();
        }
        public static string MapToCsharpType(ValueTypeEnum valueType)
        {
            string sysType = "string";
            switch (valueType)
            {
                case ValueTypeEnum.String:
                    break;
                case ValueTypeEnum.Int:
                    sysType = "int";
                    break;
                case ValueTypeEnum.Float:
                    sysType = "float";
                    break;
                case ValueTypeEnum.DateTime:
                    sysType = "datetime";
                    break;
                case ValueTypeEnum.Reference:
                    sysType = "string";
                    break;
                case ValueTypeEnum.Long:
                    sysType = "long";
                    break;
                case ValueTypeEnum.Double:
                    sysType = "double";
                    break;
                case ValueTypeEnum.Bool:
                    sysType = "bool";
                    break;
                case ValueTypeEnum.Decimal:
                    sysType = "decimal";
                    break;
                default:
                    break;
            }
            return sysType;
        }
    }
}
