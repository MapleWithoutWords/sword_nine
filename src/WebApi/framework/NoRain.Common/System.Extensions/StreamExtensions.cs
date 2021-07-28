using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System
{
    /// <summary>
    /// 流扩展
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="st"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public static string SaveFile(this Stream st, string savePath)
        {
            using (var fs = File.OpenWrite(savePath))
            {
                st.CopyTo(fs);
                return savePath;
            }
        }
    }
}
