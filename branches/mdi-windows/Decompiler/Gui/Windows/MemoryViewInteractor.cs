/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Gui.Windows.Forms;
using Decompiler.Gui.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    public class MemoryViewInteractor : IWindowPane
    {
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        private IServiceProvider sp;
        private MemoryControl ctl;


        public Control CreateControl()
        {
            ctl = new MemoryControl();
            ctl.Font = new Font("Lucida Console", 10F);     //$TODO: make this user configurable.
            ctl.SelectionChanged += new EventHandler<SelectionChangedEventArgs>(ctl_SelectionChanged);
            var uiService = (IDecompilerShellUiService)sp.GetService(typeof(IDecompilerShellUiService));
            ctl.ContextMenu = uiService.GetContextMenu(MenuIds.CtxMemoryControl);
            return ctl;
        }


        public void SetSite(IServiceProvider sp)
        {
            this.sp = sp;
        }

        public void Close()
        {
        }


        public ProgramImage ProgramImage
        {
            get { return ctl.ProgramImage; }
            set { ctl.ProgramImage = value; }
        }

        void ctl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, e);
        }

        public void InvalidateControl()
        {
            ctl.Invalidate();
        }

        public AddressRange GetSelectedAddressRange()
        {
            return ctl.GetAddressRange();
        }
    }
}
