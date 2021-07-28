using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NoRain.Common.SettingService
{
    public class LoadSettingService
    {

        public static void Load(IConfiguration configuration,string assemName)
        {
            var settingTypes = Assembly.Load(assemName).GetTypes().Where(e => e.IsAbstract == false && typeof(ISettingFac).IsAssignableFrom(e)).ToList();
            string settingName = string.Empty;
            foreach (var item in settingTypes)
            {
                settingName = item.Name;
                var properites = item.GetProperties().ToList();
                foreach (var pro in properites)
                {
                    var proType = pro.PropertyType;
                    var proStr = configuration[$"{settingName}:{pro.Name}"];
                    if (proStr==null)
                    {
                        Console.WriteLine($"配置：{settingName}。缺少{pro.Name}配置。该值将使用默认值");
                        continue;
                    }
                    var proVal = Convert.ChangeType(proStr, proType);
                    pro.SetValue(item, proVal);
                }
            }
        }
    }
}
