using System;
using System.Threading;

namespace CodeLib01;

/// <summary>
/// 定时器 2023-10-26 Ciaran
/// </summary>
public static class TimerHelper
{
    /// <summary>
    /// 一次性定时器 2023-10-26 Ciaran
    /// </summary>
    /// <param name="fn"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    public static Timer SetTimeout(Action fn, long interval)
    {
        Timer? timer1 = null;
        var callback = new TimerCallback(_ =>
        {
            try
            {
                timer1!.Dispose();
            }
            catch (Exception)
            {
                // ignore
            }

            fn.Invoke();
        });
        timer1 = new Timer(callback, null, interval, -1);

        return timer1;
    }

    /// <summary>
    /// 循环定时器 2023-10-26 Ciaran
    /// </summary>
    /// <param name="fn"></param>
    /// <param name="interval"></param>
    /// <param name="times"></param>
    /// <returns></returns>
    public static Timer SetInterval(Action fn, long interval, ulong times = 0)
    {
        Timer? timer1 = null;
        ulong times2 = times;

        var callback = times > 0
            ? new TimerCallback(_ =>
            {
                if (--times == 0)
                {
                    try
                    {
                        timer1!.Dispose();
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }
                else if (times2 <= times)
                {
                    try
                    {
                        timer1!.Dispose();
                    }
                    catch (Exception)
                    {
                        // ignore
                    }

                    return;
                }

                if (times < times2)
                {
                    times2 = times;
                }

                fn.Invoke();
            })
            : new TimerCallback(_ => { fn.Invoke(); });

        timer1 = new Timer(callback, null, interval, interval);

        return timer1;
    }
}