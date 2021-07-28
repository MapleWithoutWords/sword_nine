using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoRain.Reposition;
using Microsoft.Extensions.Logging;
using NoRain.Reposition.BaseReposition;
using Sword.Nine.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoRain.Api;
using Sword.Nine.Service;
using NoRain.Common;
using SPCS.Common.Helper;
using System.Linq;

namespace Sword.Nine.Api.Controllers
{
    /// <summary>
    /// 数据源表
    /// </summary>
    [Route("api/sndatasource")]
    [ApiController]
    public partial class SnDataSourceController : BaseApiController<ISnDataSourceService, SnDataSource>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service">数据源表服务</param>
        /// <param name="loggerFactory">日志服务</param>>
        public SnDataSourceController(ISnDataSourceService service, ILoggerFactory loggerFactory) : base(service, loggerFactory)
        {
        }

        [HttpGet("get_source_type")]
        public IActionResult GetDataSourceType()
        {
            List<dynamic> ret = new List<dynamic>();
            ret.Add(new { name = "MYSQL", value = 0 });
            ret.Add(new { name = "SQLServer", value = 1 });
            ret.Add(new { name = "Oracle", value = 2 });
            ret.Add(new { name = "SqlLite", value = 3 });

            return this.JsonApiResultData(ret);
        }

        [HttpGet("generator_code/{dataSourceId}/{templateId}")]
        public async Task<IActionResult> GeneratorCode(string dataSourceId, string templateId)
        {
            var ret = await InstanceService.GeneratorCodeAsync(dataSourceId, templateId);
            return this.Json(ret);
        }

        /// <summary>
        /// 导入数据源及数据
        /// </summary>
        /// <returns></returns>
        [HttpPost("import")]
        public async Task<IActionResult> Import()
        {
            if (this.Request.Form.Files.Count < 1)
            {
                return this.JsonErrorData("请上传文件");
            }
            var file = this.Request.Form.Files[0];
            var inStream = file.OpenReadStream();
            var workbook = NpoiExcelHelper.Instance.GetWorkBook(inStream);

            var dataSourceList = NpoiExcelHelper.Instance.GetList<DataSourceImportModel>(workbook, out string errorMsg, "数据源");
            if (dataSourceList == null)
            {
                return this.JsonErrorData($"sheet：数据源。{errorMsg}");
            }
            var directoryList = NpoiExcelHelper.Instance.GetList<DirectoryImportModel>(workbook, out errorMsg, "目录");
            if (directoryList == null)
            {
                return this.JsonErrorData($"sheet：目录。{errorMsg}");
            }
            var classList = NpoiExcelHelper.Instance.GetList<ClassImportModel>(workbook, out errorMsg, "类别");
            if (classList == null)
            {
                return this.JsonErrorData($"sheet：类别。{errorMsg}");
            }
            var attributeList = NpoiExcelHelper.Instance.GetList<AttributeImportModel>(workbook, out errorMsg, "属性");
            if (attributeList == null)
            {
                return this.JsonErrorData($"sheet：属性。{errorMsg}");
            }


            if (dataSourceList.GroupBy(e => e.SeqNo).Any(e => e.Count() > 1))
            {
                return this.JsonErrorData("数据源序号不能重复");
            }
            DataSoureceImportDto importdto = new DataSoureceImportDto
            {
                UpdateOrInsertAttributeList = attributeList,
                UpdateOrInsertClassList = classList,
                UpdateOrInsertDataSourceList = dataSourceList,
                UpdateOrInsertDirectoryList = directoryList
            };
            var result = await InstanceService.ImportAsync(importdto);


            return this.Json(result);
        }

        [HttpGet("publish/{dataSourceId}")]
        public async Task<IActionResult> PublishDataSource(string dataSourceId)
        {
            var ret = await InstanceService.PublishAsync(dataSourceId);
            return this.Json(ret);
        }
    }
}
