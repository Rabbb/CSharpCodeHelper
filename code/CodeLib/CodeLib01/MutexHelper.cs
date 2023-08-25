using System.Threading;

namespace CodeLib01;

public static class MutexHelper
{
    public static Mutex WaitOne(string name)
    {
        var mutex = new Mutex(false, name);
        try
        {
            mutex.WaitOne();
        }
        catch (AbandonedMutexException ex)
        {
        }

        return mutex;
    }

    public static void ReleaseClose(this Mutex mutex)
    {
        try
        {
            mutex.ReleaseMutex();
        }
        catch
        {
        }

        try
        {
            mutex.Close();
        }
        catch
        {
        }
    }
}