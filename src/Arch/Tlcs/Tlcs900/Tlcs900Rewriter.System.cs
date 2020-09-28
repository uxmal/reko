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
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Registers = Reko.Arch.Tlcs.Tlcs900.Tlcs900Registers;

namespace Reko.Arch.Tlcs.Tlcs900
{
    public partial class Tlcs900Rewriter
    {
        private void RewriteDecf()
        {
            //$TODO: model this as an explicit bank switch?
            m.SideEffect(host.PseudoProcedure("__decf", VoidType.Instance));
        }

        private void RewriteEi()
        {
            var ppp = host.PseudoProcedure("__ei", VoidType.Instance, RewriteSrc(instr.Operands[0]));
            m.SideEffect(ppp);
        }

        private void RewriteHalt()
        {
            var c = new ProcedureCharacteristics
            {
                Terminates = true,
            };
            m.SideEffect(host.PseudoProcedure("__halt", c, VoidType.Instance));
        }

        private void RewriteIncf()
        {
            //$TODO: model this as an explicit bank switch?
            m.SideEffect(host.PseudoProcedure("__incf", VoidType.Instance));
        }

        private void RewriteLdf()
        {
            //$TODO: model this as an explicit bank switch?
            m.SideEffect(host.PseudoProcedure("__ldf", VoidType.Instance, RewriteSrc(instr.Operands[0])));
        }

        private void RewriteSwi()
        {
            iclass = InstrClass.Transfer | InstrClass.Call;
            var xsp = binder.EnsureRegister(Registers.xsp);
            var sr = binder.EnsureRegister(Registers.sr);
            var dst = Address.Ptr32(0xFFFF00u + ((ImmediateOperand)instr.Operands[0]).Value.ToUInt32() * 4);
            m.Assign(xsp, m.ISubS(xsp, 2));
            m.Assign(m.Mem16(xsp), sr);
            m.Call(dst, 4);
        }
    }
}
