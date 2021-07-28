using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NoRain.Api
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]//如果接口不使用token的内容，调用接口，可以注释改代码，这样请求接口就可以不带token访问了。如果用到了，例如：userid这个是从token里面取出来的，就必须带token
    public class BaseAuthController : ControllerBase
    {

        /// <summary>
        /// 当前用户Id
        /// </summary>
        public string UserId
        {
            get
            {
                var ret = User.FindFirst("UserId")?.Value;
                return ret ?? Guid.Empty.ToString();
            }
        }
    }
}
