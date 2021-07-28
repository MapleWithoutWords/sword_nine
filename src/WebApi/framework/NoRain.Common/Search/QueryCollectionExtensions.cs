using NoRain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http
{
    public static class QueryCollectionExtensions
    {
        public  static List<UrlParams> GetUrlParams(this IQueryCollection queries)
        {
            List<UrlParams> list = new List<UrlParams>();
            foreach (var item in queries)
            {
                if (item.Value.Count<1)
                {
                    continue;
                }
                if (string.IsNullOrEmpty(item.Value[0]))
                {
                    continue;
                }
                UrlParams param = new UrlParams();
                param.Key = item.Key.Trim().ToFistUpper();
                foreach (var val in item.Value)
                {
                    param.Values.Add(val.Trim());
                }
                list.Add(param);
            }

            return list;
        }
    }
}
