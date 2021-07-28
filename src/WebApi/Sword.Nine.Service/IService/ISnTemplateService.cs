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
    /// 模板表服务实现类
    /// </summary>
    public partial interface ISnTemplateService : IBaseService<SnTemplate>,IInjectFac
    {
    }
}
