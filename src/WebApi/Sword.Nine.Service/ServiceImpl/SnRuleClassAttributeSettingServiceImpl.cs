using System;
using Microsoft.Extensions.Logging;
using Sword.Nine.Domain;
using Sword.Nine.Dao;
using NoRain.Reposition.BaseReposition;
using NoRain.Service;
using NoRain.Common;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Sword.Nine.Service
{
    /// <summary>
    /// 规则类别属性配置表服务实现类
    /// </summary>
    public partial class SnRuleClassAttributeSettingServiceImpl : BaseServiceImpl<SnRuleClassAttributeSetting>, ISnRuleClassAttributeSettingService
    {
        /// <sumary>
        /// 规则类别属性配置表数据层
        /// </sumary>
        public ISnRuleClassAttributeSettingDal DalImpl { get; set; }
        /// <sumary>
        /// 类别属性数据层
        /// </sumary>
        public ISnClassAttributeDal ClassAttributeDal { get; set; }
        /// <sumary>
        /// 类别属性数据层
        /// </sumary>
        public ISnRuleDal RuleDal { get; set; }
        /// <sumary>
        /// 规则类别属性配置表构造函数
        /// </sumary>
        /// <param name="loggerFactory">日志</param>
        /// <param name="baseReposition">基类数据访问层</param>
        public SnRuleClassAttributeSettingServiceImpl(ILoggerFactory loggerFactory, IBaseReposition<SnRuleClassAttributeSetting> baseReposition,
            ISnRuleClassAttributeSettingDal dalImpl,
            ISnClassAttributeDal ClassAttributeDal,
            ISnRuleDal RuleDal,
            IHttpContextAccessor conotext) : base(loggerFactory, baseReposition, conotext)
        {
            this.DalImpl = dalImpl;
            this.ClassAttributeDal = ClassAttributeDal;
            this.RuleDal = RuleDal;
        }

        /// <summary>
        /// 根据类别id获取属性规则配置
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public async Task<APIResult> QueryAttrRuleSettingAsync(string classId)
        {
            var retData = await ClassAttributeDal.GetSelectByLambdaAsync(e => e.IsDeleted == false && e.ClassId == classId, e => new RuleAttributeSettingDto
            {
                ClassId = e.ClassId,
                AttributeCode = e.Code,
                AttributeId = e.Id,
                AttributeName = e.Name,
                AttributeColumnName = e.ColumnName
            });
            var ruleSettingList = await DalImpl.GetSelectByLambdaAsync(e => e.IsDeleted == false && e.ClassId == classId, e => new RuleValue
            {
                AttributeId = e.ClassAttributeId,
                RuleId = e.RuleId,
                Value = e.Value
            });

            var allDbRuleList = await RuleDal.GetListByLambdaAsync(e => e.IsDeleted == false);

            retData.ForEach(e =>
            {
                e.RuleValues = new List<RuleValue>();
                foreach (var item in allDbRuleList)
                {
                    var rule = ruleSettingList.FirstOrDefault(x => x.AttributeId == e.AttributeId && x.RuleId == item.Id);
                    if (rule == null)
                    {
                        rule = new RuleValue()
                        {
                            AttributeId = e.AttributeId,
                        };
                    }
                    rule.RuleName = item.Name;
                    rule.RuleValueType = item.ValueType;
                    rule.RuleId = item.Id;
                    e.RuleValues.Add(rule);
                }
            });
            return APIResult.SuccessResult(retData);
        }

        /// <summary>
        /// 更新或新增规则配置
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public async Task<APIResult> UpdateOrInsertRuleSettingAsync(List<RuleAttributeSettingDto> dtos)
        {
            var classId = dtos.FirstOrDefault()?.ClassId;
            if (classId.IsNullOrEmpty())
            {
                return APIResult.ErrorResult("classId非法");
            }
            var userid = this.Context.HttpContext.GetUserId();
            List<SnRuleClassAttributeSetting> addDatas = new List<SnRuleClassAttributeSetting>();
            foreach (var item in dtos)
            {
                foreach (var ruleValue in item.RuleValues)
                {
                    if (ruleValue.Value.IsNullOrEmpty())
                    {
                        continue;
                    }
                    var data = new SnRuleClassAttributeSetting
                    {
                        ClassAttributeId = item.AttributeId,
                        ClassId = item.ClassId,
                        RuleId = ruleValue.RuleId,
                        Value = ruleValue.Value,
                    };
                    data.SetUserId(userid);
                    data.Id = Guid.NewGuid().ToString();
                    addDatas.Add(data);
                }
            }
            var delDatas = await DalImpl.GetSelectByLambdaAsync(e => e.IsDeleted == false && e.ClassId == classId, e => e.Id);
            await DalImpl.DeleteIdsAsync(delDatas, userid);
            var ret = DalImpl.InsertAsync(addDatas);
            return APIResult.SuccessResult(ret);
        }
    }
}
