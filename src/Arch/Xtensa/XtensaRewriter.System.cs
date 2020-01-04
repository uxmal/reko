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

using Reko.Core.Machine;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Xtensa
{
    public partial class XtensaRewriter
    {
        private void RewriteBreak()
        {
            m.SideEffect(host.PseudoProcedure(
                "__break",
                VoidType.Instance,
                RewriteOp(instr.Operands[0]),
                RewriteOp(instr.Operands[1])));
        }

        private void RewriteIll()
        {
            var c = new ProcedureCharacteristics
            {
                Terminates = true,
            };
            m.SideEffect(host.PseudoProcedure("__ill", c, VoidType.Instance));
        }

        private void RewriteL32e()
        {
            var dst = RewriteOp(this.instr.Operands[0]);
            var offset = ((ImmediateOperand)dasm.Current.Operands[2]).Value;
            m.Assign(
                dst,
                host.PseudoProcedure(
                    "__l32e",
                    PrimitiveType.Word32,
                    m.IAdd(
                        RewriteOp(dasm.Current.Operands[1]),
                        offset)));
        }

        private void RewriteS32e()
        {
            var src = RewriteOp(this.instr.Operands[0]);
            var offset = ((ImmediateOperand)dasm.Current.Operands[2]).Value;
            m.SideEffect(
                host.PseudoProcedure(
                    "__s32e",
                    VoidType.Instance,
                    m.IAdd(
                        RewriteOp(dasm.Current.Operands[1]),
                        offset),
                    src));
        }

        private void RewriteReserved()
        {
            m.SideEffect(host.PseudoProcedure("__reserved", VoidType.Instance));
        }

        private void RewriteWsr()
        {
            var dst = RewriteOp(dasm.Current.Operands[1]);
            var src = RewriteOp(dasm.Current.Operands[0]);
            m.Assign(dst, src);
        }
    }
}
