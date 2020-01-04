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
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Reko.Gui
{
    /// <summary>
    /// Methods for creating windows in the shell. 
    /// </summary>
    public interface IDecompilerShellUiService : IDecompilerUIService, ICommandTarget
    {
        IWindowFrame ActiveFrame { get; }
        IEnumerable<IWindowFrame> DocumentWindows { get; }
        IEnumerable<IWindowFrame> ToolWindows { get; }

        Dictionary<string,Dictionary<int,CommandID>> KeyBindings { get; set; }

        IWindowFrame CreateWindow(string windowType, string windowTitle, IWindowPane pane);
        IWindowFrame CreateDocumentWindow(string documentType, object docItem, string documentTitle, IWindowPane pane);
        IWindowFrame FindWindow(string windowType);
        IWindowFrame FindDocumentWindow(string documentType, object docItem);
        void SetContextMenu(object control, int menuID);
        DialogResult ShowModalDialog(IDialog dlg);
        void WithWaitCursor(Action p);
    }

    public enum DialogResult
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Abort = 3,
        Retry = 4,
        Ignore = 5,
        Yes = 6,
        No = 7
    }
}
