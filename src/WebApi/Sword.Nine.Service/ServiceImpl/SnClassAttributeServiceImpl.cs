using System;
using Microsoft.Extensions.Logging;
using Sword.Nine.Domain;
using Sword.Nine.Dao;
using NoRain.Reposition.BaseReposition;
using NoRain.Service;
using NoRain.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Sword.Nine.Service
{
    /// <summary>
    /// 类别属性服务实现类
    /// </summary>
    public partial class SnClassAttributeServiceImpl : BaseServiceImpl<SnClassAttributeListDto>, ISnClassAttributeService
    {
        /// <sumary>
        /// 类别属性数据层
        /// </sumary>
        public ISnClassAttributeDal DalImpl { get; set; }
        /// <summary>
        /// 类别数据层
        /// </summary>
        public ISnClassDal ClassDal { get; set; }
        /// <sumary>
        /// 类别属性构造函数
        /// </sumary>
        /// <param name="loggerFactory">日志</param>
        /// <param name="baseReposition">基类数据访问层</param>
        public SnClassAttributeServiceImpl(ILoggerFactory loggerFactory,
            IBaseReposition<SnClassAttributeListDto> baseReposition,
            ISnClassAttributeDal dalImpl,
            ISnClassDal ClassDal,
            IHttpContextAccessor conotext) : base(loggerFactory, baseReposition, conotext)
        {
            UpdateIgnorPros.Add("Code");
            UpdateIgnorPros.Add("ClassId");
            UpdateIgnorPros.Add("DataSourceId");
            UpdateIgnorPros = new List<string> { };
            this.DalImpl = dalImpl;
            this.ClassDal = ClassDal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task<APIResult> InsertAsync(SnClassAttributeListDto entity)
        {
            entity.SetUserId(Context.HttpContext.GetUserId());
            var validResult = await ValidDataAsync(entity);
            if (validResult.Code != 0)
            {
                return validResult;
            }

            var classData = await ClassDal.GetByIdAsync(entity.ClassId);
            if (classData == null)
            {
                return APIResult.ErrorResult("类别id非法");
            }
            if (await ClassDal.ValidDataAsync(e => e.IsDeleted == false && e.Path.StartsWith(classData.Path)))
            {
                var ret = await AddAttributeToChildrenClassAsync(entity, classData);
                if (ret.Code != 0)
                {
                    return ret;
                }
            }
            var res = await BaseReposition.InsertAsync(entity);
            return APIResult.SuccessResult(res);
        }

        /// <summary>
        /// 添加属性到子类下
        /// </summary>
        /// <param name="parentAttribute"></param>
        /// <param name="classData"></param>
        /// <returns></returns>
        private async Task<APIResult> AddAttributeToChildrenClassAsync(SnClassAttributeListDto parentAttribute, SnClassDto classData)
        {
            List<SnClassAttributeListDto> childrenClassAddAttributeList = new List<SnClassAttributeListDto>();
            var childrenClassIdList = await ClassDal.GetSelectByLambdaAsync(e => e.IsDeleted == false && e.TopId == classData.TopId && e.Path.StartsWith(classData.Path) && e.Id != classData.Id, e => e.Id);

            var childrenClassExistsRepeatAttributeCount = await DalImpl.CountAsync(e => e.IsDeleted == false && childrenClassIdList.Contains(e.ClassId) && (e.Code == parentAttribute.Code || e.ColumnName == parentAttribute.ColumnName));
            if (childrenClassExistsRepeatAttributeCount > 0)
            {
                return APIResult.ErrorResult("子类存在重复属性");
            }

            foreach (var item in childrenClassIdList)
            {
                var copyEntity = ObjectCopy<SnClassAttributeListDto, SnClassAttributeListDto>.Copy(parentAttribute);
                copyEntity.SetUserId(UserId, true);
                copyEntity.ClassId = item;
                copyEntity.InheritId = parentAttribute.Id;
                childrenClassAddAttributeList.Add(copyEntity);
            }
            if (childrenClassAddAttributeList.Count < 1)
            {
                return APIResult.SuccessResult();
            }
            await BaseReposition.InsertAsync(childrenClassAddAttributeList);
            return APIResult.SuccessResult();
        }

        /// <summary>
        /// 校验数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task<APIResult> ValidDataAsync(SnClassAttributeListDto entity)
        {
            if (await ClassDal.CountAsync(e => e.IsDeleted == false && entity.ClassId == e.Id) < 1)
            {
                return APIResult.ErrorResult("ClassId非法");
            }
            var repeatData = await DalImpl.CountAsync(e => e.IsDeleted == false && e.Id == entity.ClassId && (e.Code == entity.Code || e.ColumnName == entity.ColumnName));
            if (repeatData > 0)
            {
                return APIResult.ErrorResult("Code或ColumnName重复");
            }
            return await base.ValidDataAsync(entity);
        }

        /// <summary>
        /// 保存类别属性
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<APIResult> SaveAsync(List<SnClassAttributeListDto> entities)
        {
            if (entities == null || entities.Count < 1)
            {
                return APIResult.SuccessResult(entities);
            }
            entities = entities.Where(e => string.IsNullOrEmpty(e.Name) == false && string.IsNullOrEmpty(e.ColumnName) == false).ToList();
            var classId = entities.FirstOrDefault()?.ClassId;
            if (classId.IsGuidAndNoGuidEmpty() == false)
            {
                return APIResult.ErrorResult("classId非法");
            }
            var allIdList = entities.Where(e => string.IsNullOrEmpty(e.Id) == false).Select(e => e.Id);
            var allDbClassAttributeList = await DalImpl.GetListByLambdaAsync(e => e.ClassId == classId && e.IsDeleted == false);
            var delDbDataIdList = allDbClassAttributeList.Where(e => allIdList.Contains(e.Id) == false).Select(e => e.Id);
            allDbClassAttributeList = allDbClassAttributeList.Where(e => delDbDataIdList.Contains(e.Id) == false).ToList();

            var insertAndUpdateClassAttributeListResult = HandleFilterInsertAndUpdateList(entities, allDbClassAttributeList);

            var ret = await DeleteIdsAsync(delDbDataIdList);
            if (ret.Code != 0)
            {
                return ret;
            }
            ret = await InsertAsync(insertAndUpdateClassAttributeListResult.Item1);
            if (ret.Code != 0)
            {
                return ret;
            }
            ret = await UpdateAsync(insertAndUpdateClassAttributeListResult.Item2);
            if (ret.Code != 0)
            {
                return ret;
            }
            return APIResult.SuccessResult(entities.OrderBy(e => e.SeqNo));
        }

        /// <summary>
        /// 处理:过滤出新增和修改的属性数据
        /// </summary>
        /// <param name="entities">保存的数据集</param>
        /// <param name="allDbClassAttributeList">数据库中的数据</param>
        /// <returns>(新增的数据集,修改的数据集)</returns>
        private (List<SnClassAttributeListDto>, List<SnClassAttributeListDto>) HandleFilterInsertAndUpdateList(List<SnClassAttributeListDto> entities, List<SnClassAttributeListDto> allDbClassAttributeList)
        {
            var updateDbDataList = new List<SnClassAttributeListDto>();
            var insertDbDataList = new List<SnClassAttributeListDto>();
            foreach (var item in entities)
            {
                var dbData = allDbClassAttributeList.FirstOrDefault(e => e.Id == item.Id);
                if (dbData == null)
                {
                    dbData = new SnClassAttributeListDto();
                    dbData.SetValue(item);
                    dbData.SetUserId(Context.HttpContext.GetUserId());
                    insertDbDataList.Add(dbData);
                    continue;
                }
                dbData.SetValue(item);
                updateDbDataList.Add(dbData);
            }
            return (insertDbDataList, updateDbDataList);
        }

        /// <summary>
        /// 删除属性
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override async Task<APIResult> DeleteIdsAsync(IEnumerable<string> ids)
        {
            //删除继承属性
            await DalImpl.UpdateColumnAsync(e => new SnClassAttributeListDto
            {
                IsDeleted = true,
                LastUpdateTime = DateTime.Now,
                LastUpdateUser = UserId
            }, e => ids.Contains(e.InheritId) && e.IsDeleted == false);

            //删除属性，忽略继承下来的属性
            await DalImpl.UpdateColumnAsync(e => new SnClassAttributeListDto
            {
                IsDeleted = true,
                LastUpdateTime = DateTime.Now,
                LastUpdateUser = UserId
            }, e => ids.Contains(e.Id) && e.IsDeleted == false && string.IsNullOrEmpty(e.InheritId));
            return APIResult.SuccessResult();
        }
    }
}
