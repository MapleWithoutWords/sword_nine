using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class ValueTypeExtensions
    {
        /// <summary>
        /// 获取该数的二进制组成数字
        /// </summary>
        /// <param name="actionValue"></param>
        /// <returns></returns>
        public static List<long> GetTwoComposeNum(this long actionValue)
        {
            string twoVal = Convert.ToString(actionValue, 2);
            List<long> list = new List<long>();
            for (int i = twoVal.Length-1; i >=0; i--)
            {
                var element = Convert.ToInt64(twoVal[i].ToString());
                var item = element * Math.Pow(2, (twoVal.Length - 1-i));
                if (item!=0)
                {
                    list.Add(Convert.ToInt64(item));
                }
            }
            return list;

        }

        /// <summary>
        /// 获取该数字集合的二进制组成数字
        /// </summary>
        /// <param name="listVals"></param>
        /// <returns></returns>
        public static IDictionary<long, List<long>> GetTwoComposeNum(this IEnumerable<long> listVals)
        {
            IDictionary<long, List<long>> dic = new Dictionary<long, List<long>>();
            foreach (var actionValue in listVals)
            {
                dic[actionValue] = GetTwoComposeNum(actionValue);
            }
            return dic;

        }

        /// <summary>
        /// 判断该数的二进制数字是否由集合里的数组成
        /// </summary>
        /// <param name="actionValue"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsCompose(this long actionValue, List<long> list)
        {
            var arr = actionValue.GetTwoComposeNum();
            list = list.Distinct().ToList();
            if (arr.LongCount() > list.LongCount())
            {
                return false;
            }
            var intersectList = arr.Intersect(list);
            if (intersectList.SequenceEqual(arr)==false)
            {
                return false;
            }
            return true;
        }
    }
}
