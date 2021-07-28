using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sword.Nine.Domain
{
    /// <summary>
    /// 类别dto
    /// </summary>
    public class SnClassDto: SnClass
    {
        /// <summary>
        /// 目录名称
        /// </summary>
        [SugarColumn(IsIgnore =true)]
        public string DirectoryName { get; set; }
        /// <summary>
        /// 目录Code
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string DirectoryCode { get; set; }

        /// <summary>
        /// 类的属性
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<SnClassAttributeListDto> ClassAttributeList { get; set; } = new List<SnClassAttributeListDto>();
    }
}
