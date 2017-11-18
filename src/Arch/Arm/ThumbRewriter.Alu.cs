#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Gee.External.Capstone.Arm;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Arm
{
    public partial class ThumbRewriter
    {
        private void RewriteAdr()
        {
            var dst = RewriteOp(ops[0]);
            var src = instrs.Current.Address + 4 + ops[1].ImmediateValue.Value;
            m.Assign(dst, src);
        }

        private void RewriteAnd()
        {
            var dst = RewriteOp(ops[0]);
            var src = RewriteOp(ops[1]);
            m.Assign(dst, m.And(dst, src));
            if (instr.ArchitectureDetail.UpdateFlags)
                m.Assign(FlagGroup(FlagM.NF | FlagM.ZF | FlagM.CF, "NZC", PrimitiveType.Byte),
                    m.Cond(dst));
        }

    
        private void RewriteBic()
        {
            var dst = RewriteOp(ops[0]);
            var src1 = RewriteOp(ops[1]);
            var src2 = RewriteOp(ops[2]);
            m.Assign(dst, m.And(src1, m.Comp(src2)));
        }

        private void RewriteBinop(Func<Expression, Expression, Expression> op)
        {
            var dst = RewriteOp(ops[0]);
            var src = RewriteOp(ops[1]);
            m.Assign(dst, op(dst, src));
        }

        private void RewriteCmp()
        {
            var dst = RewriteOp(ops[0]);
            var src = RewriteOp(ops[1]);
            var flags = frame.EnsureFlagGroup(A32Registers.cpsr, 0x1111, "NZCV", PrimitiveType.Byte);
            m.Assign(flags, m.Cond(
                m.ISub(dst, src)));
        }

        private void RewriteDmb()
        {
            m.SideEffect(host.PseudoProcedure(
                "__dmb",
                VoidType.Instance,
                Constant.String(
                    "Not Yet Implemented",
                    StringType.NullTerminated(PrimitiveType.Char))));
        }

        private void RewriteEor()
        {
            var dst = RewriteOp(ops[0]);
            var src = RewriteOp(ops[1]);
            m.Assign(dst, m.Xor(dst, src));
            if (instr.ArchitectureDetail.UpdateFlags)
                m.Assign(FlagGroup(FlagM.NF | FlagM.ZF | FlagM.CF, "NZC", PrimitiveType.Byte),
                    m.Cond(dst));
        }

     
        private void RewriteLdr(DataType dtDst, DataType dtSrc)
        {
            var mem = ops[1].MemoryValue;
            Expression dst = RewriteOp(ops[0]);
            Expression src;
            if (ops.Length == 2 && instr.ArchitectureDetail.WriteBack)
            {
                // Pre-index operand.
                Expression baseReg = GetReg(mem.BaseRegister);
                Expression ea = EffectiveAddress(mem);
                Predicate(CurrentItStateCondition, baseReg, ea);
                src = m.Load(dtSrc, ea);
            }
            else
            {
                src = RewriteOp(ops[1], dtSrc);
            }
            if (dtDst != dtSrc)
                src = m.Cast(dtDst, src);
            if (ops.Length == 3)
            {
                // Post-index operand.
                var tmp = frame.CreateTemporary(dtDst);
                var baseReg = GetReg(ops[1].MemoryValue.BaseRegister);
                Predicate(CurrentItStateCondition, tmp, src);
                Predicate(CurrentItStateCondition, baseReg, m.IAdd(baseReg, RewriteOp(ops[2])));
                src = tmp;
            }
            Predicate(CurrentItStateCondition, dst, src);
        }

        private void RewriteLdrex()
        {
            m.SideEffect(host.PseudoProcedure(
                "__ldrex",
                VoidType.Instance,
                Constant.String(
                    "Not Yet Implemented",
                    StringType.NullTerminated(PrimitiveType.Char))));
        }

        private void RewriteShift(Func<Expression,Expression, Expression> ctor)
        {
            var dst = RewriteOp(ops[0]);
            var src1 = RewriteOp(ops[1]);
            var src2 = RewriteOp(ops[2]);
            m.Assign(dst, ctor(src1, src2));
            if (instr.ArchitectureDetail.UpdateFlags)
            {
                m.Assign(frame.EnsureFlagGroup(A32Registers.cpsr, 0xF, "NZCV", PrimitiveType.Byte), m.Cond(dst));
            }
        }

        private void RewriteMov()
        {
            var dst = GetReg(ops[0].RegisterValue.Value);
            var src = RewriteOp(ops[1]);
            Predicate(CurrentItStateCondition, new RtlAssignment(dst, src));
        }

        private void RewriteMovt()
        {
            var dst = GetReg(ops[0].RegisterValue.Value);
            var src = Constant.Word16((ushort)ops[1].ImmediateValue.Value);
            m.Assign(dst, m.Dpb(dst, src, 16));
        }

        private void RewriteMovw()
        {
            var dst = GetReg(ops[0].RegisterValue.Value);
            var src = Constant.Word32((ushort)ops[1].ImmediateValue.Value);
            m.Assign(dst, src);
        }

        private void RewriteMrc()
        {
            m.SideEffect(host.PseudoProcedure(
                "__mrc",
                VoidType.Instance,
                Constant.String(
                    "Not Yet Implemented",
                    StringType.NullTerminated(PrimitiveType.Char))));
        }

        private void RewriteMvn()
        {
            var dst = RewriteOp(ops[0]);
            m.Assign(dst, m.Comp(RewriteOp(ops[1], PrimitiveType.UInt32)));
        }

        private void RewritePop()
        {
            var sp = frame.EnsureRegister(A32Registers.sp);
            m.Assign(sp, m.IAdd(sp, Constant.Int32(ops.Length * 4)));
            int offset = ops.Length * 4;
            foreach (var op in ops.OrderByDescending(o => o.RegisterValue.Value))
            {
                Predicate(
                    ArmCodeCondition.AL,
                    GetReg(op.RegisterValue.Value),
                    m.LoadDw(m.ISub(sp, Constant.Int32(offset))));
                offset -= 4;
            }
        }

        private void RewritePush()
        {
            var sp = frame.EnsureRegister(A32Registers.sp);
            m.Assign(sp, m.ISub(sp, Constant.Int32( ops.Length * 4)));
            int offset = 0;
            foreach (var op in ops.OrderByDescending(o => o.RegisterValue.Value))
            {
                Predicate(
                    ArmCodeCondition.AL,
                    m.LoadDw(m.IAdd(sp, Constant.Int32(offset))),
                    GetReg(op.RegisterValue.Value));
                offset += 4;
            }
        }

        private void RewriteRsb()
        {
            var dst = RewriteOp(ops[0]);
            var src1 = RewriteOp(ops[2]);    // _R_everse subtract/
            var src2 = RewriteOp(ops[1]);
            m.Assign(dst, m.ISub(src1, src2));
        }

        private void RewriteStm()
        {
            var ptr = RewriteOp(ops[0]);
            int offset = 0;
            foreach (var op in ops.Skip(1).OrderBy(o => o.RegisterValue.Value))
            {
                m.Assign(
                    m.LoadDw(m.IAdd(ptr, Constant.Int32(offset))),
                    GetReg(op.RegisterValue.Value));
                offset += 4;
            }
            if (instr.ArchitectureDetail.WriteBack)
            {
                m.Assign(ptr, m.IAdd(ptr, Constant.Int32(offset)));
            }
        }

        private void RewriteStr(DataType dtDst)
        {
            var src = RewriteOp(ops[0]);
            if (dtDst.Size < PrimitiveType.Word32.Size)
                src = m.Cast(dtDst, src);
            var dst = RewriteOp(ops[1], dtDst);
            var mem = ops[1].MemoryValue;
            if (ops.Length == 2 && instr.ArchitectureDetail.WriteBack)
            {
                // Pre-index operand.
                Expression baseReg = GetReg(mem.BaseRegister);
                Expression ea = EffectiveAddress(mem);
                Predicate(CurrentItStateCondition, baseReg, ea);
                dst = m.Load(dtDst, baseReg);
            }
            else
            {
                dst = RewriteOp(ops[1], dtDst);
            }
            Predicate(CurrentItStateCondition, dst, src);
            if (ops.Length == 3)
            {
                var baseReg = GetReg(mem.BaseRegister);
                Predicate(CurrentItStateCondition, baseReg, m.IAdd(baseReg, RewriteOp(ops[2])));
            }
        }

        private void RewriteStrex()
        {
            m.SideEffect(host.PseudoProcedure(
                "__strex",
                VoidType.Instance,
                Constant.String(
                    "Not Yet Implemented",
                    StringType.NullTerminated(PrimitiveType.Char))));
        }


        private void RewriteAddw()
        {
            var dst = RewriteOp(ops[0]);
            var src1 = RewriteOp(ops[1]);
            var src2 = RewriteOp(ops[2]);
            Predicate(CurrentItStateCondition, new RtlAssignment(dst, m.IAdd(src1, src2)));
        }

        private void RewriteSubw()
        {
            var dst = RewriteOp(ops[0]);
            var src1 = RewriteOp(ops[1]);
            var src2 = RewriteOp(ops[2]);
            Predicate(CurrentItStateCondition, new RtlAssignment(dst, m.ISub(src1, src2)));
        }

        private void RewriteTst()
        {
            var dst = RewriteOp(ops[0]);
            var src = RewriteOp(ops[1], PrimitiveType.UInt32);
            m.Assign(
                FlagGroup(FlagM.NF | FlagM.ZF | FlagM.CF, "NZC", PrimitiveType.Byte),
                m.Cond(m.And(dst, src)));
        }

        private void RewriteUxth()
        {
            var dst = RewriteOp(ops[0]);
            var src = RewriteOp(ops[1]);
            Predicate(
                CurrentItStateCondition,
                dst,
                m.Cast(
                    PrimitiveType.UInt32,
                    m.Cast(
                        PrimitiveType.UInt16,
                        src)));
        }
    }
}
