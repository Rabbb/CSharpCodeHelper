using System.Text;

namespace CodeLib01;

public static class StringBuilderHelper
{
    #region IndexOf

    /// <summary>
    /// Get index for string, return -1 when not found.<br/>
    /// 2023-11-7 Ciaran
    /// </summary>
    public static int IndexOf(this StringBuilder chunk, string value) => IndexOf(chunk, 0, value.Length, value);

    /// <summary>
    /// Get index for string, return -1 when not found.<br/>
    /// 2023-11-7 Ciaran
    /// </summary>
    /// <param name="chunk"></param>
    /// <param name="index"></param>
    /// <param name="count">max match steps</param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int IndexOf(this StringBuilder chunk, int index, int count, string value)
    {
        // 2023-11-7 Ciaran
        for (int i = index; i < chunk.Length; i++)
        {
            if (count == 0)
                return -1;

            if (StartsWith(chunk, i, value.Length, value))
            {
                return i;
            }

            count--;
        }

        return -1;
    }

    #endregion

    #region StartsWith

    /// <summary>
    /// StringBuilder StartsWith
    /// 2023-11-7 Ciaran
    /// </summary>
    /// <param name="ignore_white_space">ignore starts white space.</param>
    public static bool StartsWith(this StringBuilder chunk, string value, bool ignore_white_space)
    {
        var index = 0;

        // 忽略前置空格符 2023-11-7 Ciaran
        if (ignore_white_space)
            while (index < chunk.Length && char.IsWhiteSpace(chunk[index]))
                index++;

        return StartsWith(chunk, index, value.Length, value);
    }

    /// <summary>
    /// StringBuilder StartsWith
    /// 2023-11-7 Ciaran
    /// </summary>
    /// <param name="index">start index</param>
    public static bool StartsWith(this StringBuilder chunk, string value) => StartsWith(chunk, 0, value.Length, value);

    /// <summary>
    /// StringBuilder StartsWith
    /// 2023-11-7 Ciaran
    /// </summary>
    /// <param name="chunk"></param>
    /// <param name="index">start index</param>
    /// <param name="count">max match steps</param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool StartsWith(this StringBuilder chunk, int index, int count, string value)
    {
        // 2023-11-7 Ciaran
        for (int i = 0; i < value.Length; i++)
        {
            if (index == chunk.Length || count == 0)
                return false;

            if (value[i] != chunk[index])
                return false;

            index++;
            count--;
        }

        return true;
    }

    #endregion
}