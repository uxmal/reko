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

using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Reko.Core;
using Reko.Core.Rtl;

namespace Reko.Arch.M68k
{
    /// <summary>
    /// Rewrites ALU instructions.
    /// </summary>
    public partial class Rewriter
    {
        public void RewriteArithmetic(Func<Expression, Expression, Expression> binOpGen)
        {
            var opSrc = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var opDst = orw.RewriteDst(instr.Operands[1], instr.Address,opSrc, binOpGen);
            AllConditions(opDst);
        }

        public void RewriteRotation(IntrinsicProcedure rotation)
        {
            Expression? opDst;
            if (instr.Operands.Length == 2)
            {
                var opSrc = orw.RewriteSrc(instr.Operands[0], instr.Address);
                opDst = orw.RewriteDst(instr.Operands[1], instr.Address, opSrc, (s, d) =>
                    m.Fn(rotation, d, s));
            }
            else
            {
                opDst = orw.RewriteDst(instr.Operands[0], instr.Address,
                    Constant.Byte(1), (s, d) =>
                        m.Fn(rotation, d, s));
            }
            if (opDst == null)
            {
                EmitInvalid();
                return;
            }
            m.Assign(
                binder.EnsureFlagGroup(Registers.CZN),
                m.Cond(opDst));
            m.Assign(binder.EnsureFlagGroup(Registers.V), Constant.False());
        }

        public void RewriteRotationX(IntrinsicProcedure rotation)
        {
            Expression? opDst;
            if (instr.Operands.Length == 2)
            {
                var opSrc = orw.RewriteSrc(instr.Operands[0], instr.Address);
                opDst = orw.RewriteDst(instr.Operands[1], instr.Address, opSrc, (s, d) =>
                    m.Fn(rotation.MakeInstance(d.DataType, s.DataType), d, s, binder.EnsureFlagGroup(Registers.X)));
            }
            else
            {
                opDst = orw.RewriteDst(instr.Operands[0], instr.Address,
                    Constant.Byte(1), (s, d) =>
                        m.Fn(rotation.MakeInstance(d.DataType, s.DataType), d, s, binder.EnsureFlagGroup(Registers.X)));
            }
            if (opDst is null)
            {
                EmitInvalid();
                return;
            }
            m.Assign(
                binder.EnsureFlagGroup(Registers.CZNX),
                m.Cond(opDst));
            m.Assign(binder.EnsureFlagGroup(Registers.V), Constant.False());
        }

        public void RewriteTst()
        {
            var opSrc = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var opDst = binder.EnsureFlagGroup(Registers.ZN);
            m.Assign(opDst, m.Cond(m.ISub(opSrc, 0)));
            m.Assign(binder.EnsureFlagGroup(Registers.C), Constant.False());
            m.Assign(binder.EnsureFlagGroup(Registers.V), Constant.False());
        }

        public void RewriteShift(Func<Expression, Expression, Expression> binOpGen)
        {
            Expression? opDst;
            if (instr.Operands.Length == 2)
            {
                var opSrc = orw.RewriteSrc(instr.Operands[0], instr.Address);
                opDst = orw.RewriteDst(instr.Operands[1], instr.Address, opSrc, binOpGen);
            }
            else
            {
                var opSrc = Constant.Int32(1);
                opDst = orw.RewriteDst(instr.Operands[0], instr.Address, PrimitiveType.Word16, opSrc, binOpGen);
            }
            AllConditions(opDst);
        }

        public void RewriteBchg()
        {
            var opSrc = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var tmpMask = binder.CreateTemporary(PrimitiveType.UInt32);
            m.Assign(tmpMask, m.Shl(1, opSrc));
            var opDst = orw.RewriteDst(instr.Operands[1], instr.Address, tmpMask, (s, d) => m.Xor(d, s));
            if (opDst == null)
            {
                EmitInvalid();
                return;
            }
            m.Assign(
                binder.EnsureFlagGroup(Registers.Z),
                m.Cond(m.And(opDst, tmpMask)));
        }

        public void RewriteBclrBset(IntrinsicProcedure intrinsic)
        {
            var opSrc = orw.RewriteSrc(instr.Operands[0], instr.Address);
            PrimitiveType w = (instr.Operands[1] is RegisterStorage)
                 ? PrimitiveType.Word32 
                 : PrimitiveType.Byte;
            orw.DataWidth = w;
            var opDst = orw.RewriteSrc(instr.Operands[1], instr.Address);
            m.Assign(
                binder.EnsureFlagGroup(Registers.Z),
                m.Fn(intrinsic, opDst, opSrc, m.Out(PrimitiveType.Ptr32, opDst)));
        }

        public void RewriteBfchg()
        {

        }

        public void RewriteExg()
        {
            var opSrc = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var opDst = orw.RewriteSrc(instr.Operands[1], instr.Address);
            var tmp = binder.CreateTemporary(PrimitiveType.Word32);
            m.Assign(tmp, opSrc);
            m.Assign(opSrc, opDst);
            m.Assign(opDst, tmp);
            LogicalConditions(opDst);
        }

        public void RewriteExt()
        {
            PrimitiveType dtSrc;
            PrimitiveType dtDst;
            if (instr.DataWidth!.Size == 4)
            {
                dtSrc = PrimitiveType.Int16;
                dtDst = PrimitiveType.Int32;
            }
            else
            {
                dtSrc = PrimitiveType.SByte;
                dtDst = PrimitiveType.Int16;
            }
            var dReg = binder.EnsureRegister((RegisterStorage) instr.Operands[0]);
            m.Assign(
                dReg,
                m.Convert(
                    m.Slice(dtSrc, dReg, 0),
                    dtSrc, dtDst));
            m.Assign(
                binder.EnsureFlagGroup(Registers.ZN),
                m.Cond(dReg));
        }

        public void RewriteExtb()
        {
            var dReg = orw.RewriteSrc(instr.Operands[0], instr.Address);
            m.Assign(
                dReg,
                m.Convert(m.Slice(PrimitiveType.SByte, dReg, 0), PrimitiveType.SByte, PrimitiveType.Int32));
            m.Assign(
                binder.EnsureFlagGroup(Registers.ZN),
                m.Cond(dReg));
        }

        public void RewriteLogical(Func<Expression, Expression, Expression> binOpGen)
        {
            var width = instr.DataWidth;
            var opSrc = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var opDst = orw.RewriteDst(instr.Operands[1], instr.Address, opSrc, binOpGen);
            LogicalConditions(opDst);
        }

        public void RewriteMul(Func<Expression, Expression, Expression> binOpGen)
        {
            var opSrc = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var opDst = orw.RewriteDst(instr.Operands[1], instr.Address, PrimitiveType.Int32, opSrc, binOpGen);
            if (opDst == null)
            {
                EmitInvalid();
                return;
            }
            m.Assign(binder.EnsureFlagGroup(Registers.VZN), m.Cond(opDst));
            m.Assign(binder.EnsureFlagGroup(Registers.C), Constant.False());
        }

        public void RewriteUnary(Func<Expression, Expression> unaryOpGen, Action<Expression> generateFlags)
        {
            var op = orw.RewriteUnary(instr.Operands[0], instr.Address, instr.DataWidth!, unaryOpGen);
            generateFlags(op);
        }

        private void RewriteAddSubq(Func<Expression,Expression,Expression> opGen)
        {
            var opSrc = (Constant) orw.RewriteSrc(instr.Operands[0], instr.Address);
            if (instr.Operands[1] is AddressRegister regDst)
            {
                opSrc = Constant.Int32(opSrc.ToInt32());
                var opDst = binder.EnsureRegister(regDst);
                m.Assign(opDst, opGen(opSrc, opDst));
            }
            else
            {
                var opDst = orw.RewriteDst(instr.Operands[1], instr.Address, opSrc, opGen);
                if (opDst == null)
                {
                    EmitInvalid();
                    return;
                }
                m.Assign(binder.EnsureFlagGroup(Registers.CVZNX), m.Cond(opDst));
            }
        }

        public void RewriteAddSubx(Func<Expression,Expression,Expression> opr)
        {
            // We do not take the trouble of widening the CF to the word size
            // to simplify code analysis in later stages. 
            var x = binder.EnsureFlagGroup(Registers.X);
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var dst = orw.RewriteDst(instr.Operands[1], instr.Address, src, (d, s) => 
                    opr(opr(d, s), x));
            if (dst == null)
            {
                EmitInvalid();
                return;
            }
            m.Assign(binder.EnsureFlagGroup(Registers.CVZNX), m.Cond(dst));
        }

        private void RewriteScc(ConditionCode cc, FlagGroupStorage flagsUsed)
        {
            var cond = m.Conditional(
                PrimitiveType.Byte,
                m.Test(cc, binder.EnsureFlagGroup(flagsUsed)),
                m.Byte(0xFF),
                m.Byte(0x00));
            orw.RewriteMoveDst(instr.Operands[0], instr.Address, PrimitiveType.Byte, cond);
        }
        
        private void RewriteSwap()
        {
            var r = (RegisterStorage) instr.Operands[0];
            var reg = binder.EnsureRegister(r);
            m.Assign(reg, m.Fn(swap_intrinsic, reg));
            m.Assign(binder.EnsureFlagGroup(Registers.ZN), m.Cond(reg));
            m.Assign(binder.EnsureFlagGroup(Registers.C), Constant.False());
            m.Assign(binder.EnsureFlagGroup(Registers.V), Constant.False());
        }

        private void RewriteBinOp(Func<Expression,Expression,Expression> opGen)
        {
            var opSrc = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var opDst = orw.RewriteDst(instr.Operands[1], instr.Address, opSrc, opGen);
            if (opDst == null)
            {
                EmitInvalid();
                return;
            }
        }

        private void RewriteBinOp(Func<Expression, Expression, Expression> opGen, FlagGroupStorage flags)
        {
            var opSrc = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var opDst = orw.RewriteDst(instr.Operands[1], instr.Address, opSrc, opGen);
            if (opDst == null)
            {
                EmitInvalid();
                return;
            }
            m.Assign(binder.EnsureFlagGroup(flags), m.Cond(opDst));
        }

        private void RewriteBtst()
        {
            orw.DataWidth = instr.Operands[0] is RegisterStorage ? PrimitiveType.Word32 : PrimitiveType.Byte;
            var opSrc = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var opDst = orw.RewriteSrc(instr.Operands[1], instr.Address);
            m.Assign(
                binder.EnsureFlagGroup(Registers.Z),
                m.Fn(btst_intrinsic, opDst, opSrc));
        }

        private void RewriteCas()
        {
            m.Assign(
                binder.EnsureFlagGroup(Registers.CVZN),
                host.Intrinsic("atomic_compare_exchange_weak", true, PrimitiveType.Bool,
                    m.AddrOf(PrimitiveType.Ptr32, orw.RewriteSrc(instr.Operands[2], instr.Address)),
                    orw.RewriteSrc(instr.Operands[1], instr.Address),
                    orw.RewriteSrc(instr.Operands[0], instr.Address)));
        }

        private void RewriteClr()
        {
            var src = Constant.Create(instr.DataWidth!, 0);
            var opDst = orw.RewriteMoveDst(instr.Operands[0], instr.Address, instr.DataWidth!, src);
            m.Assign(binder.EnsureFlagGroup(Registers.Z), Constant.True());
            m.Assign(binder.EnsureFlagGroup(Registers.C), Constant.False());
            m.Assign(binder.EnsureFlagGroup(Registers.N), Constant.False());
            m.Assign(binder.EnsureFlagGroup(Registers.V), Constant.False());
        }

        private void RewriteCmp()
        {
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var dst = orw.RewriteSrc(instr.Operands[1], instr.Address);
            var tmp = binder.CreateTemporary(dst.DataType);
            m.Assign(tmp, m.ISub(dst, src));
            m.Assign(
                binder.EnsureFlagGroup(Registers.CVZN),
                m.Cond(tmp));
        }

        private void RewriteCmp2()
        {
            var reg = orw.RewriteSrc(instr.Operands[1], instr.Address);
            var lowBound = orw.RewriteSrc(instr.Operands[0], instr.Address);
            if (lowBound is MemoryAccess mem)
            {
                var ea = mem.EffectiveAddress;
                var hiBound = m.Mem(lowBound.DataType, m.IAdd(ea, lowBound.DataType.Size));
                var C = binder.EnsureFlagGroup(Registers.C);
                var Z = binder.EnsureFlagGroup(Registers.Z);
                m.Assign(C, m.Cor(m.Lt(reg, lowBound), m.Gt(reg, hiBound)));
                m.Assign(Z, m.Cor(m.Eq(reg, lowBound), m.Eq(reg, hiBound)));
            }
            else
            {
                EmitInvalid();
            }
        }

        private void RewriteDiv(Func<Expression,Expression,Expression> op, DataType dt)
        {
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            Expression dividend;
            Expression quot;
            Expression rem;
            if (instr.DataWidth!.BitSize == 16)
            {
                instr.DataWidth = PrimitiveType.UInt32;
                rem = binder.CreateTemporary(dt);
                quot = binder.CreateTemporary(dt);
                dividend = binder.EnsureRegister((RegisterStorage) instr.Operands[1]);
                var r = m.Remainder(dividend, src);
                if (r.DataType.BitSize != rem.DataType.BitSize)
                {
                    r = m.Convert(r, r.DataType, rem.DataType);
                }
                var q = op(dividend, src);
                if (q.DataType.BitSize != quot.DataType.BitSize)
                {
                    q = m.Convert(q, q.DataType, quot.DataType);
                }
                m.Assign(rem, r);
                m.Assign(quot, q);
                m.Assign(dividend, m.Seq(rem, quot));
            }
            else
            {
                if (instr.Operands[1] is DoubleRegisterOperand dreg)
                {
                    rem = binder.EnsureRegister(dreg.Register1);
                    quot = binder.EnsureRegister(dreg.Register2);
                    if (instr.Mnemonic == Mnemonic.divsl)
                    {
                        dividend = quot;
                    }
                    else
                    {
                        var dtDividend = PrimitiveType.CreateWord((int) (dreg.Register1.BitSize + dreg.Register2.BitSize));
                        dividend = binder.EnsureSequence(dtDividend, dreg.Register1, dreg.Register2);
                    }
                    m.Assign(rem, m.Remainder(dividend, src));
                    m.Assign(quot, op(dividend, src));
                }
                else
                {
                    rem = orw.RewriteSrc(instr.Operands[0], instr.Address);
                    quot = orw.RewriteSrc(instr.Operands[1], instr.Address);
                    var divisor = binder.CreateTemporary(rem.DataType);
                    m.Assign(divisor, rem);
                    m.Assign(rem, m.Remainder(quot, divisor));
                    m.Assign(quot, op(quot, divisor));
                }
            }
            m.Assign(
                binder.EnsureFlagGroup(Registers.VZN),
                m.Cond(quot));
            m.Assign(
                binder.EnsureFlagGroup(Registers.C), Constant.False());
        }

        private Expression RewriteSrcOperand(MachineOperand mop)
        {
            return RewriteDstOperand(mop, null!, (s, d) => { })!;
        }

        private bool NeedsSpilling(Expression op)
        {
            //$REVIEW: May not need to spill here if opSrc is immediate / register other than reg
            if (op == null)
                return false;
            if (op is Constant)
                return false;
            return true;
        }

        private Expression? RewriteDstOperand(MachineOperand mop, Expression opSrc, Action<Expression, Expression> m)
        {
            var dataWidth = instr.DataWidth!;
            if (mop is PredecrementMemoryOperand preDec)
            {
                var reg = binder.EnsureRegister(preDec.Register);
                var t = binder.CreateTemporary(opSrc.DataType);
                if (NeedsSpilling(opSrc))
                {
                    this.m.Assign(t, opSrc);
                    opSrc = t;
                }
                this.m.Assign(reg, this.m.ISubS(reg, dataWidth.Size));
                var op = this.m.Mem(dataWidth, reg);
                m(opSrc, op);
                return op;
            }
            if (mop is PostIncrementMemoryOperand postInc)
            {
                var reg = binder.EnsureRegister(postInc.Register);
                var t = binder.CreateTemporary(dataWidth);
                if (NeedsSpilling(opSrc))
                {
                    this.m.Assign(t, opSrc);
                    opSrc = t;
                }
                m(opSrc, this.m.Mem(dataWidth, reg));
                this.m.Assign(reg, this.m.IAddS(reg, dataWidth.Size));
                return t;
            }
            return orw.RewriteSrc(mop, instr.Address);
        }

        public Expression GetEffectiveAddress(MachineOperand op)
        {
            if (op is MemoryOperand mem)
            {
                if (mem.Base == null)
                {
                    return mem.Offset!;
                }
                else if (mem.Base == Registers.pc)
                {
                    var addr = instr.Address;
                    if (mem.Offset is null)
                        return addr;
                    else 
                        return addr + mem.Offset.ToInt32();
                }
                else if (mem.Offset == null)
                {
                    return binder.EnsureRegister(mem.Base);
                }
                else
                {
                    return m.IAdd(
                        binder.EnsureRegister(mem.Base),
                        Constant.Int32(mem.Offset.ToInt32()));
                }
            }
            if (instr.Operands[0] is AddressOperand addrOp)
            {
                return addrOp.Address;
            }
            if (op is IndexedOperand indop)
            {
                var c = indop.BaseDisplacement;
                if (indop.Base == Registers.pc)
                {
                    var addr = instr.Address;
                    if (c is null)
                        return addr;
                    else 
                        return addr + c.ToInt32();
                }
                Expression ea = orw.Combine(indop.BaseDisplacement, indop.Base, instr.Address);
                if (indop.postindex)
                {
                    ea = m.Mem32(ea);
                }
                if (indop.Index != null)
                {
                    var idx = orw.Combine(null, indop.Index, instr.Address);
                    if (indop.index_reg_width!.BitSize != 32)
                        idx = m.Convert(m.Slice(PrimitiveType.Int16, idx, 0), PrimitiveType.Int16, PrimitiveType.Int32);
                    if (indop.IndexScale > 1)
                        idx = m.IMul(idx, m.Int32(indop.IndexScale));
                    ea = orw.Combine(ea, idx);
                }
                if (indop.preindex)
                {
                    ea = m.Mem32(ea);
                }
                ea = orw.Combine(ea, indop.OuterDisplacement);
                return ea;
            }
            var indIdx = instr.Operands[0] as IndirectIndexedOperand;
            if (indIdx != null)
            {
                var a = binder.EnsureRegister(indIdx.ARegister);
                var x = binder.EnsureRegister(indIdx.XRegister);
                return m.IAdd(a, x);        //$REVIEW: woefully incomplete...
            }
            throw new NotImplementedException(string.Format("{0} ({1})", op, op.GetType().Name));
        }


        public void RewriteLea()
        {
            var dst = orw.RewriteSrc(instr.Operands[1], instr.Address);
            var src = GetEffectiveAddress(instr.Operands[0]);
            m.Assign(dst, src);
        }

        public void RewritePack()
        {
            orw.DataWidth = PrimitiveType.UInt16;
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var adj = orw.RewriteSrc(instr.Operands[2], instr.Address);
            orw.DataWidth = PrimitiveType.Byte;
            var dst = orw.RewriteDst(instr.Operands[1], instr.Address, src, (s, d) =>
                m.Fn(pack_intrinsic, s, adj));
        }

        public void RewriteUnpk()
        {
            orw.DataWidth = PrimitiveType.Byte;
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var adj = orw.RewriteSrc(instr.Operands[2], instr.Address);
            orw.DataWidth = PrimitiveType.UInt16;
            var dst = orw.RewriteDst(instr.Operands[1], instr.Address, src, (s, d) =>
                //$REVIEW: shoud really be byte[2]...
                m.Fn(unpk_intrinsic, s, adj));
        }

        public void RewritePea()
        {
            var sp = binder.EnsureRegister(arch.StackRegister);
            m.Assign(sp, m.ISubS(sp, 4));
            var ea = GetEffectiveAddress(instr.Operands[0]);
            m.Assign(m.Mem32(sp), ea);
        }

        public void RewriteMove(bool setFlag)
        {
            if (GetRegister(instr.Operands[0]) == Registers.ccr)
            {
                // move from ccr.
                var src = m.Convert(binder.EnsureRegister(Registers.ccr), PrimitiveType.Byte, PrimitiveType.UInt16);
                var dst = orw.RewriteDst(instr.Operands[1], instr.Address, PrimitiveType.UInt16, src, (s, d) => s);
                return;
            }
            else if (GetRegister(instr.Operands[1]) == Registers.ccr)
            {
                var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
                var dst = orw.RewriteDst(instr.Operands[1], instr.Address, instr.DataWidth ?? (PrimitiveType)src.DataType, src, (s, d) => s);
                return;
            }
            var opSrc = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var opDst = orw.RewriteDst(instr.Operands[1], instr.Address, instr.DataWidth ?? (PrimitiveType)opSrc.DataType, opSrc, (s, d) => s);
            if (opDst == null)
            {
                EmitInvalid();
                return;
            }
            var isSr = GetRegister(instr.Operands[0]) == Registers.sr || GetRegister(instr.Operands[1]) == Registers.sr;
            if (setFlag && !isSr)
            {
                m.Assign(
                    binder.EnsureFlagGroup(Registers.CVZN),
                    m.Cond(opDst));
            }
        }

        private void RewriteMove16()
        {
            orw.DataWidth = PrimitiveType.Word128;
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            src.DataType = PrimitiveType.Word128;
            var dst = orw.RewriteDst(instr.Operands[1], instr.Address, PrimitiveType.Word128, src, (s, d) => s);
        }

        public void RewriteMoveq()
        {
            var opSrc = (sbyte) ((M68kImmediateOperand) instr.Operands[0]).Constant.ToInt32();
            var opDst = binder.EnsureRegister((RegisterStorage)instr.Operands[1]);
            m.Assign(opDst, Constant.Int32(opSrc));
            m.Assign(
                binder.EnsureFlagGroup(Registers.CVZN),
                m.Cond(opDst));
        }

        public IEnumerable<Identifier> RegisterMaskIncreasing(bool isReal, uint bitSet, Func<int,RegisterStorage> regGenerator)
        {
            int maxReg = isReal ? 8 : 16;
            int mask = isReal ? 0x80 : 0x8000;

            for (int i = 0; i < maxReg; ++i, mask >>= 1)
            {
                if ((bitSet & mask) != 0)
                {
                    yield return binder.EnsureRegister(regGenerator(i));
                }
            }
        }

        public IEnumerable<Identifier> RegisterMaskDecreasing(bool isReal, uint bitSet, Func<int, RegisterStorage> regGenerator)
        {
            int maxReg = isReal ? 8 : 16;
            for (int i = maxReg-1, mask = 1; i >= 0; --i, mask <<= 1)
            {
                if ((bitSet & mask) != 0)
                    yield return binder.EnsureRegister(regGenerator(i));
            }
        }

        public void RewriteMovem(Func<int, RegisterStorage> regGenerator)
        {
            var dataWidth = instr.DataWidth!;
            if (instr.Operands[1] is RegisterSetOperand dstRegs)
            {
                Identifier srcReg;
                if (instr.Operands[0] is PostIncrementMemoryOperand postInc)
                {
                    srcReg = binder.EnsureRegister(postInc.Register);
                }
                else
                {
                    var src = orw.RewriteSrc(instr.Operands[0], instr.Address) as MemoryAccess;
                    if (src == null)
                    {
                        EmitInvalid();
                        return;
                    }
                    srcReg = binder.CreateTemporary(src.EffectiveAddress.DataType);
                    m.Assign(srcReg, src.EffectiveAddress);
                }
                foreach (var reg in RegisterMaskIncreasing(dstRegs.Width.IsReal, dstRegs.BitSet, regGenerator))
                {
                    m.Assign(reg, m.Mem(dataWidth, srcReg));
                    m.Assign(srcReg, m.IAddS(srcReg, dataWidth.Size));
                }
                return;
            }
            if (instr.Operands[0] is RegisterSetOperand dstRegs2)
            {
                switch (instr.Operands[1])
                {
                case PredecrementMemoryOperand preDec:
                    var dstReg = binder.EnsureRegister(preDec.Register);
                    foreach (var reg in RegisterMaskDecreasing(dstRegs2.Width.IsReal, dstRegs2.BitSet, regGenerator))
                    {
                        m.Assign(dstReg, m.ISubS(dstReg, dataWidth.Size));
                        m.Assign(m.Mem(dataWidth, dstReg), reg);
                    }
                    return;
                case PostIncrementMemoryOperand postDec:
                    var srcReg = binder.CreateTemporary(dataWidth);
                    m.Assign(srcReg, binder.EnsureRegister(postDec.Register));
                    foreach (var reg in RegisterMaskIncreasing(dstRegs2.Width.IsReal, dstRegs2.BitSet, regGenerator))
                    {
                        m.Assign(reg, m.Mem(dataWidth, srcReg));
                        m.Assign(srcReg, m.IAddS(srcReg, dataWidth.Size));
                    }
                    return;
                }
            }
            // Unsupported addressing mode.
            EmitInvalid();
        }

        public void RewriteMovep()
        {
            var movep = (instr.DataWidth == PrimitiveType.Word32)
                ? movep_intrinsic.MakeInstance(PrimitiveType.Word32)
                : movep_intrinsic.MakeInstance(PrimitiveType.Word16);
            var op1 = RewriteSrcOperand(instr.Operands[0]);
            var op2 = RewriteSrcOperand(instr.Operands[1]);
            m.SideEffect(m.Fn(movep, op1, op2));
        }

        private Expression RewriteNegx(Expression expr)
        {
            expr = m.Neg(expr);
            return m.ISub(expr, binder.EnsureFlagGroup(Registers.X));
        }

        private void RewriteLink()
        {
            var aReg = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var aSp = binder.EnsureRegister(arch.StackRegister);
            var imm = ((M68kImmediateOperand) instr.Operands[1]).Constant.ToInt32();
            m.Assign(aSp, m.ISub(aSp, 4));
            m.Assign(m.Mem32(aSp), aReg);
            m.Assign(aReg, aSp);
            if (imm < 0)
            {
                m.Assign(aSp, m.ISub(aSp, -imm));
            }
            else
            {
                m.Assign(aSp, m.IAdd(aSp, -imm));
            }
        }

        private void RewriteAbcd()
        {
            // We do not take the trouble of widening the XF to the word size
            // to simplify code analysis in later stages. 
            var x = binder.EnsureFlagGroup(Registers.X);
            orw.DataWidth = PrimitiveType.Byte;
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            orw.DataWidth = PrimitiveType.Byte;
            var dst = orw.RewriteDst(instr.Operands[1], instr.Address, src, (s, d) =>
                    m.IAdd(m.IAdd(d, s), x));
            if (dst == null)
            {
                EmitInvalid();
                return;
            }
            m.Assign(binder.EnsureFlagGroup(Registers.CVZNX), m.Cond(dst));
        }

        private void RewriteSbcd()
        {
            // We do not take the trouble of widening the XF to the word size
            // to simplify code analysis in later stages. 
            var x = binder.EnsureFlagGroup(Registers.X);
            orw.DataWidth = PrimitiveType.Byte;
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            orw.DataWidth = PrimitiveType.Byte;
            var dst = orw.RewriteDst(instr.Operands[1], instr.Address, src, (d, s) =>
                    m.ISub(m.ISub(d, s), x));
            if (dst == null)
            {
                EmitInvalid();
                return;
            }
            m.Assign(binder.EnsureFlagGroup(Registers.CVZNX), m.Cond(dst));
        }

        private void RewriteNbcd()
        {
            var x = binder.EnsureFlagGroup(Registers.X);
            orw.DataWidth = PrimitiveType.Byte;
            var dst = orw.RewriteDst(instr.Operands[0], instr.Address, null!, (s, d) =>
                    m.ISub(m.ISub(Constant.Zero(d.DataType), d), x));
            if (dst == null)
            {
                EmitInvalid();
                return;
            }
            m.Assign(binder.EnsureFlagGroup(Registers.CVZNX), m.Cond(dst));
        }

        private void RewriteUnlk()
        {
            var aReg = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var aSp = binder.EnsureRegister(arch.StackRegister);
            m.Assign(aSp, aReg);
            m.Assign(aReg, m.Mem32(aSp));
            m.Assign(aSp, m.IAddS(aSp, 4));
        }

        private void Copy(Expression dst, Expression src, int bitSize)
        {
            if (dst is Identifier && dst.DataType.BitSize > bitSize)
            {
                var tmpHi = binder.CreateTemporary(PrimitiveType.CreateWord(dst.DataType.BitSize - bitSize));
                m.Assign(tmpHi, m.Slice(tmpHi.DataType, dst, bitSize));
                m.Assign(dst, m.Seq(tmpHi, src));
            }
            else
            {
                m.Assign(dst, src);
            }
        }

        private void AllConditions(Expression? expr)
        {
            if (expr == null)
            {
                EmitInvalid();
                return;
            }
            var f = binder.EnsureFlagGroup(Registers.CVZNX);
            m.Assign(f, m.Cond(expr));
        }

        private void LogicalConditions(Expression? expr)
        {
            if (expr == null)
            {
                EmitInvalid();
                return;
            }
            m.Assign(binder.EnsureFlagGroup(Registers.ZN), m.Cond(expr));
            m.Assign(binder.EnsureFlagGroup(Registers.C), Constant.False());
            m.Assign(binder.EnsureFlagGroup(Registers.V), Constant.False());
        }
    }
}
