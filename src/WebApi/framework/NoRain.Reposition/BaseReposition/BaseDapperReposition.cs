using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NoRain.Reposition
{
    public class BaseReposion
    {
        public System.Data.IDbConnection DbConnection
        {
            get
            {

                return dbFactory.CreateDbConnection();
            }
        }

        protected IDbFactory dbFactory;
        public BaseReposion(IDbFactory dbFactory)
        {
            this.dbFactory = dbFactory;
        }
    }
}
