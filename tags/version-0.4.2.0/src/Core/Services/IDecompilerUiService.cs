using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Core.Services
{
    /// <summary>
    /// Provides services for displaying items in the user interface.
    /// </summary>
    public interface IDecompilerUIService
    {
        void ShowError(Exception ex, string format, params object[] args);
        string ShowOpenFileDialog(string fileName);
        string ShowSaveFileDialog(string fileName);
        bool Prompt(string prompt);

        void ShowMessage(string msg);
    }
}
