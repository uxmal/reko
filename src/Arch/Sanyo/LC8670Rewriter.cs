#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Sanyo
{
    internal class LC8670Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private static readonly FlagGroupStorage C;
        private static readonly FlagGroupStorage V;
        private static readonly FlagGroupStorage CAV;
        private static readonly IntrinsicProcedure clr1_sig;
        private static readonly IntrinsicProcedure not1_sig;
        private static readonly IntrinsicProcedure set1_sig;
        private static readonly IntrinsicProcedure test1_sig;
        private static readonly IntrinsicProcedure reti_sig;

        private readonly LC8670Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<LC8670Instruction> dasm;
        private readonly List<RtlInstruction> rtls;
        private readonly RtlEmitter m;
        private LC8670Instruction instr;
        private InstrClass iclass;

        public LC8670Rewriter(LC8670Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new LC8670Disassembler(arch, rdr).GetEnumerator();
            this.rtls = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtls);
            this.instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = this.instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    m.Invalid();
                    iclass = InstrClass.Invalid;
                    break;
                case Mnemonic.invalid: m.Invalid(); break;
                case Mnemonic.add: RewriteAddSub(m.IAdd); break;
                case Mnemonic.addc: RewriteAddcSubc(m.IAdd); break;
                case Mnemonic.and: RewriteLogical(m.And); break;
                case Mnemonic.be: RewriteBe(m.Eq); break;
                case Mnemonic.bn: RewriteBp(true); break;
                case Mnemonic.bne: RewriteBe(m.Ne); break;
                case Mnemonic.bnz: RewriteBnzZ(m.Ne0); break;
                case Mnemonic.bp: RewriteBp(false); break;
                case Mnemonic.br: RewriteJmp(); break;
                case Mnemonic.brf: RewriteJmp(); break;
                case Mnemonic.bz: RewriteBnzZ(m.Eq0); break;
                case Mnemonic.call: RewriteCall(); break;
                case Mnemonic.callf: RewriteCall(); break;
                case Mnemonic.callr: RewriteCall(); break;
                case Mnemonic.clr1: RewriteBit1(clr1_sig); break;
                case Mnemonic.dbnz: RewriteDbnz(); break;
                case Mnemonic.dec: RewriteIncDec(m.ISub); break;
                case Mnemonic.div: RewriteDiv(); break;
                case Mnemonic.inc: RewriteIncDec(m.IAdd); break;
                case Mnemonic.jmp: RewriteJmp(); break;
                case Mnemonic.jmpf: RewriteJmp(); break;
                case Mnemonic.ld: RewriteLd(); break;
                case Mnemonic.ldc: RewriteLdc(); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.mul: RewriteMul(); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.not1: RewriteBit1(not1_sig); break;
                case Mnemonic.or: RewriteLogical(m.Or); break;
                case Mnemonic.pop: RewritePop(); break;
                case Mnemonic.push: RewritePush(); break;
                case Mnemonic.ret: RewriteRet(); break;
                case Mnemonic.reti: RewriteReti(); break;
                case Mnemonic.rol: RewriteRol(); break;
                case Mnemonic.rolc: RewriteRolc(); break;
                case Mnemonic.ror: RewriteRor(); break;
                case Mnemonic.rorc: RewriteRorc(); break;
                case Mnemonic.st: RewriteSt(); break;
                case Mnemonic.set1: RewriteBit1(set1_sig); break;
                case Mnemonic.sub: RewriteAddSub(m.ISub); break;
                case Mnemonic.subc: RewriteAddcSubc(m.ISub); break;
                case Mnemonic.xch: RewriteXch(); break;
                case Mnemonic.xor: RewriteLogical(m.Xor); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                rtls.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("LC8670Rw", this.instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private Expression Op(int op)
        {
            switch (instr.Operands[op])
            {
            case RegisterOperand reg:
                return binder.EnsureRegister(reg.Register);
            case ImmediateOperand imm:
                return imm.Value;
            case MemoryOperand mem:
                if (mem.Base is not null)
                {
                    return m.Mem8(binder.EnsureRegister(mem.Base));
                }
                else
                {
                    return m.Mem8(Address.Ptr16(mem.Offset));
                }
            }
            throw new NotImplementedException($"Operand type {instr.Operands[op].GetType()}.");
        }

        private void RewriteAddSub(Func<Expression,Expression,Expression> fn)
        {
            var acc = binder.EnsureRegister(Registers.ACC);
            m.Assign(acc, fn(acc, Op(0)));
            m.Assign(binder.EnsureFlagGroup(CAV), m.Cond(acc));
        }

        private void RewriteAddcSubc(Func<Expression, Expression, Expression> fn)
        {
            var acc = binder.EnsureRegister(Registers.ACC);
            var c = binder.EnsureFlagGroup(C);
            m.Assign(acc, fn(fn(acc, Op(0)), c));
            m.Assign(binder.EnsureFlagGroup(CAV), m.Cond(acc));
        }

        private void RewriteBe(Func<Expression, Expression, Expression> fn)
        {
            Address target;
            Expression test;
            if (instr.Operands.Length == 3)
            {
                test = fn(Op(0), Op(1));
                target = ((AddressOperand) instr.Operands[2]).Address;
            }
            else
            {
                var acc = binder.EnsureRegister(Registers.ACC);
                test = fn(acc, Op(0));
                target = ((AddressOperand) instr.Operands[1]).Address;
            }
            m.Branch(test, target);
        }

        private void RewriteBp(bool invert)
        {
            var target = ((AddressOperand) instr.Operands[2]).Address;
            Expression test = m.Fn(test1_sig, Op(0), Op(1));
            if (invert)
                test = test.Invert();
            m.Branch(test, target);
        }

        private void RewriteBnzZ(Func<Expression,Expression> fn)
        {
            var acc = binder.EnsureRegister(Registers.ACC);
            var target = ((AddressOperand) instr.Operands[0]).Address;
            m.Branch(fn(acc), target);
        }

        private void RewriteBit1(IntrinsicProcedure fn)
        {
            var src = m.Fn(fn, Op(0), Op(1));
            m.Assign(Op(0), src);
        }

        private void RewriteCall()
        {
            var target = ((AddressOperand) instr.Operands[0]).Address;
            m.Call(target, 2);
        }

        private void RewriteDbnz()
        {
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(tmp, m.ISub(Op(0), 1));
            m.Assign(Op(0), tmp);
            var target = ((AddressOperand) instr.Operands[1]).Address;
            m.Branch(m.Ne0(tmp), target);
        }

        private void RewriteDiv()
        {
            var acc_c = binder.EnsureSequence(PrimitiveType.Word16, Registers.ACC, Registers.C);
            var b = binder.EnsureRegister(Registers.B);
            var tmp = binder.CreateTemporary(acc_c.DataType);
            m.Assign(tmp, acc_c);
            m.Assign(acc_c, m.UDiv(tmp, b));
            m.Assign(b, m.Mod(tmp, b));
            m.Assign(binder.EnsureFlagGroup(C), Constant.False());
            m.Assign(binder.EnsureFlagGroup(V), m.Eq0(b));
        }
        
        private void RewriteIncDec(Func<Expression,Expression,Expression> fn)
        {
            var src = fn(Op(0), Constant.Byte(1));
            m.Assign(Op(0), src);
        }

        private void RewriteJmp()
        {
            var target = ((AddressOperand) instr.Operands[0]).Address;
            m.Goto(target);
        }

        private void RewriteLogical(Func<Expression, Expression, Expression> fn)
        {
            var acc = binder.EnsureRegister(Registers.ACC);
            m.Assign(acc, fn(acc, Op(0)));
        }

        private void RewriteLd()
        {
            var src = Op(0);
            var dst = binder.EnsureRegister(Registers.ACC);
            m.Assign(dst, src);
        }

        private void RewriteLdc()
        {
            var tr = binder.EnsureSequence(PrimitiveType.Ptr16, Registers.TRH, Registers.TRL);
            var acc = binder.EnsureRegister(Registers.ACC);
            m.Assign(acc, m.Mem8(m.IAdd(tr, acc)));
        }

        private void RewriteMov()
        {
            var src = Op(0);
            var dst = Op(1);
            m.Assign(dst, src);
        }

        private void RewriteMul()
        {
            var left = binder.EnsureSequence(PrimitiveType.Word16, Registers.ACC, Registers.C);
            var right = binder.EnsureRegister(Registers.B);
            var dt = PrimitiveType.Create(Domain.UnsignedInt, 24);
            var dst = binder.EnsureSequence(dt, Registers.ACC, Registers.C, Registers.B);
            m.Assign(dst, m.UMul(dt, left, right));
            m.Assign(binder.EnsureFlagGroup(C), Constant.False());
            m.Assign(binder.EnsureFlagGroup(V), m.Uge(dst, Constant.Create(dt, 0x1_0000)));
        }

        private void RewritePop()
        {
            var sp = binder.EnsureRegister(Registers.SP);
            m.Assign(Op(0), m.Mem8(sp));
            m.Assign(sp, m.ISubS(sp, 1));
        }

        private void RewritePush()
        {
            var sp = binder.EnsureRegister(Registers.SP);
            m.Assign(sp, m.IAddS(sp, 1));
            m.Assign(m.Mem8(sp), Op(0));
        }

        private void RewriteRet()
        {
            m.Return(2, 0);
        }

        private void RewriteReti()
        {
            m.SideEffect(m.Fn(reti_sig));
            m.Return(2, 0);
        }

        private void RewriteRol()
        {
            var acc = binder.EnsureRegister(Registers.ACC);
            m.Assign(acc, host.Intrinsic(IntrinsicProcedure.Rol, true, acc.DataType, acc, m.Int8(1)));
        }

        private void RewriteRolc()
        {
            var acc = binder.EnsureRegister(Registers.ACC);
            var c = binder.EnsureFlagGroup(C);
            m.Assign(acc, host.Intrinsic(IntrinsicProcedure.RolC, true, acc.DataType, acc, m.Int8(1), c));
            m.Assign(c, m.Cond(acc));
        }

        private void RewriteRor()
        {
            var acc = binder.EnsureRegister(Registers.ACC);
            m.Assign(acc, host.Intrinsic(IntrinsicProcedure.Ror, true, acc.DataType, acc, m.Int8(1)));
        }

        private void RewriteRorc()
        {
            var acc = binder.EnsureRegister(Registers.ACC);
            var c = binder.EnsureFlagGroup(C);
            m.Assign(acc, host.Intrinsic(IntrinsicProcedure.RorC, true, acc.DataType, acc, m.Int8(1), c));
            m.Assign(c, m.Cond(acc));
        }

        private void RewriteSt()
        {
            var src = binder.EnsureRegister(Registers.ACC);
            var dst = Op(0);
            m.Assign(dst, src);
        }

        private void RewriteXch()
        {
            var acc = binder.EnsureRegister(Registers.ACC);
            var tmp = binder.CreateTemporary(acc.DataType);
            m.Assign(tmp, acc);
            m.Assign(acc, Op(0));
            m.Assign(Op(0), tmp);
        }

        static LC8670Rewriter()
        {
            C = new FlagGroupStorage(Registers.PSW, (uint) FlagM.CY, "C", PrimitiveType.Bool);
            V = new FlagGroupStorage(Registers.PSW, (uint) FlagM.OV, "V", PrimitiveType.Bool);
            CAV = new FlagGroupStorage(Registers.PSW, (uint) (FlagM.CY|FlagM.AC|FlagM.OV), "CAV", PrimitiveType.Byte);
            clr1_sig = new IntrinsicBuilder("__clr1", false)
                .Param(PrimitiveType.Byte).Param(PrimitiveType.Byte).Returns(PrimitiveType.Byte);
            not1_sig = new IntrinsicBuilder("__not1", false)
                .Param(PrimitiveType.Byte).Param(PrimitiveType.Byte).Returns(PrimitiveType.Byte);
            set1_sig = new IntrinsicBuilder("__set1", false)
                .Param(PrimitiveType.Byte).Param(PrimitiveType.Byte).Returns(PrimitiveType.Byte);
            test1_sig = new IntrinsicBuilder("__test1", false)
                .Param(PrimitiveType.Byte).Param(PrimitiveType.Byte).Returns(PrimitiveType.Bool);
            reti_sig = new IntrinsicBuilder("__leave_isr", true).Void();
        }
    }
}