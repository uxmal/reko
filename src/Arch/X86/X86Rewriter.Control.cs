#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.X86
{
    /// <summary>
    /// Rewrite rules for control flow instructions.
    /// </summary>
    public partial class X86Rewriter
    {
        private Expression CreateTestCondition(ConditionCode cc, Mnemonic mnemonic)
        {
            var grf = orw.FlagGroup(X86Instruction.UseCc(mnemonic));
            var tc = new TestCondition(cc, grf);
            return tc;
        }

            /*
            if (i < 2)
                return tc;
            if (instrs[i-1].code != Mnemonic.test)
                return tc;
            var ah = instrs[i-1].op1 as RegisterOperand;
            if (ah == null || ah.Register != Registers.ah)
                return tc;
            var m = instrs[i-1].op2 as ImmediateOperand;
            if (m == null)
                return tc;

            if (instrs[i-2].code != Mnemonic.fstsw)
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
                if (addr.ToLinear() == (dasm.Current.Address + dasm.Current.Length).ToLinear())
                {
                    // Calling the following address. Is the call followed by a 
                    // pop?
                    var next = dasm.Peek(1);
                    if (next.Mnemonic == Mnemonic.pop && 
                        next.Operands.Length > 0 &&
                        next.Operands[0] is RegisterOperand reg)
                    {
                        // call $+5,pop<reg> idiom
                        dasm.MoveNext();
                        var r = orw.AluRegister(reg);
                        if (addr.Selector.HasValue)
                        {
                            var offset = Constant.Create(PrimitiveType.Offset16, addr.Offset);
                            m.Assign(r, offset);
                        }
                        else
                        {
                            m.Assign(r, addr);
                        }
                        this.len += 1;
                        rtlc = InstrClass.Linear;
                        return;
                    }
                }
                m.Call(addr, (byte) opsize.Size);
            }
            else
            {
                var target = SrcOp(callTarget);
                if (target.DataType.Size == 2)
                {
                    if (arch.WordWidth.Size > 2)
                    {
                        // call bx doesn't work on 32- or 64-bit architectures.
                        rtlc = InstrClass.Invalid;
                        m.Invalid();
                        return;
                    }
                    var seg = Constant.Create(PrimitiveType.SegmentSelector, instrCur.Address.Selector.Value);
                    target = m.Seq(seg, target);
                }
                m.Call(target, (byte) opsize.Size);
            }
            rtlc = InstrClass.Transfer | InstrClass.Call;
        }

        private void RewriteConditionalGoto(ConditionCode cc, MachineOperand op1)
        {
            rtlc = InstrClass.ConditionalTransfer;
            m.Branch(CreateTestCondition(cc, instrCur.Mnemonic), OperandAsCodeAddress(op1), InstrClass.ConditionalTransfer);
        }

        private void RewriteInt()
        {
            m.SideEffect(host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, SrcOp(instrCur.Operands[0])));
            rtlc |= InstrClass.Call | InstrClass.Transfer;
        }

        private void RewriteInto()
        {
            m.BranchInMiddleOfInstruction(
                m.Test(ConditionCode.NO, orw.FlagGroup(FlagM.OF)),
                instrCur.Address + instrCur.Length,
                InstrClass.ConditionalTransfer);
            m.SideEffect(
                    host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, Constant.Byte(4)));
        }

        private void RewriteJcxz()
        {
            m.Branch(
                m.Eq0(orw.AluRegister(Registers.rcx, instrCur.dataWidth)),
                OperandAsCodeAddress(instrCur.Operands[0]),
                InstrClass.ConditionalTransfer);
        }

        private void RewriteJmp()
        {
            if (IsRealModeReboot(instrCur))
			{
                //$BUG: this should really live in MsdosPlatform.
                var reboot = new ExternalProcedure(
                    "__bios_reboot",
                    new FunctionType(
                        new Identifier("", VoidType.Instance, null)))
                {
                    Characteristics = new ProcedureCharacteristics
                    {
                        Terminates = true,
                    }
                };
                m.SideEffect(m.Fn(reboot));
				return;
			}

            rtlc = InstrClass.Transfer;
			if (instrCur.Operands[0] is ImmediateOperand)
			{
				Address addr = OperandAsCodeAddress(instrCur.Operands[0]);
                m.Goto(addr);
				return;
			}
            var target = SrcOp(instrCur.Operands[0]);
            if (target.DataType.Size == 2 && arch.WordWidth.Size > 2)
            {
                rtlc = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            m.Goto(target);
        }

        private void RewriteLoop(FlagM useFlags, ConditionCode cc)
        {
            Identifier cx = orw.AluRegister(Registers.rcx, instrCur.dataWidth);
            m.Assign(cx, m.ISub(cx, 1));
            if (useFlags != 0)
            {
                m.Branch(
                    m.Cand(
                        m.Test(cc, orw.FlagGroup(useFlags)),
                        m.Ne0(cx)),
                    OperandAsCodeAddress(instrCur.Operands[0]),
                    InstrClass.ConditionalTransfer);
            }
            else
            {
                m.Branch(m.Ne0(cx), OperandAsCodeAddress(instrCur.Operands[0]), InstrClass.ConditionalTransfer);
            }
        }

        public void RewriteRet()
        {
            int extraBytesPopped = instrCur.Operands.Length == 1 
                ? ((ImmediateOperand)instrCur.Operands[0]).Value.ToInt32() 
                : 0;
            if ((extraBytesPopped & 1) == 1)
            {
                // Unlikely that an odd number of bytes are pushed on the stack.
                rtlc = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            m.Return(
                this.arch.WordWidth.Size + (instrCur.Mnemonic == Mnemonic.retf ? Registers.cs.DataType.Size : 0),
                extraBytesPopped);
        }

        public void RewriteIret()
        {
            RewritePop(
                orw.FlagGroup(FlagM.SF | FlagM.CF | FlagM.ZF | FlagM.OF), instrCur.dataWidth);
            m.Return(
                Registers.cs.DataType.Size +
                arch.WordWidth.Size, 
                0);
        }

        private void RewriteSyscall()
        {
            m.SideEffect(host.PseudoProcedure("__syscall", VoidType.Instance));
        }

        private void RewriteSysenter()
        {
            m.SideEffect(host.PseudoProcedure("__sysenter", VoidType.Instance));
        }

        private void RewriteSysexit()
        {
            m.SideEffect(host.PseudoProcedure("__sysexit", VoidType.Instance));
            m.Return(0,0);
        }

        private void RewriteSysret()
        {
            m.SideEffect(host.PseudoProcedure("__sysret", VoidType.Instance));
            m.Return(0,0);
        }


        /// <summary>
        /// A jump to 0xFFFF:0x0000 in real mode is a reboot.
        /// </summary>
        /// <param name="instrCur"></param>
        /// <returns></returns>
        private bool IsRealModeReboot(X86Instruction instrCur)
        {
            var addrOp = instrCur.Operands[0] as X86AddressOperand;
            bool isRealModeReboot = addrOp != null && addrOp.Address.ToLinear() == 0xFFFF0;
            return isRealModeReboot;
        }

        public Address OperandAsCodeAddress(MachineOperand op)
        {
            if (op is AddressOperand ado)
                return ado.Address;
            if (op is ImmediateOperand imm)
            {
                return orw.ImmediateAsAddress(instrCur.Address, imm);
            }
            return null;
        }
    }
}
