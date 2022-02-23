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

using Avalonia.Controls;
using Dock.Model.Core;
using Reko.Gui;
using Reko.Gui.Services;
using Reko.UserInterfaces.AvaloniaUI.ViewModels;
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Docks;
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Documents;
using Reko.UserInterfaces.AvaloniaUI.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    public class AvaloniaShellUiService : IDecompilerShellUiService
    {
        private readonly IServiceContainer services;
        private readonly MainWindow mainWindow;
        private readonly DockFactory dockFactory;

        public AvaloniaShellUiService(IServiceContainer services, MainWindow mainWindow, DockFactory dockFactory)
        {
            this.services = services;
            this.mainWindow = mainWindow;
            this.dockFactory = dockFactory;
        }

        public IWindowFrame ActiveFrame => throw new NotImplementedException();

        public IEnumerable<IWindowFrame> DocumentWindows
        {
            get
            {
                var customDocDock = (CustomDocumentDock) dockFactory.GetDockable<IDockable>("Documents")!;
                return customDocDock.VisibleDockables!.OfType<IWindowFrame>();
            }
        }

        public IEnumerable<IWindowFrame> ToolWindows => throw new NotImplementedException();

        public Dictionary<string, Dictionary<int, CommandID>> KeyBindings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IWindowFrame CreateDocumentWindow(string documentType, object docItem, string documentTitle, IWindowPane pane)
        {
            var frame = new DocumentFrameViewModel(documentType, docItem, pane, documentTitle);
            var customDocDock = (CustomDocumentDock) dockFactory.GetDockable<IDockable>("Documents")!;
            customDocDock.VisibleDockables!.Add(frame);
            customDocDock.ActiveDockable = frame;
            return frame;
        }

        public IWindowFrame CreateWindow(string windowType, string windowTitle, IWindowPane pane)
        {
            throw new NotImplementedException();
        }


        public IWindowFrame? FindDocumentWindow(string documentType, object? docItem)
        {
            var customDocDock = (CustomDocumentDock) dockFactory.GetDockable<IDockable>("Documents")!;
            foreach (var doc in customDocDock.VisibleDockables!)
            {
                if (doc is DocumentFrameViewModel docFrame && 
                    docFrame.DocumentType == documentType && 
                    docFrame.DocumentItem == docItem)
                {
                    return docFrame;
                }
            }
            return null;
        }

        public IWindowFrame FindWindow(string windowType)
        {
            throw new NotImplementedException();
        }

        public ValueTask<bool> Prompt(string prompt)
        {
            throw new NotImplementedException();
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            //$TODO: dispatch the command to the currently active dock window.
            //ICommandTarget ct = ActiveCommandTarget();
            //if (ct == null)
                return false;
            //return ct.QueryStatus(cmdId, status, text);
        }


        public ValueTask<bool> ExecuteAsync(CommandID cmdId)
        {
            //$TODO: dispatch the command to the currently active dock window
            return ValueTask.FromResult(false);
        }

        public void SetContextMenu(object control, int menuID)
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

        public ValueTask<TResult> ShowModalDialog<TResult>(IDialog dlg)
        {
            throw new NotImplementedException();
        }


        public async ValueTask<string?> ShowOpenFileDialog(string fileName)
        {
            var ofd = new OpenFileDialog();
            ofd.Filters.Add(new FileDialogFilter { Name = "All files", Extensions = new List<string> { "*.*" } });
            ofd.InitialFileName = fileName;
            ofd.AllowMultiple = false;
            var files = await ofd.ShowAsync(mainWindow);
            if (files is null)
                return null;
            else
                return files[0];
        }

        public ValueTask<string?> ShowSaveFileDialog(string fileName)
        {
            throw new NotImplementedException();
        }

        public void WithWaitCursor(Action p)
        {
            throw new NotImplementedException();
        }
    }
}
