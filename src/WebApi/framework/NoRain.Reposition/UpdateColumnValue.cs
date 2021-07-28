using System;
using System.Collections.Generic;
using System.Text;

namespace NoRain.Reposition
{
    /// <summary>
    /// 列的名称和值对。
    /// </summary>
    public class UpdateColumnValue
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 列值
        /// </summary>
        public object ColumnValue { get; set; }

        public void SetEntityValue(BaseEntity baseEntity)
        {
            var tType = baseEntity.GetType();
            var pro = tType.GetProperty(this.ColumnName);
            if (pro != null)
            {
                pro.SetValue(baseEntity, Convert.ChangeType(this.ColumnValue, pro.PropertyType));
            }
        }
    }

}
