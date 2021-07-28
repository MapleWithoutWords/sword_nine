using System;
using NoRain.Reposition;
using System.ComponentModel.DataAnnotations;
using SqlSugar;
using System.ComponentModel.DataAnnotations.Schema;
namespace Sword.Nine.Domain
{
    /// <summary>
    /// 实体类：类别属性
    /// </summary>
    [Table("Sn_Class_Attribute")]
    [SugarTable("Sn_Class_Attribute")]
    public partial class SnClassAttribute: BaseEntity
    {
                  ///<summary>
            ///<para>类别id：外键（类别表）</para>
            ///</summary>
            [StringLength(36,ErrorMessage = "类别id：外键（类别表） 最长不能超 36 个字符!")]
            [SugarColumn(ColumnName ="ClassId")]
            [NoNull(ErrorMessage ="ClassId不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string ClassId { get; set; }

                  ///<summary>
            ///<para>编码</para>
            ///</summary>
            [StringLength(128,ErrorMessage = "编码 最长不能超 128 个字符!")]
            [SugarColumn(ColumnName ="Code")]
            [NoNull(ErrorMessage ="Code不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string Code { get; set; }

                  ///<summary>
            ///<para>列名</para>
            ///</summary>
            [StringLength(128,ErrorMessage = "列名 最长不能超 128 个字符!")]
            [SugarColumn(ColumnName ="ColumnName")]
            [NoNull(ErrorMessage ="ColumnName不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string ColumnName { get; set; }

                  ///<summary>
            ///<para>所属数据源id</para>
            ///</summary>
            [StringLength(36,ErrorMessage = "所属数据源id 最长不能超 36 个字符!")]
            [SugarColumn(ColumnName ="DataSourceId")]
            [NoNull(ErrorMessage ="DataSourceId不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string DataSourceId { get; set; }

                  ///<summary>
            ///<para>默认值</para>
            ///</summary>
            [StringLength(128,ErrorMessage = "默认值 最长不能超 128 个字符!")]
            [SugarColumn(ColumnName ="DefaultValue")]
            [NoNull(ErrorMessage ="DefaultValue不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string DefaultValue { get; set; }

                  ///<summary>
            ///<para>描述</para>
            ///</summary>
            [StringLength(256,ErrorMessage = "描述 最长不能超 256 个字符!")]
            [SugarColumn(ColumnName ="Description")]
            [NoNull(ErrorMessage ="Description不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string Description { get; set; }

                  ///<summary>
            ///<para>继承属性id：（该属性是从上级类别哪个属性继承下来的，默认为空字符串）</para>
            ///</summary>
            [StringLength(36,ErrorMessage = "继承属性id：（该属性是从上级类别哪个属性继承下来的，默认为空字符串） 最长不能超 36 个字符!")]
            [SugarColumn(ColumnName ="InheritId")]
            [NoNull(ErrorMessage ="InheritId不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string InheritId { get; set; }

                  ///<summary>
            ///<para>是否非空</para>
            ///</summary>
            [SugarColumn(ColumnName ="IsNullable")]
            [NoNull(ErrorMessage ="IsNullable不能为空")]
            public virtual bool IsNullable { get; set; }

                  ///<summary>
            ///<para>是否主键</para>
            ///</summary>
            [SugarColumn(ColumnName ="IsPrimary")]
            [NoNull(ErrorMessage ="IsPrimary不能为空")]
            public virtual bool IsPrimary { get; set; }

                  ///<summary>
            ///<para>长度</para>
            ///</summary>
            [SugarColumn(ColumnName ="Length")]
            [NoNull(ErrorMessage ="Length不能为空")]
            public virtual int Length { get; set; }

                  ///<summary>
            ///<para>名称</para>
            ///</summary>
            [StringLength(128,ErrorMessage = "名称 最长不能超 128 个字符!")]
            [SugarColumn(ColumnName ="Name")]
            [NoNull(ErrorMessage ="Name不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string Name { get; set; }

                  ///<summary>
            ///<para>精度</para>
            ///</summary>
            [SugarColumn(ColumnName ="Precision")]
            [NoNull(ErrorMessage ="Precision不能为空")]
            public virtual int Precision { get; set; }

                  ///<summary>
            ///<para>引用l类别id：</para>
            ///</summary>
            [StringLength(36,ErrorMessage = "引用l类别id： 最长不能超 36 个字符!")]
            [SugarColumn(ColumnName ="ReferenceId")]
            [NoNull(ErrorMessage ="ReferenceId不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string ReferenceId { get; set; }

                  ///<summary>
            ///<para>备注</para>
            ///</summary>
            [StringLength(128,ErrorMessage = "备注 最长不能超 128 个字符!")]
            [SugarColumn(ColumnName ="Remark")]
            [NoNull(ErrorMessage ="Remark不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string Remark { get; set; }

                  ///<summary>
            ///<para>排序号</para>
            ///</summary>
            [SugarColumn(ColumnName ="SeqNo")]
            [NoNull(ErrorMessage ="SeqNo不能为空")]
            public virtual int SeqNo { get; set; }

                  ///<summary>
            ///<para>值类型</para>
            ///</summary>
            [SugarColumn(ColumnName ="ValueType")]
            [NoNull(ErrorMessage ="ValueType不能为空")]
            public virtual int ValueType { get; set; }

    }
}
