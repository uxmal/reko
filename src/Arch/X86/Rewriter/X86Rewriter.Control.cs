#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Diagnostics;

namespace Reko.Arch.X86.Rewriter
{
    /// <summary>
    /// Rewrite rules for control flow instructions.
    /// </summary>
    public partial class X86Rewriter
    {
        private Expression CreateTestCondition(ConditionCode cc, Mnemonic mnemonic)
        {
            var grf = orw.FlagGroup(X86Instruction.UseCc(mnemonic) ?? throw new ArgumentException("Mnemonic not setting conditions"));
            var tc = new TestCondition(cc, grf);
            return tc;
        }

            /*
            if (i < 2)
                return tc;
            if (instrs[i-1].code != Mnemonic.test)
                return tc;
            var ah = instrs[i-1].op1 as RegisterStorage;
            if (ah is null || ah.Register != Registers.ah)
                return tc;
            var m = instrs[i-1].op2 as ImmediateOperand;
            if (m is null)
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

        private void RewriteCall(MachineOperand callTarget, DataType opsize)
        {
            Address? a = OperandAsCodeAddress(callTarget);
            if (a is not null)
            {
                var addr = a.Value;
                if (addr.ToLinear() == (dasm.Current.Address + dasm.Current.Length).ToLinear())
                {
                    // Calling the following address. Is the call followed by a 
                    // pop?
                    if (dasm.TryPeek(1, out var next) &&
                        next!.Mnemonic == Mnemonic.pop && 
                        next.Operands.Length > 0 &&
                        next.Operands[0] is RegisterStorage reg)
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
                        iclass = InstrClass.Linear;
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
                        iclass = InstrClass.Invalid;
                        m.Invalid();
                        return;
                    }
                    var seg = Constant.Create(PrimitiveType.SegmentSelector, instrCur.Address.Selector!.Value);
                    target = m.SegPtr(seg, target);
                }
                m.Call(target, (byte) opsize.Size);
            }
            iclass = InstrClass.Transfer | InstrClass.Call;
        }

        private void RewriteConditionalGoto(ConditionCode cc, MachineOperand op1)
        {
            iclass = InstrClass.ConditionalTransfer;
            m.Branch(CreateTestCondition(cc, instrCur.Mnemonic), OperandAsCodeAddress(op1)!, InstrClass.ConditionalTransfer);
        }

        private void RewriteConditionalMaskGoto(Func<Expression,Expression> eq)
        {
            var k = SrcOp(0);
            m.Branch(eq(k), OperandAsCodeAddress(instrCur.Operands[1])!);
        }

        private void RewriteEndbr()
        {
            // Endbr signals an indirect jump/call target, but is otherwise a NOP. We want to avoid fusing
            // endbr's with other kinds of NOPs however, so we set the InstrClass to just linear.
            m.Nop();
            Debug.Assert(iclass == InstrClass.Linear);
        }

        private void RewriteHlt()
        {
            m.SideEffect(m.Fn(CommonOps.Halt), InstrClass.Terminates);
        }

        private void RewriteInt()
        {
            m.SideEffect(m.Fn(CommonOps.Syscall_1, SrcOp(0)));
            iclass |= InstrClass.Call | InstrClass.Transfer;
        }

        private void RewriteIcebp()
        {
            // This is not supposed to be executed, so we mark the cluster as invalid.
            //$REVIEW: the new scanner being developed should make this less necessary.
            this.iclass = InstrClass.Invalid;
            m.Invalid();
        }

        private void RewriteInto()
        {
            m.BranchInMiddleOfInstruction(
                m.Test(ConditionCode.NO, binder.EnsureFlagGroup(Registers.O)),
                instrCur.Address + instrCur.Length,
                InstrClass.ConditionalTransfer);
            m.SideEffect(m.Fn(CommonOps.Syscall_1, Constant.Byte(4)));
        }

        private void RewriteJcxz(RegisterStorage cx)
        {
            m.Branch(
                m.Eq0(orw.AluRegister(cx)),
                OperandAsCodeAddress(instrCur.Operands[0])!,
                InstrClass.ConditionalTransfer);
        }

        private void RewriteJmp()
        {
            if (IsRealModeReboot(instrCur))
			{
                //$BUG: this should really live in MsdosPlatform.
                var reboot = new ExternalProcedure(
                    "__bios_reboot",
                    FunctionType.Action())
                {
                    Characteristics = new ProcedureCharacteristics
                    {
                        Terminates = true,
                    }
                };
                m.SideEffect(m.Fn(reboot));
				return;
			}

            iclass = InstrClass.Transfer;
			Address? addr = OperandAsCodeAddress(instrCur.Operands[0]);
			if (addr is not null)
            {
                m.Goto(addr);
				return;
			}
            var target = SrcOp(0);
            if (target.DataType.Size == 2 && arch.WordWidth.Size > 2)
            {
                iclass = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            m.Goto(target);
        }

        private void RewriteJmpe()
        {
            m.SideEffect(m.Fn(jmpe_intrinsic));
        }

        private void RewriteLoop(FlagGroupStorage? useFlags, ConditionCode cc)
        {
            Identifier cx = orw.AluRegister(Registers.rcx, instrCur.DataWidth);
            m.Assign(cx, m.ISub(cx, 1));
            if (useFlags is not null)
            {
                m.Branch(
                    m.Cand(
                        m.Test(cc, binder.EnsureFlagGroup(useFlags)),
                        m.Ne0(cx)),
                    OperandAsCodeAddress(instrCur.Operands[0])!,
                    InstrClass.ConditionalTransfer);
            }
            else
            {
                m.Branch(m.Ne0(cx), OperandAsCodeAddress(instrCur.Operands[0])!, InstrClass.ConditionalTransfer);
            }
        }

        private void RewriteLtr()
        {
            m.SideEffect(m.Fn(ltr_intrinsic, SrcOp(0)));
        }

        public void RewriteRet()
        {
            int extraBytesPopped = instrCur.Operands.Length == 1 
                ? ((Constant)instrCur.Operands[0]).ToInt32() 
                : 0;
            if ((extraBytesPopped & 1) == 1)
            {
                // Unlikely that an odd number of bytes are pushed on the stack.
                iclass = InstrClass.Invalid;
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
                binder.EnsureFlagGroup(Registers.SCZO), instrCur.DataWidth);
            m.Return(
                Registers.cs.DataType.Size +
                arch.WordWidth.Size, 
                0);
        }

        private void RewriteRsm()
        {
            m.Return(0, 0);
        }

        private void RewriteStr()
        {
            m.Assign(
                SrcOp(0),
                m.Fn(str_intrinsic));
        }

        private void RewriteSyscall()
        {
            m.SideEffect(m.Fn(syscall_intrinsic));
        }

        private void RewriteSysenter()
        {
            m.SideEffect(m.Fn(sysenter_intrinsic));
        }

        private void RewriteSysexit()
        {
            m.SideEffect(m.Fn(sysexit_intrinsic));
            m.Return(0,0);
        }

        private void RewriteSysret()
        {
            m.SideEffect(m.Fn(sysret_intrinsic));
            m.Return(0,0);
        }

        private void RewriteVerrw(IntrinsicProcedure intrinsic)
        {
            var z = binder.EnsureFlagGroup(Registers.Z);
            m.Assign(z, m.Fn(intrinsic, SrcOp(0)));
        }

        private void RewriteXabort()
        {
            var op = SrcOp(0);
            m.SideEffect(m.Fn(xabort_intrinsic, op),
                InstrClass.Terminates);
        }

        /// <summary>
        /// A jump to 0xFFFF:0x0000 in real mode is a reboot.
        /// </summary>
        /// <param name="instrCur"></param>
        /// <returns></returns>
        //$TODO: this is a MS-DOS specific detail, and should be implemented as a system service.
        private bool IsRealModeReboot(X86Instruction instrCur)
        {
            bool isRealModeReboot = 
                instrCur.Operands[0] is Address addr &&
                addr.ToLinear() == 0xFFFF0;
            return isRealModeReboot;
        }

        public Address? OperandAsCodeAddress(MachineOperand op)
        {
            return op switch
            {
                Address addr => addr,
                Constant imm => orw.ImmediateAsAddress(instrCur.Address, imm),
                _ => null
            };
        }
    }
}
