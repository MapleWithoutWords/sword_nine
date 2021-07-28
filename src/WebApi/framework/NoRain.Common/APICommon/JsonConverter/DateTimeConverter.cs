using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// 时间转换
    /// </summary>
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        /// <summary>
        /// 从字符串转成datetime
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
        }
        /// <summary>
        /// 从时间转成string
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var ret = value.ToString("yyyy-MM-dd HH:mm:ss");
            if (value.IsDefaultOr1900Time())
            {
                ret = "";
            }
            writer.WriteStringValue(ret);
        }
    }
    /// <summary>
    /// 时间转换
    /// </summary>
    public class DateTimeNullableConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return string.IsNullOrEmpty(reader.GetString()) ? default(DateTime?) : DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            var ret = value?.ToString("yyyy-MM-dd HH:mm:ss");
            if (value!=null&&value.Value.IsDefaultOr1900Time())
            {
                ret = "";
            }
            writer.WriteStringValue(ret);
        }
    }
}
