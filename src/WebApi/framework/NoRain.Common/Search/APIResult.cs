using System;
using System.Collections.Generic;
using System.Text;

namespace NoRain.Common
{
    public class APIResult
    {
        /// <summary>
        /// 返回code。0表示成功，1表示失败
        /// </summary>
        public int Code { get; set; } = 0;
        /// <summary>
        /// 数据实体类
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 分页记录数
        /// </summary>
        public long Count { get; set; }
        /// <summary>
        /// 分页参数
        /// </summary>
        public PageDTO Page { get; set; }
        /// <summary>
        /// 正确结果
        /// </summary>
        /// <param name="data"></param>
        /// <param name="page"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static APIResult SuccessResult(object data=null, PageDTO page = null, string msg = "操作成功")
        {
            return new APIResult { Data = data, Msg = msg, Page = page, Count = (page == null ? default : page.Count) };
        }
        /// <summary>
        /// 错误结果
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static APIResult ErrorResult(string msg)
        {
            return new APIResult { Code = 1, Msg = msg };
        }
    }
    public class APIResult<T>
    {
        /// <summary>
        /// 返回code。0表示成功，1表示失败
        /// </summary>
        public int Code { get; set; } = 0;
        /// <summary>
        /// 数据实体类
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 分页记录数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 分页参数
        /// </summary>
        public PageDTO Page { get; set; }
    }
}
