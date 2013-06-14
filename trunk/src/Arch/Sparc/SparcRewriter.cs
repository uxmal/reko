#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Arch.Sparc;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Sparc
{
    public class SparcRewriter : IEnumerable<RtlInstructionCluster>
    {
        private SparcArchitecture arch;
        private ImageReader rdr;
        private SparcProcessorState state;
        private Frame frame;
        private IRewriterHost host;
        private LookaheadEnumerator<DisassembledInstruction> dasm;
        private DisassembledInstruction di;
        private RtlEmitter emitter;
        private RtlInstructionCluster ric;

        private class DisassembledInstruction 
        {
            public Address Address;
            public SparcInstruction Instr;
        }

        public SparcRewriter(SparcArchitecture arch, ImageReader rdr, SparcProcessorState state, Frame frame, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.frame = frame;
            this.host = host;
            this.dasm = new LookaheadEnumerator<DisassembledInstruction>(CreateDisassemblyStream(rdr));
        }

        private IEnumerator<DisassembledInstruction> CreateDisassemblyStream(ImageReader rdr)
        {
            var d = new SparcDisassembler(arch, rdr);
            while (rdr.IsValid)
            {
                var addr = d.Address;
                var instr = d.Disassemble();
                yield return new DisassembledInstruction { Address = addr, Instr = instr };
            }
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                di = dasm.Current;
                ric = new RtlInstructionCluster(di.Address, 4);
                emitter = new RtlEmitter(ric.Instructions);
                switch (di.Instr.Opcode)
                {
                default: 
                    throw new AddressCorrelatedException(string.Format("Rewriting SPARC opcode '{0}' is not supported yet.",
                        di.Instr.Opcode),
                        di.Address);
                case Opcode.add: RewriteAlu(Operator.Add); break;
                case Opcode.addcc: RewriteAluCc(Operator.Add); break;
                case Opcode.and: RewriteAlu(Operator.And); break;
                case Opcode.andcc: RewriteAluCc(Operator.And); break;
                case Opcode.call: RewriteCall(); break;
                case Opcode.or: RewriteAlu(Operator.Or); break;
                case Opcode.orcc: RewriteAluCc(Operator.Or); break;
                }
                yield return ric;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void RewriteAlu(Operator op)
        {
            var dst = RewriteOp(di.Instr.Op3);
            var src1 = RewriteOp( di.Instr.Op1);
            var src2 = RewriteOp( di.Instr.Op2);
            emitter.Assign(dst, new BinaryExpression(op, PrimitiveType.Word32, src1, src2));
        }

        private void RewriteAluCc(Operator op)
        {
            RewriteAlu(op);
            var dst = RewriteOp(di.Instr.Op3);
            emitter.Assign(
                frame.EnsureFlagGroup(0xF, "NZVC", PrimitiveType.Byte),
                emitter.Cond(dst));
        }
        private void RewriteCall()
        {
            emitter.Call(((AddressOperand) di.Instr.Op1).Address , 0);
        }

        private Expression RewriteOp(MachineOperand op)
        {
            var r = op as RegisterOperand;
            if (r != null)
                return frame.EnsureRegister(r.Register);
            var imm = op as ImmediateOperand;
            if (imm != null)
                return imm.Value;
            throw new NotImplementedException(string.Format("Unsupported operand {0} ({1})", op, op.GetType().Name));
        }
    }
}
