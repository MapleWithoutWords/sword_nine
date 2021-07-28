using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoRain.Common.SettingService;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SettingServiceExtensions
    {
        public static void AddSettingService(this IServiceCollection services, IConfiguration configuration,string assemName)
        {
            LoadSettingService.Load(configuration, assemName);
        }
    }
}
