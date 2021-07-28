using Common.APICommon;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NoRain.Common;
using NoRain.Reposition;
using NoRain.Reposition.BaseReposition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NoRain.Service
{
    public class BaseServiceImpl<TEntity> : IInjectFac
                                              where TEntity : BaseEntity
    {
        public IBaseReposition<TEntity> BaseReposition { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }
        public IHttpContextAccessor Context { get; set; }
        public string UserId { get; set; }
        public BaseServiceImpl(ILoggerFactory loggerFactory, IBaseReposition<TEntity> baseReposition, IHttpContextAccessor conotext)
        {
            BaseReposition = baseReposition;
            LoggerFactory = loggerFactory;
            Context = conotext;
            this.UserId = conotext.HttpContext.GetUserId();
        }

        /// <summary>
        /// 更新忽略某些列
        /// </summary>
        public virtual List<string> UpdateIgnorPros { get; set; } = new List<string>();

        /// <summary>
        /// 根据Id删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<APIResult> DeleteByIdAsync(string id)
        {
            await BaseReposition.DeleteIdsAsync(new List<string> { id }, Context.HttpContext.GetUserId());
            return APIResult.SuccessResult();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<APIResult> InsertAsync(TEntity entity)
        {
            entity.SetUserId(Context.HttpContext.GetUserId());
            var validResult = await ValidDataAsync(entity);
            if (validResult.Code != 0)
            {
                return validResult;
            }
            var res = await BaseReposition.InsertAsync(entity);
            return APIResult.SuccessResult(res);
        }


        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<APIResult> UpdateAsync(TEntity entity)
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
            ret = await UpdateBussinessAsync(entity, dbEntity);
            return ret;
        }
        /// <summary>
        /// 更新实体的业务
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dbEntity"></param>
        /// <returns></returns>
        public virtual async Task<APIResult> UpdateBussinessAsync(TEntity entity, TEntity dbEntity)
        {
            entity.SetUserId(Context.HttpContext.GetUserId());
            dbEntity.SetValue(entity, UpdateIgnorPros.ToArray());
            var res = await BaseReposition.UpdateAsync(dbEntity);
            return APIResult.SuccessResult(res);
        }

        /// <summary>
        /// 更新时校验数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="idEntity"></param>
        /// <returns></returns>
        public virtual async Task<APIResult> UpdateValidAsync(TEntity entity, TEntity idEntity)
        {

            var validResult = await ValidDataAsync(entity);
            if (validResult.Code != 0)
            {
                return validResult;
            }
            return APIResult.SuccessResult();
        }
        /// <summary>
        /// 验证数据合法性
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<APIResult> ValidDataAsync(TEntity entity)
        {
            return APIResult.SuccessResult();
        }

        /// <summary>
        /// 获取未删除的数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> GetAllListAsync()
        {
            return await BaseReposition.GetAllListAsync();
        }


        /// <summary>
        /// 根据Id获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> GetByIdAsync(string id)
        {
            return await BaseReposition.GetByIdAsync(id);
        }

        /// <summary>
        /// 根据url参数查询
        /// </summary>
        /// <param name="urlParams"></param>
        /// <returns></returns>
        public virtual async Task<APIResult> GetUrlParamAsync(List<UrlParams> urlParams)
        {
            var ret = await BaseReposition.GetUrlParamAsync(urlParams);
            return new APIResult { Page = ret.Item2, Data = ret.Item1 };
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<bool> ValidExistsAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await BaseReposition.ValidDataAsync(expression);
        }


        /// <summary>
        /// 根据Id和状态查询数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> GetByIdAsync(string id, int? status)
        {
            return await BaseReposition.GetByIdAsync(id, status);
        }

        /// <summary>
        /// 更新多条数据
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<APIResult> UpdateAsync(List<TEntity> entities)
        {
            var enIdList = entities.Select(e => e.Id);
            var dbEntityList = await BaseReposition.GetListByLambdaAsync(e => e.IsDeleted == false && enIdList.Contains(e.Id));
            if (dbEntityList.Count != entities.Count)
            {
                return APIResult.ErrorResult("id非法");
            }
            var resEn = await BaseReposition.UpdateAsync(entities);
            return APIResult.SuccessResult(resEn);
        }


        /// <summary>
        /// 插入多条数据
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<APIResult> InsertAsync(List<TEntity> entities)
        {
            if (entities == null || entities.Count < 1)
            {
                return APIResult.SuccessResult("");
            }

            var result = await BaseReposition.InsertAsync(entities);
            return APIResult.SuccessResult(result);
        }
        /// <summary>
        /// 根据lambda表达式筛选数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> GetListByLambdaAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await BaseReposition.GetListByLambdaAsync(expression);
        }


        /// <summary>
        /// 根据lambda表达式筛选数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<List<TResult>> GetSelectByLambdaAsync<TResult>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TResult>> selectExpression, Expression<Func<TEntity, object>> orderByEx = null, bool isAsc = true)
        {
            return await BaseReposition.GetSelectByLambdaAsync(expression, selectExpression, orderByEx, isAsc);
        }
        /// <summary>
        /// 删除多条
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual async Task<APIResult> DeleteIdsAsync(IEnumerable<string> ids)
        {
            var userid = this.Context.HttpContext.GetUserId();
            await BaseReposition.DeleteIdsAsync(ids, userid);
            return APIResult.SuccessResult();
        }

        /// <summary>
        /// 更新某些列
        /// </summary>
        /// <param name="id"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateColumnAsync(string id, List<UpdateColumnValue> list)
        {
            return await BaseReposition.UpdateColumnAsync(id, list);
        }


        /// <summary>
        /// 根据lambda表达式取第一个数据
        /// </summary>
        /// <param name="expression">where条件</param>
        /// <param name="orderEx">排序条件</param>
        /// <param name="isAsc">是否升序,默认升序</param>
        /// <returns></returns>
        public virtual async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> orderByEx = null, bool isAsc = true)
        {
            return await BaseReposition.FirstOrDefaultAsync(expression, orderByEx, isAsc);
        }

        /// <summary>
        /// 根据lambda表达式统计
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await BaseReposition.CountAsync(expression);
        }

        /// <summary>
        /// 分页获取未删除的数据
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> GetPageListAsync(PageDTO page)
        {
            return await BaseReposition.GetPageListAsync(page);
        }
    }
}
