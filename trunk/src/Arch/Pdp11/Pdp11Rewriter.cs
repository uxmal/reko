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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.Pdp11
{
    public class Pdp11Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private Pdp11Architecture arch;
        private IEnumerator<Pdp11Instruction> instrs;
        private Frame frame;
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
                var rtlCluster = new RtlInstructionCluster(instr.Address, instr.Length);
                emitter = new RtlEmitter(rtlCluster.Instructions);
                switch (instr.Opcode)
                {
                default: throw new AddressCorrelatedException(
                    instr.Address,
                    "Rewriting of PDP-11 instruction {0} not supported yet.", instr.Opcode);
                case Opcodes.xor:
                    var src = RewriteSrc(instr.op1);
                    var dst = RewriteDst(instr.op2);
                    emitter.Assign(dst, emitter.Xor(dst, src));
                    emitter.Assign(
                        frame.EnsureFlagGroup((uint) (FlagM.NF | FlagM.ZF), "NZ", PrimitiveType.Byte), 
                        emitter.Cond(dst));
                    emitter.Assign(frame.EnsureFlagGroup((uint) FlagM.CF, "C", PrimitiveType.Bool), Constant.False());
                    emitter.Assign(frame.EnsureFlagGroup((uint) FlagM.VF, "V", PrimitiveType.Bool), Constant.False());
                    break;
                }
                yield return rtlCluster;
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
                if (memOp.Mode == AddressMode.AutoIncr)
                {
                    emitter.Assign(tmp, emitter.Load(op.Width, r));
                    emitter.Assign(r, emitter.IAdd(r, memOp.Width.Size));
                    return tmp;
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

        private Expression RewriteDst(MachineOperand op)
        {
            return RewriteSrc(op);
        }
    }
}
