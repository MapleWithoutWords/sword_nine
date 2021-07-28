using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoRain.Common
{
    public static class UrlParamListExtensions
    {
        /// <summary>
        /// 尝试获取string参数
        /// </summary>
        /// <param name="list"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool TryGetString(this List<UrlParams> list, string key, out string val)
        {
            val = default;
            var parvallist = list.FirstOrDefault(e => e.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            if (parvallist == null)
            {
                return false;
            }
            if (parvallist.Values == null || parvallist.Values.Count < 1 || parvallist.Values[0].IsNullOrEmpty())
            {
                return false;
            }
            val = parvallist.Values[0];
            return true;
        }

        /// <summary>
        /// 尝试获取int参数
        /// </summary>
        /// <param name="list"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool TryGetInt(this List<UrlParams> list, string key, out int val)
        {
            val = default;
            var parvallist = list.FirstOrDefault(e => e.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            if (parvallist == null)
            {
                return false;
            }
            if (parvallist.Values == null || parvallist.Values.Count < 1 || parvallist.Values[0].IsNullOrEmpty())
            {
                return false;
            }
            return int.TryParse(parvallist.Values[0], out val);
        }

        /// <summary>
        /// 尝试获取int参数
        /// </summary>
        /// <param name="list"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool TryGetDouble(this List<UrlParams> list, string key, out double val)
        {
            val = default;
            var parvallist = list.FirstOrDefault(e => e.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            if (parvallist == null)
            {
                return false;
            }
            if (parvallist.Values == null || parvallist.Values.Count < 1 || parvallist.Values[0].IsNullOrEmpty())
            {
                return false;
            }
            return double.TryParse(parvallist.Values[0], out val);
        }

        /// <summary>
        /// 尝试获取Guid参数
        /// </summary>
        /// <param name="list"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool TryGetGuid(this List<UrlParams> list, string key, out string val)
        {
            val = default;
            var parvallist = list.FirstOrDefault(e => e.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            if (parvallist == null)
            {
                return false;
            }
            if (parvallist.Values == null || parvallist.Values.Count < 1 || parvallist.Values[0].IsNullOrEmpty())
            {
                return false;
            }
            if (Guid.TryParse(parvallist.Values[0],out Guid guidVal)==false)
            {
                return false;
            }
            val = guidVal.ToString();
            return true;
        }

        /// <summary>
        /// 尝试获取datetime参数
        /// </summary>
        /// <param name="list"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool TryGetDateTime(this List<UrlParams> list, string key, out DateTime val)
        {
            val = default;
            var parvallist = list.FirstOrDefault(e => e.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            if (parvallist == null)
            {
                return false;
            }
            if (parvallist.Values == null || parvallist.Values.Count < 1 || parvallist.Values[0].IsNullOrEmpty())
            {
                return false;
            }
            return DateTime.TryParse(parvallist.Values[0], out val);
        }

        /// <summary>
        /// 尝试获取分页参数
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static PageDTO TryGetPageParams(this List<UrlParams> list)
        {
            PageDTO page = null;
            var parvallist = list.FirstOrDefault(e => e.Key.Equals("pageindex", StringComparison.InvariantCultureIgnoreCase));
            if (parvallist == null)
            {
                return null;
            }
            if (int.TryParse(parvallist.Values[0], out int outIndex) == false)
            {
                outIndex = 1;
            }
            parvallist = list.FirstOrDefault(e => e.Key.Equals("pagedatacount", StringComparison.InvariantCultureIgnoreCase));
            if (parvallist == null || int.TryParse(parvallist.Values[0], out int outdataCount) == false)
            {
                outdataCount = 10;
            }
            page = new PageDTO
            {
                PageIndex = outIndex,
                PageDataCount = outdataCount,
            };
            return page;
        }
    }
}
