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
    /// 类别属性服务实现类
    /// </summary>
    public partial interface ISnClassAttributeDal : IBaseReposition<SnClassAttributeListDto>
    {

        /// <summary>
        /// 根据数据源id获取属性
        /// </summary>
        /// <param name="dsId"></param>
        /// <returns></returns>
        Task<List<SnClassAttributeListDto>> GetByDataSourceIdAsync(string dsId);
    }
}
