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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.PaRisc
{
    public partial class PaRiscRewriter
    {
        private void RewriteDiag()
        {
            var src = RewriteOp(instr.Operands[0]);
            m.SideEffect(host.PseudoProcedure("__diag", VoidType.Instance, src));
        }

        private void RewriteMfctl()
        {
            var src = RewriteOp(instr.Operands[0]);
            var rDst = ((RegisterOperand) instr.Operands[1]).Register;
            var n = rDst.Number - Registers.ControlRegisters[0].Number;
            if (1 <= n && n <= 7)
            {
                m.Invalid();
                return;
            }
            if (!(n == 11 || n == 26 || n == 27 || n == 16))
            {
                iclass |= InstrClass.System;
            }
            m.Assign(binder.EnsureRegister(rDst), src);
        }

        private void RewriteMtctl()
        {
            var src = RewriteOp(instr.Operands[0]);
            var rDst = ((RegisterOperand) instr.Operands[1]).Register;
            var n = rDst.Number - Registers.ControlRegisters[0].Number;
            if (1 <= n && n <= 7)
            {
                m.Invalid();
                return;
            }
            if (n != 11)
            {
                iclass |= InstrClass.System;
            }
            m.Assign(binder.EnsureRegister(rDst), src);
        }

        private void RewriteMtsm()
        {
            var src = RewriteOp(instr.Operands[0]);
            m.SideEffect(host.PseudoProcedure("__mtsm", VoidType.Instance, src));
        }

        private void RewriteMtsp()
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteOp(instr.Operands[1]);
            m.Assign(dst, src);
        }
    }
}
