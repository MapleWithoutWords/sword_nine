using System;
using System.Collections.Generic;
using System.Text;

namespace Sword.Nine.Domain
{
    /// <summary>
    /// 数据源导入
    /// </summary>
    public class DataSoureceImportDto
    {
        /// <summary>
        /// 新增或修改数据源
        /// </summary>
        public List<DataSourceImportModel> UpdateOrInsertDataSourceList { get; set; } = new List<DataSourceImportModel>();
        /// <summary>
        /// 新增或修改目录
        /// </summary>
        public List<DirectoryImportModel> UpdateOrInsertDirectoryList { get; set; } = new List<DirectoryImportModel>();
        /// <summary>
        /// 新增或修改类别
        /// </summary>
        public List<ClassImportModel> UpdateOrInsertClassList { get; set; } = new List<ClassImportModel>();
        /// <summary>
        /// 新增或修改属性
        /// </summary>
        public List<AttributeImportModel> UpdateOrInsertAttributeList { get; set; } = new List<AttributeImportModel>();
    }
}
