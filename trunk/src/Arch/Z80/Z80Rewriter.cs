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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Z80
{
    public class Z80Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private Z80ProcessorArchitecture arch;
        private ProcessorState state;
        private Frame frame;
        private IRewriterHost host;
        private IEnumerator<DisassembledInstruction> dasm;
        private RtlEmitter emitter;

        public Z80Rewriter(Z80ProcessorArchitecture arch, ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            this.arch = arch;
            this.state = state;
            this.frame = frame;
            this.host = host;
            this.dasm = CreateDasmStream(rdr).GetEnumerator();
        }

        private class DisassembledInstruction
        {
            public Address Address;
            public Z80Instruction Instruction;
            public int Size;
        }

        private IEnumerable<DisassembledInstruction> CreateDasmStream(ImageReader rdr)
        {
            var d = new Z80Disassembler(rdr);
            while (rdr.IsValid)
            {
                var addr = rdr.Address;
                var instr = d.Disassemble();
                yield return new DisassembledInstruction
                {
                    Address = addr,
                    Instruction = instr,
                    Size = rdr.Address  -addr,
                };
            }
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                var rtlc = new RtlInstructionCluster(dasm.Current.Address, (byte) dasm.Current.Size);
                emitter = new RtlEmitter(rtlc.Instructions);
                switch (dasm.Current.Instruction.Code)
                {
                default: throw new AddressCorrelatedException(
                    dasm.Current.Address,
                    "Rewriting of Z80 instruction {0} not implemented yet.",
                    dasm.Current.Instruction.Code);
                case Opcode.jp: RewriteJp(dasm.Current.Instruction); break;
                case Opcode.ld: emitter.Assign(
                    RewriteOp(dasm.Current.Instruction.Op1),
                    RewriteOp(dasm.Current.Instruction.Op2));
                    break;
                case Opcode.push: RewritePush(dasm.Current.Instruction); break;
                }
                yield return rtlc;
            }
        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EmitBranch(ConditionOperand cOp, ImmediateOperand dst)
        {
            ConditionCode cc = ConditionCode.ALWAYS;
            FlagM flags = 0;
            string name = "";
            switch (cOp.Code)
            {
            case CondCode.nz: cc = ConditionCode.NE; flags = FlagM.ZF; name="Z"; break;
            case CondCode.z: cc = ConditionCode.EQ; flags = FlagM.ZF; name="Z"; break;
            case CondCode.nc: cc = ConditionCode.UGE; flags = FlagM.CF; name="C"; break;
            case CondCode.c: cc = ConditionCode.ULT; flags = FlagM.CF; name="C"; break;
            case CondCode.po: cc = ConditionCode.PO; flags = FlagM.PF; name="P";break;
            case CondCode.pe: cc = ConditionCode.PE; flags = FlagM.PF; name="P";  break;
            case CondCode.p: cc = ConditionCode.NS; flags = FlagM.PF; name="S"; break;
            case CondCode.m: cc = ConditionCode.SG; flags = FlagM.PF; name="S"; break;
            }
            emitter.Branch(
                emitter.Test(cc, frame.EnsureFlagGroup((uint)flags, name, PrimitiveType.Bool)),
                Address.Ptr16(dst.Value.ToUInt16()),
                RtlClass.Transfer);
        }

        private void RewriteJp(Z80Instruction instr)
        {
            var cOp = instr.Op1 as ConditionOperand;
            if (cOp != null)
            {
                EmitBranch(cOp, (ImmediateOperand) instr.Op2);
            }
            else
            {
                var target = (ImmediateOperand) instr.Op1;
                emitter.Goto(target.Value);
            }
        }

        private Expression RewriteOp(MachineOperand op)
        {
            var rOp = op as RegisterOperand;
            if (rOp != null)
                return frame.EnsureRegister(rOp.Register);
            var immOp = op as ImmediateOperand;
            if (immOp != null)
                return immOp.Value;
            var memOp = op as MemoryOperand;
            if (memOp != null)
            {
                var bReg = frame.EnsureRegister(memOp.Base);
                if (memOp.Offset == null)
                {
                    return emitter.Load(memOp.Width, bReg);
                }
                else
                {
                    int s = memOp.Offset.ToInt32();
                    if (s > 0)
                    {
                        return emitter.Load(memOp.Width, emitter.IAdd(bReg, s));
                    }
                    else if (s < 0)
                    {
                        return emitter.Load(memOp.Width, emitter.ISub(bReg, -s));
                    }
                    else
                    {
                        return emitter.Load(memOp.Width, bReg);
                    }
                }
            }
            throw new NotImplementedException(string.Format("Rewriting of Z80 operand type {0} is not implemented yet.", op.GetType().FullName));
        }

        private void RewritePush(Z80Instruction instr)
        {
            var sp = frame.EnsureRegister(Registers.sp);
            emitter.Assign(sp, emitter.ISub(sp, 2));
            emitter.Assign(emitter.Load(PrimitiveType.Word16, sp), RewriteOp(instr.Op1));
        }
    }
}
