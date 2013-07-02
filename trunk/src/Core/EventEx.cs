using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Core
{
    public static class EventEx
    {
        public static void Fire(this EventHandler eh, object sender)
        {
            if (eh != null) eh(sender, EventArgs.Empty);
        }    
    }
}
