using System;
using System.Threading;

namespace CodeLib01;

public static class TimerHelper
{
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


    public static Timer SetInterval(Action fn, long interval, ulong times = 0)
    {
        Timer? timer1 = null;
        ulong times2 = times;

        var callback = times > 0 ? new TimerCallback(_ =>
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
        }) : new TimerCallback(_ =>
        {
            fn.Invoke();
        });

        timer1 = new Timer(callback, null, interval, interval);

        return timer1;
    }
}