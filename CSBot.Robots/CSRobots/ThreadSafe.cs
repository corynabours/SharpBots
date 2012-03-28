using System;
using System.ComponentModel;

namespace CSRobots
{
    public static class SynchronizeInvokeExtensions
    {
        public static void InvokeThreadSafe<T>
                    (this T @this, Action<T> action) where T : ISynchronizeInvoke
        {
            if (@this.InvokeRequired)
            {
                @this.Invoke(action, new object[] { @this });
            }
            else
            {
                action(@this);
            }
        }
    }
}
