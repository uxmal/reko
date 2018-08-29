#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm.AArch64
{
    public partial class A64Rewriter
    {
        private void RewriteAdrp()
        {
            var dst = RewriteOp(instr.ops[0]);
            var imm = ((ImmediateOperand)instr.ops[1]).Value;
            var wBase = instr.Address.ToLinear();
            wBase &= ~0xFFFul;        // Mask out lowest 12 bits.
            wBase = (ulong)((long)wBase + imm.ToInt64());
            m.Assign(dst, Address.Ptr64(wBase));
        }

        private void RewriteMaybeSimdBinary(
            Func<Expression, Expression, Expression> fn,
            string simdFormat, 
            Domain domain = Domain.None, 
            Action<Expression> setFlags = null)
        {
            if (instr.vectorData != VectorData.Invalid || instr.ops[0] is VectorRegisterOperand vr)
            {
                RewriteSimdBinary(simdFormat, domain, setFlags);
            }
            else
            {
                RewriteBinary(fn, setFlags);
            }
        }

        private void RewriteMaybeSimdUnary(
            Func<Expression, Expression> fn,
            string simdFormat,
            Domain domain = Domain.None)
        {
            if (instr.vectorData != VectorData.Invalid || 
                (instr.ops[0] is VectorRegisterOperand vr && vr.Index < 0))
            {
                RewriteSimdUnary(simdFormat, domain);
            }
            else
            {
                RewriteUnary(fn);
            }
        }


        private void RewriteBinary(Func<Expression, Expression, Expression> fn, Action<Expression> setFlags = null)
        {
            var dst = RewriteOp(instr.ops[0]);
            var left = RewriteOp(instr.ops[1]);
            var right = RewriteOp(instr.ops[2]);

            var toBitSize = left.DataType.BitSize;
            right = MaybeExtendExpression(right, toBitSize);
            m.Assign(dst, fn(left, right));
            setFlags?.Invoke(m.Cond(dst));
        }

        private Expression MaybeExtendExpression(Expression right, int toBitSize)
        {
            if (instr.shiftCode != Opcode.Invalid &&
                (instr.shiftCode != Opcode.lsl ||
                !(instr.shiftAmount is ImmediateOperand imm) ||
                !imm.Value.IsIntegerZero))
            {
                var amt = RewriteOp(instr.shiftAmount);
                switch (instr.shiftCode)
                {
                case Opcode.lsl: right = m.Shl(right, amt); break;
                case Opcode.lsr: right = m.Shr(right, amt); break;
                case Opcode.sxtb: right = SignExtend(toBitSize, PrimitiveType.SByte, right); break;
                case Opcode.sxth: right = SignExtend(toBitSize, PrimitiveType.Int16, right); break;
                case Opcode.sxtw: right = SignExtend(toBitSize, PrimitiveType.Int32, right); break;
                case Opcode.uxtb: right = ZeroExtend(toBitSize, PrimitiveType.Byte, right); break;
                case Opcode.uxth: right = ZeroExtend(toBitSize, PrimitiveType.Word16, right); break;
                case Opcode.uxtw: right = ZeroExtend(toBitSize, PrimitiveType.Word32, right); break;
                default:
                    EmitUnitTest();
                    break;
                }
            }

            return right;
        }

        private void RewriteCcmp()
        {
            var nzcv = NZCV();
            var tmp = binder.CreateTemporary(PrimitiveType.Bool);
            var cond = Invert(((ConditionOperand)instr.ops[3]).Condition);
            m.Assign(tmp, this.TestCond(cond));
            m.Assign(nzcv, RewriteOp(instr.ops[2]));
            m.BranchInMiddleOfInstruction(tmp, instr.Address + instr.Length, RtlClass.ConditionalTransfer);
            var left = RewriteOp(instr.ops[0]);
            var right = RewriteOp(instr.ops[1]);
            m.Assign(nzcv, m.Cond(m.ISub(left, right)));
        }

        private void RewriteClz()
        {
            var src = RewriteOp(instr.ops[1]);
            var dst = RewriteOp(instr.ops[0]);
            m.Assign(dst, host.PseudoProcedure("__clz", MakeInteger(Domain.SignedInt, dst.DataType), src));
        }

        private void RewriteCmp()
        {
            var left = RewriteOp(instr.ops[0]);
            var right = RewriteOp(instr.ops[1]);
            right = MaybeExtendExpression(right, left.DataType.BitSize);
            var nzcv = NZCV();
            m.Assign(nzcv, m.Cond(m.ISub(left, right)));
        }

        private void RewriteCsinc()
        {
            var dst = RewriteOp(instr.ops[0]);
            var rTrue = ((RegisterOperand)instr.ops[1]).Register;
            var rFalse = ((RegisterOperand)instr.ops[2]).Register;
            var cond = ((ConditionOperand)instr.ops[3]).Condition;
            if (rTrue.Number == 31 && rFalse.Number == 31)
            {
                m.Assign(dst, m.Cast(dst.DataType, TestCond(Invert(cond))));
                return;
            }
            var src = RewriteOp(instr.ops[1]);
            if (rFalse.Number != 31 && rTrue == rFalse)
            {
                m.BranchInMiddleOfInstruction(TestCond(Invert(cond)), instr.Address + instr.Length, RtlClass.ConditionalTransfer);
                m.Assign(dst, m.IAdd(src, 1));
                return;
            }
            NotImplementedYet();
        }

        private void RewriteLoadStorePair(bool load)
        {
            var reg1 = RewriteOp(instr.ops[0], !load);
            var reg2 = RewriteOp(instr.ops[1], !load);
            var mem = (MemoryOperand)instr.ops[2];
            Expression regBase = binder.EnsureRegister(mem.Base);
            Expression offset = RewriteEffectiveAddressOffset(mem);
            Expression ea = regBase;
            if (mem.PreIndex)
            {
                m.Assign(ea, m.IAdd(ea, offset));
            }
            else
            {
                var tmp = binder.CreateTemporary(ea.DataType);
                if (offset == null || mem.PostIndex)
                    //$DEBUG
                {
                    m.Assign(tmp, ea);
                }
                else
                {
                    m.Assign(tmp, m.IAdd(ea, offset));
                }
                ea = tmp;
            }
            if (load)
                m.Assign(reg1, m.Mem(reg1.DataType, ea));
            else
                m.Assign(m.Mem(reg1.DataType, ea), reg1);
            m.Assign(ea, m.IAdd(ea, Constant.Int(reg1.DataType, reg1.DataType.Size)));
            if (load)
                m.Assign(reg2, m.Mem(reg2.DataType, ea));
            else
                m.Assign(m.Mem(reg2.DataType, ea), reg2);
            if (mem.PostIndex && offset != null)
            {
                m.Assign(regBase, m.IAdd(regBase, offset));
            }
        }

        private void RewriteLdr(DataType dt)
        {
            var dst = RewriteOp(instr.ops[0]);
            Expression ea;
            MemoryOperand mem = null;
            Identifier baseReg = null;
            Expression postIndex = null;
            if (instr.ops[1] is AddressOperand aOp)
            {
                ea = aOp.Address;
            }
            else
            {
                mem = (MemoryOperand)instr.ops[1];
                (ea, baseReg) = RewriteEffectiveAddress(mem);
                if (mem.PreIndex)
                {
                    m.Assign(baseReg, ea);
                    ea = baseReg;
                } else if (mem.PostIndex)
                {
                    postIndex = ea;
                    ea = baseReg;
                }
            }
            if (dt == null)
            {
                m.Assign(dst, m.Mem(dst.DataType, ea));
            }
            else
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, m.Mem(dt, ea));
                m.Assign(dst, m.Cast(dst.DataType, tmp));
            }
            if (postIndex != null)
            {
                m.Assign(baseReg, postIndex);
            }
        }

        private void RewriteMaddSub(Func<Expression, Expression, Expression> op)
        {
            var op1 = RewriteOp(instr.ops[1]);
            var op2 = RewriteOp(instr.ops[2]);
            var op3 = RewriteOp(instr.ops[3]);
            var dst = RewriteOp(instr.ops[0]);

            m.Assign(dst, op(op3, m.IMul(op1, op2)));
        }

        private void RewriteMaddl(PrimitiveType dt, Func<Expression, Expression, Expression> mul)
        {
            var op1 = RewriteOp(instr.ops[1]);
            var op2 = RewriteOp(instr.ops[2]);
            var op3 = RewriteOp(instr.ops[3]);
            var dst = RewriteOp(instr.ops[0]);

            m.Assign(dst, m.IAdd(op3, m.Cast(dt, mul(op1, op2))));
        }

        private void RewriteMov()
        {
            RewriteUnary(n => n);
        }

        private void RewriteMovk()
        {
            var dst = RewriteOp(instr.ops[0]);
            var imm = ((ImmediateOperand)instr.ops[1]).Value;
            var shift = ((ImmediateOperand)instr.shiftAmount).Value;
            m.Assign(dst, m.Dpb(dst, imm, shift.ToInt32()));
        }

        private void RewriteMovn()
        {
            var src = RewriteOp(instr.ops[1]);
            var dst = RewriteOp(instr.ops[0]);
            if (src is Constant c)
            {
                src = c.Complement();
            }
            else
            {
                src = m.Comp(src);
            }
            m.Assign(dst, src);
        }

        private void RewriteMovz()
        {
            var dst = RewriteOp(instr.ops[0]);
            var imm = ((ImmediateOperand)instr.ops[1]).Value;
            var shift = ((ImmediateOperand)instr.shiftAmount).Value;
            m.Assign(dst, Constant.Word(dst.DataType.BitSize, imm.ToInt64() << shift.ToInt32()));
        }

        private void RewriteMulh(PrimitiveType dt, Func<Expression, Expression, Expression> mul)
        {
            var op1 = RewriteOp(instr.ops[1]);
            var op2 = RewriteOp(instr.ops[2]);
            var dst = RewriteOp(instr.ops[0]);

            m.Assign(dst, m.Slice(dt, mul(op1, op2), 64));
        }

        private void RewriteMull(PrimitiveType dt, Func<Expression, Expression, Expression> mul)
        {
            if (instr.ops[1] is VectorRegisterOperand)
            {
                RewriteSimdBinary("__mull_{0}", Domain.Integer);
                return;
            }
            var op1 = RewriteOp(instr.ops[1]);
            var op2 = RewriteOp(instr.ops[2]);
            var dst = RewriteOp(instr.ops[0]);

            m.Assign(dst, m.Cast(dt, mul(op1, op2)));
        }

        private (Expression, Identifier) RewriteEffectiveAddress(MemoryOperand mem)
        {
            Identifier baseReg = binder.EnsureRegister(mem.Base);
            Expression ea = baseReg;
            Expression offset = RewriteEffectiveAddressOffset(mem);
            if (offset != null)
            {
                ea = m.IAdd(ea, offset);
            }
            return (ea, baseReg);
        }

        private Expression RewriteEffectiveAddressOffset(MemoryOperand mem)
        { 
            if (mem.Offset != null && !mem.Offset.IsIntegerZero)
            {
                return Constant.Int(mem.Base.DataType, mem.Offset.ToInt32());
            }
            else if (mem.Index != null)
            {
                Expression idx = binder.EnsureRegister(mem.Index);
                switch (mem.IndexExtend)
                {
                case Opcode.lsl:
                    if (mem.IndexShift != 0)
                    {
                        var dtInt = PrimitiveType.Create(Domain.SignedInt, idx.DataType.BitSize);
                        idx = m.IMul(idx, Constant.Create(dtInt, 1 << mem.IndexShift));
                    }
                    break;
                case Opcode.sxtb:
                    idx = SignExtend(64, PrimitiveType.SByte, idx);
                    break;
                case Opcode.sxth:
                    idx = SignExtend(64, PrimitiveType.Int16, idx);
                    break;
                case Opcode.sxtw:
                    idx = SignExtend(64, PrimitiveType.Int32, idx);
                    break;
                case Opcode.uxtb:
                    idx = ZeroExtend(64, PrimitiveType.UInt8, idx);
                    break;
                case Opcode.uxth:
                    idx = ZeroExtend(64, PrimitiveType.UInt16, idx);
                    break;
                case Opcode.uxtw:
                    idx = ZeroExtend(64, PrimitiveType.UInt32, idx);
                    break;
                default:
                    throw new NotImplementedException($"Register extension {mem.IndexExtend} not implemented yet.");
                }
                return idx;
            }
            return null;
        }

        private Expression ZeroExtend(int bitsizeDst, PrimitiveType dtOrig, Expression e)
        {
            var dtUint = PrimitiveType.Create(Domain.UnsignedInt, bitsizeDst);
            return m.Cast(dtUint, m.Cast(dtOrig, e));
        }

        private Expression SignExtend(int bitsizeDst, PrimitiveType dtOrig, Expression e)
        {
            var dtUint = PrimitiveType.Create(Domain.SignedInt, bitsizeDst);
            return m.Cast(dtUint, m.Cast(dtOrig, e));
        }

        private void RewriteRev16()
        {
            RewriteMaybeSimdUnary(
                n => host.PseudoProcedure("__rev16", n.DataType, n),
                "__rev16_{0}");
        }

        private void RewriteSbfiz()
        {
            var src1 = RewriteOp(instr.ops[1], true);
            var src2 = RewriteOp(instr.ops[2], true);
            var dst = RewriteOp(instr.ops[0]);
            m.Assign(dst, host.PseudoProcedure("__sbfiz", dst.DataType, src1, src2));
        }

        private void RewriteSbfm()
        {
            var src1 = RewriteOp(instr.ops[1], true);
            var src2 = RewriteOp(instr.ops[2], true);
            var src3 = RewriteOp(instr.ops[2], true);
            var dst = RewriteOp(instr.ops[0]);
            m.Assign(dst, host.PseudoProcedure("__sbfm", dst.DataType, src1, src2, src3));
        }

        private void RewriteStr(PrimitiveType dt)
        {
            var rSrc = (RegisterOperand)instr.ops[0];
            Expression src = MaybeZeroRegister(rSrc.Register, dt ?? rSrc.Width);
            var mem = (MemoryOperand)instr.ops[1];
            var (ea, baseReg) = RewriteEffectiveAddress(mem);
            Expression postIndex = null;
            if (mem.PreIndex)
            {
                m.Assign(baseReg, ea);
                ea = baseReg;
            }
            else if (mem.PostIndex)
            {
                postIndex = ea;
                ea = baseReg;
            }
            if (dt == null || src is Constant)
            {
                m.Assign(m.Mem(src.DataType, ea), src);
            }
            else
            {
                m.Assign(m.Mem(dt, ea), m.Cast(dt, src));
            }
            if (postIndex != null)
            {
                m.Assign(baseReg, postIndex);
            }
        }

        private void RewriteTest()
        {
            var op1 = RewriteOp(instr.ops[0], true);
            var op2 = RewriteOp(instr.ops[1], true);
            NZ00(m.Cond(m.And(op1, op2)));
        }

        private void RewriteUnary(Func<Expression, Expression> fn)
        {
            var src = RewriteOp(instr.ops[1], true);
            var dst = RewriteOp(instr.ops[0]);
            m.Assign(dst, fn(src));
        }

        private void RewriteUSxt(PrimitiveType dtDst, PrimitiveType dtSrc)
        {

        }

    }
}
