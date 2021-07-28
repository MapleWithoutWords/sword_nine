using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoRain.Reposition
{
    /// <summary>
    /// sqlsugarAOP事务连接管理类
    /// </summary>
    public class SugarClient
    {
        /// <summary>
        /// 是否已经开启事务
        /// </summary>
        public bool IsTran { get; set; }
        /// <summary>
        /// 开启事务的次数
        /// </summary>
        public int TranCount { get; set; }

        /// <summary>
        /// sqlsugar实例
        /// </summary>
        public SqlSugarClient SqlSugarClient { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sqlSugarClient"></param>
        public SugarClient(SqlSugarClient sqlSugarClient)
        {
            this.SqlSugarClient = sqlSugarClient;
        }
    }
}
