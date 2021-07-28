using System;
using Microsoft.Extensions.Logging;
using Sword.Nine.Domain;
using Sword.Nine.Dao;
using NoRain.Reposition.BaseReposition;
using NoRain.Service;
using NoRain.Common;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace Sword.Nine.Service
{
    /// <summary>
    /// 模板表服务实现类
    /// </summary>
    public partial class SnTemplateServiceImpl : BaseServiceImpl<SnTemplate>, ISnTemplateService
    {
        /// <sumary>
        /// 模板表数据层
        /// </sumary>
        public ISnTemplateDal DalImpl { get; set; }

        /// <sumary>
        /// 模板表构造函数
        /// </sumary>
        /// <param name="loggerFactory">日志</param>
        /// <param name="baseReposition">基类数据访问层</param>
        public SnTemplateServiceImpl(ILoggerFactory loggerFactory, IBaseReposition<SnTemplate> baseReposition, ISnTemplateDal dalImpl, IHttpContextAccessor conotext) : base(loggerFactory, baseReposition, conotext)
        {
            this.DalImpl = dalImpl;
        }

        public override async Task<APIResult> ValidDataAsync(SnTemplate entity)
        {
            if (await DalImpl.ValidDataAsync(e => e.IsDeleted == false && e.Code == entity.Code && e.Id != entity.Id))
            {
                return APIResult.ErrorResult("编码不能重复");
            }
            var startFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, entity.AssemblyDirectory, entity.StartFileName);
            if (File.Exists(startFilePath) == false)
            {
                return APIResult.ErrorResult("启动文件不存在");
            }
            return await base.ValidDataAsync(entity);
        }


    }
}
