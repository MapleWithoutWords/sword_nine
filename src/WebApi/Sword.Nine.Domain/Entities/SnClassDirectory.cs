using System;
using NoRain.Reposition;
using System.ComponentModel.DataAnnotations;
using SqlSugar;
using System.ComponentModel.DataAnnotations.Schema;
namespace Sword.Nine.Domain
{
    /// <summary>
    /// 实体类：类别目录
    /// </summary>
    [Table("Sn_Class_Directory")]
    [SugarTable("Sn_Class_Directory")]
    public partial class SnClassDirectory: BaseEntity
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
            ///<para>内容</para>
            ///</summary>
            [SugarColumn(ColumnName ="Content")]
            [NoNull(ErrorMessage ="Content不能为空")]
            public virtual string Content { get; set; }

                  ///<summary>
            ///<para>数据源id：外键（数据源表）</para>
            ///</summary>
            [StringLength(36,ErrorMessage = "数据源id：外键（数据源表） 最长不能超 36 个字符!")]
            [SugarColumn(ColumnName ="DataSourceId")]
            [NoNull(ErrorMessage ="DataSourceId不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string DataSourceId { get; set; }

                  ///<summary>
            ///<para>名称</para>
            ///</summary>
            [StringLength(128,ErrorMessage = "名称 最长不能超 128 个字符!")]
            [SugarColumn(ColumnName ="Name")]
            [NoNull(ErrorMessage ="Name不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string Name { get; set; }

                  ///<summary>
            ///<para>上级目录id</para>
            ///</summary>
            [SugarColumn(ColumnName ="ParentId")]
             [StringLength(36,ErrorMessage = "上级目录id 最长不能超 36 个字符!")]
             public virtual string ParentId { get; set; }

                  ///<summary>
            ///<para>目录路径</para>
            ///</summary>
            [SugarColumn(ColumnName ="Path")]
             [StringLength(512,ErrorMessage = "目录路径 最长不能超 512 个字符!")]
             public virtual string Path { get; set; }

                  ///<summary>
            ///<para>排序号</para>
            ///</summary>
            [SugarColumn(ColumnName ="SeqNo")]
            [NoNull(ErrorMessage ="SeqNo不能为空")]
            public virtual int SeqNo { get; set; }

    }
}
