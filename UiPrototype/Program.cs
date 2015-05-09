using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Decompiler.UiPrototype
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#if !NO_WINFORMS
            Application.Run(new WinForms.MainForm());
#else
            Application.Run(new Wpf.MainFormHost());
#endif
        }
    }
}
