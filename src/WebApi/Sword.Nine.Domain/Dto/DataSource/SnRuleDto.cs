using System;
using System.Collections.Generic;
using System.Text;

namespace Sword.Nine.Domain
{
    /// <summary>
    /// 规则dto
    /// </summary>
    public class SnRuleDto : SnRule
    {
        /// <summary>
        /// 类别id
        /// </summary>
        public string ClassId { get; set; }

        /// <summary>
        /// 类别属性id
        /// </summary>
        public string ClassAttributeId { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}
