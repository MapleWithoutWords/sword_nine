using System;
using NoRain.Reposition;
using System.ComponentModel.DataAnnotations;
using SqlSugar;
using System.ComponentModel.DataAnnotations.Schema;
namespace Sword.Nine.Domain
{
    /// <summary>
    /// 实体类：规则表
    /// </summary>
    [Table("Sn_Rule")]
    [SugarTable("Sn_Rule")]
    public partial class SnRule: BaseEntity
    {
                  ///<summary>
            ///<para>编码</para>
            ///</summary>
            [StringLength(128,ErrorMessage = "编码 最长不能超 128 个字符!")]
            [SugarColumn(ColumnName ="Code")]
            [NoNull(ErrorMessage ="Code不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string Code { get; set; }

                  ///<summary>
            ///<para>描述</para>
            ///</summary>
            [StringLength(256,ErrorMessage = "描述 最长不能超 256 个字符!")]
            [SugarColumn(ColumnName ="Description")]
            [NoNull(ErrorMessage ="Description不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string Description { get; set; }

                  ///<summary>
            ///<para>枚举值</para>
            ///</summary>
            [SugarColumn(ColumnName ="EnumValue")]
            [NoNull(ErrorMessage ="EnumValue不能为空")]
            public virtual int EnumValue { get; set; }

                  ///<summary>
            ///<para>名称</para>
            ///</summary>
            [StringLength(128,ErrorMessage = "名称 最长不能超 128 个字符!")]
            [SugarColumn(ColumnName ="Name")]
            [NoNull(ErrorMessage ="Name不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string Name { get; set; }

                  ///<summary>
            ///<para>排序号</para>
            ///</summary>
            [SugarColumn(ColumnName ="SeqNo")]
            [NoNull(ErrorMessage ="SeqNo不能为空")]
            public virtual int SeqNo { get; set; }

                  ///<summary>
            ///<para>值类型：【0：布尔】</para>
            ///</summary>
            [SugarColumn(ColumnName ="ValueType")]
            [NoNull(ErrorMessage ="ValueType不能为空")]
            public virtual int ValueType { get; set; }

    }
}
