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

using System;
using System.Collections.Generic;
using System.Text;
using Reko.Core;

namespace Reko.Arch.Mips
{
    public class Mips16eRewriter : MipsRewriter
    {
        public Mips16eRewriter(MipsProcessorArchitecture arch, EndianImageReader rdr, IEnumerable<MipsInstruction> instrs, IStorageBinder binder, IRewriterHost host) 
            : base(arch, rdr, instrs, binder, host)
        {
        }

        protected override void RewriteSave(MipsInstruction instr)
        {
            host.Error(
                instr.Address,
                string.Format("MIPS16e instruction '{0}' is not supported yet.", instr));
            EmitUnitTest();
            iclass = InstrClass.Invalid; m.Invalid();
        }
    }
}
