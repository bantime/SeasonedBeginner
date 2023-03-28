using System;
using System.Collections.Generic;
using System.Text;

namespace Tools
{
    public static class Extension
    {
        public static void SafeDispose(this IDisposable disposable, Action beforeDispose = null)
        {
            try
            {
                try
                {
                    beforeDispose?.Invoke();
                }
                catch (Exception e)
                {
                    //Log
                }
                disposable?.Dispose();
            }
            catch (Exception e)
            {
                //Log
            }
        }
    }
}
