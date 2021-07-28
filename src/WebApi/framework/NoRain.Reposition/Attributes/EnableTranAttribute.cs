using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public class EnableTranAttribute : Attribute
    {
        public bool IsTran { get; set; }
        public EnableTranAttribute(bool istran = true)
        {
            IsTran = istran;
        }
    }
}
