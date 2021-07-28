using System;
using System.Collections.Generic;
using System.Text;

namespace ThreeLayerCode
{
    public class ConfigModel
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> Reference { get; set; } = new List<string>();
        /// <summary>
        /// 
        /// </summary>
        public bool IsCreateDto { get; set; }
        /// <summary>
        /// entity文件输出目录名称
        /// </summary>
        public string FileOutputPath { get; set; }
        public string NameSpace { get; set; }

    }
}
