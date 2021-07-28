using System;
using Microsoft.Extensions.Logging;
using Sword.Nine.Domain;
using Sword.Nine.Dao;
using NoRain.Reposition.BaseReposition;
using NoRain.Service;
using System.Threading.Tasks;
using NoRain.Common;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Sword.Nine.Domain.Dto.DataSource.directory;

namespace Sword.Nine.Service
{
    /// <summary>
    /// 类别目录服务实现类
    /// </summary>
    public partial class SnClassDirectoryServiceImpl : BaseServiceImpl<SnClassDirectory>, ISnClassDirectoryService
    {
        public ISnClassDirectoryDal DalImpl { get; set; }
        public SnClassDirectoryServiceImpl(ILoggerFactory loggerFactory, IBaseReposition<SnClassDirectory> baseReposition, ISnClassDirectoryDal dalImpl, IHttpContextAccessor conotext) : base(loggerFactory, baseReposition, conotext)
        {
            this.DalImpl = dalImpl;
            UpdateIgnorPros.Add("Content");
        }
        /// <summary>
        /// 获取目录树
        /// </summary>
        /// <param name="urlParams"></param>
        /// <returns></returns>
        public async Task<APIResult> GetDirectoryTreeAsync(List<UrlParams> urlParams)
        {
            Expression<Func<SnClassDirectory, bool>> expression = e => e.IsDeleted == false;
            if (urlParams.TryGetString("dataSourceId", out string dataSourceId))
            {
                expression = e => e.IsDeleted == false && e.DataSourceId == dataSourceId;
            }
            if (urlParams.TryGetString("keyword", out string keyword))
            {
                expression = e => e.IsDeleted == false && e.DataSourceId == dataSourceId && (e.Name.Contains(keyword) || e.Code.Contains(keyword));
            }
            var allDirectoryData = await DalImpl.GetSelectByLambdaAsync(expression, e => new DirectoryClassTreeDto
            {
                Id = e.Id,
                Code = e.Code,
                Name = e.Name,
                ParentId = e.ParentId,
                Type = 0
            }, e => e.SeqNo);
            var data = new List<DirectoryClassTreeDto>();
            foreach (var item in allDirectoryData.Where(e => e.ParentId == Guid.Empty.ToString()))
            {
                item.AddChildrens(allDirectoryData);
                data.Add(item);
            }
            return APIResult.SuccessResult(data);
        }

        ///// <summary>
        ///// 新增目录
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        //public override async Task<APIResult> InsertAsync(SnClassDirectory entity)
        //{
        //    entity.ParentId = entity.ParentId.IsGuidAndNoGuidEmpty() ? entity.ParentId : Guid.Empty.ToString();
        //    var existsData = await DalImpl.FirstOrDefaultAsync(e => e.IsDeleted == false && e.Name == entity.Name || e.Code == entity.Code && e.ParentId == entity.ParentId);
        //    if (existsData != null)
        //    {
        //        return APIResult.ErrorResult("目录名称或编码已存在，名称或编码在同一个目录下唯一，请修改");
        //    }
        //    var parentData = await DalImpl.FirstOrDefaultAsync(e => e.IsDeleted == false && e.Id == entity.ParentId);
        //    entity.Path = $"|{entity.Name}";
        //    if (parentData != null)
        //    {
        //        entity.Path = $"{parentData.Path}|{entity.Name}";
        //    }
        //    return await base.InsertAsync(entity);
        //}

        //public override async Task<APIResult> InsertValidAsync(List<SnClassDirectory> entities)
        //{
        //    var nameList = entities.Select(e => e.Name).Distinct().Where(e => string.IsNullOrEmpty(e) == false).ToList();
        //    var codeList = entities.Select(e => e.Code).Distinct().Where(e => string.IsNullOrEmpty(e) == false).ToList();
        //    var parentIdList = entities.Select(e => e.ParentId).Distinct().Where(e => string.IsNullOrEmpty(e) == false).ToList();
        //    var nameOrCodeDbExistsData = await DalImpl.GetListByLambdaAsync(e => e.IsDeleted == false && (nameList.Contains(e.Name) || codeList.Contains(e.Code)) && parentIdList.Contains(e.ParentId));

        //    var parentDbExistsData = await DalImpl.GetListByLambdaAsync(e => e.IsDeleted == false && parentIdList.Contains(e.Id));

        //    foreach (var entity in entities)
        //    {

        //        entity.ParentId = entity.ParentId.IsGuidAndNoGuidEmpty() ? entity.ParentId : Guid.Empty.ToString();
        //        var existsData = nameOrCodeDbExistsData.FirstOrDefault(e => e.Name == entity.Name || e.Code == entity.Code && e.ParentId == entity.ParentId);
        //        if (existsData != null)
        //        {
        //            return APIResult.ErrorResult("目录名称或编码已存在，名称或编码在同一个目录下唯一，请修改");
        //        }
        //        var parentData = parentDbExistsData.FirstOrDefault(e => e.Id == entity.ParentId);
        //        entity.Path = $"|{entity.Name}";
        //        if (parentData != null)
        //        {
        //            entity.Path = $"{parentData.Path}|{entity.Name}";
        //        }
        //    }
        //    return await base.InsertValidAsync(entities);
        //}

        ////public override async Task<APIResult> UpdateAsync(SnClassDirectory entity)
        ////{
        ////    entity.ParentId = entity.ParentId.IsGuidAndNoGuidEmpty() ? entity.ParentId : Guid.Empty.ToString();
        ////    var existsData = await DalImpl.FirstOrDefaultAsync(e => e.IsDeleted == false && e.Name == entity.Name || e.Code == entity.Code && e.ParentId == entity.ParentId);
        ////    if (existsData != null && existsData.Id != entity.Id)
        ////    {
        ////        return APIResult.ErrorResult("目录名称或编码已存在，名称或编码在同一个目录下唯一，请修改");
        ////    }
        ////    var parentData = await DalImpl.FirstOrDefaultAsync(e => e.IsDeleted == false && e.Id == entity.ParentId);
        ////    entity.Path = $"|{entity.Name}";
        ////    if (parentData != null)
        ////    {
        ////        entity.Path = $"{parentData.Path}|{entity.Name}";
        ////    }
        ////    return await base.UpdateAsync(entity);
        ////}

        //public override async Task<APIResult> UpdateValidAsync(List<SnClassDirectory> dbEntity, List<SnClassDirectory> entities)
        //{
        //    var nameList = entities.Select(e => e.Name).Distinct().Where(e => string.IsNullOrEmpty(e) == false).ToList();
        //    var codeList = entities.Select(e => e.Code).Distinct().Where(e => string.IsNullOrEmpty(e) == false).ToList();
        //    var parentIdList = entities.Select(e => e.ParentId).Distinct().Where(e => string.IsNullOrEmpty(e) == false).ToList();
        //    var nameOrCodeDbExistsData = await DalImpl.GetListByLambdaAsync(e => e.IsDeleted == false && (nameList.Contains(e.Name) || codeList.Contains(e.Code)) && parentIdList.Contains(e.ParentId));

        //    var parentDbExistsData = await DalImpl.GetListByLambdaAsync(e => e.IsDeleted == false && parentIdList.Contains(e.Id));

        //    foreach (var entity in entities)
        //    {

        //        entity.ParentId = entity.ParentId.IsGuidAndNoGuidEmpty() ? entity.ParentId : Guid.Empty.ToString();
        //        var existsData = nameOrCodeDbExistsData.FirstOrDefault(e => e.Name == entity.Name || e.Code == entity.Code && e.ParentId == entity.ParentId);
        //        if (existsData != null)
        //        {
        //            return APIResult.ErrorResult("目录名称或编码已存在，名称或编码在同一个目录下唯一，请修改");
        //        }
        //        var parentData = parentDbExistsData.FirstOrDefault(e => e.Id == entity.ParentId);
        //        entity.Path = $"|{entity.Name}";
        //        if (parentData != null)
        //        {
        //            entity.Path = $"{parentData.Path}|{entity.Name}";
        //        }
        //    }
        //    return await base.UpdateValidAsync(dbEntity, entities);
        //}

        /// <summary>
        /// 新增目录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task<APIResult> InsertAsync(SnClassDirectory entity)
        {
            entity.SetUserId(Context.HttpContext.GetUserId());
            var validResult = await ValidDataAsync(entity);
            if (validResult.Code != 0)
            {
                return validResult;
            }
            var parentDirectoryEntity = await DalImpl.GetByIdAsync(entity.ParentId);
            entity.Path = $"{parentDirectoryEntity?.Path}|{entity.Code}";
            var res = await BaseReposition.InsertAsync(entity);
            return APIResult.SuccessResult(res);
        }
        /// <summary>
        /// 校验数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task<APIResult> ValidDataAsync(SnClassDirectory entity)
        {
            entity.ParentId = entity.ParentId.IsGuid() ? entity.ParentId : Guid.Empty.ToString();
            if (await DalImpl.ValidDataAsync(e => e.Id != entity.Id && e.IsDeleted == false && (e.Name == entity.Name || e.Code == entity.Code) && e.ParentId == entity.ParentId && e.DataSourceId == entity.DataSourceId))
            {
                return APIResult.ErrorResult("名称或code已存在");
            }
            return await base.ValidDataAsync(entity);
        }


        public override async Task<APIResult> UpdateBussinessAsync(SnClassDirectory entity, SnClassDirectory dbEntity)
        {
            if (entity.ParentId != dbEntity.ParentId)
            {
                var newParentDirectory = await DalImpl.GetByIdAsync(entity.ParentId);

                entity.Path = $"{newParentDirectory?.Path}|{entity.Code}";
            }

            //更新子目录数据
            var allDbChildDirectoryList = await DalImpl.GetListByLambdaAsync(e => e.IsDeleted == false && e.Path.StartsWith(dbEntity.Path) && e.Id != entity.Id);
            allDbChildDirectoryList.ForEach(e =>
            {

                e.Path = $"{entity.Path}|{e.Code}";
                e.LastUpdateTime = DateTime.Now;
                e.LastUpdateUser = UserId;
            });
            await DalImpl.UpdateAsync(allDbChildDirectoryList);

            return await base.UpdateBussinessAsync(entity, dbEntity);
        }

        /// <summary>
        /// 保存绘图
        /// </summary>
        /// <param name="cellDtos"></param>
        /// <returns></returns>
        public async Task<APIResult> SaveDrawingAsync(DrawingSaveDto dto)
        {
            var directoryEntity = await DalImpl.GetByIdAsync(dto.DirectoryId);
            if (directoryEntity == null)
            {
                return APIResult.ErrorResult("绘图不存在");
            }
            directoryEntity.Content = dto.Content;//JsonConvert.SerializeObject(cellDtos);
            var ret = await DalImpl.UpdateAsync(directoryEntity);
            return APIResult.SuccessResult("");
        }
        public override async Task<SnClassDirectory> GetByIdAsync(string id, int? status)
        {
            var ret = await BaseReposition.GetByIdAsync(id, status);
            //ret.GraphModel = JsonConvert.DeserializeObject<List<MxCellDto>>(ret.Content);
            //ret.Content = null;
            return ret;
        }
    }
}
