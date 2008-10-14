using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Gui
{
    /// <summary>
    /// A find result is something that can be clicked upon. When this happens, the method NavigateTo is called,.
    /// </summary>
    public abstract class FindResult
    {
        public abstract void NavigateTo();
    }
}
