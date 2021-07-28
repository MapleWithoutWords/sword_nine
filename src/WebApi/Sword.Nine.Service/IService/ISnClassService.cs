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
    /// 类别表服务实现类
    /// </summary>
    public partial interface ISnClassService : IBaseService<SnClassDto>,IInjectFac
    {
        Task<int> PublishAsync(List<string> classIds);

        /// <summary>
        /// 获取目录类别树
        /// </summary>
        /// <param name="dsId"></param>
        /// <returns></returns>
        Task<APIResult> GetDirectoryClassAsync(List<UrlParams> urlParams);

        /// <summary>
        /// 获取类别信息以及属性
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        Task<APIResult> GetInfoAndAttributeAsync(string classId);
    }
}
