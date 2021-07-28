using System;
using System.Collections.Generic;
using System.Text;

namespace Sword.Nine.Untils
{
    public static class SystemConst
    {
        static SystemConst()
        {
            #region 属性值类型
            DicValueType["字符串"] = 0;
            DicValueType["整数"] = 1;
            DicValueType["小数"] = 2;
            DicValueType["时间"] = 3;
            DicValueType["引用"] = 4;
            DicValueType["长整数"] = 5;
            DicValueType["双精度小数"] = 6;
            DicValueType["布尔"] = 7;
            DicValueType["Decimal"] = 8;
            #endregion

            #region 数据源类型
            DataSourceType["MYSQL"] = 0;
            DataSourceType["SQLServer"] = 1;
            DataSourceType["Oracle"] = 2;
            DataSourceType["SqlLite"] = 3; 
            #endregion
        }

        /// <summary>
        /// 属性值类型
        /// </summary>
        public static IDictionary<string, int> DicValueType { get; private set; } = new Dictionary<string, int>();
        /// <summary>
        /// 数据源类型
        /// </summary>
        public static IDictionary<string, int> DataSourceType { get; private set; } = new Dictionary<string, int>();
    }
}
