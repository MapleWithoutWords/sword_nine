using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wanna.EMS.Api.SettingConfig
{
    /// <summary>
    /// token配置
    /// </summary>
    public class TokenManagement : ISettingFac
    {
        /// <summary>
        /// Secret
        /// </summary>
        public static string Secret { get; set; }
        /// <summary>
        /// Issuer
        /// </summary>
        public static string Issuer { get; set; }
        /// <summary>
        /// Audience
        /// </summary>
        public static string Audience { get; set; }
        /// <summary>
        /// AccessExpiration
        /// </summary>
        public static int AccessExpiration { get; set; }
        /// <summary>
        /// RefreshExpiration
        /// </summary>
        public static int RefreshExpiration { get; set; }
    }
}
