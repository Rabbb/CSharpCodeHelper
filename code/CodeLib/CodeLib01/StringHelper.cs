using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace CodeLib01;

public static class StringHelper
{
    /// <summary>
    /// 2023-7-3 Ciaran 非(中英文标点符号, 空格换行制表符, 文字, 英文字母, 数字)的字符
    /// </summary>
    public static bool IsInvalidChar(this char c)
    {
        // var reg = new Regex(@"(?![\p{P}\s\wA-Za-z0-9]).", RegexOptions.Compiled|RegexOptions.CultureInvariant);
        var reg = RegexHelper.RgInvalidChar;
        if (reg.IsMatch("" + c))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 2023-7-3 Ciaran 移除字符串中的非(中英文标点符号, 空格换行制表符, 文字, 英文字母, 数字)的字符
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string RemoveInvalidChars(this string s)
    {
        // var reg = new Regex(@"(?![\p{P}\s\wA-Za-z0-9]).", RegexOptions.Compiled|RegexOptions.CultureInvariant);
        var reg = RegexHelper.RgInvalidChar;
        return reg.Replace(s, "");
    }

    public static string Csv2Json(this string csv_string, char split, out string[] titles)
    {
        var lines = new StringReader(csv_string);
        titles = new string[0];
        var data = new List<JObject>();

        for (var i = 0; lines.ReadLine() is {} line1; i++)
        {
            var items = line1.Split(split);
            if (i == 0)
            {
                titles = items;
            }
            else
            {
                var row1 = new JObject();
                for (var j = 0; j < items.Length; j++)
                {
                    var item = items[j];
                    row1[titles[j]] = new JValue(item);
                }

                data.Add(row1);
            }
        }

        return data.ToJson();
    }

    public static string SafeSubstring(this string? s, int start_index)
    {
        if (s is null) return "";
        if (start_index >= s.Length) return "";
        return s.Substring(start_index);
    }

    public static string SafeSubstring(this string? s, int start_index, int length)
    {
        if (s is null) return "";
        if (start_index >= s.Length) return "";
        if (start_index + length >= s.Length) length = s.Length - start_index;

        return s.Substring(start_index, length);
    }

    /// <summary>
    /// 2023-2-9 Ciaran 当字符串长度超出最大长度时, 截取字符串的开始和结束部分
    /// </summary>
    /// <param name="s"></param>
    /// <param name="max_len">最大长度</param>
    /// <param name="start_len">开始段的长度</param>
    /// <param name="spliter">连接两段字符串的分隔符</param>
    /// <returns></returns>
    public static string SubStartEnd(this string s, int max_len, int start_len = int.MaxValue, string spliter = " ... ")
    {
        if (start_len > max_len)
            start_len = max_len / 2;
        if (s.Length > max_len)
        {
            int len1 = start_len;
            int len2 = max_len - start_len;
            int index2 = s.Length - len2;
            return s.Substring(0, len1) + spliter + s.Substring(index2, len2);
        }

        return s;
    }


    /// <summary>
    /// 2022-8-18 Ciaran 从字符串中删除字符, 并返回新的字符串
    /// </summary>
    /// <param name="s"></param>
    /// <param name="chars"></param>
    /// <returns></returns>
    public static string RemoveChars(this string s, params char[] chars)
    {
        if (chars.Length > 0)
        {
            return new string(s.Where(c => !chars.Contains(c)).ToArray());
        }

        return s;
    }


    /// <summary>
    /// 2022-8-18 Ciaran 从字符串中删除字符, 并返回新的字符串
    /// </summary>
    /// <param name="s"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    public static string? RemoveChars(this string? s, Func<char, bool>? match)
    {
        if (match != null && s != null)
        {
            return new string(s.WhereNot(match).ToArray());
        }

        return s;
    }

}