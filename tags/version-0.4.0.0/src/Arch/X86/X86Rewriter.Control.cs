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
using Decompiler.Core.Operators;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.X86
{
    /// <summary>
    /// Rewrite rules for control flow instructions.
    /// </summary>
    public partial class X86Rewriter
    {
        private Expression CreateTestCondition(ConditionCode cc, Opcode opcode)
        {
            var grf = orw.FlagGroup(IntelInstruction.UseCc(opcode));
            var tc = new TestCondition(cc, grf);
            return tc;
        }

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
            emitter.Branch(CreateTestCondition(cc, instrCur.code), OperandAsCodeAddress(op1), RtlClass.ConditionalTransfer);
        }

        private void RewriteInt()
        {
            emitter.SideEffect(PseudoProc("__syscall", VoidType.Instance, SrcOp(instrCur.op1)));
        }

        private void RewriteJcxz()
        {
            emitter.Branch(
                emitter.Eq0(orw.AluRegister(Registers.ecx, instrCur.dataWidth)),
                OperandAsCodeAddress(instrCur.op1),
                RtlClass.ConditionalTransfer);
        }

        private void RewriteJmp()
        {
            if (IsRealModeReboot(instrCur))
			{
                PseudoProcedure reboot = host.EnsurePseudoProcedure("__bios_reboot", VoidType.Instance, 0);
                reboot.Characteristics = new Decompiler.Core.Serialization.ProcedureCharacteristics();
                reboot.Characteristics.Terminates = true;
                emitter.SideEffect(PseudoProc(reboot, VoidType.Instance));
				return;
			}
				
			if (instrCur.op1 is ImmediateOperand)
			{
				Address addr = OperandAsCodeAddress(instrCur.op1);
                emitter.Goto(addr);
				return;
			}
            emitter.Goto(SrcOp(instrCur.op1));
        }

        private void RewriteLoop(FlagM useFlags, ConditionCode cc)
        {
            Identifier cx = orw.AluRegister(Registers.ecx, instrCur.dataWidth);
            emitter.Assign(cx, emitter.ISub(cx, 1));
            if (useFlags != 0)
            {
                emitter.Branch(
                    new BinaryExpression(Operator.Cand, PrimitiveType.Bool,
                        new TestCondition(cc, orw.FlagGroup(useFlags)),
                        emitter.Ne0(cx)),
                    OperandAsCodeAddress(instrCur.op1),
                    RtlClass.ConditionalTransfer);
            }
            else
            {
                emitter.Branch(emitter.Ne0(cx), OperandAsCodeAddress(instrCur.op1), RtlClass.ConditionalTransfer);
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
            var topOfLoop = instrCur.Address;
            var regCX = orw.AluRegister(Registers.ecx, instrCur.addrWidth);
            dasm.MoveNext();
            instrCur = dasm.Current;
            ric.Length += (byte) instrCur.Length;
            var strFollow = dasm.Peek(1);
            emitter.BranchInMiddleOfInstruction(emitter.Eq0(regCX), strFollow.Address, RtlClass.ConditionalTransfer);
            RewriteStringInstruction();
            emitter.Assign(regCX, emitter.ISub(regCX, 1));

            switch (instrCur.code)
            {
            case Opcode.cmps:
            case Opcode.cmpsb:
            case Opcode.scas:
            case Opcode.scasb:
                {
                    var cc = (instrCur.code == Opcode.repne)
                        ? ConditionCode.NE
                        : ConditionCode.EQ;
                    emitter.Branch(new TestCondition(cc, orw.FlagGroup(FlagM.ZF)).Invert(), topOfLoop, RtlClass.ConditionalTransfer);
                    break;
                }
            default:
                emitter.Goto(topOfLoop);
                break;
            }
        }

        public void RewriteRet()
        {
            emitter.Return(
                this.arch.WordWidth.Size + (instrCur.code == Opcode.retf ? Registers.cs.DataType.Size : 0),
                instrCur.Operands == 1 ? ((ImmediateOperand)instrCur.op1).Value.ToInt32() : 0);
        }

        public void RewriteIret()
        {
            RewritePop(
                orw.FlagGroup(FlagM.SF | FlagM.CF | FlagM.ZF | FlagM.OF), instrCur.dataWidth);
            emitter.Return(
                Registers.cs.DataType.Size +
                arch.WordWidth.Size, 
                0);
        }

        /// <summary>
        /// A jump to 0xFFFF:0x0000 in real mode is a reboot.
        /// </summary>
        /// <param name="instrCur"></param>
        /// <returns></returns>
        private bool IsRealModeReboot(IntelInstruction instrCur)
        {
            X86AddressOperand addrOp = instrCur.op1 as X86AddressOperand;
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
                if (arch.ProcessorMode == ProcessorMode.Protected32)
                {
                    return new Address(imm.Value.ToUInt32());
                }
                else
                    return new Address(instrCur.Address.Selector, imm.Value.ToUInt32());
            }
            return null;
        }
    }
}
