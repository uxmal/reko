using Reko.Gui.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Gui.Forms
{
    public interface ICallHierarchyView
    {
        IServiceProvider Services { get; set; }

        ITreeView CallTree { get; }
        IButton DeleteButton { get; }
    }
}
