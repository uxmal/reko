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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Gui
{
    /// <summary>
    /// Used in search results to navigate to basic blocks. When NavigateTo is invoked, 
    /// a code viewer is displayed at the address of the basic block.
    /// </summary>
    public class BlockNavigator : ICodeLocation
    {
        private IServiceProvider sp;
        private Program program;

        public BlockNavigator(Program program, Block block, IServiceProvider sp)
        {
            this.program = program;
            this.Block = block;
            this.sp = sp;
        }

        public Block Block { get; private set; }

        #region ICodeLocation Members

        public string Text
        {
            get { return Block.Name; }
        }

        public void NavigateTo()
        {
            var codeSvc = sp.GetService<ICodeViewerService>();
            if (codeSvc != null)
                codeSvc.DisplayProcedure(program, Block.Procedure, program.NeedsScanning);
        }

        #endregion
    }
}
