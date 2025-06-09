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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.Forms
{
    public class BlockNameInteractor
    {
        private readonly IBlockNameDialog dlg;
        private readonly Procedure proc;
        private readonly Block block;

        public BlockNameInteractor(IBlockNameDialog dlg, Procedure proc, Block block)
        {
            this.dlg = dlg;
            this.proc = proc;
            this.block = block;

            dlg.BlockName.Text = block.DisplayName;
            dlg.BlockId.Text = "";
            dlg.ErrorMessage.Text = "";
            dlg.BlockName.TextChanged += BlockName_TextChanged;
        }

        private void BlockName_TextChanged(object? sender, EventArgs e)
        {
            var newName = dlg.BlockName.Text;
            if (IsNameAcceptable(newName))
            {
                dlg.ErrorMessage.Text = "";
                dlg.OkButton.Enabled = true;
            }
            else
            {
                dlg.ErrorMessage.Text = $"'{newName}' conflicts with an existing block label.";
                dlg.OkButton.Enabled = false;
            }
        }

        private bool IsNameAcceptable(string newName)
        {
            return proc.ControlGraph.Blocks.Any(b =>
                b != block &&
                b.Id != newName &&
                (b.UserLabel is null || b.UserLabel != newName));
        }
    }
}
