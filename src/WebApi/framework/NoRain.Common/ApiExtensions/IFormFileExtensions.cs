using Microsoft.AspNetCore.Http;
using SPCS.Common.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.AspNetCore.Http
{
    public static class IFormFileExtensions
    {
        /// <summary>
        /// 将文件保存在wwwroot目录下
        /// </summary>
        /// <param name="item">文件对象</param>
        /// <param name="direname">目录名称:例如:staticFile/maintain,就在:C:/xxx/xxx/xxx/Wanna.EMS.Api/wwwroot/staticFile/maintain</param>
        /// <returns></returns>
        public static string SaveToWwwrootAsync(this IFormFile item, string direname, out string md5val)
        {
            var ext = Path.GetExtension(item.FileName);

            using (var st = item.OpenReadStream())
            {

                md5val = MD5Helper.CalcMD5(st);
                var rootDire = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");

                foreach (var dirItem in direname.Trim('/').Split('/'))
                {
                    rootDire = Path.Combine(rootDire, dirItem);
                    rootDire.CreateDirectory();
                }
                var filePath = Path.Combine(rootDire, $"{md5val}{ext}");
                if (File.Exists(filePath))
                {
                    return $"/{direname.Trim('/')}/{md5val}{ext}";
                }
                st.SaveFile(filePath);
                return $"/{direname.Trim('/')}/{md5val}{ext}";

            }
        }

        /// <summary>
        /// 保存文件集合到wwwroot目录下,多个文件,路径以逗号分隔
        /// </summary>
        /// <param name="formFiles"></param>
        /// <param name="fileExtStr">文件上传限制后缀,</param>
        /// <param name="direName"></param>
        /// <returns>Item1:文件路径,Item2:错误消息</returns>
        public static (string, string) SaveToWwwrootAsync(this IFormFileCollection formFiles, string fileExtStr, string direName)
        {
            //获取文件上传后缀限制
            List<string> list = new List<string>();
            foreach (var item in formFiles)
            {
                var ext = Path.GetExtension(item.FileName);
                //如果配置为空，则不限制文件后缀。当配置不为空，则进行判断
                if (fileExtStr.IsNoNullAndNoEmpty())
                {
                    if (fileExtStr.Contains(ext) == false)
                    {
                        return (null, "不支持的文件类型");
                    }
                }
                list.Add(item.SaveToWwwrootAsync(direName, out string md5val));
            }
            return (string.Join(",", list), null);
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fileName"></param>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static string SaveToDirePathAsync(this IFormFile item,string fileName,string dirPath)
        {
            using (var st = item.OpenReadStream())
            {

                dirPath.CreateDirectory();
                var filePath = Path.Combine(dirPath, fileName);
                st.SaveFile(filePath);
                return $"{dirPath.TrimEnd('/')}/{fileName}";

            }
        }
    }
}
