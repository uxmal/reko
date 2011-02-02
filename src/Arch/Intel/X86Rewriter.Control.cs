#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.Intel
{
    public partial class X86Rewriter
    {
        private TestCondition CreateTestCondition(ConditionCode cc, Identifier identifier)
        {
            var tc = new TestCondition(cc, identifier);
            return tc;
            /*
            if (i < 2)
                return tc;
            if (instrs[i-1].code != Opcode.test)
                return tc;
            var ah = instrs[i-1].op1 as RegisterOperand;
            if (ah == null || ah.Register != Registers.ah)
                return tc;
            var m = instrs[i-1].op2 as ImmediateOperand;
            if (m == null)
                return tc;

            if (instrs[i-2].code != Opcode.fstsw)
                return tc;
            int mask = m.Value.ToInt32();

            var fpuf = orw.FlagGroup(FlagM.FPUF);
            switch (cc)
            {
            case ConditionCode.PE:
                if (mask == 0x05) return new TestCondition(ConditionCode.LE, fpuf);
                if (mask == 0x41) return new TestCondition(ConditionCode.GE, fpuf);
                if (mask == 0x44) return new TestCondition(ConditionCode.NE, fpuf);
                break;
            case ConditionCode.PO:
                if (mask == 0x44) return new TestCondition(ConditionCode.EQ, fpuf);
                if (mask == 0x41) return new TestCondition(ConditionCode.GE, fpuf);
                if (mask == 0x05) return new TestCondition(ConditionCode.GT, fpuf);
                break;
            case ConditionCode.EQ:
                if (mask == 0x40) return new TestCondition(ConditionCode.NE, fpuf);
                if (mask == 0x41) return new TestCondition(ConditionCode.LT, fpuf);
                break;
            case ConditionCode.NE:
                if (mask == 0x40) return new TestCondition(ConditionCode.EQ, fpuf);
                if (mask == 0x41) return new TestCondition(ConditionCode.GE, fpuf);
                if (mask == 0x01) return new TestCondition(ConditionCode.GT, fpuf);
                break;
            }
            throw new NotImplementedException(string.Format(
                "FSTSW/TEST AH,0x{0:X2}/J{1} not implemented.", mask, cc));
             */
        }

        private void RewriteCall(MachineOperand callTarget, PrimitiveType opsize)
        {
            Address addr = OperandAsCodeAddress(callTarget);
            if (addr != null)
            {
                emitter.Call(addr, (byte) opsize.Size);
            }
            else
            {
                var target = SrcOp(callTarget);
                if (target.DataType.Size == 2)
                    target = emitter.Seq(orw.AluRegister(Registers.cs), target);
                emitter.Call(target, (byte) opsize.Size);
            }
        }

        private void RewriteConditionalGoto(ConditionCode cc, MachineOperand op1)
        {
            emitter.Branch(CreateTestCondition(cc, orw.FlagGroup(IntelInstruction.UseCc(di.Instruction.code))), OperandAsCodeAddress(op1));
        }


        private void RewriteInt()
        {
            emitter.SideEffect(PseudoProc("__syscall", PrimitiveType.Void, SrcOp(di.Instruction.op1)));
        }

        private void RewriteJcxz()
        {
            emitter.Branch(emitter.Eq0(orw.AluRegister(Registers.ecx, di.Instruction.dataWidth)), OperandAsCodeAddress(di.Instruction.op1));
        }

        private void RewriteJmp()
        {
            if (IsRealModeReboot(di.Instruction))
			{
                PseudoProcedure reboot = host.EnsurePseudoProcedure("__bios_reboot", PrimitiveType.Void, 0);
                reboot.Characteristics = new Decompiler.Core.Serialization.ProcedureCharacteristics();
                reboot.Characteristics.Terminates = true;
                emitter.SideEffect(PseudoProc(reboot, PrimitiveType.Void));
				return;
			}
				
			if (di.Instruction.op1 is ImmediateOperand)
			{
				Address addr = OperandAsCodeAddress(di.Instruction.op1);
                emitter.Goto(addr);
				return;
			}
            emitter.Goto(SrcOp(di.Instruction.op1));
        }

        private void RewriteLoop(FlagM useFlags, ConditionCode cc)
        {
            Identifier cx = orw.AluRegister(Registers.ecx, di.Instruction.dataWidth);
            emitter.Assign(cx, emitter.Sub(cx, 1));
            if (useFlags != 0)
            {
                emitter.Branch(new BinaryExpression(Operator.Cand, PrimitiveType.Bool,
                    new TestCondition(cc, orw.FlagGroup(useFlags)),
                    emitter.Ne0(cx)),
                    OperandAsCodeAddress(di.Instruction.op1));
                // new TestCondition(ConditionCode.NE, orw.FlagGroup(FlagM.ZF)
                //CodeEmitter e = emitter;

                //// Splice in a new block.

                //blockNew = proc.AddBlock(state.InstructionAddress.GenerateName("l", "_loop"));
                //proc.AddEdge(blockHead, blockNew);

                //emitter = ProcedureRewriter.CreateEmitter(blockNew);
                //Block tgt = EmitBranchInstruction(emitter.Eq0(cx), instrCur.op1);
                //e.Branch(new TestCondition(cc, orw.FlagGroup(useFlags)), tgt);
            }
            else
            {
                emitter.Branch(emitter.Ne0(cx), OperandAsCodeAddress(di.Instruction.op1));
            }
        }


        ///<summary>
        /// Converts a rep [string instruction] into a loop: 
        /// <code>
        /// while ([e]cx != 0)
        ///		[string instruction]
        ///		--ecx;
        ///		if (zF)				; only cmps[b] and scas[b]
        ///			goto follow;
        /// follow: ...	
        /// </code>
        ///</summary>
        private void RewriteRep()
        {
            var topOfLoop = di.Address;
            var regCX = orw.AluRegister(Registers.ecx, di.Instruction.addrWidth);
            dasm.MoveNext();
            di = dasm.Current;
            ric.Length += (byte) di.Length;
            var strFollow = dasm.Peek(1);
            emitter.MachineInstructionLength += (byte) di.Length;
            emitter.BranchInMiddleOfInstruction(emitter.Eq0(regCX), strFollow.Address);
            RewriteStringInstruction();
            emitter.Assign(regCX, emitter.Sub(regCX, 1));

            switch (di.Instruction.code)
            {
            case Opcode.cmps:
            case Opcode.cmpsb:
            case Opcode.scas:
            case Opcode.scasb:
                {
                    var cc = (di.Instruction.code == Opcode.repne)
                        ? ConditionCode.NE
                        : ConditionCode.EQ;
                    emitter.Branch(new TestCondition(cc, orw.FlagGroup(FlagM.ZF)).Invert(), topOfLoop);
                    break;
                }
            default:
                emitter.Goto(topOfLoop);
                break;
            }
        }

        public void RewriteRet()
        {
            EmitReturnInstruction(
                this.arch.WordWidth.Size + (di.Instruction.code == Opcode.retf ? PrimitiveType.Word16.Size : 0),
                di.Instruction.Operands == 1 ? ((ImmediateOperand)di.Instruction.op1).Value.ToInt32() : 0);
        }

        private void EmitReturnInstruction(int cbReturnAddress, int cbBytesPop)
        {
            emitter.Return(cbReturnAddress, cbBytesPop);
        }

        /// <summary>
        /// A jump to 0xFFFF:0x0000 in real mode is a reboot.
        /// </summary>
        /// <param name="instrCur"></param>
        /// <returns></returns>
        private bool IsRealModeReboot(IntelInstruction instrCur)
        {
            AddressOperand addrOp = instrCur.op1 as AddressOperand;
            bool isRealModeReboot = addrOp != null && addrOp.Address.Linear == 0xFFFF0;
            return isRealModeReboot;
        }

        public Address OperandAsCodeAddress(MachineOperand op)
        {
            AddressOperand ado = op as AddressOperand;
            if (ado != null)
                return ado.Address;
            ImmediateOperand imm = op as ImmediateOperand;
            if (imm != null)
            {
                if (arch.ProcessorMode == ProcessorMode.ProtectedFlat)
                {
                    return new Address(imm.Value.ToUInt32());
                }
                else
                    return new Address(di.Address.Selector, imm.Value.ToUInt32());
            }
            return null;
        }

    }
}
