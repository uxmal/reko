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

using Reko.Core;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UserInterfaces.WindowsForms
{
    public class ViewImportsService : IViewImportsService
    {
        private IServiceProvider services;

        public ViewImportsService(IServiceProvider services)
        {
            this.services = services;
        }

        public void ShowImports(Program program)
        {
            var uiSvc = services.RequireService<IDecompilerShellUiService>();
            var w = uiSvc.FindDocumentWindow(typeof(ViewImportsService).ToString(), program);
            if (w is null)
            {
                var pane = new ViewImportsPane(program);
                w = uiSvc.CreateDocumentWindow(typeof(ViewImportsService).ToString(), program, "Imports - " + program.Name, pane);
            }
            w.Show();
        }
    }
}
