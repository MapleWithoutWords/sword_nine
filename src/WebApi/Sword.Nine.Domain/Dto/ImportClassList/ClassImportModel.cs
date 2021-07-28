using Sword.Nine.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sword.Nine.Domain
{
    public class ClassImportModel
    {
        ///<summary>
        ///<para>类别目录id：外键（类别目录表）</para>
        ///</summary>
        [ExcelImport("目录序号")]
        public virtual string ClassDirectoryId { get; set; }

        ///<summary>
        ///<para>编码</para>
        ///</summary>
        [ExcelImport("编码")]
        public virtual string Code { get; set; }

        ///<summary>
        ///<para>数据源id：外键（数据源表）</para>
        ///</summary>
        [ExcelImport("数据源序号")]
        public virtual string DataSourceId { get; set; }

        ///<summary>
        ///<para>名称</para>
        ///</summary>
        [ExcelImport("名称")]
        public virtual string Name { get; set; }

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
        ///<para>表名称</para>
        ///</summary>
        [ExcelImport("表名称")]
        public virtual string TableName { get; set; }

        public SnClassDto ToEntity(string userId)
        {
            if (int.TryParse(this.SeqNo, out int seqno) == false)
            {
                seqno = -1;
            }
            var en = new SnClassDto
            {
                SeqNo = seqno,
                ClassDirectoryId = this.ClassDirectoryId,
                Code = this.Code,
                DataSourceId = this.DataSourceId,
                Name = this.Name,
                Remark = this.Remark,
                TableName = this.TableName
            };
            en.SetUserId(userId);
            return en;
        }
    }
}
