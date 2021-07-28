using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NoRain.Reposition._DbCon
{
    public static class DbConnectionExtensions
    {

        public static IDictionary<string, string> DicSqlCache { get; set; }

    }
}
