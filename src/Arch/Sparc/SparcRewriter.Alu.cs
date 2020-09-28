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
        private void RewriteAddxSubx(Func<Expression,Expression,Expression> op, bool emitCc)
        {
            var dst = RewriteRegister(instrCur.Operands[2]);
            var src1 = RewriteOp(instrCur.Operands[0]);
            var src2 = RewriteOp(instrCur.Operands[1]);
            var C = binder.EnsureFlagGroup(Registers.C);
            m.Assign(
                dst,
                op(op(src1, src2), C));
            if (emitCc)
            {
                EmitCc(dst);
            }
        }

        private void RewriteAlu(Func<Expression,Expression,Expression> op, bool negateOp2)
        {
            var dst = RewriteRegister(instrCur.Operands[2]);
            var src1 = RewriteOp(instrCur.Operands[0]);
            var src2 = RewriteOp(instrCur.Operands[1]);
            if (negateOp2)
            {
                src2 = m.Comp(src2);
            }
            m.Assign(dst, op(src1, src2));
        }

        private void RewriteAluCc(Func<Expression, Expression, Expression> op, bool negateOp2)
        {
            RewriteAlu(op, negateOp2);
            var dst = RewriteRegister(instrCur.Operands[2]);
            EmitCc(dst);
        }

        private void RewriteDLoad(PrimitiveType size)
        {
            throw new NotImplementedException();
        }

        private void RewriteLdstub()
        {
            var mem = (MemoryAccess)RewriteOp(instrCur.Operands[0]);
            var dst = RewriteOp(instrCur.Operands[1]);
            var bTmp = binder.CreateTemporary(PrimitiveType.Byte);
            var tmpHi = binder.CreateTemporary(PrimitiveType.CreateWord(dst.DataType.BitSize - bTmp.DataType.BitSize));
            var intrinsic = host.PseudoProcedure("__ldstub", bTmp.DataType, m.AddrOf(arch.PointerType, mem));
            m.Assign(bTmp, intrinsic);
            m.Assign(tmpHi, m.Slice(tmpHi.DataType, dst, bTmp.DataType.BitSize));
            m.Assign(dst, m.Seq(tmpHi, bTmp));
        }

        private void RewriteLoad(PrimitiveType size)
        {
            var dst = RewriteOp(instrCur.Operands[1]);
            var src = RewriteMemOp(instrCur.Operands[0], size);
            if (size.Size < dst.DataType.Size)
            {
                size = (size.Domain == Domain.SignedInt) ? PrimitiveType.Int32 : PrimitiveType.Word32;
                src = m.Cast(size, src);
            }
            m.Assign(dst, src);
        }

        private void RewriteMulscc()
        {
            var dst = RewriteOp(instrCur.Operands[2]);
            var src1 = RewriteOp(instrCur.Operands[0]);
            var src2 = RewriteOp(instrCur.Operands[1]);
            m.Assign(
                dst,
                host.PseudoProcedure("__mulscc", PrimitiveType.Int32, src1, src2));
            EmitCc(dst);
        }

        private void RewriteRestore()
        {
            var dst = RewriteOp(instrCur.Operands[2]);
            var src1 = RewriteOp(instrCur.Operands[0]);
            var src2 = RewriteOp(instrCur.Operands[1]);
            Identifier tmp = null;
            if (dst is Identifier && ((Identifier)dst).Storage != Registers.g0)
            {
                tmp = binder.CreateTemporary(dst.DataType);
                m.Assign(tmp, m.IAdd(src1, src2));
            }
            for (int i = 0; i < Registers.OutRegisters.Length; ++i)
            {
                Copy(Registers.InRegisters[i], Registers.OutRegisters[i]);
            }

            if (tmp != null)
            {
                m.Assign(dst, tmp);
            }
        }

        private void RewriteSave()
        {
            var dst = RewriteOp(instrCur.Operands[2]);
            var src1 = RewriteOp(instrCur.Operands[0]);
            var src2 = RewriteOp(instrCur.Operands[1]);
            Identifier tmp = null;
            if (((Identifier)dst).Storage != Registers.g0)
            {
                tmp = binder.CreateTemporary(dst.DataType);
                m.Assign(tmp, m.IAdd(src1, src2));
            }
            for (int i = 0; i < Registers.InRegisters.Length; ++i)
            {
                Copy(Registers.OutRegisters[i], Registers.InRegisters[i]);
            }
            if (tmp != null)
            {
                m.Assign(dst, tmp);
            }
        }


        private void Copy(RegisterStorage src, RegisterStorage dst)
        {
            m.Assign(
                binder.EnsureRegister(dst),
                binder.EnsureRegister(src));
        }

        private void RewriteSethi()
        {
            var rDst = (RegisterOperand)instrCur.Operands[1];
            if (rDst.Register == Registers.g0)
            {
                m.Nop();
            }
            else
            {
                //$TODO: check relocations for a symbol at instrCur.Address.
                var dst = binder.EnsureRegister(rDst.Register);
                var src = (ImmediateOperand)instrCur.Operands[0];
                m.Assign(dst, Constant.Word32(src.Value.ToUInt32() << 10));
            }
        }

        private void RewriteStore(PrimitiveType size)
        {
            var src = RewriteOp(instrCur.Operands[0]);
            var dst = RewriteMemOp(instrCur.Operands[1], size);
            if (size.Size < src.DataType.Size)
                src = m.Cast(size, src);
            m.Assign(dst, src);
        }
    }
}
