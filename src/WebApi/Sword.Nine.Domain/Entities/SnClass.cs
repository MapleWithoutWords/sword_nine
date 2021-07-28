using System;
using NoRain.Reposition;
using System.ComponentModel.DataAnnotations;
using SqlSugar;
using System.ComponentModel.DataAnnotations.Schema;
namespace Sword.Nine.Domain
{
    /// <summary>
    /// 实体类：类别表
    /// </summary>
    [Table("Sn_Class")]
    [SugarTable("Sn_Class")]
    public partial class SnClass: BaseEntity
    {
                  ///<summary>
            ///<para>类别目录id：外键（类别目录表）</para>
            ///</summary>
            [StringLength(36,ErrorMessage = "类别目录id：外键（类别目录表） 最长不能超 36 个字符!")]
            [SugarColumn(ColumnName ="ClassDirectoryId")]
            [NoNull(ErrorMessage ="ClassDirectoryId不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string ClassDirectoryId { get; set; }

                  ///<summary>
            ///<para>编码</para>
            ///</summary>
            [StringLength(128,ErrorMessage = "编码 最长不能超 128 个字符!")]
            [SugarColumn(ColumnName ="Code")]
            [NoNull(ErrorMessage ="Code不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string Code { get; set; }

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
            ///<para>父类、父类必须是抽象类</para>
            ///</summary>
            [SugarColumn(ColumnName ="ParentId")]
             [StringLength(36,ErrorMessage = "父类、父类必须是抽象类 最长不能超 36 个字符!")]
             public virtual string ParentId { get; set; }

                  ///<summary>
            ///<para>路径</para>
            ///</summary>
            [SugarColumn(ColumnName ="Path")]
             [StringLength(2048,ErrorMessage = "路径 最长不能超 2048 个字符!")]
             public virtual string Path { get; set; }

                  ///<summary>
            ///<para>备注</para>
            ///</summary>
            [StringLength(256,ErrorMessage = "备注 最长不能超 256 个字符!")]
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
            ///<para>表名称</para>
            ///</summary>
            [StringLength(128,ErrorMessage = "表名称 最长不能超 128 个字符!")]
            [SugarColumn(ColumnName ="TableName")]
            [NoNull(ErrorMessage ="TableName不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string TableName { get; set; }

                  ///<summary>
            ///<para>顶级类别id</para>
            ///</summary>
            [SugarColumn(ColumnName ="TopId")]
             [StringLength(36,ErrorMessage = "顶级类别id 最长不能超 36 个字符!")]
             public virtual string TopId { get; set; }

                  ///<summary>
            ///<para>类型：【0：业务类别】【1：抽象类】</para>
            ///</summary>
            [SugarColumn(ColumnName ="Type")]
            [NoNull(ErrorMessage ="Type不能为空")]
            public virtual int Type { get; set; }

    }
}
