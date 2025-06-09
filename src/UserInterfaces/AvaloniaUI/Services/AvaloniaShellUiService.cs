#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Dock.Model.Core;
using MsBox.Avalonia.Enums;
using Reko.Core.Diagnostics;
using Reko.Gui;
using Reko.Gui.Services;
using Reko.Gui.ViewModels;
using Reko.UserInterfaces.AvaloniaUI.ViewModels;
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Docks;
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Documents;
using Reko.UserInterfaces.AvaloniaUI.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    public class AvaloniaShellUiService : IDecompilerShellUiService
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(AvaloniaShellUiService), "");

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

        public async ValueTask<bool> Prompt(string prompt)
        {
            var msgBox = MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard(
                "Reko Decompiler",
                prompt,
                ButtonEnum.YesNo);
            var result = await msgBox.ShowWindowDialogAsync(mainWindow);
            return result == ButtonResult.Yes;
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            //$TODO: dispatch the command to the currently active dock window.
            //ICommandTarget ct = ActiveCommandTarget();
            //if (ct is null)
                return false;
            //return ct.QueryStatus(cmdId, status, text);
        }


        public ValueTask<bool> ExecuteAsync(CommandID cmdId)
        {
            //$TODO: dispatch the command to the currently active dock window
            return ValueTask.FromResult(false);
        }

        public object GetContextMenu(int menuID)
        {
            return GetDecompilerCommandItems().GetContextMenu(menuID);
        }

        public void SetContextMenu(object control, int menuID)
        {
            var aControl = (ICommandItemSource) control;
            aControl.CommandItems = GetDecompilerCommandItems().GetContextMenu(menuID);
        }

        public void SetStatusForMenuItems(IList menuItems)
        {
            GetDecompilerCommandItems().SetStatusForMenuItems((IList<CommandItem>)menuItems);
        }


        public async ValueTask ShowError(Exception ex, string format, params object[] args)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(format, args);
            Exception? e = ex;
            while (e is { })
            {
                sb.Append(" ");
                sb.Append(e.Message);
                e = e.InnerException;
            }
            await Dispatcher.UIThread.InvokeAsync(async delegate ()
            {
                var msgBox = MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard(
                    "Reko Decompiler",
                    sb.ToString(),
                    ButtonEnum.Ok,
                    Icon.Error);
                await msgBox.ShowWindowDialogAsync(mainWindow);
            });
        }

        public async ValueTask ShowMessage(string msg)
        {
            var msgBox = MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard(
                "Reko Decompiler",
                msg,
                ButtonEnum.YesNo,
                Icon.Info);
            await msgBox.ShowWindowDialogAsync(mainWindow);
        }

        public async ValueTask<DialogResult> ShowModalDialog(IDialog dlg)
        {
            //$BUG: need to change all dialogs to se the other overload of ShowModalDialog
            await ((Window) dlg).ShowDialog(mainWindow);
            return DialogResult.OK;
        }

        public async ValueTask<TResult> ShowModalDialog<TResult>(IDialog<TResult> dlg)
        {
            await ((Window) dlg).ShowDialog(mainWindow);
            return dlg.Value;
        }

        public async ValueTask<string?> ShowOpenFileDialog(string? fileName)
        {
            var options = new FilePickerOpenOptions
            {
                FileTypeFilter = [new("All files") { Patterns = ["*.*"] }],
                SuggestedFileName = fileName,
                AllowMultiple = false,
            };
            var files = await this.mainWindow.StorageProvider.OpenFilePickerAsync(options);
            if (files is null || files.Count == 0)
                return null;
            else
                return files[0].Path.AbsolutePath;
        }

        public ValueTask<string?> ShowSaveFileDialog(string? fileName)
        {
            throw new NotImplementedException();
        }

        public void WithWaitCursor(Action p)
        {
            throw new NotImplementedException();
        }

        private DecompilerCommandItems GetDecompilerCommandItems()
        {
            var vm = (MainViewModel) mainWindow.DataContext!;
            return vm.CommandItems;
        }
    }
}
