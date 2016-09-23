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
            emitter.Assign(dst, src);
        }

        private void RewriteAnd()
        {
            var dst = RewriteOp(ops[0]);
            var src = RewriteOp(ops[1]);
            emitter.Assign(dst, emitter.And(dst, src));
            if (instr.ArchitectureDetail.UpdateFlags)
                emitter.Assign(FlagGroup(FlagM.NF | FlagM.ZF | FlagM.CF, "NZC", PrimitiveType.Byte),
                    emitter.Cond(dst));
        }

    
        private void RewriteBic()
        {
            var dst = RewriteOp(ops[0]);
            var src1 = RewriteOp(ops[1]);
            var src2 = RewriteOp(ops[2]);
            emitter.Assign(dst, emitter.And(src1, emitter.Comp(src2)));
        }

        private void RewriteBinop(Func<Expression, Expression, Expression> op)
        {
            var dst = RewriteOp(ops[0]);
            var src = RewriteOp(ops[1]);
            emitter.Assign(dst, op(dst, src));
        }

        private void RewriteCmp()
        {
            var dst = RewriteOp(ops[0]);
            var src = RewriteOp(ops[1]);
            var flags = frame.EnsureFlagGroup(A32Registers.cpsr, 0x1111, "NZCV", PrimitiveType.Byte);
            emitter.Assign(flags, emitter.Cond(
                emitter.ISub(dst, src)));
        }

        private void RewriteDmb()
        {
            emitter.SideEffect(host.PseudoProcedure(
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
            emitter.Assign(dst, emitter.Xor(dst, src));
            if (instr.ArchitectureDetail.UpdateFlags)
                emitter.Assign(FlagGroup(FlagM.NF | FlagM.ZF | FlagM.CF, "NZC", PrimitiveType.Byte),
                    emitter.Cond(dst));
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
                Predicate(itStateCondition, baseReg, ea);
                src = emitter.Load(dtSrc, ea);
            }
            else
            {
                src = RewriteOp(ops[1], dtSrc);
            }
            if (dtDst != dtSrc)
                src = emitter.Cast(dtDst, src);
            if (ops.Length == 3)
            {
                // Post-index operand.
                var tmp = frame.CreateTemporary(dtDst);
                var baseReg = GetReg(ops[1].MemoryValue.BaseRegister);
                Predicate(itStateCondition, tmp, src);
                Predicate(itStateCondition, baseReg, emitter.IAdd(baseReg, RewriteOp(ops[2])));
                src = tmp;
            }
            Predicate(itStateCondition, dst, src);
        }

        private void RewriteLdrex()
        {
            emitter.SideEffect(host.PseudoProcedure(
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
            emitter.Assign(dst, ctor(src1, src2));
            if (instr.ArchitectureDetail.UpdateFlags)
            {
                emitter.Assign(frame.EnsureFlagGroup(A32Registers.cpsr, 0xF, "NZCV", PrimitiveType.Byte), emitter.Cond(dst));
            }
        }

        private void RewriteMov()
        {
            var dst = GetReg(ops[0].RegisterValue.Value);
            var src = RewriteOp(ops[1]);
            Predicate(itStateCondition, new RtlAssignment(dst, src));
        }

        private void RewriteMovt()
        {
            var dst = GetReg(ops[0].RegisterValue.Value);
            var src = Constant.Word16((ushort)ops[1].ImmediateValue.Value);
            emitter.Assign(dst, emitter.Dpb(dst, src, 16));
        }

        private void RewriteMovw()
        {
            var dst = GetReg(ops[0].RegisterValue.Value);
            var src = Constant.Word32((ushort)ops[1].ImmediateValue.Value);
            emitter.Assign(dst, src);
        }

        private void RewriteMrc()
        {
            emitter.SideEffect(host.PseudoProcedure(
                "__mrc",
                VoidType.Instance,
                Constant.String(
                    "Not Yet Implemented",
                    StringType.NullTerminated(PrimitiveType.Char))));
        }

        private void RewriteMvn()
        {
            var dst = RewriteOp(ops[0]);
            emitter.Assign(dst, emitter.Comp(RewriteOp(ops[1], PrimitiveType.UInt32)));
        }

        private void RewritePop()
        {
            var sp = frame.EnsureRegister(A32Registers.sp);
            emitter.Assign(sp, emitter.IAdd(sp, Constant.Int32(ops.Length * 4)));
            int offset = ops.Length * 4;
            foreach (var op in ops.OrderByDescending(o => o.RegisterValue.Value))
            {
                Predicate(
                    ArmCodeCondition.AL,
                    GetReg(op.RegisterValue.Value),
                    emitter.LoadDw(emitter.ISub(sp, Constant.Int32(offset))));
                offset -= 4;
            }
        }

        private void RewritePush()
        {
            var sp = frame.EnsureRegister(A32Registers.sp);
            emitter.Assign(sp, emitter.ISub(sp, Constant.Int32( ops.Length * 4)));
            int offset = 0;
            foreach (var op in ops.OrderByDescending(o => o.RegisterValue.Value))
            {
                Predicate(
                    ArmCodeCondition.AL,
                    emitter.LoadDw(emitter.IAdd(sp, Constant.Int32(offset))),
                    GetReg(op.RegisterValue.Value));
                offset += 4;
            }
        }

        private void RewriteRsb()
        {
            var dst = RewriteOp(ops[0]);
            var src1 = RewriteOp(ops[2]);    // _R_everse subtract/
            var src2 = RewriteOp(ops[1]);
            emitter.Assign(dst, emitter.ISub(src1, src2));
        }

        private void RewriteStm()
        {
            var ptr = RewriteOp(ops[0]);
            int offset = 0;
            foreach (var op in ops.Skip(1).OrderBy(o => o.RegisterValue.Value))
            {
                emitter.Assign(
                    emitter.LoadDw(emitter.IAdd(ptr, Constant.Int32(offset))),
                    GetReg(op.RegisterValue.Value));
                offset += 4;
            }
            if (instr.ArchitectureDetail.WriteBack)
            {
                emitter.Assign(ptr, emitter.IAdd(ptr, Constant.Int32(offset)));
            }
        }

        private void RewriteStr(DataType dtDst)
        {
            var src = RewriteOp(ops[0]);
            if (dtDst.Size < PrimitiveType.Word32.Size)
                src = emitter.Cast(dtDst, src);
            var dst = RewriteOp(ops[1], dtDst);
            var mem = ops[1].MemoryValue;
            if (ops.Length == 2 && instr.ArchitectureDetail.WriteBack)
            {
                // Pre-index operand.
                Expression baseReg = GetReg(mem.BaseRegister);
                Expression ea = EffectiveAddress(mem);
                Predicate(itStateCondition, baseReg, ea);
                dst = emitter.Load(dtDst, baseReg);
            }
            else
            {
                dst = RewriteOp(ops[1], dtDst);
            }
            Predicate(itStateCondition, dst, src);
            if (ops.Length == 3)
            {
                var baseReg = GetReg(mem.BaseRegister);
                Predicate(itStateCondition, baseReg, emitter.IAdd(baseReg, RewriteOp(ops[2])));
            }
        }

        private void RewriteStrex()
        {
            emitter.SideEffect(host.PseudoProcedure(
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
            Predicate(itStateCondition, new RtlAssignment(dst, emitter.IAdd(src1, src2)));
        }

        private void RewriteSubw()
        {
            var dst = RewriteOp(ops[0]);
            var src1 = RewriteOp(ops[1]);
            var src2 = RewriteOp(ops[2]);
            Predicate(itStateCondition, new RtlAssignment(dst, emitter.ISub(src1, src2)));
        }

        private void RewriteTst()
        {
            var dst = RewriteOp(ops[0]);
            var src = RewriteOp(ops[1], PrimitiveType.UInt32);
            emitter.Assign(
                FlagGroup(FlagM.NF | FlagM.ZF | FlagM.CF, "NZC", PrimitiveType.Byte),
                emitter.Cond(emitter.And(dst, src)));
        }

        private void RewriteUxth()
        {
            var dst = RewriteOp(ops[0]);
            var src = RewriteOp(ops[1]);
            Predicate(
                itStateCondition,
                dst,
                emitter.Cast(
                    PrimitiveType.UInt32,
                    emitter.Cast(
                        PrimitiveType.UInt16,
                        src)));
        }
    }
}
