using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#pragma warning disable CS0219,CS8600,CS8602,CS8603,CS8604,CS8618,CS8619,CS8625,CS8714
namespace CodeLib01;

public static class NumberHelper
{
    #region explit

    public static short? AsInt16(this string? value) => (short?)AsDouble(value);
    public static int? AsInt32(this string? value) => (int?)AsDouble(value);
    public static long? AsInt64(this string? value) => (long?)AsDouble(value);
    public static decimal? AsDecimal(this string? value) => value is null || !decimal.TryParse(value, out var result) ? null : result;
    public static decimal? AsDecimal(this double? value) => value is null ? null : Convert.ToDecimal(value.Value);
    public static double? AsDouble(this string? value) => value is null || !double.TryParse(value, out var result) ? null : result;
    public static double? AsDouble(this decimal? value) => value is null ? null : Convert.ToDouble(value.Value);

    #endregion


    #region 10进制和32进制转换

    /// <summary>
    /// 整数转换32进制数字符串, 最大/小值为long最大小值
    /// </summary>
    /// <returns></returns>
    public static string To32System(long number)
    {
        char FromBinary(int decimal_number)
        {
            switch (decimal_number)
            {
                case < 0:
                    throw new ArgumentOutOfRangeException(nameof(decimal_number), "32进制值范围0~31");
                case < 10:
                    return (char)(decimal_number + '0');
                case < 32:
                {
                    int new_char = (decimal_number - 10 + 'A');
                    int new_char1 = new_char;
                    if (new_char1 > 'H') new_char++;
                    if (new_char1 > 'M') new_char++;
                    if (new_char1 > 'P') new_char++;
                    return (char)new_char;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(decimal_number), "32进制值范围0~31");
            }
        }

        StringBuilder result = new StringBuilder();
        byte mask = 0b_11111; // 31 = 0b 0001 1111
        long store_number = number;
        int use_chars = 1; // 有效字符数
        // 13 = 64/5 + 1
        char c1;
        for (int i = 1; i < 14; i++)
        {
            c1 = FromBinary((int)(mask & store_number));
            result.Insert(0, c1);
            if (c1 > '0')
            {
                use_chars = i;
            }

            store_number >>= 5;
        }

        result.Remove(0, result.Length - use_chars);
        return result.ToString();
    }


    /// <summary>
    /// 32进制数转10进制数, 最大/小值为long最大小值
    /// </summary>
    /// <param name="s_32sys_num"></param>
    /// <returns></returns>
    public static long From32System(string s_32sys_num)
    {
        uint ToBinary(char c_32sys_num)
        {
            switch (c_32sys_num)
            {
                case < '0':
                    throw new ArgumentOutOfRangeException(nameof(c_32sys_num), "32进制值范围0~31");
                // '9' + 1 = ':'
                case < ':':
                    return (uint)c_32sys_num - '0';
                case < 'A':
                    throw new ArgumentOutOfRangeException(nameof(c_32sys_num), "32进制值范围0~31");
                case < 'Z':
                {
                    uint new_char = (uint)c_32sys_num - 'A' + 10;
                    if (c_32sys_num > 'R') new_char--;
                    if (c_32sys_num > 'N') new_char--;
                    if (c_32sys_num > 'H') new_char--;
                    return new_char;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(c_32sys_num), "32进制值范围0~31");
            }
        }

        long result = 0L;
        for (int i = 0; i < 13 && i < s_32sys_num.Length; i++)
        {
            result <<= 5;
            result |= ToBinary(s_32sys_num[i]);
        }

        return result;
    }

    #endregion


    /// <summary>
    /// 数字1-9转换为中文数字, 否则返回原字符串 2023-11-1 Ciaran
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string OneBitNumberToChinese(string num)
    {
        var numStr = "123456789".Select(c => c.ToString()).ToList();
        var chineseStr = "一二三四五六七八九".Select(c => c.ToString()).ToList();
        int numIndex = numStr.IndexOf(num);
        if (numIndex > -1)
        {
            return chineseStr[numIndex];
        }

        return num;
    }

    /// <summary>
    /// 金额转中文大写
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static string GetNumber2CN(this double number)
    {
        StringBuilder cnStr = new StringBuilder();
        string CnNumber = "零壹贰叁肆伍陆柒捌玖";
        string CnUnit = "分角元拾佰仟万拾佰仟亿拾佰仟万";
        string numStr = number.ToString("#0.00").Replace(".", "");
        for (int i = 0; i < numStr.Length; i++)
        {
            int num = int.Parse(numStr.Substring(i, 1));
            cnStr.Append(CnNumber.Substring(num, 1) + CnUnit.Substring(numStr.Length - i - 1, 1));
        }

        while (Regex.IsMatch(cnStr.ToString(), "零零|零拾|零佰|零仟|零万|零亿|亿万"))
        {
            cnStr = cnStr.Replace("零拾", "零");
            cnStr = cnStr.Replace("零佰", "零");
            cnStr = cnStr.Replace("零仟", "零");
            cnStr = cnStr.Replace("零万", "万");
            cnStr = cnStr.Replace("零亿", "亿");
            cnStr = cnStr.Replace("零零", "零");
            cnStr = cnStr.Replace("亿万", "亿零");
        }

        cnStr = cnStr.Replace("零元", "元").Replace("零角", "").Replace("零分", "");
        if (cnStr.ToString().StartsWith("元"))
        {
            return cnStr.ToString().Substring(1);
        }

        if (cnStr.ToString().EndsWith("元"))
        {
            return cnStr.ToString() + "整";
        }

        return cnStr.ToString();
    }
}