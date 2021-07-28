using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sword.Nine.Domain;
using NoRain.Service;
using System.ComponentModel.DataAnnotations.Schema;
using Common.APICommon;
using NoRain.Common;
namespace Sword.Nine.Service
{
    /// <summary>
    /// 类别属性服务实现类
    /// </summary>
    public partial interface ISnClassAttributeService : IBaseService<SnClassAttributeListDto>,IInjectFac
    {

        /// <summary>
        /// 保存类别属性
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<APIResult> SaveAsync(List<SnClassAttributeListDto> entities);
    }
}
