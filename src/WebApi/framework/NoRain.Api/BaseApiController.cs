using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NoRain.Common;
using NoRain.Reposition;
using NoRain.Service;

namespace NoRain.Api
{
    /// <summary>
    /// 基础接口
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController<TService, TEntity> : BaseAuthController
                                                    where TEntity : BaseEntity
                                                    where TService : IBaseService<TEntity>
    {
        /// <summary>
        /// 更新忽略某些属性
        /// </summary>
        protected List<string> UpdateIgnorPros { get; set; } = new List<string>();

        /// <summary>
        /// 服务接口
        /// </summary>
        protected TService InstanceService { get; set; }
        protected ILogger Logger { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="InstanceService"></param>
        public BaseApiController(TService InstanceService, ILoggerFactory loggerFactory)
        {
            this.InstanceService = InstanceService;
            Logger = loggerFactory.CreateLogger<BaseApiController<TService, TEntity>>();
        }

        /// <summary>
        /// 根据Url参数来查询数据
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 例如：/api/xxx/getbyurl?xxx[like]=val1&amp;age[gte]=18&amp;id[$in]=1,3,5&amp;pageindex=1&amp;pagedatacount=10
        /// <span>搜索参数符号说明</span>
        /// <code>
        ///     [like]:模糊查询
        ///     [gt]:大于
        ///     [lt]:小于
        ///     [gte]:大于等于
        ///     [lte]:小于等于
        ///     [in]:在集合里面，
        ///     [nin]:不在集合中
        ///     [ne]:不等于
        /// </code>
        /// 搜索通用参数:
        ///     pageindex:当前页
        ///     pagedatacount：一页取多少数据
        /// </remarks>
        [HttpGet("getbyurl")]
        [ProducesDefaultResponseType(typeof(APIResult<List<BaseEntity>>))]
        public virtual async Task<IActionResult> GetByUrl()
        {
            var urlParams = Request.Query.GetUrlParams();
            var model = await InstanceService.GetUrlParamAsync(urlParams);
            return this.Json(model);
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        [HttpPut("insert")]
        public virtual async Task<IActionResult> Insert([FromBody] TEntity en)
        {
            //en.SetUserId(UserId);
            var model = await InstanceService.InsertAsync(en);
            return this.Json(model);
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        [HttpPost("update")]
        public virtual async Task<IActionResult> Update([FromBody] TEntity en)
        {
            var model = await InstanceService.UpdateAsync(en);
            return this.Json(model);
        }


        /// <summary>
        /// 根据Id删除实体
        /// </summary>
        /// <param name="id">实体id字段</param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResult<string>))]
        public virtual async Task<IActionResult> Delete(string id)
        {
            await InstanceService.DeleteByIdAsync(id);
            return this.JsonApiResultData(null);
        }

        /// <summary>
        /// 根据Id和状态查询数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet("getbyid/{id}")]
        public virtual async Task<IActionResult> GetById(string id, int status = 0)
        {
            var model = await InstanceService.GetByIdAsync(id, status);
            if (model == null)
            {
                return this.JsonErrorData("id不存在");
            }
            return this.JsonApiResultData(model);
        }

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <param name="ids">id集合</param>
        /// <returns></returns>
        [HttpDelete("deleteids")]
        public virtual async Task<IActionResult> DeleteIds([FromBody] string[] ids)
        {
            var ret = await InstanceService.DeleteIdsAsync(ids);
            return this.Json(ret);
        }


        /// <summary>
        /// 更新某些列
        /// </summary>
        /// <param name="id">数据id</param>
        /// <param name="list">属性名和属性值的集合</param>
        /// <returns></returns>
        [HttpPost("updatecolumn/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResult<int>))]
        public async Task<IActionResult> UpdateColumnAsync(string id, [FromBody] List<UpdateColumnValue> list)
        {
            var ret = await InstanceService.UpdateColumnAsync(id, list);
            return this.JsonApiResultData(ret);
        }
    }
}