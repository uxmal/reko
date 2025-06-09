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

using Microsoft.Msagl.GraphViewerGdi;
using Reko.Core;
using Reko.Gui.ViewModels;
using System;
using System.Windows.Forms;

#nullable enable

namespace Reko.UserInterfaces.WindowsForms
{
    public class CallGraphPane : IWindowPane
    {
        private Program program;

        public CallGraphPane(Program program)
        {
            this.program = program;
        }

        public GViewer? Control { get; private set; }
        public IWindowFrame? Frame { get; set; }

        public void Close()
        {
            if (Control is not null)
            {
                Control.Dispose();
            }
            Control = null;
        }

        public object CreateControl()
        {
            this.Control = new GViewer();
            this.Control.Dock = DockStyle.Fill;
            this.Control.Visible = true;
            this.Control.PanButtonPressed = true;
            this.Control.ToolBarIsVisible = true;
            this.Control.Graph = CallGraphGenerator.Generate(program);
            return Control;
        }

        public void SetSite(IServiceProvider sp)
        {
        }
    }
}
