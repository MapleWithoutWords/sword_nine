using Common.APICommon;
using NoRain.Common;
using NoRain.Reposition;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NoRain.Service
{
    public interface IBaseService<TEntity>:IInjectFac
                                            where TEntity:BaseEntity
    {

        public List<string> UpdateIgnorPros { get; set; }
        /// <summary>
        /// 根据Id删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<APIResult> DeleteByIdAsync(string id);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<APIResult> InsertAsync(TEntity entity);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<APIResult> UpdateAsync(TEntity entity);

        /// <summary>
        /// 获取未删除的数据
        /// </summary>
        /// <returns></returns>
        Task<List<TEntity>> GetAllListAsync();

        /// <summary>
        /// 分页获取未删除的数据
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        Task<List<TEntity>> GetPageListAsync(PageDTO page);

        /// <summary>
        /// 根据Id获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TEntity> GetByIdAsync(string id);

        /// <summary>
        /// 根据url参数查询
        /// </summary>
        /// <param name="urlParams"></param>
        /// <returns></returns>
        Task<APIResult> GetUrlParamAsync(List<UrlParams> urlParams);

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<bool> ValidExistsAsync(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 判断数据合法性
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<APIResult> ValidDataAsync(TEntity entity);

        /// <summary>
        /// 根据Id和状态查询数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<TEntity> GetByIdAsync(string id, int? status);

        /// <summary>
        /// 更新多条数据
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<APIResult> UpdateAsync(List<TEntity> entities);
        /// <summary>
        /// 插入多条数据
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<APIResult> InsertAsync(List<TEntity> entities);
        /// <summary>
        /// 根据lambda表达式筛选数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<List<TEntity>> GetListByLambdaAsync(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 根据lambda表达式筛选数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<List<TResult>> GetSelectByLambdaAsync<TResult>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TResult>> selectExpression, Expression<Func<TEntity, object>> orderByEx = null, bool isAsc = true);
        /// <summary>
        /// 删除多条
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<APIResult> DeleteIdsAsync(IEnumerable<string> ids);


        /// <summary>
        /// 更新某些列
        /// </summary>
        /// <param name="id"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<int> UpdateColumnAsync(string id, List<UpdateColumnValue> list);

        /// <summary>
        /// 根据lambda表达式取第一个数据
        /// </summary>
        /// <param name="expression">where条件</param>
        /// <param name="orderEx">排序条件</param>
        /// <param name="isAsc">是否升序,默认升序</param>
        /// <returns></returns>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> orderByEx = null, bool isAsc = true);

        /// <summary>
        /// 根据lambda表达式统计
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> expression);
    }
}
