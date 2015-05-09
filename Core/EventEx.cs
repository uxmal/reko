using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Core
{
    /// <summary>
    /// Extension methods to simplify firing of events.
    /// </summary>
    public static class EventEx
    {
        public static void Fire(this EventHandler eh, object sender)
        {
            if (eh != null) eh(sender, EventArgs.Empty);
        }

        public static void Fire<T>(this EventHandler<T> eh, object sender, T e) where T : EventArgs
        {
            if (eh != null) eh(sender, e);
        }
    }
}
