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
    public partial class SnClassAttributeListDto : SnClassAttribute
    {
        ///<summary>
        ///<para>类别表名称</para>
        ///</summary>
        ///</summary>
        [SugarColumn(IsIgnore = true)]
        [IgnorProperty]
        public virtual string ClassTableName { get; set; }
        ///<summary>
        ///<para>类别id：外键（类别表）</para>
        ///</summary>
        ///</summary>
        [SugarColumn(IsIgnore = true)]
        [IgnorProperty]
        public virtual string ClassName { get; set; }
        ///<summary>
        ///<para>类别编码</para>
        ///</summary>
        ///</summary>
        [SugarColumn(IsIgnore = true)]
        [IgnorProperty]
        public virtual string ClassCode { get; set; }
        ///<summary>
        ///<para>引用类别code</para>
        ///</summary>
        ///</summary>
        [SugarColumn(IsIgnore = true)]
        [IgnorProperty]
        public virtual string ReferenceClassCode { get; set; }
        ///<summary>
        ///<para>引用类别表明</para>
        ///</summary>
        ///</summary>
        [SugarColumn(IsIgnore = true)]
        [IgnorProperty]
        public virtual string ReferenceClassTableName { get; set; }
        ///<summary>
        ///<para>引用类别名称</para>
        ///</summary>
        ///</summary>
        [SugarColumn(IsIgnore = true)]
        [IgnorProperty]
        public virtual string ReferenceClassName { get; set; }
    }
}
