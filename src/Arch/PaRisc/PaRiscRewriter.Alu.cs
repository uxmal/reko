#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.PaRisc
{
    partial class PaRiscRewriter
    {
        private void RewriteAdd()
        {
            var src1 = RewriteOp(instr.Operands[0]);
            var src2 = RewriteOp(instr.Operands[1]);
            var dst = RewriteOp(instr.Operands[2]);
            m.Assign(dst, m.IAdd(src1, src2));
            if (MaybeSkipNextInstruction(InstrClass.ConditionalTransfer, false, dst, null))
            {
                this.iclass = InstrClass.ConditionalTransfer;
            }
        }

        private void RewriteAddi()
        {
            var src1 = RewriteOp(instr.Operands[1]);
            var src2 = RewriteOp(instr.Operands[0]);
            var dst = RewriteOp(instr.Operands[2]);
            m.Assign(dst, m.IAdd(src1, src2));
            MaybeSkipNextInstruction(iclass, false, dst, null);
        }

        private void RewriteExtrw()
        {
            var src = RewriteOp(instr.Operands[0]);
            var bePos = ((ImmediateOperand) instr.Operands[1]).Value.ToInt32();
            var len = ((ImmediateOperand) instr.Operands[2]).Value.ToInt32();
            var dtSlice = PrimitiveType.CreateWord(len);
            var lePos = 32 - bePos;
            if (lePos < 0)
            {
                host.Warn(instr.Address, "Odd extrw instruction {0}", instr);
                iclass = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            var dst = RewriteOp(instr.Operands[3]);
            var dt = (instr.Sign == SignExtension.s)
                ? PrimitiveType.Int32
                : PrimitiveType.UInt32;
            m.Assign(dst, m.Cast(dt, m.Slice(dtSlice, src, lePos)));
        }

        private void RewriteLd(PrimitiveType size)
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteOp(instr.Operands[1]);
            if (src.DataType.BitSize < dst.DataType.BitSize)
            {
                src = m.Cast(PrimitiveType.Create(Domain.UnsignedInt, dst.DataType.BitSize), src);
            }
            m.Assign(dst, src);
        }

        private void RewriteLdo()
        {
            var src = (MemoryAccess) RewriteOp(instr.Operands[0]);
            var dst = RewriteOp(instr.Operands[1]);
            m.Assign(dst, src.EffectiveAddress);
        }

        private void RewriteOr()
        {
            var src1 = RewriteOp(instr.Operands[0]);
            var src2 = RewriteOp(instr.Operands[1]);
            var rDst = ((RegisterOperand) instr.Operands[2]).Register;
            if (rDst == Registers.GpRegs[0])
            {
                // Result thrown away == NOP.
                iclass = InstrClass.Padding | InstrClass.Linear;
                m.Nop();
                return;
            }
            var dst = binder.EnsureRegister(rDst);
            m.Assign(dst, m.IAdd(src1, src2));
            MaybeSkipNextInstruction(iclass, false, dst, null);
        }

        private void RewriteShladd()
        {
            var addend= RewriteOp(instr.Operands[0]);
            var sh = Constant.Int32(((ImmediateOperand) instr.Operands[1]).Value.ToInt32());
            Expression e = m.Shl(addend, sh);
            var src = RewriteOp(instr.Operands[2]);
            e = m.IAdd(src, e);
            var dst = RewriteOp(instr.Operands[3]);
            m.Assign(dst, e);
            MaybeSkipNextInstruction(iclass, false, e, null);
        }

        private void RewriteSt(PrimitiveType size)
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteOp(instr.Operands[1]);
            if (src.DataType.BitSize > dst.DataType.BitSize)
            {
                src = m.Slice(dst.DataType, src, 0);
            }
            m.Assign(dst, src);
        }

        private void RewriteSub()
        {
            var src1 = RewriteOp(instr.Operands[0]);
            var src2 = RewriteOp(instr.Operands[1]);
            var dst = RewriteOp(instr.Operands[2]);
            m.Assign(dst, m.ISub(src1, src2));
            MaybeSkipNextInstruction(iclass, false, dst, null);
        }

        private void RewriteFldw()
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteOp(instr.Operands[1]);
            m.Assign(dst, src);
        }

        private void RewriteFstw()
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteOp(instr.Operands[1]);
            m.Assign(dst, src);
        }
    }
}
