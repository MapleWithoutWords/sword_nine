using Sword.Nine.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sword.Nine.Domain
{
    public class DirectoryImportModel
    {
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
        ///<para>上级目录id</para>
        ///</summary>
        [ExcelImport("上级目录序号")]
        public virtual string ParentId { get; set; }

        ///<summary>
        ///<para>排序号</para>
        ///</summary>
        [ExcelImport("序号")]
        public virtual string SeqNo { get; set; }

        public SnClassDirectory ToEntity( string userId)
        {
            if (int.TryParse(this.SeqNo, out int seqno) == false)
            {
                seqno = -1;
            }
            var en = new SnClassDirectory
            {
                 Code=this.Code,
                  DataSourceId=this.DataSourceId,
                   Name=this.Name,
                    ParentId=this.ParentId,
                     SeqNo= seqno,
            };
            en.SetUserId(userId);
            return en;
        }
    }
}
