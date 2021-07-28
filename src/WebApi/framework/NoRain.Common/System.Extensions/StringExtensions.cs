using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace System
{
    public static class StringExtensions
    {
        public static string ToFistUpper(this string str)
        {
            if (str.Length < 1)
            {
                return string.Empty;
            }
            if (str[0] >= 'A' && str[0] <= 'Z')
            {
                return str;
            }
            return (char)(str[0] - 32) + str.Substring(1);
        }
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
        public static bool IsNoNullAndNoEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }
        /// <summary>
        /// 字符串不为空，不为空guid。是一个Guid
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsGuidAndNoGuidEmpty(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return false;
            }
            if (str == Guid.Empty.ToString())
            {
                return false;
            }
            if (Guid.TryParse(str, out Guid res) == false)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 字符串是否为guid
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsGuid(this string str)
        {
            if (Guid.TryParse(str, out Guid res) == false)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 判断是否是eamil
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmail(this string str)
        {
            if (str.IsNoNullAndNoEmpty())
            {
                return false;
            }
            if (Regex.IsMatch(str, @"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+") == false)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 字符串为null或者'' 或者000000-000000-00000-00000
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyOrGuidEmpty(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return true;
            }
            if (str == Guid.Empty.ToString())
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 尝试转换成int
        /// </summary>
        /// <param name="str"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool TryPartInt(this string str,out int val)
        {
            return int.TryParse(str, out val);
        }
        /// <summary>
        /// 尝试转换成float
        /// </summary>
        /// <param name="str"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool TryPartFloat(this string str, out float val)
        {
            return float.TryParse(str, out val);
        }
        /// <summary>
        /// 尝试转换成double
        /// </summary>
        /// <param name="str"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool TryPartDouble(this string str, out double val)
        {
            return double.TryParse(str, out val);
        }
        /// <summary>
        /// 尝试转换成datetime
        /// </summary>
        /// <param name="str"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool TryPartDateTime(this string str, out DateTime val)
        {
            return DateTime.TryParse(str, out val);
        }

        /// <summary>
        /// 反序列化xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlContent"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(this string xmlContent) where T : class, new()
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent)))
            {
                return xs.Deserialize(ms) as T;
            }
        }

        /// <summary>
        /// 在该目录下创建年月日目录
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string CreateDateDirectory(this string filePath)
        {
            filePath = filePath.CreateDirectory();

            filePath = Path.Combine(filePath, DateTime.Now.Year.ToString());
            if (Directory.Exists(filePath) == false)
            {
                Directory.CreateDirectory(filePath);
            }
            filePath = Path.Combine(filePath, DateTime.Now.Month.ToString());
            if (Directory.Exists(filePath) == false)
            {
                Directory.CreateDirectory(filePath);
            }
            filePath = Path.Combine(filePath, DateTime.Now.Date.ToString());
            if (Directory.Exists(filePath) == false)
            {
                Directory.CreateDirectory(filePath);
            }
            return filePath;
        }

        /// <summary>
        /// 目录不存在则创建
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string CreateDirectory(this string filePath)
        {
            if (Directory.Exists(filePath) == false)
            {
                Directory.CreateDirectory(filePath);
            }
            return filePath;
        }
    }
}
