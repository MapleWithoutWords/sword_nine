using System;
using System.Collections.Generic;
using System.Text;

namespace Sword.Nine.Domain
{
    public class RuleAttributeSettingDto
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string AttributeName { get; set; }
        /// <summary>
        /// 属性编码
        /// </summary>
        public string AttributeCode { get; set; }
        /// <summary>
        /// 属性列名
        /// </summary>
        public string AttributeColumnName { get; set; }

        /// <summary>
        /// 类别id
        /// </summary>
        public string ClassId { get; set; }
        /// <summary>
        /// 属性id
        /// </summary>
        public string AttributeId { get; set; }
        /// <summary>
        /// 规则值
        /// </summary>
        public List<RuleValue> RuleValues { get; set; }
    }
    /// <summary>
    /// 规则值
    /// </summary>
    public class RuleValue
    {
        /// <summary>
        /// 规则id
        /// </summary>
        public string RuleId { get; set; }
        public string RuleName { get; set; }
        public int RuleValueType { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 所属属性id
        /// </summary>
        public string AttributeId { get; set; }
    }

}
