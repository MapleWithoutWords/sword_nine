using NoRain.Common;
using SqlSugar;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NoRain.Reposition
{
    public static class UrlParamListExtensions
    {
        /// <summary>
        /// sql操作符
        /// </summary>
        public static IDictionary<string, string> Operators { get; set; } = new Dictionary<string, string>();
        static UrlParamListExtensions()
        {
            Operators["[like]"] = "like";
            Operators["[in]"] = "in";
            Operators["[nin]"] = "not in";
            Operators["[lt]"] = "<";        //小于
            Operators["[gt]"] = ">";        // 大于
            Operators["[lte]"] = "<=";        // 小于等于
            Operators["[gte]"] = ">=";        // 大于等于
            Operators["[ne]"] = "!=";        // 不等于
            Operators["="] = "=";        // 不等于
        }
        public static List<WhereParam> GetWhereParams(this List<UrlParams> list)
        {
            List<WhereParam> res = new List<WhereParam>();
            foreach (var item in list)
            {
                WhereParam whereParam = new WhereParam();
                whereParam.ColumnName = item.Key;
                Regex rg = new Regex(@"(\[.+])");
                string op = "=";
                var matchCollection = rg.Matches(item.Key);
                if (matchCollection.Count > 0)
                {
                    foreach (Match mc in matchCollection)
                    {
                        if (Operators.ContainsKey(mc.Value))
                        {
                            op = mc.Value;
                        }
                    }
                }

                if (op != "=" && string.IsNullOrEmpty(op) == false)
                {
                    whereParam.ColumnName = item.Key.Replace(op, "");
                }
                whereParam.Operation = Operators[op];
                if (item.Values.Count <= 0)
                {
                    continue;
                }
                item.Values.Remove(" ");
                item.Values.Remove("");

                if (item.Values.Count <= 0)
                {
                    continue;
                }
                whereParam.Value = item.Values;


                res.Add(whereParam);
            }
            return res;
        }
        /// <summary>
        /// 获取搜索查询条件
        /// </summary>
        /// <param name="list"></param>
        /// <param name="propertyNames"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public static SearchCondition GetCondition(this List<UrlParams> list, List<string> propertyNames, ref IDictionary<string, object> pairs, string priex = null)
        {

            SearchCondition condition = new SearchCondition();
            ///获取where参数以及操作符
            var listWhereParams = list.GetWhereParams();

            pairs = new Dictionary<string, object>();
            ///sql条件实体类
            List<IConditionalModel> conditionalModels = new List<IConditionalModel>();

            ///获取where条件字符串形式
            var whereCondition = listWhereParams.GetWhereCondition(ref conditionalModels, ref pairs, propertyNames, priex);

            condition.WhereCondition = $"{(priex.IsNullOrEmpty() ? "" : $"{priex}.")}IsDeleted=0 {whereCondition}";
            condition.ConditionalModels = conditionalModels;
            condition.ConditionalModels.Add(ConvetToConditionalModel(new WhereParam { ColumnName = $"isDeleted", Operation = "=", Value = new List<string> { "0" } }, priex));
            var sortValues = listWhereParams.FirstOrDefault(e => e.ColumnName.ToLower() == "sort");

            if (sortValues == null)
            {
                sortValues = new WhereParam() { Value = new List<string> { $"createTime desc" } };
            }
            if (sortValues.Value[0].Split(',').Any(e => e.Split(' ').Length != 2 ||
            (e.Split(' ')[1].Equals("asc", StringComparison.InvariantCultureIgnoreCase) &&
            e.Split(' ')[1].Equals("desc", StringComparison.InvariantCultureIgnoreCase))))
            {
                sortValues = new WhereParam() { Value = new List<string> { $"{(priex.IsNoNullAndNoEmpty() ? $"{priex}." : "")}createTime desc " } };
            }
            var sortStringArr = sortValues.Value[0].Split(",");
            List<string> orderList = new List<string>();
            for (int i = 0; i < sortStringArr.Length; i++)
            {
                var element = sortStringArr[i].Split(' ');
                if (propertyNames.Any(e => e.Equals( element[0], StringComparison.InvariantCultureIgnoreCase)))
                {
                    orderList.Add(sortStringArr[i]);
                }
            }
            string order = string.Join(",", orderList);
            if (priex.IsNullOrEmpty() == false)
            {
                order = string.Join(",", orderList.Select(e => $"{priex}.{e}"));
            }
            pairs["sort"] = order;
            condition.OrderCondition = order;
            var pageIndexWhere = listWhereParams.FirstOrDefault(e => e.ColumnName.ToLower() == "pageindex");
            var pagedatacountWhere = listWhereParams.FirstOrDefault(e => e.ColumnName.ToLower() == "pagedatacount");
            var recordcountWhere = listWhereParams.FirstOrDefault(e => e.ColumnName.ToLower() == "recordcount");
            if (pageIndexWhere != null && (pagedatacountWhere != null || recordcountWhere != null))
            {
                condition.Page = new Common.PageDTO();
                if (int.TryParse(pageIndexWhere.Value[0], out int pageindex))
                {
                    condition.Page.PageIndex = pageindex;
                }
                if (pagedatacountWhere != null)
                {
                    if (int.TryParse(pagedatacountWhere.Value[0], out int pagedatacount))
                    {
                        condition.Page.PageDataCount = pagedatacount;
                    }
                }
                if (recordcountWhere != null)
                {
                    if (long.TryParse(recordcountWhere.Value[0], out long recordCount))
                    {
                        condition.Page.Count = recordCount;
                    }
                }
            }
            return condition;

        }

        /// <summary>
        /// 获取where查询条件字符串
        /// </summary>
        /// <param name="listWhereParams"></param>
        /// <param name="conditionalModels"></param>
        /// <param name="pairs"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        public static String GetWhereCondition(this List<WhereParam> listWhereParams, ref List<IConditionalModel> conditionalModels, ref IDictionary<string, object> pairs, List<string> propertyNames = null, string priex = null)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in listWhereParams)
            {
                if (item.ColumnName.ToLower() == "pageindex" ||
                    item.ColumnName.ToLower() == "pagedatacount")
                {
                    continue;
                }

                string sqlop = item.Operation;

                if (sqlop == "like")
                {
                    pairs[item.ColumnName] = $"%{item.Value[0]}%";
                }
                else if (sqlop == "in" || sqlop == "not in")
                {
                    pairs[item.ColumnName] = item.Value[0].Split(',');
                }
                else if (sqlop == ">" || sqlop == "<" || sqlop == "<=" || sqlop == ">=" || sqlop == "!=" || sqlop == "=")
                {
                    if (item.Value.Count > 0)
                    {
                        pairs[item.ColumnName] = item.Value[0];
                    }
                }
                else
                {
                    if (item.Value.Count > 0)
                    {
                        pairs[item.ColumnName] = item.Value[0];
                    }
                }
                if (propertyNames != null && propertyNames.Count > 0)
                {
                    if (propertyNames.Contains(item.ColumnName) == false)
                    {
                        continue;
                    }
                }
                if (priex.IsNullOrEmpty())
                {
                    sb.Append($"and {item.ColumnName} {sqlop} @{item.ColumnName} ");
                    conditionalModels.Add(ConvetToConditionalModel(item));
                }
                else
                {
                    sb.Append($"and {priex}.{item.ColumnName} {sqlop} @{item.ColumnName} ");
                    conditionalModels.Add(ConvetToConditionalModel(item, priex));
                }

            }


            return sb.ToString();
        }


        /// <summary>
        /// 将where条件值转换，例如:"123"->"'123'"
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static string GetWhereValue(string val)
        {

            if (int.TryParse(val, out int i) ||
                double.TryParse(val, out double d) ||
                long.TryParse(val, out long l) ||
                short.TryParse(val, out short sh) ||
                float.TryParse(val, out float f)
                )
            {
                return val;
            }
            return $"'{val}'";
        }

        /// <summary>
        /// 将传递的条件转化成sugar查询条件
        /// </summary>
        /// <param name="param">条件对象</param>
        /// <returns></returns>
        private static IConditionalModel ConvetToConditionalModel(this WhereParam param, string priex = null)
        {
            if (param.Value.Count < 1)
            {
                return null;
            }
            ConditionalModel m = new ConditionalModel();
            if (priex.IsNullOrEmpty())
            {
                m.FieldName = param.ColumnName;
            }
            else
            {
                m.FieldName = $"{priex}.{param.ColumnName}";
            }
            switch (param.Operation)
            {
                case "=":
                    m.ConditionalType = ConditionalType.Equal;
                    m.FieldValue = param.Value[0];
                    break;
                case ">":
                    m.ConditionalType = ConditionalType.GreaterThan;
                    m.FieldValue = param.Value[0];
                    break;
                case ">=":
                    m.ConditionalType = ConditionalType.GreaterThanOrEqual;
                    m.FieldValue = param.Value[0];
                    break;
                case "<":
                    m.ConditionalType = ConditionalType.LessThan;
                    m.FieldValue = param.Value[0];
                    break;
                case "<=":
                    m.ConditionalType = ConditionalType.LessThanOrEqual;
                    m.FieldValue = param.Value[0];
                    break;
                case "like":
                    m.ConditionalType = ConditionalType.Like;
                    m.FieldValue = param.Value[0];
                    break;
                case "in":
                    m.ConditionalType = ConditionalType.In;
                    m.FieldValue = string.Join(",", param.Value);
                    break;
                case "not in":
                    m.ConditionalType = ConditionalType.NotIn;
                    m.FieldValue = string.Join(",", param.Value);
                    break;

            }
            return m;
        }

    }

    public class WhereParam
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 操作符
        /// </summary>
        public string Operation { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public List<string> Value { get; set; }
    }
}
