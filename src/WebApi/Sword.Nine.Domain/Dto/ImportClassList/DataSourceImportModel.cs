using Sword.Nine.Domain;
using Sword.Nine.Untils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sword.Nine.Domain
{
    public class DataSourceImportModel
    {
        ///<summary>
        ///<para>编码</para>
        ///</summary>
        [ExcelImport("编码")]
        public string Code { get; set; }

        ///<summary>
        ///<para>数据库名称</para>
        ///</summary>
        [ExcelImport("数据库名称")]
        public string DatabaseName { get; set; }

        ///<summary>
        ///<para>描述</para>
        ///</summary>
        [ExcelImport("描述")]
        public string Description { get; set; }

        ///<summary>
        ///<para>主机</para>
        ///</summary>
        [ExcelImport("主机")]
        public string Host { get; set; }

        ///<summary>
        ///<para>名称</para>
        ///</summary>
        [ExcelImport("名称")]
        public string Name { get; set; }

        ///<summary>
        ///<para>命名空间</para>
        ///</summary>
        [ExcelImport("命名空间")]
        public string NameSpace { get; set; }

        ///<summary>
        ///<para>密码</para>
        ///</summary>
        [ExcelImport("密码")]
        public string Password { get; set; }

        ///<summary>
        ///<para>端口</para>
        ///</summary>
        [ExcelImport("端口", "number")]
        public int Port { get; set; }

        ///<summary>
        ///<para>排序号</para>
        ///</summary>
        [ExcelImport("序号")]
        public string SeqNo { get; set; }

        ///<summary>
        ///<para>类型：【0：MYSQL】、【1：SQLServer】、【2：Oracle】、【3：SqlLite】</para>
        ///</summary>
        [ExcelImport("类型")]
        public string Type { get; set; }

        ///<summary>
        ///<para>用户名</para>
        ///</summary>
        [ExcelImport("用户名")]
        public string UserName { get; set; }
        /// <summary>
        /// 转换成数据库实体
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public SnDataSource ToEntity(string userId)
        {
            if (int.TryParse(this.SeqNo, out int seqno) == false)
            {
                seqno = -1;
            }
            int type = 0;
            if (SystemConst.DataSourceType.ContainsKey(this.Type))
            {
                type = SystemConst.DataSourceType[this.Type];
            }
            var ret = new SnDataSource
            {
                Code = this.Code,
                DatabaseName = this.DatabaseName,
                Description = this.Description,
                Host = this.Host,
                Name = this.Name,
                NameSpace = this.NameSpace,
                Password = this.Password,
                Port = this.Port,
                UserName = this.UserName,
                SeqNo = seqno,
                Type = type
            };
            ret.SetUserId(userId);
            return ret;
        }
    }
}
