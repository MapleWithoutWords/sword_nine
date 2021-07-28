using Sword.Nine.Domain;
using Sword.Nine.Untils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sword.Nine.Domain
{
    public class AttributeImportModel
    {
        ///<summary>
        ///<para>类别id：外键（类别表）</para>
        ///</summary>
        [ExcelImport("类别序号")]
        public virtual string ClassId { get; set; }

        ///<summary>
        ///<para>编码</para>
        ///</summary>
        [ExcelImport("编码")]
        public virtual string Code { get; set; }

        ///<summary>
        ///<para>列名</para>
        ///</summary>
        [ExcelImport("列名")]
        public virtual string ColumnName { get; set; }

        ///<summary>
        ///<para>默认值</para>
        ///</summary>
        [ExcelImport("默认值")]
        public virtual string DefaultValue { get; set; }

        ///<summary>
        ///<para>描述</para>
        ///</summary>
        [ExcelImport("描述")]
        public virtual string Description { get; set; }

        ///<summary>
        ///<para>是否非空</para>
        ///</summary>
        [ExcelImport("是否非空")]
        public virtual string IsNullable { get; set; }

        ///<summary>
        ///<para>是否主键</para>
        ///</summary>
        [ExcelImport("是否主键")]
        public virtual string IsPrimary { get; set; }

        ///<summary>
        ///<para>长度</para>
        ///</summary>
        [ExcelImport("长度", "number")]
        public virtual int Length { get; set; }

        ///<summary>
        ///<para>名称</para>
        ///</summary>
        [ExcelImport("名称")]
        public virtual string Name { get; set; }

        ///<summary>
        ///<para>精度</para>
        ///</summary>
        [ExcelImport("精度", "number")]
        public virtual int Precision { get; set; }

        ///<summary>
        ///<para>引用类别id：</para>
        ///</summary>
        [ExcelImport("引用类别序号")]
        public virtual string ReferenceId { get; set; }

        ///<summary>
        ///<para>备注</para>
        ///</summary>
        [ExcelImport("备注")]
        public virtual string Remark { get; set; }

        ///<summary>
        ///<para>排序号</para>
        ///</summary>
        [ExcelImport("序号")]
        public virtual string SeqNo { get; set; }

        ///<summary>
        ///<para>值类型</para>
        ///</summary>
        [ExcelImport("值类型")]
        public virtual string ValueType { get; set; }

        public SnClassAttributeListDto ToEntity(string userId)
        {
            if (int.TryParse(this.SeqNo, out int seqno) == false)
            {
                seqno = -1;
            }
            bool isnullable = true;
            switch (this.IsNullable?.Trim())
            {
                case "是":
                    isnullable = true;
                    break;
                case "否":
                    isnullable = false;
                    break;
                default:
                    break;
            }
            bool isprimary = false;
            switch (this.IsPrimary?.Trim())
            {
                case "是":
                    isprimary = true;
                    break;
                case "否":
                    isprimary = false;
                    break;
                default:
                    break;
            }
            int valueType = 0;
            if (string.IsNullOrEmpty(this.ValueType)==false && SystemConst.DicValueType.ContainsKey(this.ValueType))
            {
                valueType = SystemConst.DicValueType[this.ValueType];
            }
            var en = new SnClassAttributeListDto
            {
                SeqNo = seqno,
                Code = this.Code,
                Name = this.Name,
                Remark = this.Remark,
                ClassId = this.ClassId,
                ColumnName = this.ColumnName,
                DefaultValue = this.DefaultValue==null?"": this.DefaultValue,
                Description = this.Description,
                IsNullable = isnullable,
                IsPrimary = isprimary,
                Length = this.Length,
                Precision = this.Precision,
                ValueType = valueType,
                ReferenceId = this.ReferenceId,
            };
            en.SetUserId(userId);
            return en;
        }
    }
}
