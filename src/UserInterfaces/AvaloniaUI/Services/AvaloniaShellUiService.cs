#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Reko.Gui;
using Reko.Gui.Services;
using Reko.UserInterfaces.AvaloniaUI.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    public class AvaloniaShellUiService : IDecompilerShellUiService
    {
        private IServiceContainer services;
        private MainWindow mainWindow;

        public AvaloniaShellUiService(IServiceContainer services, MainWindow mainWindow)
        {
            this.services = services;
            this.mainWindow = mainWindow;
        }

        public IWindowFrame ActiveFrame => throw new NotImplementedException();

        public IEnumerable<IWindowFrame> DocumentWindows => throw new NotImplementedException();

        public IEnumerable<IWindowFrame> ToolWindows => throw new NotImplementedException();

        public Dictionary<string, Dictionary<int, CommandID>> KeyBindings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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

        public bool Prompt(string prompt)
        {
            throw new NotImplementedException();
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            //$TODO
            //ICommandTarget ct = ActiveCommandTarget();
            //if (ct == null)
                return false;
            //return ct.QueryStatus(cmdId, status, text);
        }

        public void SetContextMenu(object control, int menuID)
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

        public void WithWaitCursor(Action p)
        {
            throw new NotImplementedException();
        }
    }
}
