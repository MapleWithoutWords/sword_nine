using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sword.Nine.Domain
{
    public partial class SnClassDirectory
    {
        ///<summary>
        ///<para>编码</para>
        ///</summary>
        [SugarColumn(IsIgnore =true)]
        public virtual List<MxCellDto> GraphModel { get; set; }
    }
}
