using Microsoft.Extensions.Logging;
using NoRain.Common;
using NoRain.Reposition.BaseReposition;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NoRain.Reposition
{
    public class BaseReposition<TEntity> : BaseReposion, IBaseReposition<TEntity> where TEntity : BaseEntity, new()
    {
        public ILogger _logger { get; private set; }


        protected string EmptyGuid = Guid.Empty.ToString();
        public SqlSugarClient Db { get; private set; }//用来处理事务多表查询和复杂的操作

        /// <summary>
        /// 所有的数据库相关实体集合
        /// </summary>
        public IDictionary<string, List<string>> DicClassName { get { return TableColumns; } }

        /// <summary>
        /// 搜索的时候，加入搜索的列
        /// </summary>
        protected static IDictionary<string, List<string>> TableColumns { get; set; } = new Dictionary<string, List<string>>();

        /// <summary>
        /// 用来处理T表的常用操作
        /// </summary>
        public SimpleClient<TEntity> DbContext { get { return new SimpleClient<TEntity>(Db); } }//用来处理T表的常用操作

        /// <summary>
        /// sqlsugar搜索需要加载数据库实体类所在程序集的名称
        /// </summary>
        public string sugarSearchAssemName { get; private set; }

        public BaseReposition(ILoggerFactory loggerFactory, IDbFactory dbFactory) : base(dbFactory)
        {
            sugarSearchAssemName = DbSetting.SugarSearchAssemblyName;
            LoadTable();
            this._logger = loggerFactory.CreateLogger(typeof(BaseReposition<TEntity>));

            //自定义扩展方法
            var expMethods = new List<SqlFuncExternal>();
            expMethods.Add(new SqlFuncExternal()
            {
                UniqueMethodName = "GroupConcat",
                MethodValue = (expInfo, dType, expContext) =>
                {
                    if (dType == SqlSugar.DbType.MySql)
                        return string.Format("GROUP_CONCAT({0})", expInfo.Args[0].MemberName);
                    else
                        throw new Exception("未实现GroupConcat方法");
                }
            });


            //Db = SugarManager.GetDbClient().SqlSugarClient;

            SqlSugar.DbType dbType = SqlSugar.DbType.MySql;
            switch (DbSetting.Privoder)
            {
                case "sqlserver":
                    dbType = SqlSugar.DbType.SqlServer;
                    break;
                case "mysql":
                    dbType = SqlSugar.DbType.MySql;
                    break;
                case "oracle":
                    dbType = SqlSugar.DbType.Oracle;
                    break;
                case "sqlite":
                    dbType = SqlSugar.DbType.Sqlite;
                    break;
                case "postgresql":
                    dbType = SqlSugar.DbType.PostgreSQL;
                    break;
                default:
                    throw new ApplicationException("不支持的数据库");
            }
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = DbSetting.ConnectStr,
                DbType = dbType,
                InitKeyType = InitKeyType.Attribute,//从特性读取主键和自增列信息
                IsAutoCloseConnection = true,//开启自动释放模式和EF原理一样我就不多解释了
            });
            Db.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices
            {
                SqlFuncServices = expMethods//set ext method
            };
            //执行之前
            Db.Aop.OnLogExecuting = (sql, pars) =>
            {

            };
            //执行完成
            Db.Aop.OnLogExecuted = (sql, pars) =>
            {
                _logger.LogDebug("执行花费时间：" + Db.Ado.SqlExecutionTime.TotalMilliseconds + "mm");
                //
                if (Db.Ado.SqlExecutionTime.TotalMilliseconds > 1000)
                {

                }
            };
            //发生异常
            Db.Aop.OnError = (exp) =>
            {
                _logger.LogError("sql=" + exp.Sql + @"\r\n参数为：" +
                                Db.Utilities.SerializeObject(((SugarParameter[])exp.Parametres).ToDictionary(it => it.ParameterName,
                                    it => it.Value)));

                _logger.LogError("sql执行发送异常：", exp.InnerException);
            };
        }

        /// <summary>
        /// 合并group by 之后的字段值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GroupConcat<T>(T str)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// 加载所有数据实体类在数据库中的属性
        /// </summary>
        private void LoadTable()
        {
            if (TableColumns.Count > 0)
            {
                return;
            }
            var tables = Assembly.Load(sugarSearchAssemName).GetTypes().Where(e => e.IsAbstract == false && typeof(BaseEntity).IsAssignableFrom(e));
            foreach (var tab in tables)
            {
                List<string> cols = new List<string>();
                foreach (var pro in tab.GetProperties())
                {
                    var ignorAttrs = pro.GetCustomAttributes<IgnorPropertyAttribute>();
                    if (ignorAttrs.Count() > 0)
                    {
                        continue;
                    }
                    cols.Add(pro.Name);
                }
                TableColumns[tab.Name] = cols;
            }
        }

        /// <summary>
        /// 获取当前表字段集合
        /// </summary>
        /// <returns></returns>
        protected List<string> GetTableName()
        {
            var list = new List<string>();
            if (TableColumns.ContainsKey(typeof(TEntity).Name))
            {
                list = TableColumns[typeof(TEntity).Name];
            }
            if (list == null)
            {
                throw new ApplicationException("不存在的表名");
            }
            return list;
        }



        /// <summary>
        /// 获取未删除的数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> GetAllListAsync()
        {
            return await DbContext.AsQueryable().Where(c => c.IsDeleted == false).ToListAsync();
        }
        /// <summary>
        /// 根据url参数查询
        /// </summary>
        /// <param name="urlParams"></param>
        /// <returns></returns>
        public virtual async Task<(List<TEntity>, PageDTO)> GetUrlParamAsync(List<UrlParams> urlParams)
        {
            List<string> list = GetTableName();
            IDictionary<string, object> pairs = null;
            //获取查询条件
            var condition = urlParams.GetCondition(list, ref pairs);
            var query = DbContext.AsQueryable().Where(condition.ConditionalModels).OrderBy(condition.OrderCondition);
            var result = await ToPageListAsync(query, condition);
            return (result, condition.Page);
        }

        /// <summary>
        /// 根据Id删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task DeleteByIdAsync(string id, string thisUserId)
        {
            await Db.Updateable<TEntity>().SetColumns(e => new TEntity { IsDeleted = true, LastUpdateTime = DateTime.Now, LastUpdateUser = thisUserId }).Where(e => id == e.Id && e.IsDeleted == false).ExecuteCommandAsync();
        }
        /// <summary>
        /// 删除多条
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual async Task DeleteIdsAsync(IEnumerable<string> ids, string thisUserId)
        {
            await Db.Updateable<TEntity>().SetColumns(e => new TEntity { IsDeleted = true, LastUpdateTime = DateTime.Now, LastUpdateUser = thisUserId }).Where(e => ids.Contains(e.Id) && e.IsDeleted == false).ExecuteCommandAsync();
        }
        /// <summary>
        /// 根据lambda表达式删除数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> expression, string thisUserId)
        {
            return await Db.Updateable<TEntity>().SetColumns(e => new TEntity
            {
                IsDeleted = true,
                LastUpdateTime = DateTime.Now,
                LastUpdateUser = thisUserId
            }).Where(expression).ExecuteCommandAsync();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> InsertAsync(TEntity entity)
        {
            var result = await Db.Insertable(entity).ExecuteCommandAsync();
            return result > 0 ? entity : null;
        }
        /// <summary>
        /// 插入多条数据
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual async Task<bool> InsertAsync(List<TEntity> entities)
        {
            if (entities.Count < 1)
            {
                return true;
            }
            var count = await Db.Insertable<TEntity>(entities).ExecuteCommandAsync();
            return count > 0;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var result = await Db.Updateable(entity).ExecuteCommandAsync();
            return result > 0 ? entity : null;
        }
        /// <summary>
        /// 更新多条数据
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(List<TEntity> entities)
        {
            if (entities.Count < 1)
            {
                return true;
            }
            var count = await Db.Updateable<TEntity>(entities).ExecuteCommandAsync();
            return count > 0;
        }
        /// <summary>
        /// 更新某些列
        /// </summary>
        /// <param name="id"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateColumnAsync(string id, List<UpdateColumnValue> list)
        {
            var idEntity = await GetByIdAsync(id);
            if (idEntity == null)
            {
                throw new ApplicationException("id非法");
            }
            foreach (var item in list)
            {
                item.SetEntityValue(idEntity);
            }
            var ret = await UpdateAsync(idEntity);
            return ret != null ? 1 : 0;
        }
        /// <summary>
        /// 根据lambda表达式删除数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateColumnAsync(Expression<Func<TEntity, TEntity>> setColumns, Expression<Func<TEntity, bool>> whereExpression)
        {
            return await Db.Updateable<TEntity>().SetColumns(setColumns).Where(whereExpression).ExecuteCommandAsync();
        }

        /// <summary>
        /// 根据Id和状态查询数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> GetByIdAsync(string id, int? status)
        {
            var query = DbContext.AsQueryable().Where(e => e.Id == id);
            if (status.HasValue)
            {
                query = query.Where(e => e.Status == status.Value);
            }
            return await query.SingleAsync();
        }
        /// <summary>
        /// 根据Id获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> GetByIdAsync(string id)
        {
            return await DbContext.AsQueryable().Where(e => e.IsDeleted == false && e.Id == id).SingleAsync();
        }



        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<bool> ValidDataAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await DbContext.AsQueryable().AnyAsync(expression);
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
            var query = DbContext.AsQueryable().Where(expression);
            if (orderByEx != null)
            {
                if (isAsc)
                {
                    query = query.OrderBy(orderByEx, OrderByType.Asc);
                }
                else
                {
                    query = query.OrderBy(orderByEx, OrderByType.Desc);
                }
            }


            return await query.FirstAsync();
        }
        /// <summary>
        /// 根据lambda表达式统计
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await Db.Queryable<TEntity>().CountAsync(expression);
        }
        /// <summary>
        /// 根据lambda表达式筛选数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> GetListByLambdaAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await DbContext.AsQueryable().Where(expression).ToListAsync();
        }
        /// <summary>
        /// 根据lambda表达式筛选数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<List<TResult>> GetSelectByLambdaAsync<TResult>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TResult>> selectExpression, Expression<Func<TEntity, object>> orderByEx = null, bool isAsc = true)
        {
            var query = DbContext.AsQueryable().Where(expression);

            if (orderByEx != null)
            {
                if (isAsc)
                {
                    query = query.OrderBy(orderByEx, OrderByType.Asc);
                }
                else
                {
                    query = query.OrderBy(orderByEx, OrderByType.Desc);
                }
            }
            return await query.Select(selectExpression).ToListAsync();
        }



        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newQuery"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<List<T>> ToPageListAsync<T>(ISugarQueryable<T> newQuery, SearchCondition condition)
        {
            List<T> result = new List<T>();
            //如果传了页码，就进行分页展示
            if (condition != null && condition.Page != null)
            {
                var refcount = new RefAsync<int>(0);
                var data = await newQuery.ToPageListAsync(condition.Page.PageIndex, condition.Page.PageDataCount, refcount);

                result = data;
                condition.Page.Count = refcount;
            }
            else
            {
                result = await newQuery.ToListAsync();
            }
            return result;
        }

        /// <summary>
        /// 分页获取未删除的数据
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> GetPageListAsync(PageDTO page)
        {
            RefAsync<int> refint = 0;
            var res = await DbContext.AsQueryable().Where(e => e.IsDeleted == false).ToPageListAsync(page.PageIndex, page.PageDataCount, refint);
            page.Count = refint;
            return res;
        }
    }
}
