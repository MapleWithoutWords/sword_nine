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
    /// 数据源表服务实现类
    /// </summary>
    public partial interface ISnDataSourceService : IBaseService<SnDataSource>,IInjectFac
    {

        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="dataSourceId"></param>
        /// <param name="templateId"></param>
        /// <returns></returns>
        Task<APIResult> GeneratorCodeAsync(string dataSourceId, string templateId);

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<APIResult> ImportAsync(DataSoureceImportDto dto);
        /// <summary>
        /// 发布数据源
        /// </summary>
        /// <param name="dataSourceId"></param>
        /// <returns></returns>
        Task<APIResult> PublishAsync(string dataSourceId);
    }
}
