#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Sparc
{
    public partial class SparcRewriter
    {
        private void RewriteAddxSubx(Operator op, bool emitCc)
        {
            var dst = RewriteRegister(instrCur.Op3);
            var src1 = RewriteOp(instrCur.Op1);
            var src2 = RewriteOp(instrCur.Op2);
            var C = frame.EnsureFlagGroup(Registers.C);
            emitter.Assign(
                dst,
                new BinaryExpression(
                    op,
                    dst.DataType,
                    new BinaryExpression(op, src1.DataType, src1, src2),
                    C));
            if (emitCc)
            {
                EmitCc(dst);
            }
        }

        private void RewriteAlu(Operator op, bool negateOp2)
        {
            var dst = RewriteRegister(instrCur.Op3);
            var src1 = RewriteOp(instrCur.Op1);
            var src2 = RewriteOp(instrCur.Op2);
            if (negateOp2)
            {
                src2 = emitter.Comp(src2);
            }
            emitter.Assign(dst, new BinaryExpression(op, PrimitiveType.Word32, src1, src2));
        }

        private void RewriteAluCc(Operator op, bool negateOp2)
        {
            RewriteAlu(op, negateOp2);
            var dst = RewriteRegister(instrCur.Op3);
            EmitCc(dst);
        }

        private void RewriteDLoad(PrimitiveType size)
        {
            throw new NotImplementedException();
        }

        private void RewriteLoad(PrimitiveType size)
        {
            var dst = RewriteOp(instrCur.Op2);
            var src = RewriteMemOp(instrCur.Op1, size);
            if (size.Size < dst.DataType.Size)
            {
                size = (size.Domain == Domain.SignedInt) ? PrimitiveType.Int32 : PrimitiveType.Word32;
                src = emitter.Cast(size, src);
            }
            emitter.Assign(dst, src);
        }

        private void RewriteMulscc()
        {
            var dst = RewriteOp(instrCur.Op3);
            var src1 = RewriteOp(instrCur.Op1);
            var src2 = RewriteOp(instrCur.Op2);
            emitter.Assign(
                dst,
                host.PseudoProcedure("__mulscc", PrimitiveType.Int32, src1, src2));
            EmitCc(dst);
        }

        private void RewriteRestore()
        {
            var dst = RewriteOp(instrCur.Op3);
            var src1 = RewriteOp(instrCur.Op1);
            var src2 = RewriteOp(instrCur.Op2);
            Identifier tmp = null;
            if (dst is Identifier && ((Identifier)dst).Storage != Registers.g0)
            {
                tmp = frame.CreateTemporary(dst.DataType);
                emitter.Assign(tmp, emitter.IAdd(src1, src2));
            }
            Copy(Registers.i0, Registers.o0);
            Copy(Registers.i1, Registers.o1);
            Copy(Registers.i2, Registers.o2);
            Copy(Registers.i3, Registers.o3);
            Copy(Registers.i4, Registers.o4);
            Copy(Registers.i5, Registers.o5);
            Copy(Registers.i6, Registers.sp);
            Copy(Registers.i7, Registers.o7);
            if (tmp != null)
            {
                emitter.Assign(dst, tmp);
            }
        }

        private void RewriteSave()
        {
            var dst = RewriteOp(instrCur.Op3);
            var src1 = RewriteOp(instrCur.Op1);
            var src2 = RewriteOp(instrCur.Op2);
            Identifier tmp = null;
            if (((Identifier)dst).Storage != Registers.g0)
            {
                tmp = frame.CreateTemporary(dst.DataType);
                emitter.Assign(tmp, emitter.IAdd(src1, src2));
            }
            Copy(Registers.o0, Registers.i0);
            Copy(Registers.o1, Registers.i1);
            Copy(Registers.o2, Registers.i2);
            Copy(Registers.o3, Registers.i3);
            Copy(Registers.o4, Registers.i4);
            Copy(Registers.o5, Registers.i5);
            Copy(Registers.sp, Registers.i6);
            Copy(Registers.o7, Registers.i7);
            if (tmp != null)
            {
                emitter.Assign(dst, tmp);
            }
        }


        private void Copy(RegisterStorage src, RegisterStorage dst)
        {
            emitter.Assign(
                frame.EnsureRegister(dst),
                frame.EnsureRegister(src));
        }

        private void RewriteSethi()
        {
            var rDst = (RegisterOperand)instrCur.Op2;
            if (rDst.Register == Registers.g0)
            {
                emitter.Nop();
            }
            else
            {
                //$TODO: check relocations for a symbol at instrCur.Address.
                var dst = frame.EnsureRegister(rDst.Register);
                var src = (ImmediateOperand)instrCur.Op1;
                emitter.Assign(dst, Constant.Word32(src.Value.ToUInt32() << 10));
            }
        }

        private void RewriteStore(PrimitiveType size)
        {
            var src = RewriteOp(instrCur.Op1);
            var dst = RewriteMemOp(instrCur.Op2, size);
            if (size.Size < src.DataType.Size)
                src = emitter.Cast(size, src);
            emitter.Assign(dst, src);
        }
    }
}
