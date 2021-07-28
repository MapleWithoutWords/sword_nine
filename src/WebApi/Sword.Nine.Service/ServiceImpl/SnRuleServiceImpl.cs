using System;
using Microsoft.Extensions.Logging;
using Sword.Nine.Domain;
using Sword.Nine.Dao;
using NoRain.Reposition.BaseReposition;
using NoRain.Service;
using NoRain.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Sword.Nine.Service
{
    /// <summary>
    /// 规则表服务实现类
    /// </summary>
    public partial class SnRuleServiceImpl : BaseServiceImpl<SnRule>, ISnRuleService
    {
        /// <sumary>
        /// 规则表数据层
        /// </sumary>
        public ISnRuleDal DalImpl { get; set; }
        /// <sumary>
        /// 规则表构造函数
        /// </sumary>
        /// <param name="loggerFactory">日志</param>
        /// <param name="baseReposition">基类数据访问层</param>
        public SnRuleServiceImpl(ILoggerFactory loggerFactory, IBaseReposition<SnRule> baseReposition, ISnRuleDal dalImpl, IHttpContextAccessor conotext) : base(loggerFactory, baseReposition, conotext)
        {
            this.DalImpl = dalImpl;
            UpdateIgnorPros.Add("EmunValue");
        }
        /// <summary>
        /// 验证数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task<APIResult> ValidDataAsync(SnRule entity)
        {
            if (await DalImpl.ValidDataAsync(e => e.IsDeleted == false &&
                                                (e.Name == entity.Name || e.Code == entity.Code || e.EnumValue == entity.EnumValue) &&
                                                e.Id != entity.Id))
            {
                return APIResult.SuccessResult("编码,名称或枚举值错误");
            }
            return await base.ValidDataAsync(entity);
        }

        /// <summary>
        /// 获取所有类别属性规则
        /// </summary>
        /// <param name="attrId">属性id</param>
        /// <returns></returns>
        public async Task<List<SnRuleDto>> GetByAttributeIdAsync(string attrId)
        {
            return await DalImpl.GetByAttributeIdAsync(attrId);
        }

        /// <summary>
        /// 保存类别属性
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<APIResult> SaveAsync(List<SnRule> entities)
        {
            if (entities == null || entities.Count < 1)
            {
                return APIResult.SuccessResult(entities);
            }
            if (entities.GroupBy(e => e.EnumValue).Any(e => e.Count() > 1))
            {
                return APIResult.ErrorResult("存在重复的枚举值");
            }
            var allIdList = entities.Where(e => string.IsNullOrEmpty(e.Id) == false).Select(e => e.Id);
            var allDbDataList = await DalImpl.GetListByLambdaAsync(e => e.IsDeleted == false);
            //查询出被删除的属性
            var delDbDataIdList = allDbDataList.Where(e => allIdList.Contains(e.Id) == false).Select(e => e.Id);
            allDbDataList = allDbDataList.Where(e => delDbDataIdList.Contains(e.Id) == false).ToList();

            var updateDbDataList = new List<SnRule>();
            var insertDbDataList = new List<SnRule>();
            foreach (var item in entities)
            {
                var dbData = allDbDataList.FirstOrDefault(e => e.Id == item.Id);
                if (dbData == null)
                {
                    dbData = new SnRule();
                    dbData.SetValue(item);
                    dbData.SetUserId(Context.HttpContext.GetUserId());
                    insertDbDataList.Add(dbData);
                    continue;
                }
                dbData.SetValue(item);
                updateDbDataList.Add(dbData);
            }

            var ret = await DeleteIdsAsync(delDbDataIdList);
            if (ret.Code != 0)
            {
                return ret;
            }
            ret = await InsertAsync(insertDbDataList);
            if (ret.Code != 0)
            {
                return ret;
            }
            ret = await UpdateAsync(updateDbDataList);
            if (ret.Code != 0)
            {
                return ret;
            }
            return APIResult.SuccessResult(entities.OrderBy(e => e.SeqNo));
        }
    }
}
