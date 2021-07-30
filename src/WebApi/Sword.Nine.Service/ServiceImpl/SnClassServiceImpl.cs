using System;
using Microsoft.Extensions.Logging;
using Sword.Nine.Domain;
using Sword.Nine.Dao;
using NoRain.Reposition.BaseReposition;
using NoRain.Service;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using NoRain.Common;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace Sword.Nine.Service
{
    /// <summary>
    /// 类别表服务实现类
    /// </summary>
    public partial class SnClassServiceImpl : BaseServiceImpl<SnClassDto>, ISnClassService
    {
        public ISnDataSourceDal DataSourceDal { get; set; }
        public ISnClassDal DalImpl { get; set; }
        public ISnClassDirectoryDal ClassDirectoryDal { get; set; }
        public ISnClassAttributeDal ClassAttributeDal { get; set; }
        public SnClassServiceImpl(ILoggerFactory loggerFactory,
            ISnClassDirectoryDal ClassDirectoryDal,
                                  IBaseReposition<SnClassDto> baseReposition,
                                  ISnClassDal dalImpl,
                                  ISnClassAttributeDal classAttributeDal,
                                  ISnDataSourceDal dataSourceDal,
                                  IHttpContextAccessor conotext) : base(loggerFactory, baseReposition, conotext)
        {
            this.DataSourceDal = dataSourceDal;
            this.DalImpl = dalImpl;
            this.ClassDirectoryDal = ClassDirectoryDal;
            this.ClassAttributeDal = classAttributeDal;
            UpdateIgnorPros.Add(nameof(SnClassDto.TopId));
        }


        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task<APIResult> InsertAsync(SnClassDto entity)
        {
            var userId = Context.HttpContext.GetUserId();
            entity.SetUserId(userId);
            entity.TopId = entity.Id;
            var validResult = await ValidDataAsync(entity);
            if (validResult.Code != 0)
            {
                return validResult;
            }

            entity.ParentId = entity.ParentId.IsGuidAndNoGuidEmpty() ? entity.ParentId : Guid.Empty.ToString();
            var parentClassEntity = await DalImpl.GetByIdAsync(entity.ParentId);
            if ((parentClassEntity == null || parentClassEntity.Type != 1) && entity.ParentId.IsGuidAndNoGuidEmpty())
            {
                return APIResult.ErrorResult("上级类别非法");
            }
            //处理path路径
            entity.Path = $"{parentClassEntity?.Path}|{entity.Code}";
            if (parentClassEntity != null)
            {
                entity.TopId = parentClassEntity.Id;
                //获取父类下所有属性
                var addDbAttributeList = await ClassAttributeDal.GetListByLambdaAsync(e => e.IsDeleted == false && e.ClassId == parentClassEntity.Id);
                addDbAttributeList.ForEach(e =>
                {
                    e.SetUserId(userId, true);
                    e.ClassId = entity.Id;
                    e.InheritId = e.InheritId.IsGuidAndNoGuidEmpty() ? e.InheritId : parentClassEntity.Id;
                });
                await ClassAttributeDal.InsertAsync(addDbAttributeList);
            }

            var res = await BaseReposition.InsertAsync(entity);
            return APIResult.SuccessResult(res);
        }

        /// <summary>
        /// 删除类别
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override async Task<APIResult> DeleteIdsAsync(IEnumerable<string> ids)
        {
            var existData = await ClassAttributeDal.ValidDataAsync(e => e.IsDeleted == false && ids.Contains(e.ClassId));
            if (existData)
            {
                return APIResult.ErrorResult("类别下存在属性");
            }
            return await base.DeleteIdsAsync(ids);
        }

        /// <summary>
        /// 更新类别
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task<APIResult> UpdateAsync(SnClassDto entity)
        {
            var dbEntity = await BaseReposition.GetByIdAsync(entity.Id);
            if (dbEntity == null)
            {
                return APIResult.ErrorResult("id非法");
            }
            var ret = await UpdateValidAsync(entity, dbEntity);
            if (ret.Code != 0)
            {
                return ret;
            }

            entity.ParentId = entity.ParentId.IsGuid() ? entity.ParentId : Guid.Empty.ToString();
            //处理父类变更
            if (dbEntity.ParentId != entity.ParentId)
            {
                var parentDbClassEntity = await DalImpl.GetByIdAsync(entity.ParentId);
                if ((parentDbClassEntity == null || parentDbClassEntity.Type != 1) && entity.ParentId.IsGuidAndNoGuidEmpty())
                {
                    return APIResult.ErrorResult("上级类别非法");
                }
                dbEntity.TopId = parentDbClassEntity.TopId;
                entity.Path = $"{parentDbClassEntity.Path}|{entity.Code}";
                //处理子类的code及子类属性
                var childrenClassIdList = await DalImpl.GetSelectByLambdaAsync(e => e.IsDeleted == false && e.Path.StartsWith(dbEntity.Path) && e.Id != dbEntity.Id, e => e.Id);
                if (childrenClassIdList.Count > 0)
                {
                    await DalImpl.UpdateColumnAsync(e => new SnClassDto
                    {
                        Path = $"{entity.Path}|{e.Code}",
                        LastUpdateTime = DateTime.Now,
                        LastUpdateUser = UserId
                    }, e => childrenClassIdList.Contains(e.Id));

                }
                childrenClassIdList.Add(dbEntity.Id);

                //删除自身以及自身下的子类的继承自旧上级类别的属性
                var oldParentAttributeIdList = await ClassAttributeDal.GetSelectByLambdaAsync(e => e.IsDeleted == false && e.ClassId == dbEntity.ParentId, e => e.Id);
                if (oldParentAttributeIdList.Count > 0 && childrenClassIdList.Count > 0)
                {
                    await ClassAttributeDal.UpdateColumnAsync(e => new SnClassAttributeListDto
                    {
                        IsDeleted = true,
                        LastUpdateTime = DateTime.Now,
                        LastUpdateUser = UserId
                    }, e => (childrenClassIdList.Contains(e.ClassId) || e.ClassId == entity.Id) && oldParentAttributeIdList.Contains(e.InheritId));
                }

                //新增继承自父类的属性
                if (parentDbClassEntity != null)
                {
                    var newParentAttributeList = await ClassAttributeDal.GetListByLambdaAsync(e => e.IsDeleted == false && e.ClassId == parentDbClassEntity.Id);
                    List<SnClassAttributeListDto> addDbAttributeList = new List<SnClassAttributeListDto>();
                    foreach (var item in childrenClassIdList)
                    {
                        foreach (var attrItem in newParentAttributeList)
                        {
                            var newAttr = ObjectCopy<SnClassAttributeListDto, SnClassAttributeListDto>.Copy(attrItem);
                            newAttr.SetUserId(UserId, true);
                            newAttr.InheritId = newAttr.ClassId;
                            newAttr.ClassId = item;
                            addDbAttributeList.Add(newAttr);
                        }
                    }
                    await ClassAttributeDal.InsertAsync(addDbAttributeList);
                }


            }


            entity.SetUserId(Context.HttpContext.GetUserId());
            dbEntity.SetValue(entity, UpdateIgnorPros.ToArray());
            var res = await BaseReposition.UpdateAsync(dbEntity);
            return APIResult.SuccessResult(res);
        }

        /// <summary>
        /// 获取目录类别树
        /// </summary>
        /// <param name="dsId"></param>
        /// <returns></returns>
        public async Task<APIResult> GetDirectoryClassAsync(List<UrlParams> urlParams)
        {
            if (urlParams.TryGetString("dataSourceId", out string dataSourceId) == false)
            {
                return APIResult.ErrorResult("dataSourceId 必传");
            }
            var allDirectoryClasList = await ClassDirectoryDal.GetSelectByLambdaAsync(e => e.IsDeleted == false && e.DataSourceId == dataSourceId, e => new DirectoryClassTreeDto
            {
                Code = e.Code,
                Id = e.Id,
                Type = 0,
                SeqNo = e.SeqNo,
                Name = e.Name,
                ParentId = e.ParentId,

            });
            var allClassList = await DalImpl.GetSelectByLambdaAsync(e => e.IsDeleted == false && e.DataSourceId == dataSourceId, e => new DirectoryClassTreeDto
            {
                Code = e.Code,
                Id = e.Id,
                Type = 1,
                SeqNo = e.SeqNo,
                Name = e.Name,
                ParentId = e.ClassDirectoryId,
                ClassDirectoryId = e.ClassDirectoryId,
                TableName = e.TableName,
            });
            allDirectoryClasList = allDirectoryClasList.OrderBy(e => e.SeqNo).ToList();
            allDirectoryClasList.AddRange(allClassList.OrderBy(e => e.SeqNo));
            if (urlParams.TryGetString("keyword", out string keyword))
            {
                allDirectoryClasList = allDirectoryClasList.Where(e => e.Name.Contains(keyword) || e.Code.Contains(keyword)).ToList();
            }
            List<DirectoryClassTreeDto> ret = new List<DirectoryClassTreeDto>();
            foreach (var item in allDirectoryClasList.Where(e => e.ParentId == Guid.Empty.ToString()))
            {
                item.AddChildrens(allDirectoryClasList);
                ret.Add(item);
            }
            return APIResult.SuccessResult(ret);
        }

        /// <summary>
        /// 获取类别信息以及属性
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public async Task<APIResult> GetInfoAndAttributeAsync(string classId)
        {
            var classEntity = await DalImpl.GetByIdAsync(classId);
            if (classEntity == null)
            {
                return APIResult.ErrorResult("类别不存在");
            }
            var allClassAttributeRet = await ClassAttributeDal.GetUrlParamAsync(new List<UrlParams>
            {
                new UrlParams{  Key="ClassId", Values=new List<string>{ classId} },
                new UrlParams{  Key="sort", Values=new List<string>{ "SeqNo asc"} },
            });
            classEntity.ClassAttributeList = allClassAttributeRet.Item1;
            return APIResult.SuccessResult(classEntity);
        }

        /// <summary>
        /// 校验数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task<APIResult> ValidDataAsync(SnClassDto entity)
        {
            var userId = Context.HttpContext.GetUserId();


            if (entity.DataSourceId.IsGuidAndNoGuidEmpty() == false)
            {
                return APIResult.ErrorResult("数据源id非法");
            }
            var dsResult = await DataSourceDal.ValidDataAsync(e => e.IsDeleted == false && e.Id == entity.DataSourceId);
            if (dsResult == false)
            {
                return APIResult.ErrorResult("数据源id不存在");
            }
            if (entity.ClassDirectoryId.IsGuidAndNoGuidEmpty())
            {
                var directoryValidResult = await ClassDirectoryDal.ValidDataAsync(e => e.IsDeleted == false && e.Id == entity.ClassDirectoryId);
                if (directoryValidResult == false)
                {
                    return APIResult.ErrorResult("目录id不存在");
                }
            }

            Expression<Func<SnClassDto, bool>> expression = null;
            if (entity.Id.IsGuidAndNoGuidEmpty())
            {
                expression = e => e.IsDeleted == false && e.DataSourceId == entity.DataSourceId && e.Type == entity.Type && (e.TableName == entity.TableName || entity.Code == e.Code) && e.Id != entity.Id;
            }
            else
            {
                expression = e => e.IsDeleted == false && e.DataSourceId == entity.DataSourceId && e.Type == entity.Type && (e.TableName == entity.TableName || entity.Code == e.Code) && e.Id != entity.Id;
            }
            var repeatData = await DalImpl.ValidDataAsync(expression);
            if (repeatData)
            {
                return APIResult.ErrorResult("编码或表名重复，请修改");
            }


            return APIResult.SuccessResult();
        }

        /// <summary>
        /// 发布类别
        /// </summary>
        /// <param name="classIds"></param>
        /// <returns></returns>
        public async Task<int> PublishAsync(List<string> classIds)
        {
            return await DalImpl.PublishAsync(classIds);
        }

    }
}
