using System;
using System.Collections.Generic;
using System.Text;

namespace NoRain.Common
{
  public   class PageDTO
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; } = 1;
        /// <summary>
        /// 一页显示多少数据
        /// </summary>
        public int PageDataCount { get; set; } = 10;
        /// <summary>
        /// 总数据数
        /// </summary>
        public long Count { get; set; } = -1;

    }
}
