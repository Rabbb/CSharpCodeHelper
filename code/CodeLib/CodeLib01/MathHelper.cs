using System;

#pragma warning disable CS0219,CS8600,CS8602,CS8603,CS8604,CS8618,CS8619,CS8625,CS8714
namespace CodeLib01;

public static class MathHelper
{
    /// <summary>
    /// 增长率
    /// </summary>
    /// <param name="value"></param>
    /// <param name="compare_value"></param>
    /// <returns></returns>
    public static decimal? GrowthRate(this decimal value, decimal compare_value)
    {
        if (compare_value == 0m)
            return null;
        return (value - compare_value) / compare_value;
    }


    /// <summary>
    /// 向上舍入
    /// </summary>
    /// <param name="i"></param>
    /// <param name="length"></param>
    public static decimal RoundUp(double i, int length)
    {
        double ii = Math.Pow(10, length);
        return Convert.ToDecimal((i > 0 ? Math.Ceiling(i * ii) : Math.Floor(i * ii)) / ii);
    }


}