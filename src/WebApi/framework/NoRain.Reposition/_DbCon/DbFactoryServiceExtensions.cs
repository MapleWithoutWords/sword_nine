using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DbFactoryServiceExtensions
    {
        public static void AddDbFactoryService(this IServiceCollection services, IConfiguration configuration)
        {
            DbSetting.ConnectStr = configuration["DbSetting:ConnectStr"];
            DbSetting.Privoder = configuration["DbSetting:Privoder"];
            DbSetting.DataBase = configuration["DbSetting:DataBase"];
            DbSetting.SugarSearchAssemblyName = configuration["DbSetting:SugarSearchAssemblyName"];
            
            services.AddSingleton<IDbFactory, DbFactory>();
        }
    }
}
