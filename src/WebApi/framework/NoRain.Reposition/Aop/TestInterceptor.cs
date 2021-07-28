using AspectCore.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NoRain.Reposition.Aop
{
    public class TestInterceptor : AbstractInterceptor
    {

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            SugarClient db = SugarManager.GetDbClient();
            var res = await db.SqlSugarClient.UseTranAsync(() =>
            {
                //执行被拦截的方法
                next(context).Wait();
            });
        }
    }
}
