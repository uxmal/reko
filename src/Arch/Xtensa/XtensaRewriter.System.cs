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

        private void RewriteCacheFn(string fnName)
        {
            var ptr = RewriteOp(instr.Operands[0]);
            var off = RewriteOp(instr.Operands[1]);
            var location = m.IAdd(ptr, off);
            location.DataType = PrimitiveType.Ptr32;
            m.SideEffect(host.PseudoProcedure(fnName, VoidType.Instance, location));
        }

        private void RewriteIll()
        {
            var c = new ProcedureCharacteristics
            {
                Terminates = true,
            };
            m.SideEffect(host.PseudoProcedure("__ill", c, VoidType.Instance));
        }

        private void RewriteL32ai()
        {
            var ptr = m.IAdd(
                RewriteOp(instr.Operands[1]),
                RewriteOp(instr.Operands[2]));
            ptr.DataType = PrimitiveType.Ptr32;
            var dst = RewriteOp(instr.Operands[0]);
            m.Assign(dst, host.PseudoProcedure(
                "__l32ai",
                PrimitiveType.Word32,
                ptr));
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

        private void RewriteRer()
        {
            var src = RewriteOp(instr.Operands[1]);
            var dst = RewriteOp(instr.Operands[0]);
            m.Assign(dst, host.PseudoProcedure("__rer", PrimitiveType.Word32, src));
        }

        private void RewriteRsync()
        {
            m.SideEffect(host.PseudoProcedure("__rsync", VoidType.Instance));
        }

        private void RewriteS32c1i()
        {
            var ea = m.IAdd(
                RewriteOp(instr.Operands[1]),
                RewriteOp(instr.Operands[2]));
            ea.DataType = new Pointer(PrimitiveType.Word32, 32);
            var scomp = binder.EnsureRegister(Registers.SCOMPARE1);
            var dst = RewriteOp(instr.Operands[0]);
            m.Assign(dst, host.PseudoProcedure(
                "__s32c1i",
                PrimitiveType.Word32,
                ea,
                scomp));
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

        private void RewriteSyscall()
        {
            m.SideEffect(host.PseudoProcedure("__syscall", VoidType.Instance));
        }

        private void RewriteWer()
        {
            var dst = RewriteOp(dasm.Current.Operands[1]);
            var src = RewriteOp(dasm.Current.Operands[0]);
            m.SideEffect(host.PseudoProcedure("__wer", VoidType.Instance, dst, src));
        }

        private void RewriteWsr()
        {
            var dst = RewriteOp(dasm.Current.Operands[1]);
            var src = RewriteOp(dasm.Current.Operands[0]);
            m.Assign(dst, src);
        }

        private void RewriteXsr()
        {
            var src = RewriteOp(dasm.Current.Operands[1]);
            var dst = RewriteOp(dasm.Current.Operands[0]);
            m.Assign(dst, host.PseudoProcedure("__xsr", PrimitiveType.Word32, dst, src));
        }
    }
}
