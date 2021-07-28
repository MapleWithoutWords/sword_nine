using AspectCore.DynamicProxy;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NoRain.Reposition
{
    public class CustomInterceptorAttribute : AbstractInterceptorAttribute
    {
        /// <summary>
        /// 是否开启事务
        /// </summary>
        public bool IsTran { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isTran"></param>
        public CustomInterceptorAttribute(bool isTran = false)
        {
            IsTran = isTran;
        }

        /// <summary>
        /// 拦截器执行
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            SugarClient db = SugarManager.GetDbClient();
            if (IsTran)
            {
                var res = await db.SqlSugarClient.UseTranAsync(() =>
                {
                    //执行被拦截的方法
                    next(context).Wait();
                });
            }
            else
            {
                await next(context);
            }
        }
    }
}
