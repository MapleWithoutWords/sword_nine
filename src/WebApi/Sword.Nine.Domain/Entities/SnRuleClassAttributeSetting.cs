using System;
using NoRain.Reposition;
using System.ComponentModel.DataAnnotations;
using SqlSugar;
using System.ComponentModel.DataAnnotations.Schema;
namespace Sword.Nine.Domain
{
    /// <summary>
    /// 实体类：规则类别属性配置表
    /// </summary>
    [Table("Sn_Rule_Class_Attribute_Setting")]
    [SugarTable("Sn_Rule_Class_Attribute_Setting")]
    public partial class SnRuleClassAttributeSetting: BaseEntity
    {
                  ///<summary>
            ///<para>属性id</para>
            ///</summary>
            [StringLength(36,ErrorMessage = "属性id 最长不能超 36 个字符!")]
            [SugarColumn(ColumnName ="ClassAttributeId")]
            [NoNull(ErrorMessage ="ClassAttributeId不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string ClassAttributeId { get; set; }

                  ///<summary>
            ///<para>类别id</para>
            ///</summary>
            [StringLength(36,ErrorMessage = "类别id 最长不能超 36 个字符!")]
            [SugarColumn(ColumnName ="ClassId")]
            [NoNull(ErrorMessage ="ClassId不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string ClassId { get; set; }

                  ///<summary>
            ///<para>规则id</para>
            ///</summary>
            [StringLength(36,ErrorMessage = "规则id 最长不能超 36 个字符!")]
            [SugarColumn(ColumnName ="RuleId")]
            [NoNull(ErrorMessage ="RuleId不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string RuleId { get; set; }

                  ///<summary>
            ///<para>排序号</para>
            ///</summary>
            [SugarColumn(ColumnName ="SeqNo")]
            [NoNull(ErrorMessage ="SeqNo不能为空")]
            public virtual int SeqNo { get; set; }

                  ///<summary>
            ///<para>值</para>
            ///</summary>
            [StringLength(2048,ErrorMessage = "值 最长不能超 2048 个字符!")]
            [SugarColumn(ColumnName ="Value")]
            [NoNull(ErrorMessage ="Value不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string Value { get; set; }

    }
}
