using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Reko.Gui;
using ContextMenu = System.Windows.Forms.ContextMenu;

namespace Reko.WindowsItp
{
    public class FakeDecompilerShellUiService : IDecompilerShellUiService
    {
        public FakeDecompilerShellUiService()
        {
        }

        public IWindowFrame ActiveFrame
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<IWindowFrame> DocumentWindows
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<IWindowFrame> ToolWindows
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IWindowFrame CreateDocumentWindow(string documentType, object docItem, string documentTitle, IWindowPane pane)
        {
            throw new NotImplementedException();
        }

        public IWindowFrame CreateWindow(string windowType, string windowTitle, IWindowPane pane)
        {
            throw new NotImplementedException();
        }

        public bool Execute(CommandID cmdId)
        {
            throw new NotImplementedException();
        }

        public IWindowFrame FindDocumentWindow(string documentType, object docItem)
        {
            throw new NotImplementedException();
        }

        public IWindowFrame FindWindow(string windowType)
        {
            throw new NotImplementedException();
        }

        public ContextMenu GetContextMenu(int menuID)
        {
            return new ContextMenu();
        }

        public bool Prompt(string prompt)
        {
            throw new NotImplementedException();
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            throw new NotImplementedException();
        }

        public void ShowError(Exception ex, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string msg)
        {
            throw new NotImplementedException();
        }

        public DialogResult ShowModalDialog(IDialog dlg)
        {
            throw new NotImplementedException();
        }

        public string ShowOpenFileDialog(string fileName)
        {
            throw new NotImplementedException();
        }

        public string ShowSaveFileDialog(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}