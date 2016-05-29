#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Reko.Gui.Windows.Forms
{
    public class JumpTableInteractor
    {
        private JumpTableDialog dlg;

        public void Attach(JumpTableDialog dlg)
        {
            this.dlg = dlg;
            dlg.JumpTableStartAddress.Validating += JumpTableStartAddress_Validating;
            dlg.Load += Dlg_Load;
        }

        private void Dlg_Load(object sender, EventArgs e)
        {
            dlg.Text = string.Format("Jump table for {0}", dlg.IndirectJump.Address);
            dlg.IndirectJumpLabel.Text = dlg.IndirectJump.ToString().Replace('\t', ' ');
        }

        private void JumpTableStartAddress_Validating(object sender, CancelEventArgs e)
        {
            Address addr;
            if (!dlg.Program.Platform.TryParseAddress(dlg.JumpTableStartAddress.Text, out addr))
            {
                e.Cancel = true;
            } 
            else
            {
                e.Cancel = !dlg.Program.SegmentMap.IsValidAddress(addr);
            }
            if (e.Cancel)
            {
                dlg.ErrorProvider.SetError(dlg.JumpTableStartAddress, "Invalid address");
            }
            else
            {
                dlg.ErrorProvider.SetError(dlg.JumpTableStartAddress, "");
            }
        }
    }
}
