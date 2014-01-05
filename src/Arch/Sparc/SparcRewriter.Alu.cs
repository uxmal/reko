#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Sparc
{
    public partial class SparcRewriter
    {
        private void RewriteAlu(Operator op)
        {
            var dst = RewriteOp(di.Instr.Op3);
            var src1 = RewriteOp(di.Instr.Op1);
            var src2 = RewriteOp(di.Instr.Op2);
            emitter.Assign(dst, new BinaryExpression(op, PrimitiveType.Word32, src1, src2));
        }

        private void RewriteAluCc(Operator op)
        {
            RewriteAlu(op);
            var dst = RewriteOp(di.Instr.Op3);
            EmitCc(dst);
        }

        private void RewriteLoad(PrimitiveType size)
        {
            var dst = RewriteOp(di.Instr.Op2);
            var src = RewriteMemOp(di.Instr.Op1, size);
            if (size.Size < dst.DataType.Size)
            {
                size = (size.Domain == Domain.SignedInt) ? PrimitiveType.Int32 : PrimitiveType.Word32;
                src = emitter.Cast(size, src);
            }
            emitter.Assign(dst, src);
        }

        private void RewriteMulscc()
        {
            var dst = RewriteOp(di.Instr.Op3);
            var src1 = RewriteOp(di.Instr.Op1);
            var src2 = RewriteOp(di.Instr.Op2);
            emitter.Assign(
                dst,
                PseudoProc("__mulscc", PrimitiveType.Int32, src1, src2));
            EmitCc(dst);
        }

        private void RewriteSethi()
        {
            var dst = RewriteOp(di.Instr.Op2);
            var src = (ImmediateOperand) di.Instr.Op1;
            emitter.Assign(dst, Constant.Word32(src.Value.ToUInt32() << 10));
        }

        private void RewriteStore(PrimitiveType size)
        {
            var src = RewriteOp(di.Instr.Op1);
            var dst = RewriteMemOp(di.Instr.Op2, size);
            if (size.Size < src.DataType.Size)
                src = emitter.Cast(size, src);
            emitter.Assign(dst, src);
        }
    }
}
