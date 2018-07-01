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

using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Diagnostics;
using System.Linq;

namespace Reko.Arch.Arm.AArch32
{
    public partial class ArmRewriter
    {
        private void RewriteAdcSbc(Func<Expression, Expression, Expression> opr, bool reverse)
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var opSrc1 = this.Operand(Src1());
            var opSrc2 = this.Operand(Src2());
            if (reverse)
            {
                var tmp = opSrc1;
                opSrc1 = opSrc2;
                opSrc2 = tmp;
            }
            // We do not take the trouble of widening the CF to the word size
            // to simplify code analysis in later stages. 
            var c = binder.EnsureFlagGroup(Registers.cpsr, (uint)FlagM.CF, "C", PrimitiveType.Bool);
            m.Assign(
                opDst,
                opr(
                    opr(opSrc1, opSrc2),
                    c));
            MaybeUpdateFlags(opDst);
        }

        private void RewriteAddw()
        {
            var dst = Operand(Dst(), PrimitiveType.Word32, true);
            var src1 = Operand(Src1());
            var src2 = Operand(Src2());
            m.Assign(dst, m.IAdd(src1, src2));
        }

        static class Bits
        {
            public
                static UInt32 Mask32(int lsb, int bitsize)
            {
                return ((1u << bitsize) - 1) << lsb;
            }
        };

        private void RewriteAdr()
        {
            var dst = Operand(Dst(), PrimitiveType.Word32, true);
            var src = (AddressOperand)Src1();
            m.Assign(dst, src.Address);
        }

        private void RewriteBfc()
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var lsb = ((ImmediateOperand)instr.ops[1]).Value.ToInt32();
            var bitsize = ((ImmediateOperand)instr.ops[2]).Value.ToInt32();
            m.Assign(opDst, m.And(opDst, Constant.UInt32(~Bits.Mask32(lsb, bitsize))));
        }

        private void RewriteBfi()
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var opSrc = this.Operand(Src1());
            var tmp = binder.CreateTemporary(PrimitiveType.Word32);
            var lsb = ((ImmediateOperand)instr.ops[2]).Value.ToInt32();
            var bitsize = ((ImmediateOperand)instr.ops[3]).Value.ToInt32();
            m.Assign(tmp, m.Slice(opSrc, 0, bitsize));
            m.Assign(opDst, m.Dpb(opDst, tmp, lsb));
        }

        private void RewriteBinOp(Func<Expression, Expression, Expression> op)
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            if (instr.ops.Length > 2)
            {
                var src1 = this.Operand(Src1());
                var src2 = this.Operand(Src2());
                m.Assign(opDst, op(src1, src2));
            }
            else
            {
                var dst = Operand(Dst(), PrimitiveType.Word32, true);
                var src = Operand(Src1());
                m.Assign(dst, op(dst, src));
            }
            if (instr.UpdateFlags)
            {
                m.Assign(NZCV(), m.Cond(opDst));
            }
        }

        private void RewriteLogical(Func<Expression, Expression, Expression> cons)
        {
            var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
            if (instr.ops.Length > 2)
            {
                var opSrc1 = this.Operand(Src1());
                var opSrc2 = this.Operand(Src2());
                m.Assign(dst, cons(opSrc1, opSrc2));
            }
            else
            {
                var src = Operand(Src1());
                m.Assign(dst, cons(dst, src));
            }
            if (instr.UpdateFlags)
            {
                m.Assign(NZC(), m.Cond(dst));
            }
        }

        private void RewriteRev()
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var intrinsic = host.PseudoProcedure("__rev", PrimitiveType.Word32, this.Operand(Src1()));
            m.Assign(opDst, intrinsic);
        }

        private void RewriteRevBinOp(Func<Expression, Expression, Expression> op, bool setflags)
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            if (instr.ops.Length > 2)
            {
                var src1 = this.Operand(Src1());
                var src2 = this.Operand(Src2());
                m.Assign(opDst, op(src2, src1));
            }
            else
            {
                var dst = Operand(Dst(), PrimitiveType.Word32, true);
                var src = Operand(Src1());
                m.Assign(dst, op(dst, src));
            }
            if (setflags)
            {
                m.Assign(NZCV(), m.Cond(opDst));
            }
        }

        private void RewriteUnaryOp(Func<Expression, Expression> op)
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var opSrc = this.Operand(Src1());
            m.Assign(opDst, op(opSrc));
            if (instr.UpdateFlags)
            {
                m.Assign(NZCV(), m.Cond(opDst));
            }
        }

        private void RewriteBic()
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var opSrc1 = this.Operand(Src1());
            var opSrc2 = this.Operand(Src2());
            m.Assign(opDst, m.And(opSrc1, m.Comp(opSrc2)));
        }

        private void RewriteClz()
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var opSrc = this.Operand(Src1());
            var ppp = host.PseudoProcedure("__clz", PrimitiveType.Int32, opSrc);
            m.Assign(opDst, ppp);
        }

        private void RewriteCmp(Func<Expression, Expression, Expression> op)
        {
            var dst = Operand(Dst(), PrimitiveType.Word32, true);
            var src = Operand(Src1());
            var flags = FlagGroup((FlagM)(0x0F), "NZCV", PrimitiveType.Byte);
            m.Assign(flags, m.Cond(
                op(dst, src)));
        }

        private void RewriteDiv(Func<Expression, Expression, Expression> op)
        {
            var dst = Operand(Dst(), PrimitiveType.Word32, true);
            var src1 = Operand(Src1());
            var src2 = Operand(Src2());
            m.Assign(dst, op(src1, src2));
        }

        private void RewriteTeq()
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var opSrc = this.Operand(Src1());
            m.Assign(
                NZCV(),
                m.Cond(m.Xor(opDst, opSrc)));
        }

        private void RewriteTst()
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var opSrc = this.Operand(Src1());
            m.Assign(
                NZC(),
                m.Cond(m.And(opDst, opSrc)));
        }

        private void RewriteLdr(PrimitiveType dtDst, PrimitiveType dtSrc)
        {
            var mem = (MemoryOperand)Src1();
            var rDst = (RegisterOperand)instr.ops[0];
            bool isJump = rDst.Register == Registers.pc;

            Expression dst = Operand(rDst, PrimitiveType.Word32, true);
            Expression src;
            if (mem.PreIndex && instr.Writeback)
            {
                // Pre-index operand.
                Expression baseReg = Reg(mem.BaseRegister);
                Expression ea = EffectiveAddress(mem);
                m.Assign(baseReg, ea);
                src = m.Mem(dtSrc, baseReg);
            }
            else
            {
                src = Operand(Src1(), dtSrc);
            }
            if (dtDst != dtSrc)
            {
                src = m.Cast(dtDst, src);
            }
            if (!mem.PreIndex && instr.Writeback)
            {
                // Post-index operand.
                var tmp = binder.CreateTemporary(dtDst);
                m.Assign(tmp, src);
                var baseReg = binder.EnsureRegister(mem.BaseRegister);
                m.Assign(baseReg, m.IAdd(baseReg, mem.Offset));
                src = tmp;
            }
            if (isJump)
            {
                rtlClass = RtlClass.Transfer;
                m.Goto(src);
            }
            else
            {
                m.Assign(dst, src);
            }
        }



        private void RewriteLdrd()
        {
            var regLo = ((RegisterOperand)instr.ops[0]).Register;
            var regHi = ((RegisterOperand)instr.ops[1]).Register;
            var opDst = binder.EnsureSequence(regHi, regLo, PrimitiveType.Word64);
            var opSrc = this.Operand(instr.ops[2]);
            m.Assign(opDst, opSrc);
            MaybePostOperand(instr.ops[2]);
        }

        private void RewriteStr(PrimitiveType size)
        {
            var opSrc = this.Operand(Dst(), PrimitiveType.Word32, true);
            var opDst = this.Operand(Src1());
            if (size != PrimitiveType.Word32)
            {
                opSrc = m.Cast(size, opSrc);
            }
            m.Assign(opDst, opSrc);
            MaybePostOperand(Src1());
        }

        private void RewriteStrd()
        {
            var regLo = ((RegisterOperand)instr.ops[0]).Register;
            var regHi = ((RegisterOperand)instr.ops[1]).Register;
            var opSrc = binder.EnsureSequence(regHi, regLo, PrimitiveType.Word64);
            var opDst = this.Operand(instr.ops[2]);
            m.Assign(opDst, opSrc);
            MaybePostOperand(instr.ops[2]);
        }

        private void RewriteStrex()
        {
            var ppp = host.EnsurePseudoProcedure("__strex", VoidType.Instance, 0);
            m.SideEffect(m.Fn(ppp));
        }

        private void RewriteSubw()
        {
            var dst = Operand(Dst(), PrimitiveType.Word32, true);
            var src1 = Operand(Src1());
            var src2 = Operand(Src2());
            m.Assign(dst, m.ISub(src1, src2));
        }

        private void RewriteMultiplyAccumulate(Func<Expression, Expression, Expression> op)
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var opSrc1 = this.Operand(Src1());
            var opSrc2 = this.Operand(Src2());
            var opSrc3 = this.Operand(Src3());
            m.Assign(opDst, op(opSrc3, m.IMul(opSrc1, opSrc2)));
            if (instr.UpdateFlags)
            {
                m.Assign(NZCV(), m.Cond(opDst));
            }
        }

        private void RewriteHint()
        {
            var ppp = host.PseudoProcedure("__ldrex", VoidType.Instance, Operand(Dst()));
            m.SideEffect(m.Fn(ppp));
        }

        private void RewriteLdm(int initialOffset, Func<Expression, Expression, Expression> op)
        {
            var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
            RewriteLdm(dst, 1, initialOffset, op, instr.Writeback);
        }

        private void RewriteLdm(Expression dst, int skip, int offset, Func<Expression, Expression, Expression> op, bool writeback)
        {
            bool pcRestored = false;
            var mop = (MultiRegisterOperand)instr.ops[instr.ops.Length > 1 ? 1 : 0];
            foreach (var r in mop.GetRegisters())
            {
                Expression ea = offset != 0
                    ? op(dst, m.Int32(offset))
                    : dst;
                if (r == Registers.pc)
                {
                    pcRestored = true;
                }
                else
                {
                    var dstReg = Reg(r);
                    m.Assign(dstReg, m.Mem32(ea));
                }
                offset += 4;
            }
            if (writeback)
            {
                m.Assign(dst, m.IAdd(dst, m.Int32(offset)));
            }
            if (pcRestored)
            {
                rtlClass = instr.condition == ArmCondition.AL
                    ? RtlClass.Transfer
                    : RtlClass.ConditionalTransfer;
                m.Return(0, 0);
            }
        }

        private void RewriteLdrex()
        {
            var ppp = host.EnsurePseudoProcedure("__ldrex", VoidType.Instance, 0);
            m.SideEffect(m.Fn(ppp));
        }

        private void RewriteShift(Func<Expression, Expression, Expression> ctor)
        {
            var dst = Operand(Dst(), PrimitiveType.Word32, true);
            var src1 = Operand(Src1());
            var src2 = Operand(Src2());
            m.Assign(dst, ctor(src1, src2));
            if (instr.UpdateFlags)
            {
                m.Assign(this.NZC(), m.Cond(dst));
            }
        }

        private void RewriteMla(bool hiLeft, bool hiRight, PrimitiveType dt, Func<Expression, Expression, Expression> op)
        {
            var dst = Operand(Dst(), PrimitiveType.Word32, true);

            var left = Operand(Src1());
            left = hiLeft ? m.Sar(left, m.Int32(16)) : left;
            left = m.Cast(dt, left);

            var right = Operand(Src2());
            right = hiRight ? m.Sar(right, m.Int32(16)) : right;
            right = m.Cast(dt, right);

            m.Assign(dst, m.IAdd(op(left, right), Operand(Src3())));
            m.Assign(Q(), m.Cond(dst));
        }

        private void RewriteMov()
        {
            if (Dst() is RegisterOperand rOp && rOp.Register == Registers.pc)
            {
                rtlClass = RtlClass.Transfer;
                if (Src1() is RegisterOperand ropSrc && ropSrc.Register == Registers.lr)
                {
                    m.Return(0, 0);
                }
                else
                {
                    m.Goto(Operand(Src1()));
                }
                return;
            }
            var opDst = Operand(Dst(), PrimitiveType.Word32, true);
            var opSrc = Operand(Src1());
            m.Assign(opDst, opSrc);
        }

        private void RewriteMovt()
        {
            var opDst = Operand(Dst(), PrimitiveType.Word32, true);
            var iSrc = ((ImmediateOperand)Src1()).Value;
            Debug.Assert(iSrc.DataType.BitSize == 16);
            var opSrc = m.Dpb(opDst, iSrc, 16);
            m.Assign(opDst, opSrc);
        }

        private void RewriteMovw()
        {
            var dst = Operand(Dst(), PrimitiveType.Word32, true);
            var src = m.Word32(((ImmediateOperand)Src1()).Value.ToUInt32());
            m.Assign(dst, src);
        }

        private void RewriteMulbb(bool hiLeft, bool hiRight, PrimitiveType dt, Func<Expression, Expression, Expression> mul)
        {
            var dst = Operand(Dst(), PrimitiveType.Word32, true);

            var left = Operand(Src1());
            left = hiLeft ? m.Sar(left, m.Int32(16)) : left;
            left = m.Cast(dt, left);

            var right = Operand(Src2());
            right = hiRight ? m.Sar(right, m.Int32(16)) : right;
            right = m.Cast(dt, right);

            m.Assign(dst, mul(left, right));
        }

        private void RewriteMull(PrimitiveType dtResult, Func<Expression, Expression, Expression> op)
        {
            var regLo = ((RegisterOperand)instr.ops[0]).Register;
            var regHi = ((RegisterOperand)instr.ops[1]).Register;

            var opDst = binder.EnsureSequence(regHi, regLo, dtResult);
            var opSrc1 = this.Operand(Src3());
            var opSrc2 = this.Operand(Src2());
            m.Assign(opDst, op(opSrc1, opSrc2));
            if (instr.UpdateFlags)
            {
                m.Assign(NZCV(), m.Cond(opDst));
            }
        }

        private void RewritePop()
        {
            var sp = Reg(Registers.sp);
            RewriteLdm(sp, 0, 0, m.IAdd, true);
        }

        private void RewritePush()
        {
            Expression dst = Reg(Registers.sp);
            var regs = ((MultiRegisterOperand)instr.ops[0]).GetRegisters().ToArray();
            m.Assign(dst, m.ISub(dst, m.Int32(regs.Length * 4)));

            int offset = 0;
            foreach (var reg in regs)
            { 
                var ea = offset != 0
                    ? m.IAdd(dst, m.Int32(offset))
                    : dst;
                m.Assign(m.Mem32(ea), Reg(reg));
                offset += 4;
            }
        }

        private void RewriteQAddSub(Func<Expression, Expression, Expression> op)
        {
            var dst = Operand(Dst(), PrimitiveType.Word32, true);
            var src1 = Operand(Src1());
            var src2 = Operand(Src2());
            var sum = op(src1, src2);
            var sat = host.PseudoProcedure("__signed_sat_32", PrimitiveType.Int32, sum);
            m.Assign(dst, m.Fn(sat));
            m.Assign(
                Q(),
                m.Cond(dst));
        }

        private void RewriteQDAddSub(Func<Expression, Expression, Expression> op)
        {
            var dst = Operand(Dst(), PrimitiveType.Word32, true);
            var src1 = m.SMul(Operand(Src1()), m.Int32(2));

            var sat = host.PseudoProcedure("__signed_sat_32", PrimitiveType.Int32, src1);
            src1 = m.Fn(sat);
            var src2 = Operand(Src2());
            var sum = op(src2, src1);
            m.Assign(dst, m.Fn(sat, sum));
            m.Assign(
                binder.EnsureFlagGroup(Registers.cpsr, 0x10, "Q", PrimitiveType.Bool),
                m.Cond(dst));
        }


        private void RewriteSbfx()
        {
            var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var src = m.Cast(
                PrimitiveType.Int32,
                m.Slice(
                    this.Operand(Src1()),
                    ((ImmediateOperand)Src2()).Value.ToInt32(),
                    ((ImmediateOperand)Src3()).Value.ToInt32()));
            m.Assign(dst, src);
        }


        private void RewriteSmlal()
        {
            var r1 = ((RegisterOperand)instr.ops[0]).Register;
            var r2 = ((RegisterOperand)instr.ops[1]).Register;
            var dst = binder.EnsureSequence(r1, r2, PrimitiveType.Int64);
            var fac1 = Operand(Src2());
            var fac2 = Operand(Src3());
            m.Assign(dst, m.IAdd(m.SMul(fac1, fac2), dst));
        }

        private void RewriteMlal(bool hiLeft, bool hiRight, PrimitiveType dt, Func<Expression, Expression, Expression> op)
        {
            var r1 = ((RegisterOperand)instr.ops[0]).Register;
            var r2 = ((RegisterOperand)instr.ops[1]).Register;
            var dst = binder.EnsureSequence(r1, r2, PrimitiveType.Int64);

            var left = Operand(Src2());
            left = hiLeft ? m.Sar(left, m.Int32(16)) : left;
            left = m.Cast(dt, left);

            var right = Operand(Src3());
            right = hiRight ? m.Sar(right, m.Int32(16)) : right;
            right = m.Cast(dt, right);

            m.Assign(dst, m.IAdd(op(left, right), dst));
        }

        private void RewriteMlxd(bool swap, PrimitiveType dt, Func<Expression, Expression, Expression> mul, Func<Expression, Expression, Expression> addSub)
        {
            // The ARM manual states that the double return value is in [op2,op1]
            var r1 = ((RegisterOperand)instr.ops[1]).Register;
            var r2 = ((RegisterOperand)instr.ops[0]).Register;
            var dst = binder.EnsureSequence(r1, r2, PrimitiveType.Int64);

            var left = Operand(Src2());
            var right = Operand(Src3());

            var product1 = mul(
                m.Cast(dt, left),
                swap ? m.Sar(right, m.Int32(16)) : (Expression)m.Cast(dt, right));
            var product2 = mul(
                m.Sar(left, m.Int32(16)),
                swap ? m.Cast(dt, right) : (Expression)m.Sar(right, m.Int32(16)));

            m.Assign(dst, m.IAdd(dst, addSub(product1, product2)));
        }

        private void RewriteMxd(bool swap, PrimitiveType dt, Func<Expression, Expression, Expression> mul, Func<Expression, Expression, Expression> addSub)
        {
            var dst = Operand(instr.ops[0]);
            var left = Operand(Src1());
            var right = Operand(Src2());

            var product1 = mul(
                m.Cast(dt, left),
                swap ? m.Sar(right, m.Int32(16)) : (Expression)m.Cast(dt, right));
            var product2 = mul(
                m.Sar(left, m.Int32(16)),
                swap ? m.Cast(dt, right) : (Expression)m.Sar(right, m.Int32(16)));

            m.Assign(dst, m.IAdd(dst, addSub(product1, product2)));
        }

        private void RewriteSmlaw(bool highPart)
        {
            var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var fac1 = this.Operand(Src1());
            var fac2 = this.Operand(Src2());
            fac2 = m.Cast(PrimitiveType.Int16, highPart ? m.Sar(fac2, m.Int32(16)) : fac2);

            var acc = this.Operand(Src3());
            m.Assign(dst, m.IAdd(
                m.Sar(
                    m.SMul(fac1, fac2),
                    m.Int32(16)),
                acc));
        }

        private void RewriteMulw(bool highPart)
        {
            var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var fac1 = this.Operand(Src1());
            var fac2 = this.Operand(Src2());
            fac2 = m.Cast(PrimitiveType.Int16, highPart ? m.Sar(fac2, m.Int32(16)) : fac2);
            m.Assign(dst, m.Sar(
                m.SMul(fac1, fac2),
                m.Int32(16)));
        }


        private void RewriteStm(int offset, bool inc)
        {
            var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var mul = (MultiRegisterOperand)Src1();
            var increment = inc ? 4 : -4;
            foreach (var r in mul.GetRegisters())
            {
                var ea = offset > 0
                    ? m.IAdd(dst, m.Int32(offset))
                    : offset < 0
                    ? m.ISub(dst, m.Int32(Math.Abs(offset)))
                    : dst;
                var srcReg = Reg(r);
                m.Assign(m.Mem32(ea), srcReg);
                offset += increment;
            }
            if (instr.Writeback)
            {
                if (offset > 0)
                {
                    m.Assign(dst, m.IAdd(dst, m.Int32(offset)));
                }
                else if (offset < 0)
                {
                    m.Assign(dst, m.ISub(dst, m.Int32(Math.Abs(offset))));
                }
            }
        }

        private void RewriteStmib()
        {
            throw new NotImplementedException();
            /*
	var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
	var begin = &instr.detail.arm.operands[1];	// Skip the dst register
	var end = begin + instr.detail.arm.op_count - 1;
	int offset = 4;
	for (var r = begin; r != end; ++r)
	{
		var ea = m.IAdd(dst, m.Int32(offset));
		var srcReg = Reg(r.reg);
		m.Assign(m.Mem32(ea), srcReg);
		offset += 4;
	}
	if (offset != 4 && instr.detail.arm.writeback)
	{
		m.Assign(dst, m.IAdd(dst, m.Int32(offset)));
	}
    */
        }

        private void RewriteUbfx()
        {
            var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var src = m.Cast(
                PrimitiveType.UInt32,
                m.Slice(
                    this.Operand(Src1()),
                    ((ImmediateOperand)Src2()).Value.ToInt32(),
                    ((ImmediateOperand)Src3()).Value.ToInt32()));
            m.Assign(dst, src);
        }

        private void RewriteUmaal()
        {
            var tmp = binder.CreateTemporary(PrimitiveType.UInt64);
            var lo = Operand(Dst(), PrimitiveType.Word32, true);
            var hi = Operand(Src1());
            var rn = Operand(Src2());
            var rm = Operand(Src3());
            var dst = binder.EnsureSequence(
                ((RegisterOperand)Src1()).Register,
                ((RegisterOperand)Dst()).Register,
                PrimitiveType.UInt64);
            m.Assign(tmp, m.UMul(rn, rm));
            m.Assign(tmp, m.IAdd(tmp, m.Cast(PrimitiveType.UInt64, hi)));
            m.Assign(dst, m.IAdd(tmp, m.Cast(PrimitiveType.UInt64, lo)));
        }

        private void RewriteUmlal()
        {
            var dst = binder.EnsureSequence(
                ((RegisterOperand)Src1()).Register,
                ((RegisterOperand)Dst()).Register,
                PrimitiveType.Word64);
            var left = this.Operand(Src2());
            var right = this.Operand(Src3());
            m.Assign(dst, m.IAdd(m.UMul(left, right), dst));
            MaybeUpdateFlags(dst);
        }

        private void RewriteXtab(PrimitiveType dt)
        {
            var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
            Expression src = Reg(((RegisterOperand)Src2()).Register);
            if (instr.ShiftType == Opcode.ror)
            {
                src = m.Shr(src, Operand(instr.ShiftValue));
            }
            src = m.Cast(dt, src);
            m.Assign(dst, m.IAdd(this.Operand(Src1()), src));
        }

        private void RewriteXtb(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
            Expression src = Reg(((RegisterOperand)Src1()).Register);
            if (instr.ShiftType == Opcode.ror)
            {
                src = m.Shr(src,Operand(instr.ShiftValue));
            }
            src = m.Cast(dtSrc, src);
            src = m.Cast(dtDst, src);
            m.Assign(dst, src);
        }

        private void RewriteYield()
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var ppp = host.EnsurePseudoProcedure("__yield", PrimitiveType.Word32, 0);
            m.Assign(opDst, m.Fn(ppp));
        }
    }
}

