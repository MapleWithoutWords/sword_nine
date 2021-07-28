using Common.APICommon;
using Microsoft.Extensions.DependencyInjection;
using SAM.Common.APICommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ICollectionServiceExtensions
    {
        public static void RegisterSingleton(this IServiceCollection services, string assemName)
        {
            try
            {
                var assemTypes = Assembly.Load(assemName).GetTypes().Where(e => e.IsAbstract == false && typeof(IInjectFac).IsAssignableFrom(e));
                foreach (var item in assemTypes)
                {
                    foreach (var inface in item.GetInterfaces().Where(e=>e.Name!= typeof(IInjectFac).Name))
                    {
                        services.AddSingleton(inface, item);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public static void RegisterTransient(this IServiceCollection services, string assemName)
        {
            var assemTypes = Assembly.Load(assemName).GetTypes().Where(e => e.IsAbstract == false && typeof(IInjectFac).IsAssignableFrom(e));
            foreach (var item in assemTypes)
            {
                foreach (var inface in item.GetInterfaces().Where(e => e.Name != typeof(IInjectFac).Name))
                {
                    services.AddTransient(inface, item);
                }
            }
        }
        public static void RegisterScoped(this IServiceCollection services, string assemName)
        {
            var assemTypes = Assembly.Load(assemName).GetTypes().Where(e => e.IsAbstract == false && typeof(IInjectFac).IsAssignableFrom(e));
            foreach (var item in assemTypes)
            {
                foreach (var inface in item.GetInterfaces().Where(e => e.Name != typeof(IInjectFac).Name))
                {
                    services.AddScoped(inface, item);
                }
            }
        }
    }
}
