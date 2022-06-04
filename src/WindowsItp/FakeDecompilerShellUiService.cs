using Reko.Gui;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using Form = System.Windows.Forms.Form;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
namespace Reko.WindowsItp
{
    public class FakeDecompilerShellUiService : IDecompilerShellUiService
    {
        private Form form;

        public FakeDecompilerShellUiService(Form form)
        {
            this.form = form;
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

        public Dictionary<string,Dictionary<int,CommandID>> KeyBindings { get; set; }

        public IWindowFrame CreateDocumentWindow(string documentType, object docItem, string documentTitle, IWindowPane pane)
        {
            throw new NotImplementedException();
        }

        public IWindowFrame CreateWindow(string windowType, string windowTitle, IWindowPane pane)
        {
            throw new NotImplementedException();
        }

        public ValueTask<bool> ExecuteAsync(CommandID cmdId)
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

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            throw new NotImplementedException();
        }

        public void SetContextMenu(object control, int menuID)
        {
        }

        public ValueTask<bool> Prompt(string prompt)
        {
            throw new NotImplementedException();
        }

        public ValueTask ShowError(Exception ex, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public ValueTask ShowMessage(string msg)
        {
            throw new NotImplementedException();
        }

        public ValueTask<DialogResult> ShowModalDialog(IDialog dlg)
        {
            throw new NotImplementedException();
        }

        public ValueTask<T> ShowModalDialog<T>(IDialog<T> dlg)
        {
            throw new NotImplementedException();
        }

        public ValueTask<string> ShowOpenFileDialog(string fileName)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.FileName = fileName;
                if (DialogResult.OK == (DialogResult) ofd.ShowDialog(form))
                {
                    return ValueTask.FromResult(ofd.FileName);
                }
                else
                {
                    return ValueTask.FromResult<string>(null);
                }
            }
        }

        public ValueTask<string> ShowSaveFileDialog(string fileName)
        {
            throw new NotImplementedException();
        }

        public void WithWaitCursor(Action action)
        {
            action();
        }
    }
}