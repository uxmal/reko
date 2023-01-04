#region License
/* 
 * Copyright (C) 1999-2023 Pavel Tomin.
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

#nullable enable

using Reko.Core.Scripts;
using Reko.Gui;
using Reko.Gui.Services;
using Reko.UserInterfaces.WindowsForms.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Reko.UserInterfaces.WindowsForms
{
    public class StackTraceService : IStackTraceService
    {
        private readonly IServiceProvider services;
        private readonly MainForm mainForm;
        private StackTraceDialog? stackTraceDialog;
        private DiagnosticsInteractor? stackTraceInteractor;

        public StackTraceService(IServiceProvider services, MainForm mainForm)
        {
            this.services = services;
            this.mainForm = mainForm;
        }

        public void DisplayStackTrace(IList<ScriptStackFrame> stackFrames)
        {
            var interactor = ShowDialog();
            interactor.ClearDiagnostics();
            foreach (var stackFrame in stackFrames)
            {
                interactor.Inform(
                    new FileNavigator(
                        stackFrame.FileName,
                        stackFrame.LineNumber,
                        services)
                    {
                        ShowFileName = false,
                    },
                    stackFrame.ToString());
            }
        }

        public void Clear()
        {
            stackTraceDialog?.Close();
        }

        private DiagnosticsInteractor ShowDialog()
        {
            if (stackTraceDialog is null || stackTraceDialog.IsDisposed)
            {
                this.stackTraceDialog = new StackTraceDialog();
                stackTraceDialog.Owner = mainForm;
                stackTraceDialog.Show();
                PlaceStackTrace(stackTraceDialog);
                this.stackTraceInteractor = null;
            }
            if (stackTraceInteractor is null)
            {
                this.stackTraceInteractor = new DiagnosticsInteractor();
                stackTraceInteractor.Attach(
                    stackTraceDialog.StackTraceListView,
                    null);
            }
            stackTraceDialog.Focus();
            return stackTraceInteractor;
        }

        private void PlaceStackTrace(StackTraceDialog dialog)
        {
            var scrTabLocation = mainForm.TabControl.Parent.PointToScreen(
                mainForm.TabControl.Location);
            dialog.Location = new Point(
                scrTabLocation.X - dialog.Width + mainForm.TabControl.Width,
                scrTabLocation.Y);
            dialog.Height = mainForm.TabControl.Height;
        }
    }
}
