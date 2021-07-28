using System;
using NoRain.Reposition;
using System.ComponentModel.DataAnnotations;
using SqlSugar;
using System.ComponentModel.DataAnnotations.Schema;
namespace Sword.Nine.Domain
{
    /// <summary>
    /// 实体类：模板表
    /// </summary>
    [Table("Sn_Template")]
    [SugarTable("Sn_Template")]
    public partial class SnTemplate: BaseEntity
    {
                  ///<summary>
            ///<para>程序所在目录</para>
            ///</summary>
            [StringLength(256,ErrorMessage = "程序所在目录 最长不能超 256 个字符!")]
            [SugarColumn(ColumnName ="AssemblyDirectory")]
            [NoNull(ErrorMessage ="AssemblyDirectory不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string AssemblyDirectory { get; set; }

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
            ///<para>模板名称</para>
            ///</summary>
            [StringLength(256,ErrorMessage = "模板名称 最长不能超 256 个字符!")]
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
            ///<para>启动文件名称</para>
            ///</summary>
            [StringLength(256,ErrorMessage = "启动文件名称 最长不能超 256 个字符!")]
            [SugarColumn(ColumnName ="StartFileName")]
            [NoNull(ErrorMessage ="StartFileName不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string StartFileName { get; set; }

                  ///<summary>
            ///<para>类型：【0：后端】、【1·：前端】</para>
            ///</summary>
            [SugarColumn(ColumnName ="Type")]
            [NoNull(ErrorMessage ="Type不能为空")]
            public virtual int Type { get; set; }

    }
}
