using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wanna.EMS.Api.SettingConfig
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class SysSetting:ISettingFac
    {
        /// <summary>
        /// 是否启动登录超时
        /// </summary>
        public static bool EnableLoginTimeOut { get; set; }

        /// <summary>
        /// 是否启动日志显示
        /// </summary>
        public static bool EnableShowLogs { get; set; } = true;

        /// <summary>
        /// 登录超时时间：分钟
        /// </summary>
        public static double LoginTimeOut { get; set; }

        /// <summary>
        /// 应用系统字典类别code
        /// </summary>
        public static string SysTypeCateCode { get; set; }
        /// <summary>
        /// 文件上传限制类别cateCode
        /// </summary>
        public static string FileUploadSettingCateCode { get; set; }

        /// <summary>
        /// 图片文件限制上传code
        /// </summary>
        public static string ImageFileUploadSettingCateCode { get; set; }
        /// <summary>
        /// 设备文档限制上传字典类别code
        /// </summary>
        public static string EquipDocLimitCateCode { get; set; }

        /// <summary>
        /// 设备文档类型Code
        /// </summary>
        public static string EquipDocTypeCateCode { get; set; }

        /// <summary>
        /// 设备类别字典类别code
        /// </summary>
        public static string EquipTypeCateCode { get; set; }
    }
}
