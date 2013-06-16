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
    public partial class SparcRewriter : IEnumerable<RtlInstructionCluster>
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

        public class DisassembledInstruction 
        {
            public Address Address;
            public SparcInstruction Instr;
        }

        public SparcRewriter(SparcArchitecture arch, ImageReader rdr, SparcProcessorState state, Frame frame, IRewriterHost host)
        {
            this.arch = arch;
            this.state = state;
            this.frame = frame;
            this.host = host;
            this.dasm = new LookaheadEnumerator<DisassembledInstruction>(CreateDisassemblyStream(rdr));
        }

        public SparcRewriter(SparcArchitecture arch, IEnumerable<DisassembledInstruction> instrs, SparcProcessorState state, Frame frame, IRewriterHost host)
        {
            this.arch = arch;
            this.state = state;
            this.frame = frame;
            this.host = host;
            this.dasm = new LookaheadEnumerator<DisassembledInstruction>(instrs);
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
                case Opcode.add: RewriteAlu(Operator.IAdd); break;
                case Opcode.addcc: RewriteAluCc(Operator.IAdd); break;
                case Opcode.and: RewriteAlu(Operator.And); break;
                case Opcode.andcc: RewriteAluCc(Operator.And); break;
                case Opcode.call: RewriteCall(); break;
                case Opcode.fitod: RewriteFitod(); break;
                case Opcode.fitoq: RewriteFitoq(); break;
                case Opcode.fitos: RewriteFitos(); break;
                case Opcode.ldsb: RewriteLoad(PrimitiveType.SByte); break;
                case Opcode.or: RewriteAlu(Operator.Or); break;
                case Opcode.orcc: RewriteAluCc(Operator.Or); break;
                case Opcode.sethi: RewriteSethi(); break;
                case Opcode.sdiv: RewriteAlu(Operator.SDiv); break;
                case Opcode.sdivcc: RewriteAlu(Operator.SDiv); break;
                case Opcode.sll: RewriteAlu(Operator.Shl); break;
                case Opcode.smul: RewriteAlu(Operator.SMul); break;
                case Opcode.smulcc: RewriteAlu(Operator.SMul); break;
                case Opcode.sth: RewriteStore(PrimitiveType.Word16); break;
                case Opcode.udiv: RewriteAlu(Operator.UDiv); break;
                case Opcode.udivcc: RewriteAluCc(Operator.UDiv); break;
                case Opcode.umul: RewriteAlu(Operator.UMul); break;
                case Opcode.umulcc: RewriteAluCc(Operator.UMul); break;
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
            var src1 = RewriteOp(di.Instr.Op1);
            var src2 = RewriteOp(di.Instr.Op2);
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
            emitter.Call(((AddressOperand) di.Instr.Op1).Address , 0, false);
        }

        private void RewriteLoad(PrimitiveType size)
        {
            var dst = RewriteOp(di.Instr.Op2);
            var src = RewriteMemOp(di.Instr.Op1, size);
            if (size.Size < dst.DataType.Size)
            {
                size = (size.Domain == Domain.SignedInt) ? PrimitiveType.Int32 : PrimitiveType.Word32;
                src = emitter.Cast(size, src);
            }
            emitter.Assign(dst, src);
        }

        private Expression RewriteOp(MachineOperand op)
        {
            var r = op as RegisterOperand;
            if (r != null)
            {
                if (r.Register == Registers.g0)
                    return Constant.Zero(PrimitiveType.Word32);
                else 
                    return frame.EnsureRegister(r.Register);
            }
            var imm = op as ImmediateOperand;
            if (imm != null)
                return imm.Value;
            throw new NotImplementedException(string.Format("Unsupported operand {0} ({1})", op, op.GetType().Name));
        }

        private Expression RewriteMemOp(MachineOperand op, PrimitiveType size)
        {
            var m = op as MemoryOperand;
            Expression baseReg;
            Expression offset;
            if (m != null)
            {
                baseReg = m.Base == Registers.g0 ? null : frame.EnsureRegister(m.Base);
                offset = m.Offset.IsIntegerZero ? null : m.Offset;
            }
            else
            {
                var i = op as IndexedMemoryOperand;
                if (i != null)
                {
                    baseReg = i.Base == Registers.g0 ? null : frame.EnsureRegister(i.Base);
                    offset = i.Index == Registers.g0 ? null : frame.EnsureRegister(i.Index);
                }
                else
                    throw new NotImplementedException(string.Format("Unknown memory operand {0} ({1})", op, op.GetType().Name));
            }
            Expression ea;
            if (baseReg == null && offset == null)
                ea = Constant.Zero(PrimitiveType.Pointer32);
            else if (baseReg == null)
                ea = offset;
            else if (offset == null)
                ea = baseReg;
            else
                ea = emitter.IAdd(baseReg, offset);
            return new MemoryAccess(ea, size);
        }

        private void RewriteSethi()
        {
            var dst = RewriteOp(di.Instr.Op2);
            var src = (ImmediateOperand) di.Instr.Op1;
            emitter.Assign(dst, Constant.Word32(src.Value.ToUInt32() << 10));
        }

        private void RewriteStore(PrimitiveType size)
        {
            var src = RewriteOp(di.Instr.Op1);
            var dst = RewriteMemOp(di.Instr.Op2, size);
            if (size.Size < src.DataType.Size)
                src = emitter.Cast(size, src);
            emitter.Assign(dst, src);
        }
    }
}
