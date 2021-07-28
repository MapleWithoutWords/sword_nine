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
    /// 数据源表服务实现类
    /// </summary>
    public partial interface ISnDataSourceDal : IBaseReposition<SnDataSource>
    {
    }
}
