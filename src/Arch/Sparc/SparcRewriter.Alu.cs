#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;

namespace Reko.Arch.Sparc
{
    public partial class SparcRewriter
    {
        private void RewriteAddxSubx(Func<Expression,Expression,Expression> op, bool emitCc)
        {
            var dst = RewriteRegister(2);
            var src1 = RewriteOp(0);
            var src2 = RewriteOp(1);
            var C = binder.EnsureFlagGroup(arch.Registers.C);
            m.Assign(
                dst,
                op(op(src1, src2), C));
            if (emitCc)
            {
                EmitCc(arch.Registers.NZVC, arch.Registers.xNZVC, dst);
            }
        }

        private void RewriteAlu(Func<Expression,Expression,Expression> op, bool negateOp2)
        {
            var dst = RewriteRegister(2);
            var src1 = RewriteOp(0);
            var src2 = RewriteOp(1);
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
            EmitCc(arch.Registers.NZVC, arch.Registers.xNZVC, dst);

        }

        private void RewriteBinaryCc(IntrinsicProcedure intrinsic)
        {
            var src1 = RewriteOp(0);
            var src2 = RewriteOp(1);
            var dst = RewriteOp(2);
            m.Assign(dst, m.Fn(intrinsic.MakeInstance(src1.DataType), src1, src2));
            EmitCc(arch.Registers.NZVC, arch.Registers.xNZVC, dst);
        }

        private void RewriteLdd()
        {
            var size = PrimitiveType.Word64;
            var rDstHi = (RegisterStorage) instrCur.Operands[1];
            var rDstLo = arch.Registers.GetRegister((uint)rDstHi.Number + 1);
            var dst = binder.EnsureSequence(size, rDstHi, rDstLo);
            var src = RewriteMemOp(instrCur.Operands[0], size);
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
            var mem = (MemoryAccess)RewriteOp(0);
            var dst = RewriteOp(1);
            var bTmp = binder.CreateTemporary(PrimitiveType.Byte);
            var tmpHi = binder.CreateTemporary(PrimitiveType.CreateWord(dst.DataType.BitSize - bTmp.DataType.BitSize));
            m.Assign(bTmp, m.Fn(
                ldstub_intrinsic.MakeInstance(arch.PointerType.BitSize, bTmp.DataType),
                m.AddrOf(arch.PointerType, mem)));
            m.Assign(tmpHi, m.Slice(dst, tmpHi.DataType, bTmp.DataType.BitSize));
            m.Assign(dst, m.Seq(tmpHi, bTmp));
        }

        private void RewriteLoad(PrimitiveType size)
        {
            var dst = RewriteOp(instrCur.Operands[1]);
            var src = RewriteMemOp(instrCur.Operands[0], size);
            if (size.Size < dst.DataType.Size)
            {
                size = PrimitiveType.Create(size.Domain, dst.DataType.BitSize);
                src = m.Convert(src, src.DataType, size);
            }
            m.Assign(dst, src);
        }

        private void RewriteLoada(PrimitiveType size)
        {
            var dst = RewriteOp(instrCur.Operands[1]);
            var src = RewriteMemOp(instrCur.Operands[0], size);
            src = m.Fn(
                loada_intrinsic.MakeInstance(arch.PointerType.BitSize, size), 
                m.AddrOf(arch.PointerType, src));
            if (size.Size < dst.DataType.Size)
            {
                size = PrimitiveType.Create(size.Domain, dst.DataType.BitSize);
                var tmp = binder.CreateTemporary(src.DataType);
                m.Assign(tmp, src);
                src = m.Convert(tmp, src.DataType, size);
            }
            m.Assign(dst, src);
        }

        private void RewriteLogicalCc(Func<Expression, Expression, Expression> op, bool negateOp2)
        {
            RewriteAlu(op, negateOp2);
            var dst = RewriteRegister(2);
            EmitCc(arch.Registers.NZ, arch.Registers.xNZ, dst);
            EmitCc(arch.Registers.V, 0);
            EmitCc(arch.Registers.C, 0);
        }

        private void RewriteMovcc(ConditionCode cc, FlagGroupStorage grf32, FlagGroupStorage grf64)
        {
            var consequent = RewriteOp(1);
            var dst = RewriteOp(2);
            if (cc == ConditionCode.ALWAYS)
            {
                m.Assign(dst, consequent);
                return;
            }
            else if (cc == ConditionCode.NEVER)
            {
                m.Assign(dst, dst);
                return;
            }
            var ccField = ((ConditionCodeOperand) instrCur.Operands[0]).Field;
            FlagGroupStorage grf;
            switch (ccField)
            {
            case ConditionField.icc: grf = grf32; break;
            case ConditionField.xcc: grf = grf64; break;
            default: 
                EmitUnitTest();
                iclass = InstrClass.Invalid; m.Invalid();
                return;
            }
            var test = m.Test(cc, binder.EnsureFlagGroup(grf));
            m.Assign(dst, m.Conditional(dst.DataType, test, consequent, dst));
        }

        private void RewriteMovfcc()
        {
            EmitUnitTest();
            iclass = InstrClass.Invalid; m.Invalid();
            return;
        }

        private void RewriteMulscc()
        {
            var dst = RewriteOp(2);
            var src1 = RewriteOp(0);
            var src2 = RewriteOp(1);
            m.Assign(
                dst,
                m.Fn(mulscc_intrinsic.MakeInstance(PrimitiveType.Int32), src1, src2));
            EmitCc(arch.Registers.NZVC, arch.Registers.xNZVC, dst);
        }

        private void RewriteRestore()
        {
            var dst = RewriteOp(instrCur.Operands[2]);
            var src1 = RewriteOp(instrCur.Operands[0]);
            var src2 = RewriteOp(instrCur.Operands[1]);
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
            var sp = binder.EnsureRegister(arch.StackRegister);
            var tmpSp = binder.CreateTemporary(sp.DataType);
            m.Assign(tmpSp, sp);

            // Now perform the frame window shift. Place the values of i0..i7
            // in o0..07
            for (int i = 0; i < arch.Registers.InRegisters.Length; ++i)
            {
                Copy(arch.Registers.InRegisters[i], arch.Registers.OutRegisters[i]);
            }

            // Restore registers previously saved on stack.
            // This doesn't actually happen immediately on Sparc,
            // but we have to simulate the restoration of the i0-i7
            // and l0-l7 registers
            int stackOffset = 0;
            for (int i = 0; i < arch.Registers.InRegisters.Length; ++i)
            {
                var reg = binder.EnsureRegister(arch.Registers.InRegisters[i]);
                m.Assign(
                    reg,
                    m.Mem(reg.DataType, m.AddSubSignedInt(tmpSp, stackOffset)));
                stackOffset += reg.DataType.Size;
            }
            for (int i = 0; i < arch.Registers.LocalRegisters.Length; ++i)
            {
                var reg = binder.EnsureRegister(arch.Registers.LocalRegisters[i]);
                m.Assign(
                    reg,
                    m.Mem(reg.DataType, m.AddSubSignedInt(tmpSp, stackOffset)));
                stackOffset += reg.DataType.Size;
            }

            if (tmp is not null)
            {
                m.Assign(dst, tmp);
            }
        }

        private void RewriteSave()
        {
            var dst = RewriteOp(2);
            var src1 = RewriteOp(0);
            var src2 = RewriteOp(1);
            Identifier? tmp = null;
            if (dst is Identifier)
            {
                tmp = binder.CreateTemporary(dst.DataType);
                m.Assign(tmp, m.IAdd(src1, src2));
            }
            // Save registers on stack. This doesn't actually happen
            // immediately on Sparc, but we have to simulate the 
            // preservation of the i0-i7 and l0-l7 registers
            int stackOffset = 0;
            var sp = instrCur.Operands[2] == arch.StackRegister
                ? tmp!
                : binder.EnsureRegister(arch.StackRegister);
            for (int i = 0; i < arch.Registers.InRegisters.Length; ++i)
            {
                var reg = binder.EnsureRegister(arch.Registers.InRegisters[i]);
                m.Assign(
                    m.Mem(reg.DataType, m.AddSubSignedInt(sp, stackOffset)),
                    reg);
                stackOffset += reg.DataType.Size;
            }
            for (int i = 0; i < arch.Registers.LocalRegisters.Length; ++i)
            {
                var reg = binder.EnsureRegister(arch.Registers.LocalRegisters[i]);
                m.Assign(
                    m.Mem(reg.DataType, m.AddSubSignedInt(sp, stackOffset)),
                    reg);
                stackOffset += reg.DataType.Size;
            }
            // Now perform the frame window shift. The previous values
            // in i0..i7 have been preserved by the stack stores above.
            for (int i = 0; i < arch.Registers.InRegisters.Length; ++i)
            {
                Copy(arch.Registers.OutRegisters[i], arch.Registers.InRegisters[i]);
            }
            if (tmp is not null)
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
                var src = (Constant)instrCur.Operands[0];
                m.Assign(dst, Constant.Word32(src.ToUInt32() << 10));
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
                src = m.Slice(src, size);
            }
            m.Assign(dst, src);
        }

        private void RewriteStore(PrimitiveType size)
        {
            var src = RewriteOp(instrCur.Operands[0]);
            var dst = RewriteMemOp(instrCur.Operands[1], size);
            if (size.Size < src.DataType.Size)
            {
                src = m.Slice(src, size);
            }
            m.Assign(dst, src);
        }

        private void RewriteSwap(IntrinsicProcedure intrinsic)
        {
            var src = RewriteMemOp(instrCur.Operands[0], instrCur.Operands[0].DataType);
            var dst = RewriteOp(instrCur.Operands[1]);
            m.Assign(dst, m.Fn(
                intrinsic.MakeInstance(arch.PointerType.BitSize, dst.DataType),
                dst, 
                m.AddrOf(arch.PointerType, src)));
        }

        private static IntrinsicProcedure ldstub_intrinsic = new IntrinsicBuilder("__ldstub", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns("T");
        private static readonly IntrinsicProcedure mulscc_intrinsic = IntrinsicBuilder.GenericBinary("__mulscc");
        private static readonly IntrinsicProcedure taddcc_intrinsic = IntrinsicBuilder.GenericBinary("__taddcc");
    }
}
