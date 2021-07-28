using System;
using NoRain.Reposition;
using System.ComponentModel.DataAnnotations;
using SqlSugar;
using System.ComponentModel.DataAnnotations.Schema;
namespace Sword.Nine.Domain
{
    /// <summary>
    /// 实体类：数据源表
    /// </summary>
    [Table("Sn_Data_Source")]
    [SugarTable("Sn_Data_Source")]
    public partial class SnDataSource: BaseEntity
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
            ///<para>数据库名称</para>
            ///</summary>
            [StringLength(64,ErrorMessage = "数据库名称 最长不能超 64 个字符!")]
            [SugarColumn(ColumnName ="DatabaseName")]
            [NoNull(ErrorMessage ="DatabaseName不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string DatabaseName { get; set; }

                  ///<summary>
            ///<para>描述</para>
            ///</summary>
            [StringLength(256,ErrorMessage = "描述 最长不能超 256 个字符!")]
            [SugarColumn(ColumnName ="Description")]
            [NoNull(ErrorMessage ="Description不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string Description { get; set; }

                  ///<summary>
            ///<para>主机</para>
            ///</summary>
            [StringLength(256,ErrorMessage = "主机 最长不能超 256 个字符!")]
            [SugarColumn(ColumnName ="Host")]
            [NoNull(ErrorMessage ="Host不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string Host { get; set; }

                  ///<summary>
            ///<para>名称</para>
            ///</summary>
            [StringLength(128,ErrorMessage = "名称 最长不能超 128 个字符!")]
            [SugarColumn(ColumnName ="Name")]
            [NoNull(ErrorMessage ="Name不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string Name { get; set; }

                  ///<summary>
            ///<para>命名空间</para>
            ///</summary>
            [StringLength(256,ErrorMessage = "命名空间 最长不能超 256 个字符!")]
            [SugarColumn(ColumnName ="NameSpace")]
            [NoNull(ErrorMessage ="NameSpace不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string NameSpace { get; set; }

                  ///<summary>
            ///<para>密码</para>
            ///</summary>
            [StringLength(256,ErrorMessage = "密码 最长不能超 256 个字符!")]
            [SugarColumn(ColumnName ="Password")]
            [NoNull(ErrorMessage ="Password不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string Password { get; set; }

                  ///<summary>
            ///<para>端口</para>
            ///</summary>
            [SugarColumn(ColumnName ="Port")]
            [NoNull(ErrorMessage ="Port不能为空")]
            public virtual int Port { get; set; }

                  ///<summary>
            ///<para>排序号</para>
            ///</summary>
            [SugarColumn(ColumnName ="SeqNo")]
            [NoNull(ErrorMessage ="SeqNo不能为空")]
            public virtual int SeqNo { get; set; }

                  ///<summary>
            ///<para>类型：【0：MYSQL】、【1：SQLServer】、【2：Oracle】、【3：SqlLite】</para>
            ///</summary>
            [SugarColumn(ColumnName ="Type")]
            [NoNull(ErrorMessage ="Type不能为空")]
            public virtual int Type { get; set; }

                  ///<summary>
            ///<para>用户名</para>
            ///</summary>
            [StringLength(128,ErrorMessage = "用户名 最长不能超 128 个字符!")]
            [SugarColumn(ColumnName ="UserName")]
            [NoNull(ErrorMessage ="UserName不能为空")]
            [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText="")]
            public virtual string UserName { get; set; }

    }
}
