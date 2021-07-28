using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data
{
    public static class DbSetting
    {
        public static string ConnectStr { get; set; }
        public static string Privoder { get; set; }
        public static string DataBase { get; set; }

        /// <summary>
        /// SqlSugar搜索的表实体类所在程序集名称
        /// </summary>
        public static string SugarSearchAssemblyName { get; set; }
    }
}
