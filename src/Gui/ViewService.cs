#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

using Reko.Core;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Gui
{
    public class ViewService
    {
        private readonly IServiceProvider sp;

        public ViewService(IServiceProvider sp)
        {
            this.sp = sp;
        }

        protected IWindowFrame ShowWindow(string windowType, string windowTitle, object docItem, IWindowPane innerPane)
        {
            var shellUiSvc = sp.RequireService<IDecompilerShellUiService>();
            var windowFrame = shellUiSvc.FindDocumentWindow(windowType, docItem);
            if (windowFrame == null)
            {
                windowFrame = shellUiSvc.CreateDocumentWindow(windowType, docItem, windowTitle, innerPane);
            }
            windowFrame.Show();
            return windowFrame;
        }
    }
}
