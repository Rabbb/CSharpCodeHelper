using System.Threading;

namespace CodeLib01;

public static class MutexHelper
{

    /// <summary>
    /// 等待一个锁
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Mutex WaitOne(string name)
    {
        var mutex = new Mutex(false, name);
        try
        {
            mutex.WaitOne();
        }
        catch (AbandonedMutexException)
        {
            // ignored
        }

        return mutex;
    }

    /// <summary>
    /// 释放锁, 并消耗当前对象
    /// </summary>
    /// <param name="mutex"></param>
    public static void ReleaseClose(this Mutex mutex)
    {
        try
        {
            mutex.ReleaseMutex();
        }
        catch
        {
            // ignored
        }

        try
        {
            mutex.Close();
        }
        catch
        {
            // ignored
        }
    }
}