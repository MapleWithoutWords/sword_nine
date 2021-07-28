using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 文件转换类
    /// </summary>
    public static class FileConvertHelper
    {
        public static List<string> ExtList { get; set; } = new List<string>
        {
            ".xlsx",
            ".xls",
            ".docx",
        };

        /// <summary>
        /// 转换excel文件和word文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>如果是excel、word文件则转换返回新文件路径，其他文件返回空字符串</returns>
        public static string ExcelWordToHml(string filePath)
        {
            var ext = Path.GetExtension(filePath);
            if (ExtList.Contains(ext) == false)
            {
                return "";
            }
            string reviewFilePath = filePath.Replace(ext, ".html");
            Task.Run(() =>
            {
                string htmlContent = "";
                switch (ext)
                {
                    case ".docx":
                        htmlContent = ce.office.extension.WordHelper.ToHtml(filePath);
                        break;
                    case ".xlsx":
                    case ".xls":
                        htmlContent = ce.office.extension.ExcelHelper.ToHtml(filePath);
                        break;
                }
                File.AppendAllText(filePath.Replace(ext, ".html"), htmlContent);

            });
            return reviewFilePath;
        }
    }
}
