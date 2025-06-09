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

using Reko.Gui;
using Reko.Gui.Services;
using Reko.Gui.ViewModels;
using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    public class TabControlHost : ITabControlHostService
    {
        private readonly TabControl tabCtrl;
        private readonly IServiceProvider services;

        public TabControlHost(IServiceProvider services, TabControl tabCtrl)
        {
            this.services = services;
            this.tabCtrl = tabCtrl;
        }

        public IWindowFrame Attach(IWindowPane pane, object tabPage)
        {
            var frame = new TabControlWindowFrame(tabCtrl, (TabPage)tabPage, pane, services);
            return frame;
        }

        public IWindowFrame Add(IWindowPane pane, string tabCaption)
        {
            var tabPage = new TabPage(tabCaption);
            tabCtrl.TabPages.Add(tabPage);
            var frame = new TabControlWindowFrame(tabCtrl, tabPage, pane, services);
            return frame;
        }

        public IWindowFrame ActiveFrame
        {
            get { return tabCtrl.SelectedTab is not null ? (IWindowFrame) tabCtrl.SelectedTab.Tag : null; }
            set { var page = FindPage(value); if (page is not null) tabCtrl.SelectedTab = page; }
        }

        public bool ContainsFocus
        {
            get { return tabCtrl.ContainsFocus; } 
        }

        private TabPage FindPage(IWindowFrame frame)
        {
            return tabCtrl.TabPages.Cast<TabPage>().Where(p => p.Tag == frame).FirstOrDefault();
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            var frame = ActiveFrame;
            if (frame is null)
                return false;
            var ct = frame.Pane as ICommandTarget;
            if (ct is null)
                return false;
            return ct.QueryStatus(cmdId, status, text);
        }

        public ValueTask<bool> ExecuteAsync(CommandID cmdId)
        {
            var frame = ActiveFrame;
            if (frame is not null && frame.Pane is ICommandTarget ct)
            {
                return ct.ExecuteAsync(cmdId);
            }
            return ValueTask.FromResult(false);

        }
    }
}
