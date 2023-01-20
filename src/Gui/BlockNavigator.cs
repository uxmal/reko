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

using Reko.Core;
using Reko.Core.Services;
using Reko.Gui.Services;
using System;
using System.Threading.Tasks;

namespace Reko.Gui
{
    /// <summary>
    /// Used in search results to navigate to basic blocks. When <see cref="NavigateTo" /> is invoked,
    /// a code viewer is displayed at the address of the basic block.
    /// </summary>
    public class BlockNavigator : ICodeLocation
    {
        private readonly IServiceProvider sp;
        private readonly Program program;

        public BlockNavigator(IReadOnlyProgram program, Block block, IServiceProvider sp)
        {
            this.program = (Program) program;
            this.Block = block;
            this.sp = sp;
        }

        public Block Block { get; private set; }

        #region ICodeLocation Members

        public string Text
        {
            get { return Block.DisplayName; }
        }

        public ValueTask NavigateTo()
        {
            var codeSvc = sp.GetService<ICodeViewerService>();
            if (codeSvc != null)
                codeSvc.DisplayProcedure(program, Block.Procedure, program.NeedsScanning);
            return ValueTask.CompletedTask;
        }

        #endregion
    }
}
