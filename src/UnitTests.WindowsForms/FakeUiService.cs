#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Core.Services;
using Reko.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Text;
using Form = System.Windows.Forms.Form;
using ContextMenu = System.Windows.Forms.ContextMenu;

namespace Reko.UnitTests.Mocks
{
    public class FakeUiService : IDecompilerUIService
    {
        private bool simulateUserCancel;
        private string lastFileName;
        private Form lastDialogShown;

        public void ShowError(Exception ex, string format, params object[] args)
        {
            Debug.Print("*** Exception reported: {0}", ex);
            Debug.Print("{0}", ex.StackTrace);
            throw new ApplicationException(string.Format(format, args), ex);
        }

        public bool Prompt(string s)
        {
            throw new NotImplementedException();
        }

        public DialogResult ShowModalDialog(Form dlg)
        {
            lastDialogShown = dlg;
            return  simulateUserCancel
                    ? DialogResult.Cancel
                    : DialogResult.OK;
        }

        public DialogResult ShowModalDialog(IDialog dlg)
        {
            return ShowModalDialog((Form)dlg);
        }

        public string ShowOpenFileDialog(string fileName)
        {
            if (!simulateUserCancel)
            {
                return lastFileName;
            }
            else
                return null;
        }

        public string ShowSaveFileDialog(string fileName)
        {
            throw new NotImplementedException();
        }

        // Fake members ///////

        public string OpenFileResult
        {
            get { return lastFileName; }
            set { lastFileName = value; }
        }

        public string SaveFileResult
        {
            get { return lastFileName; }
            set { lastFileName = value; }
        }


        public bool SimulateUserCancel
        {
            get { return simulateUserCancel; }
            set { simulateUserCancel = value; }
        }


        public Form ProbeLastShownDialog
        {
            get { return lastDialogShown; }
        }


        public void ShowMessage(string msg)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeShellUiService :
        FakeUiService,
        IDecompilerShellUiService,
        ICommandTarget
    {
        public IEnumerable<IWindowFrame> DocumentWindows { get; set; }

        public IEnumerable<IWindowFrame> ToolWindows { get; set; }

        public Dictionary<string, Dictionary<int, CommandID>> KeyBindings { get; set; }
 
        public void SetContextMenu(object control, int menuId)
        {
        }

        public IWindowFrame FindWindow(string windowType)
        {
            throw new NotImplementedException();
        }

        public IWindowFrame CreateWindow(string windowType, string windowTitle, IWindowPane pane)
        {
            throw new NotImplementedException();
        }

        public IWindowFrame FindDocumentWindow(string documentType, object docItem)
        {
            throw new NotImplementedException();
        }

        public IWindowFrame CreateDocumentWindow(string documentType, object docItem, string documentTitle, IWindowPane pane)
        {
            throw new NotImplementedException();
        }

        public void WithWaitCursor(Action action)
        {
            action();
        }

        #region ICommandTarget Members

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            return false;
        }

        public bool Execute(CommandID cmdId)
        {
            return false;
        }

        #endregion


        public IWindowFrame ActiveFrame
        {
            get { throw new NotImplementedException(); }
        }
    }

}
