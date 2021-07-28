using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace System.Data
{
    public interface IDbFactory
    {
        IDbConnection CreateDbConnection();
        IDbConnection CreateDbConnection(string connStr, string privoder = "mysql");
    }
}
