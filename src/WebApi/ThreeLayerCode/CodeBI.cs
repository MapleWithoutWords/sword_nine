using Sword.Nine.Rule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ThreeLayerCode
{
    public class CodeBI : ICodeBI
    {
        /// <summary>
        /// 代码生成运行
        /// </summary>
        /// <param name="dataSourceDto">数据源信息</param>
        /// <param name="saveDir">生成的压缩包，存放的目录</param>
        /// <param name="fileNameOutExtensions">文件名称，不包含后缀名</param>
        /// <returns>压缩包路径</returns>
        public string Run(DataSourceDto dataSourceDto, string saveDir)
        {
           var newDir = Path.Combine(saveDir, dataSourceDto.NameSpace);
            if (Directory.Exists(newDir))
            {
                Directory.Delete(newDir, true);
            }
            if (Directory.Exists(Path.Combine(saveDir, dataSourceDto.ProjectName)))
            {
                Directory.Delete(Path.Combine(saveDir, dataSourceDto.ProjectName), true);
            }
            var result = StartTemplate(saveDir, dataSourceDto.NameSpace);
            List<IBuilderCode> list = new List<IBuilderCode>();
            var typeList = Assembly.GetExecutingAssembly().GetTypes().Where(e => e.IsAbstract == false && typeof(IBuilderCode).IsAssignableFrom(e)).ToList();

            foreach (var item in typeList)
            {
                var obj = (IBuilderCode)Activator.CreateInstance(item);
                obj.Generator(newDir, dataSourceDto);
            }
            //Directory.Move(newDir, Path.Combine(saveDir, dataSourceDto.ProjectName));

            return result;
        }

        public string StartTemplate(string dirPath, string projectName)
        {
            var drive = dirPath.Substring(0, 2);
            var p = ProcessHelper.Create("cmd.exe");
            ProcessHelper.Push(p, $"{drive}");
            ProcessHelper.Push(p, $"cd {dirPath}");
            ProcessHelper.Push(p, $"dotnet new sdtl --name {projectName}&exit");
            var result = ProcessHelper.GetResult(p);
            //p.Kill();
            return result;
        }
    }
}
