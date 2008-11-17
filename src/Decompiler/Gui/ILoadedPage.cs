using System;

namespace Decompiler.Gui
{
    public interface ILoadedPage
    {
        System.Windows.Forms.TextBox Disassembly { get; }
        Decompiler.WindowsGui.Controls.MemoryControl MemoryControl { get; }
    }
}
