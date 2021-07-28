using DapperExtensions.Sql;
using Hangfire.PostgreSql;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using NoRain.Reposition;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace System.Data
{
    public class DbFactory: IDbFactory
    {
        public DbFactory()
        {
        }
        public IDbConnection CreateDbConnection()
        {
            return CreateDbConnection(DbSetting.ConnectStr, DbSetting.Privoder);
        }
        public IDbConnection CreateDbConnection(string connStr,string privoder="mysql")
        {
            //DbSetting.ConnectStr = connStr;
            //DbSetting.Privoder = privoder;
            IDbConnection con = null;
            DapperExtensions.DapperExtensions.DefaultMapper = typeof(DapperTableNameMapper<>);
            switch (privoder)
            {
                case "sqlserver":
                    DapperExtensions.DapperExtensions.SqlDialect = new SqlServerDialect();
                    con = new SqlConnection(connStr);
                    break;
                case "mysql":
                    DapperExtensions.DapperExtensions.SqlDialect = new MySqlDialect();
                    con = new MySqlConnection(connStr);
                    break;
                case "oracle":
                    DapperExtensions.DapperExtensions.SqlDialect = new OracleDialect();
                    con = new OracleConnection(connStr);
                    break;
                case "sqlite":
                    DapperExtensions.DapperExtensions.SqlDialect = new SqliteDialect();
                    con = new SqliteConnection(connStr);
                    break;
                case "postgresql":
                    DapperExtensions.DapperExtensions.SqlDialect = new PostgreSqlDialect();                    
                    con = new NpgsqlConnection(connStr);
                    break;
                default:
                    throw new Exception("不支持的数据库");
            }
            
            con.Open();
            return con;
        }
    }
}
