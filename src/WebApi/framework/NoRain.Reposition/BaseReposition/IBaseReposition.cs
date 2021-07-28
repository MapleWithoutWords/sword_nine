using Common.APICommon;
using NoRain.Common;
using SAM.Common.APICommon;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NoRain.Reposition.BaseReposition
{
    public interface IBaseReposition<TEntity> : IInjectFac where TEntity:BaseEntity
    {
        /// <summary>
        /// 所有的数据库相关实体集合
        /// </summary>
        public IDictionary<string,List<string>> DicClassName { get; }

        /// <summary>
        /// 根据Id删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteByIdAsync(string id, string thisUserId);
        /// <summary>
        /// 删除多条
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task DeleteIdsAsync(IEnumerable<string> ids, string thisUserId);
        /// <summary>
        /// 根据lambda表达式删除数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(Expression<Func<TEntity, bool>> expression, string thisUserId);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntity> InsertAsync(TEntity entity);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(TEntity entity);
        /// <summary>
        /// 根据lambda表达式删除数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<int> UpdateColumnAsync(Expression<Func<TEntity, TEntity>> setColumns, Expression<Func<TEntity, bool>> whereExpression);

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
        Task<(List<TEntity>, PageDTO)> GetUrlParamAsync(List<UrlParams> urlParams);

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<bool> ValidDataAsync(Expression<Func<TEntity, bool>> expression);


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
        Task<bool> UpdateAsync(List<TEntity> entities);
        /// <summary>
        /// 插入多条数据
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<bool> InsertAsync(List<TEntity> entities);
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
