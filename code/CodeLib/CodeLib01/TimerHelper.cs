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
        var callback = new TimerCallback(_ =>
        {
            if (times > 0)
            {
                times--;
                if (times == 0 || times2 <= times)
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
                if (times < times2)
                {
                    times2 = times;
                }
            }

            fn.Invoke();
        });
        timer1 = new Timer(callback, null, interval, interval);

        return timer1;
    }
}