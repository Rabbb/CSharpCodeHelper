using System;
using System.Text.RegularExpressions;

namespace CodeLib01;

public static class RegexHelper
{
    /// <summary>
    /// 2023-7-3 Ciaran 字符串中的非(中英文标点符号, 空格换行制表符, 文字, 英文字母, 数字)的字符
    /// </summary>
    public static Regex RgInvalidChar => rgInvalidChar.Value;


    #region private

    /// <summary>
    /// 2023-7-3 Ciaran 字符串中的非(中英文标点符号, 空格换行制表符, 文字, 英文字母, 数字)的字符
    /// </summary>
    private static readonly Lazy<Regex> rgInvalidChar = new(() => new Regex(@"(?![\p{P}\s\wA-Za-z0-9]).", RegexOptions.Compiled|RegexOptions.CultureInvariant));


    #endregion
}