#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
            var flags = frame.EnsureFlagGroup(0x1111, "NZCV", PrimitiveType.Byte);
            emitter.Assign(flags, emitter.Cond(
                emitter.ISub(dst, src)));
        }

        private void RewriteDmb()
        {
            emitter.SideEffect(PseudoProc(
                "__dmb",
                VoidType.Instance,
                Constant.String(
                    "Not Yet Implemented",
                    StringType.NullTerminated(PrimitiveType.Char))));
        }

        private void RewriteLdr()
        {
            var dst = RewriteOp(ops[0]);
            var src = RewriteOp(ops[1], PrimitiveType.Word32);
            emitter.Assign(dst, src);
        }

        private void RewriteLdrPart(DataType dt)
        {
            var dst = RewriteOp(ops[0]);
            var src = RewriteOp(ops[1], dt);
            emitter.Assign(dst, emitter.Cast(PrimitiveType.UInt32, src));
        }

        private void RewriteLdrex()
        {
            emitter.SideEffect(PseudoProc(
                "__ldrex",
                VoidType.Instance,
                Constant.String(
                    "Not Yet Implemented",
                    StringType.NullTerminated(PrimitiveType.Char))));
        }

        private void RewriteLsl()
        {
            var dst = RewriteOp(ops[0]);
            var src1 = RewriteOp(ops[1]);
            var src2 = RewriteOp(ops[2]);
            emitter.Assign(dst, emitter.Shl(src1, src2));
            if (instr.ArchitectureDetail.UpdateFlags)
            {
                emitter.Assign(frame.EnsureFlagGroup(0x1111, "NZCV", PrimitiveType.Byte), emitter.Cond(dst));
            }
        }

        private void RewriteMov()
        {
            var dst = GetReg(ops[0].RegisterValue.Value);
            var src = RewriteOp(ops[1]);
            emitter.Assign(dst, src);
        }

        private void RewriteMovt()
        {
            var dst = GetReg(ops[0].RegisterValue.Value);
            var src = Constant.Word16((ushort)ops[1].ImmediateValue.Value);
            emitter.Assign(dst, emitter.Dpb(dst, src, 16, 16));
        }

        private void RewriteMovw()
        {
            var dst = GetReg(ops[0].RegisterValue.Value);
            var src = Constant.Word32((ushort)ops[1].ImmediateValue.Value);
            emitter.Assign(dst, src);
        }

        private void RewriteMrc()
        {
            emitter.SideEffect(PseudoProc(
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
            int offset = ops.Length * 4;
            foreach (var op in ops.OrderByDescending(o => o.RegisterValue.Value))
            {
                offset -= 4;
                emitter.Assign(
                    GetReg(op.RegisterValue.Value),
                    emitter.LoadDw(emitter.IAdd(sp, Constant.Int32(offset))));
            }
            emitter.Assign(sp, emitter.IAdd(sp, Constant.Int32( ops.Length * 4)));
        }

        private void RewritePush()
        {
            var sp = frame.EnsureRegister(A32Registers.sp);
            emitter.Assign(sp, emitter.ISub(sp, Constant.Int32( ops.Length * 4)));
            int offset = 0;
            foreach (var op in ops.OrderBy(o => o.RegisterValue.Value))
            {
                emitter.Assign(
                    emitter.LoadDw(emitter.IAdd(sp, Constant.Int32(offset))),
                    GetReg(op.RegisterValue.Value));
                offset += 4;
            }
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

        private void RewriteStr()
        {
            var src = RewriteOp(ops[0]);
            var dst = RewriteOp(ops[1], PrimitiveType.Word32);
            emitter.Assign(dst, src);
        }

        private void RewriteStrb()
        {
            var src = emitter.Cast(PrimitiveType.Byte, RewriteOp(ops[0]));
            var dst = RewriteOp(ops[1], PrimitiveType.Byte);
            emitter.Assign(dst, src);
        }

        private void RewriteStrex()
        {
            emitter.SideEffect(PseudoProc(
                "__strex",
                VoidType.Instance,
                Constant.String(
                    "Not Yet Implemented",
                    StringType.NullTerminated(PrimitiveType.Char))));
        }

        private void RewriteTst()
        {
            var dst = RewriteOp(ops[0]);
            var src = RewriteOp(ops[1], PrimitiveType.UInt32);
            emitter.Assign(
                FlagGroup(FlagM.NF | FlagM.ZF | FlagM.CF, "NZC", PrimitiveType.Byte),
                emitter.Cond(emitter.And(dst, src)));
        }
    }
}
