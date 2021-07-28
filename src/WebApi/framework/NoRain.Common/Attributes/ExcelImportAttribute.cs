using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    /// <summary>
    /// excel导入
    /// </summary>
    public class ExcelImportAttribute: Attribute
    {
        /// <summary>
        /// 对应的excel列名
        /// </summary>
        public string ExcelColumnName { get; set; }

        /// <summary>
        /// string,datetime,bool,number
        /// </summary>
        public string ExcelColumnType { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">对应excel列名</param>
        public ExcelImportAttribute(string name,string excelColumnType="string")
        {
            ExcelColumnName = name;
            ExcelColumnType = excelColumnType;
        }
    }
}
