using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#pragma warning disable CS0219,CS8600,CS8602,CS8603,CS8604,CS8618,CS8619,CS8625,CS8714

namespace CodeLib01;

public static class JsonHelper
{
    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly JsonSerializer DefaultJsonSerializer = new JsonSerializer
    {
        NullValueHandling = NullValueHandling.Ignore,
        ObjectCreationHandling = ObjectCreationHandling.Replace,
        MissingMemberHandling = MissingMemberHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        DateFormatString = "yyyy-MM-dd HH:mm:ss",
    };


    /// <summary>
    /// 序列化数据为Json数据格式.
    /// </summary>
    /// <param name="value">被序列化的对象</param>
    /// <returns></returns>
    public static string ToJson(this object value)
    {
        Type type = value.GetType();
        StringWriter sw = new StringWriter();
        JsonTextWriter writer = new JsonTextWriter(sw);
        writer.Formatting = Formatting.None;
        writer.QuoteChar = '"';
        DefaultJsonSerializer.Serialize(writer, value);
        string output = sw.ToString();
        writer.Close();
        sw.Close();
        return output;
    }

    /// <summary>
    /// 序列化数据为Json数据格式.
    /// </summary>
    /// <param name="value">被序列化的对象</param>
    /// <returns></returns>
    public static string ToJsonDate(this object value)
    {
        Type type = value.GetType();
        JsonSerializer json = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateFormatString = "yyyy/MM/dd HH:mm",
        };
        StringWriter sw = new StringWriter();
        JsonTextWriter writer = new JsonTextWriter(sw);
        writer.Formatting = Formatting.None;
        writer.QuoteChar = '"';
        json.Serialize(writer, value);
        string output = sw.ToString();
        writer.Close();
        sw.Close();
        return output;
    }


    /// <summary>
    /// 序列化数据为Json数据格式.
    /// </summary>
    /// <param name="value">被序列化的对象</param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static string ToJsonFormatDate(this object value, string format)
    {
        Type type = value.GetType();
        JsonSerializer json = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateFormatString = format,
        };
        StringWriter sw = new StringWriter();
        JsonTextWriter writer = new JsonTextWriter(sw);
        writer.Formatting = Formatting.None;
        writer.QuoteChar = '"';
        json.Serialize(writer, value);
        string output = sw.ToString();
        writer.Close();
        sw.Close();
        return output;
    }

    public static string ToJsonIncludeNull(this object value)
    {
        Type type = value.GetType();
        JsonSerializer json = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Include,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
        };
        StringWriter sw = new StringWriter();
        JsonTextWriter writer = new JsonTextWriter(sw);
        writer.Formatting = Formatting.None;
        writer.QuoteChar = '"';
        json.Serialize(writer, value);
        string output = sw.ToString();
        writer.Close();
        sw.Close();
        return output;
    }

    /// <summary>
    /// 将Json数据转为对象
    /// </summary>
    /// <typeparam name="T">目标对象</typeparam>
    /// <param name="jsonText">json数据字符串</param>
    /// <returns></returns>
    public static T FromJson<T>(this string jsonText)
    {
        StringReader sr = new StringReader(jsonText);
        JsonTextReader reader = new JsonTextReader(sr);
        T result = (T)DefaultJsonSerializer.Deserialize(reader, typeof(T));
        reader.Close();
        return result;
    }

    public static JToken? FromJson(this string jsonText)
    {
        try
        {
            return FromJson<JToken>(jsonText);
        }
        catch
        {
            return null; // 改报错时, 返回NULL 2022-4-27 17:44:45 Ciaran
        }
    }

    public static bool HasProperty(this JObject j, string prop_name)
    {
        return j.Property(prop_name) != null;
    }

    public static T ToModel<T>(this JToken? jtoken, T @default = default(T)) where T : class?
    {
        try
        {
            return jtoken?.ToObject<T>(DefaultJsonSerializer) ?? @default;
        }
        catch (Exception)
        {
            return @default;
        }
    }

    public static T? ToModel<T>(this JToken? jtoken, T? @default = default(T?), int i = 0) where T : struct
    {
        try
        {
            return jtoken?.ToObject<T>(DefaultJsonSerializer) ?? @default;
        }
        catch
        {
            return @default;
        }
    }

    public static object? ToTypeModel(this JToken? jtoken, Type type, object? @default = null)
    {
        try
        {
            if (@default != null && !type.IsInstanceOfType(@default))
            {
                @default = null;
            }

            return jtoken != null ? jtoken.ToObject(type, DefaultJsonSerializer) : DefaultValue();
        }
        catch
        {
            return DefaultValue();
        }

        object DefaultValue() => (@default ?? (type.IsValueType ? Activator.CreateInstance(type) : null));
    }

    public static object ToTypeObject(this JToken jtoken, Type type) => jtoken.ToObject(type, DefaultJsonSerializer);
}