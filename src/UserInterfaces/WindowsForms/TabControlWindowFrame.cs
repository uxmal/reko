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

using Reko.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    /// <summary>
    /// Implementation of an IWindowFrame that is hosted in a tab control page.
    /// </summary>
    public class TabControlWindowFrame : IWindowFrame
    {
        private TabControl ctrl;
        private TabPage page;
        private ServiceContainer sc;

        public TabControlWindowFrame(TabControl ctrl, TabPage page, IWindowPane pane, IServiceProvider services)
        {
            this.ctrl = ctrl;
            this.page = page;
            this.Pane = pane;
            this.sc = new ServiceContainer(services);
            sc.AddService(typeof(IWindowFrame), this);
            page.Tag = this;
            pane.SetSite(sc);
        }

        public IWindowPane Pane { get; private set; }
        public string Title { get { return page.Text; } set { page.Text = value; } }

        public void Show()
        {
            ctrl.SelectedTab = page;
        }

        public void Close()
        {
            ctrl.TabPages.Remove(page);
            page.Dispose();
            page = null;
        }
    }
}
