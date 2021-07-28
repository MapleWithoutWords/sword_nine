using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// .net core自带序列化扩展转换方法
    /// </summary>
   public class StringToIntConvert: JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (Utf8Parser.TryParse(span, out int number, out int bytesConsumed) && span.Length == bytesConsumed)
                {
                    return number;
                }
                if (Int32.TryParse(reader.GetString(), out number))
                {
                    return number;
                }
            }
            return reader.GetInt32();
        }
        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }

    }
    /// <summary>
    /// .net core自带序列化扩展转换方法
    /// </summary>
    public class StringToDoubleConvert : JsonConverter<double>
    {
        public override double Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (Utf8Parser.TryParse(span, out double number, out int bytesConsumed) && span.Length == bytesConsumed)
                {
                    return number;
                }
                if (double.TryParse(reader.GetString(), out number))
                {
                    return number;
                }
            }
            return reader.GetInt32();
        }
        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }

    }
    /// <summary>
    /// .net core自带序列化扩展转换方法
    /// </summary>
    public class StringToFloatConvert : JsonConverter<float>
    {
        public override float Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (Utf8Parser.TryParse(span, out float number, out int bytesConsumed) && span.Length == bytesConsumed)
                {
                    return number;
                }
                if (float.TryParse(reader.GetString(), out number))
                {
                    return number;
                }
            }
            return reader.GetInt32();
        }
        public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }

    }
}
