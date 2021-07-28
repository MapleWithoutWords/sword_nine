using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System
{
    /// <summary>
    /// 字符串路径扩展类
    /// </summary>
    public static class StringPathExtensions
    {

        public static T GetModel<T>(this string jsonPath) where T:class,new()
        {
            T model = null;
            //读取json文件  
            using (StreamReader sr = new StreamReader(jsonPath, System.Text.Encoding.Default))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Converters.Add(new JavaScriptDateTimeConverter());
                serializer.NullValueHandling = NullValueHandling.Ignore;
                //构建Json.net的读取流  
                JsonReader reader = new JsonTextReader(sr);
                //对读取出的Json.net的reader流进行反序列化，并装载到模型中  
                model = serializer.Deserialize<T>(reader);
            }
            return model;
        }

        public static string CreateDirectory(this string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                return dirPath;
            }
            Directory.CreateDirectory(dirPath);
            return dirPath;
        }
    }
}
