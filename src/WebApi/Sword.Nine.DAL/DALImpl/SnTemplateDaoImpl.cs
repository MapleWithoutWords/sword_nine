using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoRain.Reposition;
using Microsoft.Extensions.Logging;
using NoRain.Reposition.BaseReposition;
using Sword.Nine.Domain;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
namespace Sword.Nine.Dao
{
    /// <summary>
    /// 模板表服务实现类
    /// </summary>
    public partial class SnTemplateDalImpl: BaseReposition<SnTemplate>,ISnTemplateDal
    {
        /// <summary>
        /// {desc}构造函数
        /// </summary>
        /// <param name="loggerFactory">日志</param>
        /// <param name="dbFactory">数据库工厂</param>
        public SnTemplateDalImpl(ILoggerFactory loggerFactory, IDbFactory dbFactory) : base(loggerFactory, dbFactory)
        {
        }
    }
}
