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
    /// 规则类别属性配置表服务实现类
    /// </summary>
    public partial interface ISnRuleClassAttributeSettingDal : IBaseReposition<SnRuleClassAttributeSetting>
    {
    }
}
