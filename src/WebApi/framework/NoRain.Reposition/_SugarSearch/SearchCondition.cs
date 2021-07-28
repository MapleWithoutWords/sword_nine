using NoRain.Common;
using SAM.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoRain.Reposition
{
    public class SearchCondition
    {
        public string WhereCondition { get; set; }
        public string OrderCondition { get; set; }
        
        public List<IConditionalModel> ConditionalModels { get; set; }
        public PageDTO Page { get; set; } 
    }
}
