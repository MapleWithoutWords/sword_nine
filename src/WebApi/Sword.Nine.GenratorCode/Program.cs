using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sword.Nine.GenratorCode
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            string rootDire = configuration.GetSection("GeneratorRootPath").Value;

            List<IBuilderCode> list = new List<IBuilderCode>();
            var typeList = Assembly.GetExecutingAssembly().GetTypes().Where(e => e.IsAbstract == false && typeof(IBuilderCode).IsAssignableFrom(e)).ToList();
            bool.TryParse(configuration.GetSection("IsNotExistsDelete").Value, out bool IsNotExistsDelete);
            var config = new ConfigModel()
            {
                ConnectionString = configuration.GetSection("ConnectionString").Value,
                DataBase = configuration.GetSection("DataBase").Value,
                Table = configuration.GetSection("Table").Value,
                IsNotExistsDelete = IsNotExistsDelete,
            };
            DbHelper db = new DbHelper();
            var tableList = db.GetTableInfo(new List<string>(), config);
            foreach (var item in typeList)
            {
                var obj = (IBuilderCode)Activator.CreateInstance(item);
                obj.Generator(rootDire, tableList);
            }

            Console.WriteLine("OK");
            Console.ReadKey();
        }
    }
}
