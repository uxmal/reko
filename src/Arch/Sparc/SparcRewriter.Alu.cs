#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
            var dst = RewriteRegister(2);
            var src1 = RewriteOp(0)!;
            var src2 = RewriteOp(1)!;
            var C = binder.EnsureFlagGroup(arch.Registers.C);
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
            var dst = RewriteRegister(2);
            var src1 = RewriteOp(0)!;
            var src2 = RewriteOp(1)!;
            if (negateOp2)
            {
                src2 = m.Comp(src2);
            }
            m.Assign(dst, op(src1, src2));
        }

        private void RewriteAluCc(Func<Expression, Expression, Expression> op, bool negateOp2)
        {
            RewriteAlu(op, negateOp2);
            var dst = RewriteRegister(2);
            EmitCc(dst);
        }

        private void RewriteDLoad(PrimitiveType size)
        {
            throw new NotImplementedException();
        }

        private void RewriteLdd()
        {
            var size = PrimitiveType.Word64;
            var rDstHi = (RegisterStorage) instrCur.Operands[1];
            var rDstLo = arch.Registers.GetRegister((uint)rDstHi.Number + 1);
            var dst = binder.EnsureSequence(size, rDstHi, rDstLo);
            var src = RewriteMemOp(instrCur.Operands[0], size)!;
            //$TODO: what about sparc64? Instruction is deprecated there...
            if (size.Size < dst.DataType.Size)
            {
                size = PrimitiveType.Create(size.Domain, dst.DataType.BitSize);
                src = m.Convert(src, src.DataType, size);
            }
            m.Assign(dst, src);
        }

        private void RewriteLdstub()
        {
            var mem = (MemoryAccess)RewriteOp(0)!;
            var dst = RewriteOp(1)!;
            var bTmp = binder.CreateTemporary(PrimitiveType.Byte);
            var tmpHi = binder.CreateTemporary(PrimitiveType.CreateWord(dst.DataType.BitSize - bTmp.DataType.BitSize));
            var intrinsic = host.Intrinsic("__ldstub", true, bTmp.DataType, m.AddrOf(arch.PointerType, mem));
            m.Assign(bTmp, intrinsic);
            m.Assign(tmpHi, m.Slice(tmpHi.DataType, dst, bTmp.DataType.BitSize));
            m.Assign(dst, m.Seq(tmpHi, bTmp));
        }

        private void RewriteLoad(PrimitiveType size)
        {
            var dst = RewriteOp(instrCur.Operands[1])!;
            var src = RewriteMemOp(instrCur.Operands[0], size)!;
            if (size.Size < dst.DataType.Size)
            {
                size = PrimitiveType.Create(size.Domain, dst.DataType.BitSize);
                src = m.Convert(src, src.DataType, size);
            }
            m.Assign(dst, src);
        }

        private void RewriteMulscc()
        {
            var dst = RewriteOp(instrCur.Operands[2])!;
            var src1 = RewriteOp(instrCur.Operands[0])!;
            var src2 = RewriteOp(instrCur.Operands[1])!;
            m.Assign(
                dst,
                host.Intrinsic("__mulscc", false, PrimitiveType.Int32, src1, src2));
            EmitCc(dst);
        }

        private void RewriteRestore()
        {
            var dst = RewriteOp(instrCur.Operands[2])!;
            var src1 = RewriteOp(instrCur.Operands[0])!;
            var src2 = RewriteOp(instrCur.Operands[1])!;
            RestoreRegisterWindow(dst, src1, src2);
        }

        private void RestoreRegisterWindow(Expression dst, Expression src1, Expression src2)
        {
            Identifier? tmp = null;
            if (dst is Identifier identifier && identifier.Storage != arch.Registers.g0)
            {
                tmp = binder.CreateTemporary(dst.DataType);
                m.Assign(tmp, m.IAdd(src1, src2));
            }
            for (int i = 0; i < arch.Registers.OutRegisters.Length; ++i)
            {
                Copy(arch.Registers.InRegisters[i], arch.Registers.OutRegisters[i]);
            }

            if (tmp != null)
            {
                m.Assign(dst, tmp);
            }
        }

        private void RewriteSave()
        {
            var dst = RewriteOp(2)!;
            var src1 = RewriteOp(0)!;
            var src2 = RewriteOp(1)!;
            Identifier? tmp = null;
            if (dst is Identifier)
            {
                tmp = binder.CreateTemporary(dst.DataType);
                m.Assign(tmp, m.IAdd(src1, src2));
            }
            for (int i = 0; i < arch.Registers.InRegisters.Length; ++i)
            {
                Copy(arch.Registers.OutRegisters[i], arch.Registers.InRegisters[i]);
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
            var rDst = (RegisterStorage)instrCur.Operands[1];
            if (rDst == arch.Registers.g0)
            {
                m.Nop();
            }
            else
            {
                //$TODO: check relocations for a symbol at instrCur.Address.
                var dst = binder.EnsureRegister(rDst);
                var src = (ImmediateOperand)instrCur.Operands[0];
                m.Assign(dst, Constant.Word32(src.Value.ToUInt32() << 10));
            }
        }

        private void RewriteStd()
        {
            var size = PrimitiveType.Word64;
            var rSrcHi = (RegisterStorage) instrCur.Operands[0];
            var rSrcLo = arch.Registers.GetRegister((uint) rSrcHi.Number + 1);
            Expression src = binder.EnsureSequence(size, rSrcHi, rSrcLo);
            //$TODO: sparc64 deprecates this instruction.

            var dst = RewriteMemOp(instrCur.Operands[1], size);
            if (size.Size < src.DataType.Size)
            {
                src = m.Slice(size, src, 0);
            }
            m.Assign(dst, src);
        }

        private void RewriteStore(PrimitiveType size)
        {
            var src = RewriteOp(instrCur.Operands[0])!;
            var dst = RewriteMemOp(instrCur.Operands[1], size);
            if (size.Size < src.DataType.Size)
            {
                src = m.Slice(size, src, 0);
            }
            m.Assign(dst, src);
        }
    }
}
