using System;
using System.Collections.Generic;
using System.Text;

namespace Sword.Nine.Rule
{
    /// <summary>
    /// 代码生成
    /// </summary>
    public interface ICodeBI
    {
        /// <summary>
        /// 代码生成运行
        /// </summary>
        /// <param name="dataSourceDto">数据源信息</param>
        /// <param name="saveDir">生成的压缩包，存放的目录</param>
        /// <param name="fileNameOutExtensions">文件名称，不包含后缀名</param>
        /// <returns>压缩包路径</returns>
        string Run(DataSourceDto dataSourceDto, string saveDir);
    }
}
