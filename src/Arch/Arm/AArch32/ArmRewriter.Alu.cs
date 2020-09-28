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

using Reko.Core;
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
            Expression opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            Expression opSrc1;
            Expression opSrc2;
            if (instr.Operands.Length == 3)
            {
                opSrc1 = this.Operand(Src1());
                opSrc2 = this.Operand(Src2());
            }
            else
            {
                opSrc1 =  this.Operand(Dst(), PrimitiveType.Word32);
                opSrc2 = this.Operand(Src1());
            }
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
            var lsb = ((ImmediateOperand)instr.Operands[1]).Value.ToInt32();
            var bitsize = ((ImmediateOperand)instr.Operands[2]).Value.ToInt32();
            m.Assign(opDst, m.And(opDst, Constant.UInt32(~Bits.Mask32(lsb, bitsize))));
        }

        private void RewriteBfi()
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var opSrc = this.Operand(Src1());
            var tmp = binder.CreateTemporary(PrimitiveType.Word32);
            var lsb = ((ImmediateOperand)instr.Operands[2]).Value.ToInt32();
            var bitsize = ((ImmediateOperand)instr.Operands[3]).Value.ToInt32();
            m.Assign(tmp, m.Slice(opSrc, 0, bitsize));
            m.Assign(opDst, m.Dpb(opDst, tmp, lsb));
        }

        private void RewriteBinOp(Func<Expression, Expression, Expression> op)
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            if (instr.Operands.Length > 2)
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
            if (instr.SetFlags)
            {
                m.Assign(NZCV(), m.Cond(opDst));
            }
        }

        private void RewriteLogical(Func<Expression, Expression, Expression> cons)
        {
            var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
            if (instr.Operands.Length > 2)
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
            if (instr.SetFlags)
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

        private void RewriteRevsh()
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var intrinsic = m.Cast(PrimitiveType.Int32, host.PseudoProcedure("__rev_16", PrimitiveType.Word16, m.Slice(PrimitiveType.Word16, this.Operand(Src1()), 0)));
            m.Assign(opDst, intrinsic);
        }

        private void RewriteRevBinOp(Func<Expression, Expression, Expression> op, bool setflags)
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            if (instr.Operands.Length > 2)
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

        private Expression Bic(Expression a, Expression b)
        {
            return m.And(a, m.Comp(b));
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

        private void RewriteCrc(string fnName, DataType dt)
        {
            var src1 = this.Operand(Src1());
            var src2 = this.Operand(Src2());
            if (src1.DataType.BitSize > dt.BitSize)
            {
                src2 = EmitNarrowingSlice(src2, dt);
            }
            var dst = this.Operand(Dst());
            var intrinsic = host.PseudoProcedure(fnName, PrimitiveType.UInt32, src1, src2);
            m.Assign(dst, intrinsic);
        }


        private void RewriteDiv(Func<Expression, Expression, Expression> op)
        {
            var dst = Operand(Dst(), PrimitiveType.Word32, true);
            var src1 = Operand(Src1());
            var src2 = Operand(Src2());
            m.Assign(dst, op(src1, src2));
        }

        private void RewriteTableBranch(DataType elemSize)
        {
            this.rtlClass = InstrClass.Transfer;
            var mem = (MemoryOperand) instr.Operands[0];
            Expression tableBase;
            if (mem.BaseRegister == Registers.pc)
            {
                // If the base register is PC, the table starts right after the instruction.
                tableBase = instr.Address + instr.Length;
            }
            else
            {
                tableBase = binder.EnsureRegister(mem.BaseRegister);
            }
            var idxReg = binder.EnsureRegister(mem.Index);
            Expression ea;
            if (elemSize.Size != 1)
            {
                ea = m.IAdd(tableBase, m.IMul(idxReg, elemSize.Size));
            }
            else
            {
                ea = m.IAdd(tableBase, idxReg);
            }
            m.Goto(m.IAdd(instr.Address + this.pcValueOffset, m.IMul(m.Mem(elemSize, ea), 2)));
        }

        private void RewriteTeq()
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var opSrc = this.Operand(Src1());
            m.Assign(
                NZC(),
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
            var rDst = (RegisterOperand)instr.Operands[0];
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
                var baseReg = binder.EnsureRegister(mem.BaseRegister);
                if (isJump && instr.IsSinglePop())
                {
                    //$TODO: this is a cheat; we could be popping
                    // something other than the LR (or continuation)
                    // of this procedure. That requires more advanced 
                    // analyses than Reko can manage presently.
                    rtlClass = instr.condition == ArmCondition.AL
                        ? InstrClass.Transfer
                        : InstrClass.ConditionalTransfer;
                    m.Assign(baseReg, m.IAdd(baseReg, mem.Offset));
                    m.Return(0, 0);
                    return;
                }
                var tmp = binder.CreateTemporary(dtDst);
                m.Assign(tmp, src);
                Expression ea = baseReg;
                if (mem.Offset != null)
                {
                    ea = m.IAdd(baseReg, mem.Offset);
                }
                m.Assign(baseReg, ea);
                src = tmp;
            }
            if (isJump)
            {
                rtlClass = InstrClass.Transfer;
                m.Goto(src);
            }
            else
            {
                m.Assign(dst, src);
            }
        }



        private void RewriteLdrd()
        {
            var regLo = ((RegisterOperand)instr.Operands[0]).Register;
            var regHi = ((RegisterOperand)instr.Operands[1]).Register;
            var opDst = binder.EnsureSequence(PrimitiveType.Word64, regHi, regLo);
            var opSrc = this.Operand(instr.Operands[2]);
            m.Assign(opDst, opSrc);
            MaybePostOperand(instr.Operands[2]);
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
            var regLo = ((RegisterOperand)instr.Operands[0]).Register;
            var regHi = ((RegisterOperand)instr.Operands[1]).Register;
            var opSrc = binder.EnsureSequence(PrimitiveType.Word64, regHi, regLo);
            var opDst = this.Operand(instr.Operands[2]);
            m.Assign(opDst, opSrc);
            MaybePostOperand(instr.Operands[2]);
        }

        private void RewriteStrex()
        {
            var intrinsic = host.PseudoProcedure("__strex", VoidType.Instance);
            m.SideEffect(intrinsic);
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
            if (instr.SetFlags)
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
            var mop = (MultiRegisterOperand)instr.Operands[instr.Operands.Length > 1 ? 1 : 0];
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
                m.Assign(dst, m.IAddS(dst, offset));
            }
            if (pcRestored)
            {
                rtlClass = instr.condition == ArmCondition.AL
                    ? InstrClass.Transfer
                    : InstrClass.ConditionalTransfer;
                m.Return(0, 0);
            }
        }

        private void RewriteLdrex()
        {
            var intrinsic = host.PseudoProcedure("__ldrex", VoidType.Instance);
            m.SideEffect(intrinsic);
        }

        private void RewriteShift(Func<Expression, Expression, Expression> ctor)
        {
            var dst = Operand(Dst(), PrimitiveType.Word32, true);
            var src1 = Operand(Src1());
            Expression src2;
            if (instr.Operands.Length == 2)
            {
                src2 = src1;
                src1 = dst;
            }
            else
            {
                src2 = Operand(Src2());
            }
            m.Assign(dst, ctor(src1, src2));
            if (instr.SetFlags)
            {
                m.Assign(this.NZC(), m.Cond(dst));
            }
        }

        private Expression Ror(Expression left, Expression right)
        {
            var intrinsic = host.PseudoProcedure(PseudoProcedure.Ror, left.DataType, left, right);
            return intrinsic;
        }

        private Expression Rrx(Expression left, Expression right)
        {
            var intrinsic = host.PseudoProcedure(PseudoProcedure.RorC, left.DataType, left, right, C());
            return intrinsic;
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
                rtlClass = InstrClass.Transfer;
                if (Src1() is RegisterOperand ropSrc && ropSrc.Register == Registers.lr)
                {
                    m.Return(0, 0);
                }
                else if (Src1() is ImmediateOperand imm)
                {
                    m.Goto(arch.MakeAddressFromConstant(imm.Value, true));
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
            if (instr.SetFlags)
            {
                m.Assign(NZC(), m.Cond(opDst));
            }
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
            var regLo = ((RegisterOperand)instr.Operands[0]).Register;
            var regHi = ((RegisterOperand)instr.Operands[1]).Register;

            var opDst = binder.EnsureSequence(dtResult, regHi, regLo);
            var opSrc1 = this.Operand(Src3());
            var opSrc2 = this.Operand(Src2());
            m.Assign(opDst, op(opSrc1, opSrc2));
            if (instr.SetFlags)
            {
                m.Assign(NZCV(), m.Cond(opDst));
            }
        }

        private void RewritePk(string name)
        {
            var src1 = Operand(Src1());
            var src2 = Operand(Src2());
            var dst = Operand(Dst());
            m.Assign(dst, host.PseudoProcedure(name, dst.DataType, src1, src2));
        }

        private void RewritePld(string name)
        {
            var dst = ((MemoryAccess) this.Operand(Dst())).EffectiveAddress;
               m.SideEffect(host.PseudoProcedure(
                   name,
                   VoidType.Instance,
                   dst));
        }

        private void RewritePop()
        {
            var sp = Reg(Registers.sp);
            RewriteLdm(sp, 0, 0, m.IAdd, true);
        }

        private void RewritePush()
        {
            Expression dst = Reg(Registers.sp);
            var regs = ((MultiRegisterOperand)instr.Operands[0]).GetRegisters().ToArray();
            m.Assign(dst, m.ISubS(dst, regs.Length * 4));

            int offset = 0;
            foreach (var reg in regs)
            { 
                var ea = offset != 0
                    ? m.IAddS(dst, offset)
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
            m.Assign(dst, sat);
            m.Assign(
                Q(),
                m.Cond(dst));
        }

        private void RewriteQasx(string name)
        {
            var src1 = Operand(Src1());
            var src2 = Operand(Src2());
            var dst = Operand(Dst());
            var dtArray = new ArrayType(PrimitiveType.Int16, 2);
            var qasx = host.PseudoProcedure(name, dtArray, src1, src2);
            m.Assign(dst, qasx);
        }

        private void RewriteQDAddSub(Func<Expression, Expression, Expression> op)
        {
            var dst = Operand(Dst(), PrimitiveType.Word32, true);
            var src2 = m.SMul(Operand(Src2()), m.Int32(2));
            var sat = host.PseudoProcedure("__signed_sat_32", PrimitiveType.Int32, src2);
            src2 = sat;
            var src1 = Operand(Src1());
            var sum = op(src1, src2);
            sat = host.PseudoProcedure("__signed_sat_32", PrimitiveType.Int32, sum);
            m.Assign(dst, sat);
            m.Assign(
                Q(),
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
            var r1 = ((RegisterOperand)instr.Operands[0]).Register;
            var r2 = ((RegisterOperand)instr.Operands[1]).Register;
            var dst = binder.EnsureSequence(PrimitiveType.Int64, r1, r2);
            var fac1 = Operand(Src2());
            var fac2 = Operand(Src3());
            m.Assign(dst, m.IAdd(m.SMul(fac1, fac2), dst));
        }

        private void RewriteMlal(bool hiLeft, bool hiRight, PrimitiveType dt, Func<Expression, Expression, Expression> op)
        {
            var r1 = ((RegisterOperand)instr.Operands[0]).Register;
            var r2 = ((RegisterOperand)instr.Operands[1]).Register;
            var dst = binder.EnsureSequence(PrimitiveType.Int64, r1, r2);

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
            var r1 = ((RegisterOperand)instr.Operands[1]).Register;
            var r2 = ((RegisterOperand)instr.Operands[0]).Register;
            var dst = binder.EnsureSequence(PrimitiveType.Int64, r1, r2);

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
            var dst = Operand(instr.Operands[0]);
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

        private void RewriteSmml(Func<Expression,Expression,Expression> fn)
        {
            var src1 = Operand(Src1());
            var src2 = Operand(Src2());
            var src3 = Operand(Src3());
            var dst = Operand(Dst());
                
            var mul = m.SMul(src1, src2);
            mul.DataType = PrimitiveType.Int64;
            m.Assign(dst, fn(
                m.Cast(PrimitiveType.Int32, m.Sar(mul, m.Int32(32))),
                src3));
        }

        private void RewriteSmmul()
        {
            var src1 = Operand(Src1());
            var src2 = Operand(Src2());
            var dst = Operand(Dst());

            var mul = m.SMul(src1, src2);
            mul.DataType = PrimitiveType.Int64;
            m.Assign(dst, m.Cast(PrimitiveType.Int32, m.Sar(mul, m.Int32(32))));
        }

        private void RewriteSmusd()
        {
            var s16 = PrimitiveType.Int16;
            var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var rn = this.Operand(Src1());
            var rm = this.Operand(Src2());
            var p1 = binder.CreateTemporary(PrimitiveType.Int32);
            var p2 = binder.CreateTemporary(PrimitiveType.Int32);
            m.Assign(p1, m.SMul(m.Slice(s16, rn, 0), m.Slice(s16, rm, 0)));
            m.Assign(p2, m.SMul(m.Slice(s16, rn, 16), m.Slice(s16, rm, 16)));
            m.Assign(dst, m.ISub(p1, p2));
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

        private void RewriteSsat()
        {
            var dst = this.Operand(Dst());
            var src1 = this.Operand(Src1());
            var src2 = this.Operand(Src2());
            var intrinsic = host.PseudoProcedure("__ssat", PrimitiveType.Int32, src1, src2);
            m.Assign(dst, intrinsic);
            m.Assign(Q(), m.Cond(dst));
        }

        private void RewriteStm(bool add, bool updateAfter)
        {
            var regs = ((MultiRegisterOperand)Src1()).GetRegisters().ToArray();
            if (regs.Length == 0)
            {
                Invalid();
                return;
            }
            var rSrc = this.Operand(Dst(), PrimitiveType.Word32, true);
            int regSize = regs[0].DataType.Size;
            int totalRegsize = regs.Length * regSize;
            int offset;
            if (add)
            {
                offset = updateAfter ? 0 : regSize;
            }
            else
            {
                offset = -totalRegsize;
                if (updateAfter)
                    offset += regSize;
            }
            foreach (var r in regs)
            {
                var dst = Reg(r);
                Expression ea =
                    offset != 0
                    ? m.IAddS(rSrc, offset)
                    : rSrc;
                m.Assign(m.Mem(r.DataType, ea), dst);
                offset += r.DataType.Size;
            }
            if (instr.Writeback)
            {
                if (add)
                {
                    m.Assign(rSrc, m.IAddS(rSrc, totalRegsize));
                }
                else
                {
                    m.Assign(rSrc, m.ISubS(rSrc, totalRegsize));
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

        private void RewriteUasx()
        {
            var diff = binder.CreateTemporary(PrimitiveType.UInt16);
            var sum = binder.CreateTemporary(PrimitiveType.UInt16);
            var rn = Operand(instr.Operands[1]);
            var rm = Operand(instr.Operands[2]);
            m.Assign(diff, m.ISub(m.Slice(rn, 0, 16), m.Slice(rm, 16, 16)));
            m.Assign(sum, m.IAdd(m.Slice(rn, 16, 16), m.Slice(rm, 0, 16)));
            var rd = Operand(Dst());
            m.Assign(rd, m.Seq(sum, diff));
            //$REVIEW: flags?
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
                PrimitiveType.UInt64,
                ((RegisterOperand)Src1()).Register,
                ((RegisterOperand)Dst()).Register);
            m.Assign(tmp, m.UMul(rn, rm));
            m.Assign(tmp, m.IAdd(tmp, m.Cast(PrimitiveType.UInt64, hi)));
            m.Assign(dst, m.IAdd(tmp, m.Cast(PrimitiveType.UInt64, lo)));
        }

        private void RewriteUmlal()
        {
            var dst = binder.EnsureSequence(
                PrimitiveType.Word64,
                ((RegisterOperand)Src1()).Register,
                ((RegisterOperand)Dst()).Register);
            var left = this.Operand(Src2());
            var right = this.Operand(Src3());
            m.Assign(dst, m.IAdd(m.UMul(left, right), dst));
            MaybeUpdateFlags(dst);
        }

        private void RewriteUnaryOp(Func<Expression, Expression> op)
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var opSrc = this.Operand(Src1());
            m.Assign(opDst, op(opSrc));
            if (instr.SetFlags)
            {
                m.Assign(NZCV(), m.Cond(opDst));
            }
        }

        private void RewriteUsada8()
        {
            var opDst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var opSrc1 = this.Operand(Src1());
            var opSrc2 = this.Operand(Src2());
            var opAcc = this.Operand(Src3());
            var ab_4 = new ArrayType(PrimitiveType.Byte, 4);
            var vSrc1 = binder.CreateTemporary(ab_4);
            var vSrc2 = binder.CreateTemporary(ab_4);
            m.Assign(vSrc1, opSrc1);
            m.Assign(vSrc2, opSrc2);
            var intrinsic = host.PseudoProcedure("__usada8", PrimitiveType.Word32, vSrc1, vSrc2);
            m.Assign(opDst, intrinsic);
        }

        private void RewriteUsat()
        {
            var dst = this.Operand(Dst());
            var src1 = this.Operand(Src1());
            var src2 = this.Operand(Src2());
            var intrinsic = host.PseudoProcedure("__usat", PrimitiveType.UInt32, src1, src2);
            m.Assign(dst, intrinsic);
            m.Assign(Q(), m.Cond(dst));
        }

        private void RewriteSat16(PrimitiveType elemType, string intrinsicName)
        {
            var dst = this.Operand(Dst());
            var src1 = this.Operand(Src1());
            var src2 = this.Operand(Src2());
            var arrSrc = new ArrayType(elemType, 2);
            var arrDst = new ArrayType(elemType, 2);

            var intrinsic = host.PseudoProcedure(intrinsicName, arrDst, src1, src2);
            m.Assign(dst, intrinsic);
            m.Assign(Q(), m.Cond(dst));
        }


        private void RewriteUsax()
        {
            var sum = binder.CreateTemporary(PrimitiveType.UInt16);
            var diff = binder.CreateTemporary(PrimitiveType.UInt16);
            var rn = Operand(instr.Operands[1]);
            var rm = Operand(instr.Operands[2]);
            m.Assign(sum, m.IAdd(m.Slice(rn, 0, 16), m.Slice(rm, 16, 16)));
            m.Assign(diff, m.ISub(m.Slice(rn, 16, 16), m.Slice(rm, 0, 16)));
            var rd = Operand(Dst());
            m.Assign(rd, m.Seq(diff, sum));
            //$REVIEW: flags?
        }

        private void RewriteXtab(PrimitiveType dt)
        {
            var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
            Expression src = Reg(((RegisterOperand)Src2()).Register);
            if (instr.ShiftType == Mnemonic.ror)
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
            if (instr.ShiftType == Mnemonic.ror)
            {
                src = m.Shr(src,Operand(instr.ShiftValue));
            }
            src = m.Cast(dtSrc, src);
            src = m.Cast(dtDst, src);
            m.Assign(dst, src);
        }

        private void RewriteYield()
        {
            var intrinsic = host.PseudoProcedure("__yield", VoidType.Instance);
            m.SideEffect(intrinsic);
        }
    }
}

