#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using System;
using System.Windows;
using System.Windows.Forms;
using Reko.Core;
using Reko.Gui;
using Reko.Gui.Services;

namespace Reko.UserInterfaces.WindowsForms
{
    public class WindowsStatusBarService : StatusBarService
    {
        private readonly StatusStrip statusStrip;
        private readonly ToolStripLabel selectedAddressLabel;
        private readonly ISelectedAddressService selAddrSvc;

        public WindowsStatusBarService(
            StatusStrip statusStrip,
            ToolStripLabel selectedAddressLabel,
            ISelectedAddressService selAddrSvc)
        {
            this.statusStrip = statusStrip;
            this.selectedAddressLabel = selectedAddressLabel;
            this.selAddrSvc = selAddrSvc;
            selAddrSvc.SelectedAddressChanged += selAddrSvc_SelectedAddressChanged;
        }

        public override void HideProgress()
        {
            throw new NotImplementedException();
        }

        public override void SetSubtext(string v)
        {
            throw new NotImplementedException();
        }

        public override void SetText(string text)
        {
            statusStrip.Items[0].Text = text;
        }

        public override void ShowProgress(int percentDone)
        {
            throw new NotImplementedException();
        }

        private void selAddrSvc_SelectedAddressChanged(object sender, EventArgs e)
        {
            string message = RenderAddressSelection(selAddrSvc.SelectedAddressRange);
            selectedAddressLabel.Text = message;
        }
    }
}