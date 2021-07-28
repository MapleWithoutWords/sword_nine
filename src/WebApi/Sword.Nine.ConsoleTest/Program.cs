using Newtonsoft.Json;
using Sword.Nine.Domain;
using Sword.Nine.Rule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ThreeLayerCode;

namespace Sword.Nine.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //ICodeBI code = new CodeBI();

            //DataSourceDto ds = new DataSourceDto();
            //ds.NameSpace = "wanna.spcs";
            //ds.ProjectName = "SPCS1";

            //TableDto tb = new TableDto();
            //tb.Code = "SpcsUser";
            //tb.Name = "权限管理系统";
            //tb.DirectoryCode = "UserOrg";
            //tb.Remark = "权限管理系统";
            //tb.TableName = "spec_user";

            //AttributeDto attr = new AttributeDto()
            //{
            //    Code = "Name",
            //    ColumnName = "Name",
            //    Length = 64,
            //    Name = "名称",
            //    Remark = "名称",
            //    ValueType = ValueTypeEnum.String,
            //};

            //attr.RuleDatas.Add(new RuleDto
            //{
            //    RuleType = RuleEnum.Regex,
            //    Data = "^[a-zA-Z0-9_]$",

            //});


            //tb.AttrDatas.Add(attr);


            //ds.TableList.Add(tb);

            //code.Run(ds, @"E:\projects\SwordNine\sword-nine\src\WebApi\Sword.Nine.Api\bin\Debug\netcoreapp3.1\codeBI\backstage");

            var jsonstr = File.ReadAllText(@"C:\Users\liming\Desktop\liming\预防性维修开发环境.json");

            List<MxCellDto> list = JsonConvert.DeserializeObject<List<MxCellDto>>(jsonstr);
            foreach (var item in list)
            {
                if (item.Edge==false)
                {
                    continue;
                }
                List<MxCellDto> childList = new List<MxCellDto>();
                foreach (var childItem in item.Children)
                {
                    if (childList.Any(e=>e.Children[1].DataId == childItem.Children[1].DataId))
                    {
                        continue;
                    }
                    childList.Add(childItem);
                }
                item.Children = childList;
            }

            File.WriteAllText(@"C:\Users\liming\Desktop\liming\1.json", JsonConvert.SerializeObject(list));

            Console.WriteLine("ok");
            Console.ReadKey();
        }
    }
}
