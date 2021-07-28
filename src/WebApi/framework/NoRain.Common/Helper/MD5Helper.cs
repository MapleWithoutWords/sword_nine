using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SPCS.Common.Helper
{
    public class MD5Helper
    {
        /// <summary>
        /// 计算字符串的md5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CalcMD5(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return CalcMD5(bytes);
        }

        /// <summary>
        /// 计算byte数组的md5
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string CalcMD5(byte[] bytes)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] computeBytes = md5.ComputeHash(bytes);
                string result = "";
                for (int i = 0; i < computeBytes.Length; i++)
                {
                    result += computeBytes[i].ToString("x").Length == 1 ? "0" + computeBytes[i].ToString("x") : computeBytes[i].ToString("x");
                }
                return result;
            }
        }

        /// <summary>
        /// 计算流的MD5
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string CalcMD5(Stream stream)
        {
            using (MD5 md5 = MD5.Create())
            {
                stream.Position = 0;
                byte[] computeBytes = md5.ComputeHash(stream);
                string result = "";
                for (int i = 0; i < computeBytes.Length; i++)
                {
                    result += computeBytes[i].ToString("x").Length == 1 ? "0" + computeBytes[i].ToString("x") : computeBytes[i].ToString("x");
                }
                stream.Position = 0;
                return result;
            }
        }

        /// <summary>
        /// 获取一个随机的特殊字符串
        /// </summary>
        /// <param name="saltLen"></param>
        /// <returns></returns>
        public static string GetSalt(int saltLen)
        {
            StringBuilder sb = new StringBuilder();
            Random rd = new Random(Guid.NewGuid().GetHashCode());
            int len = rd.Next(6, saltLen);
            char[] ch = { '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', '{', '}', '|', '?', ':', '<', '>', '？', '·', '。', '！', '‘', '’', '“', '”', '：', '1', '2' };
            for (int i = 0; i < len; i++)
            {
                sb.Append(ch[rd.Next(ch.Length)]);
            }
            return sb.ToString();
        }


    }
}
