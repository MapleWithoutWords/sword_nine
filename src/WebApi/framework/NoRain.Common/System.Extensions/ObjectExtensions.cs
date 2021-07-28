using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace System
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// 转换成int
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ToInt(this object obj)
        {
            return Convert.ToInt32(obj);
        }
        /// <summary>
        /// json序列化对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }


    }
    /// <summary>
    /// 对象拷贝
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TRes"></typeparam>
    public class ObjectCopy<TEntity, TRes> where TEntity : class, new() where TRes : class, new()
    {
        private static IDictionary<string, Func<TEntity, TRes>> mapperDicts = new Dictionary<string, Func<TEntity, TRes>>();
        /// <summary>
        /// 实体映射，暂且不能映射导航属性。规则是：类中的属性名字必须一样
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TRes Copy(TEntity obj)
        {
            Type enType = typeof(TEntity);
            Type resType = typeof(TRes);
            string key = $"mapperKey_{enType.Name}_{resType.Name}";
            //判断缓存里是否存在该委托
            if (mapperDicts.ContainsKey(key))
            {
                return mapperDicts[key](obj);
            }
            //创建表达式树参数
            ParameterExpression parameter = Expression.Parameter(enType, "e");
            List<MemberBinding> list = new List<MemberBinding>();
            //遍历属性
            foreach (var item in resType.GetProperties())
            {
                var pro = enType.GetProperty(item.Name);
                if (pro == null)
                {
                    continue;
                }
                MemberExpression propertyExpression = Expression.Property(parameter, pro);
                MemberBinding bind = Expression.Bind(item, propertyExpression);
                list.Add(bind);
            }
            //遍历字段
            foreach (var item in resType.GetFields())
            {
                var filed = enType.GetField(item.Name);
                if (filed == null)
                {
                    continue;
                }
                MemberExpression property = Expression.Field(parameter, filed);
                MemberBinding bind = Expression.Bind(item, property);
                list.Add(bind);
            }
            //将属性值和字段值复制给新的对象，
            MemberInitExpression init = Expression.MemberInit(Expression.New(resType), list);
            //获取表达式树
            Expression<Func<TEntity, TRes>> expression = Expression.Lambda<Func<TEntity, TRes>>(init, parameter);
            Func<TEntity, TRes> func = expression.Compile();
            //将委托缓存起来
            mapperDicts[key] = func;
            return func(obj);
        }
    }
}
