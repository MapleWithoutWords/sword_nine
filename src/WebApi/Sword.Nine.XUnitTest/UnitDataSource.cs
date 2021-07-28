using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NoRain.Common;
using Sword.Nine.Api.Controllers;
using Sword.Nine.Dao;
using Sword.Nine.Domain;
using Sword.Nine.Service;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Sword.Nine.XUnitTest
{
    public class UnitDataSource
    {
        SnDataSourceController snDataSourceController;
        ISnDataSourceService dataSourceService1;
        public UnitDataSource()
        {
            var logger = new Mock<ILoggerFactory>();
            var dataSourceService = new Mock<ISnDataSourceService>();
            dataSourceService1 = dataSourceService.Object;
            snDataSourceController = new SnDataSourceController(dataSourceService.Object, logger.Object);
        }
        [Fact]
        public async Task Insert()
        {
            var ds = new SnDataSource
            {
                Password = "123456",
                Code = "SPCS",
                DatabaseName = "SPCS",
                Description = "",
                Host = "127.0.0.1",
                Name = "SPCS",
                NameSpace = "SPCS",
                Port = 3306,
                UserName = "root",
                Type = 0
            };
            ds.SetUserId(Guid.Empty.ToString());
            var result = (JsonResult)await snDataSourceController.Insert(ds);
            Assert.True(result != null);
           
            //if (result!=null)
            //{
            //    Assert.True(result.Code == 1);
            //    result = await dataSourceService1.DeleteByIdAsync(ds.Id);
            //    Assert.True(result.Code == 0);
            //}
        }
    }
}
