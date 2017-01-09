#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.PowerPC
{
    partial class PowerPcRewriter
    {
        private void RewriteIsync()
        {
            emitter.SideEffect(host.PseudoProcedure("__isync", VoidType.Instance));
        }

        private void RewriteMfmsr()
        {
            var dst = RewriteOperand(instr.op1);
            emitter.Assign(dst, host.PseudoProcedure("__read_msr", PrimitiveType.Word32));
        }

        private void RewriteMtspr()
        {
            var spr = RewriteOperand(instr.op1);
            var reg = RewriteOperand(instr.op2);
            emitter.SideEffect(host.PseudoProcedure("__write_spr", PrimitiveType.Word32, spr, reg));
        }
    }
}
