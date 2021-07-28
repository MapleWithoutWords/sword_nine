using SqlSugar;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace NoRain.Reposition
{
    public class SugarManager
    {
        private static ConcurrentDictionary<string, SugarClient> _cache = new ConcurrentDictionary<string, SugarClient>();
        private static ThreadLocal<string> _threadLocal;
        static SugarManager()
        {
            _threadLocal = new ThreadLocal<string>();
        }

        /// <summary>
        /// 创建一个sqlsugar连接
        /// </summary>
        /// <returns></returns>
        public static SqlSugarClient CreateSqlSugar()
        {
            SqlSugar.DbType dbType = SqlSugar.DbType.MySql;
            switch (DbSetting.Privoder)
            {
                case "sqlserver":
                    dbType = SqlSugar.DbType.SqlServer;
                    break;
                case "mysql":
                    dbType = SqlSugar.DbType.MySql;
                    break;
                case "oracle":
                    dbType = SqlSugar.DbType.Oracle;
                    break;
                case "sqlite":
                    dbType = SqlSugar.DbType.Sqlite;
                    break;
                case "postgresql":
                    dbType = SqlSugar.DbType.PostgreSQL;
                    break;
                default:
                    throw new ApplicationException("不支持的数据库");
            }
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = DbSetting.ConnectStr,
                DbType = dbType,
                InitKeyType = InitKeyType.Attribute,//从特性读取主键和自增列信息
                IsAutoCloseConnection = true,//开启自动释放模式和EF原理一样我就不多解释了
            });


            var key = Guid.NewGuid().ToString().Replace("-", "");
            if (!_cache.ContainsKey(key))
            {
                _cache.TryAdd(key, new SugarClient(db));
                _threadLocal.Value = key;
                return db;
            }
            throw new ApplicationException("创建SqlSugarClient失败");
        }

        /// <summary>
        /// 获取当前线程的sqlsugar
        /// </summary>
        /// <returns></returns>
        public static SugarClient GetDbClient()
        {
            if (string.IsNullOrEmpty(_threadLocal.Value) || !_cache.ContainsKey(_threadLocal.Value))
            {
                return new SugarClient(CreateSqlSugar());
            }
            return _cache[_threadLocal.Value];
        }
    }
}
