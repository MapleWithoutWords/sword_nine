using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public class BasciSetting:ISettingFac
    {

        /// <summary>
        /// 注册dao层的名称
        /// </summary>
        public static string RegisterDaoName { get; set; }

        /// <summary>
        /// 注册service层名称
        /// </summary>
        public static string RegisterServiceName { get; set; }
    }
}
