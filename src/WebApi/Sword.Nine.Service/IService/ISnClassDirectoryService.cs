using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sword.Nine.Domain;
using NoRain.Service;
using System.ComponentModel.DataAnnotations.Schema;
using Common.APICommon;
using NoRain.Common;
using Sword.Nine.Domain.Dto.DataSource.directory;

namespace Sword.Nine.Service
{
    /// <summary>
    /// 类别目录服务实现类
    /// </summary>
    public partial interface ISnClassDirectoryService : IBaseService<SnClassDirectory>,IInjectFac
    {
        /// <summary>
        /// 获取目录树
        /// </summary>
        /// <param name="urlParams"></param>
        /// <returns></returns>
        Task<APIResult> GetDirectoryTreeAsync(List<UrlParams> urlParams);
        /// <summary>
        /// 保存绘图
        /// </summary>
        /// <param name="cellDtos"></param>
        /// <returns></returns>
        Task<APIResult> SaveDrawingAsync(DrawingSaveDto dto);
    }
}
