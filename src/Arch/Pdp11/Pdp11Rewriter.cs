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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Pdp11
{
    public class Pdp11Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private Pdp11Architecture arch;
        private IEnumerator<Pdp11Instruction> instrs;
        private Frame frame;
        private RtlInstructionCluster rtlCluster;
        private RtlEmitter emitter;

        private Pdp11Rewriter(Pdp11Architecture arch)
        {
            this.arch = arch;
        }

        public Pdp11Rewriter(Pdp11Architecture arch, IEnumerable<Pdp11Instruction> instrs, Frame frame)
        {
            this.arch = arch;
            this.instrs = instrs.GetEnumerator();
            this.frame = frame;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (instrs.MoveNext())
            {
                var instr = instrs.Current;
                this.rtlCluster = new RtlInstructionCluster(instr.Address, instr.Length);
                this.rtlCluster.Class = RtlClass.Linear;
                emitter = new RtlEmitter(rtlCluster.Instructions);
                Expression src;
                Expression dst;
                switch (instr.Opcode)
                {
                default: throw new AddressCorrelatedException(
                    instr.Address,
                    "Rewriting of PDP-11 instruction {0} not supported yet.", instr.Opcode);
                case Opcodes.clrb:
                    dst = RewriteDst(instr.op1, emitter.Byte(0), s => s);
                    SetFlags(dst, 0, FlagM.NF | FlagM.CF | FlagM.VF, FlagM.ZF);
                    break;
                case Opcodes.mov:
                    src = RewriteSrc(instr.op1);
                    dst = RewriteDst(instr.op2, src, s => s);
                    SetFlags(dst, FlagM.ZF | FlagM.NF, FlagM.VF, 0);
                    break;
                case Opcodes.movb:
                    src = RewriteSrc(instr.op1);
                    dst = RewriteDst(instr.op2, src, s => emitter.Cast(PrimitiveType.Int16, s));
                    SetFlags(dst, FlagM.ZF | FlagM.NF, FlagM.VF, 0);
                    break;
                case Opcodes.xor:
                    src = RewriteSrc(instr.op1);
                    dst = RewriteDst(instr.op2, src, (s, d) => emitter.Xor(d, s));
                    SetFlags(dst, FlagM.ZF | FlagM.NF, FlagM.CF| FlagM.VF, 0);
                    break;
                }
                yield return rtlCluster;
            }
        }

        private void SetFlags(Expression e, FlagM changed, FlagM zeroed, FlagM set)
        {
            uint uChanged = (uint)changed;
            if (uChanged != 0)
            {
                var grfChanged = frame.EnsureFlagGroup(this.arch.GetFlagGroup(uChanged));
                emitter.Assign(grfChanged, emitter.Cond(e));
            }
            uint grfMask = 1;
            while (grfMask <= (uint)zeroed)
            {
                if ((grfMask & (uint)zeroed) != 0)
                {
                    var grfZeroed = frame.EnsureFlagGroup(this.arch.GetFlagGroup(grfMask));
                    emitter.Assign(grfZeroed, 0);
                }
                grfMask <<= 1;
            }
            grfMask = 1;
            while (grfMask <= (uint)set)
            {
                if ((grfMask & (uint)set) != 0)
                {
                    var grfZeroed = frame.EnsureFlagGroup(this.arch.GetFlagGroup(grfMask));
                    emitter.Assign(grfZeroed, 1);
                }
                grfMask <<= 1;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Expression RewriteSrc(MachineOperand op)
        {
            var memOp = op as MemoryOperand;
            if (memOp != null)
            {
                var r = frame.EnsureRegister(memOp.Register);
                var tmp = frame.CreateTemporary(op.Width);
                switch (memOp.Mode)
                {
                default:
                    throw new NotImplementedException(string.Format("Not implemented: addressing mode {0}.", memOp.Mode));
                case AddressMode.RegDef:
                    return emitter.Load(this.instrs.Current.DataWidth, r);
                case AddressMode.AutoIncr:
                    emitter.Assign(tmp, emitter.Load(op.Width, r));
                    emitter.Assign(r, emitter.IAdd(r, memOp.Width.Size));
                    break;
                }
                return tmp;
            }
            var regOp = op as RegisterOperand;
            if (regOp != null)
            {
                return frame.EnsureRegister(regOp.Register);
            }
            throw new NotImplementedException();
        }

        private Expression RewriteDst(MachineOperand op, Expression src, Func<Expression, Expression> gen)
        {
            var regOp = op as RegisterOperand;
            if (regOp != null)
            {
                var dst = frame.EnsureRegister(regOp.Register);
                emitter.Assign(dst, gen(src));
                return dst;
            }
            var memOp = op as MemoryOperand;
            if (memOp != null)
            {
                var r = frame.EnsureRegister(memOp.Register);
                var tmp = frame.CreateTemporary(instrs.Current.DataWidth);
                switch (memOp.Mode)
                {
                default:
                    throw new NotImplementedException(string.Format("Not implemented: addressing mode {0}.", memOp.Mode));
                case AddressMode.AutoIncr:
                    emitter.Assign(tmp, gen(src));
                    emitter.Assign(emitter.Load(tmp.DataType, r), tmp);
                    emitter.Assign(r, emitter.IAdd(r, tmp.DataType.Size));
                    break;
                }
                return tmp;
            }
            throw new NotImplementedException(string.Format("Not implemented: addressing mode {0}.", op.GetType().Name));

        }

        private Expression RewriteDst(MachineOperand op, Expression src, Func<Expression, Expression, Expression> gen)
        {
            var regOp = op as RegisterOperand;
            if (regOp != null)
            {
                var dst = frame.EnsureRegister(regOp.Register);
                emitter.Assign(dst, gen(src, dst));
                return dst;
            }
            var memOp = op as MemoryOperand;
            if (memOp != null)
            {
                var r = frame.EnsureRegister(memOp.Register);
                var tmp = frame.CreateTemporary(instrs.Current.DataWidth);
                switch (memOp.Mode)
                {
                default:
                    throw new NotImplementedException(string.Format("Not implemented: addressing mode {0}.", memOp.Mode));
                case AddressMode.AutoIncr:
                    emitter.Assign(tmp, gen(src, emitter.Load(tmp.DataType, r)));
                    emitter.Assign(emitter.Load(tmp.DataType, r), tmp);
                    emitter.Assign(r, emitter.IAdd(r, tmp.DataType.Size));
                    break;
                }
                return tmp;
            }
            throw new NotImplementedException(string.Format("Not implemented: addressing mode {0}.", op.GetType().Name));
        }
    }
}
