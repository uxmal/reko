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
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Avr.Avr8
{
    public class Avr8Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly Avr8Architecture arch;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly LookaheadEnumerator<Avr8Instruction> dasm;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private Avr8Instruction instr;
        private RtlEmitter m;
        private InstrClass iclass;
        private List<RtlInstruction> rtlInstructions;
        private List<RtlInstructionCluster> clusters;

        public Avr8Rewriter(Avr8Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.dasm = new LookaheadEnumerator<Avr8Instruction>(new Avr8Disassembler(arch, rdr).GetEnumerator());
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.instr = null!;
            this.m = null!;
            this.rtlInstructions = null!;
            this.clusters = null!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.clusters = new List<RtlInstructionCluster>();
                Rewrite(dasm.Current);
                foreach (var cluster in clusters)
                {
                    yield return cluster;
                }
            }
        }

        public void Rewrite(Avr8Instruction instr)
        {
            this.instr = instr;
            this.rtlInstructions = new List<RtlInstruction>();
            this.iclass = instr.InstructionClass;
            this.m = new RtlEmitter(rtlInstructions);
            switch (instr.Mnemonic)
            {
            case Mnemonic.adc: RewriteAdcSbc(m.IAdd); break;
            case Mnemonic.add: RewriteBinOp(m.IAdd, CmpFlags); break;
            case Mnemonic.adiw: RewriteAddSubIW(m.IAdd); break;
            case Mnemonic.and: RewriteBinOp(m.And, Registers.SNZ, Registers.V); break;
            case Mnemonic.andi: RewriteBinOp(m.And, Registers.SNZ, Registers.V); break;
            case Mnemonic.asr: RewriteAsr(); break;
            case Mnemonic.brcc: RewriteBranch(ConditionCode.UGE, Registers.C); break;
            case Mnemonic.brcs: RewriteBranch(ConditionCode.ULT, Registers.C); break;
            case Mnemonic.breq: RewriteBranch(ConditionCode.EQ, Registers.Z); break;
            case Mnemonic.brge: RewriteBranch(ConditionCode.GE, Registers.VN); break;
            case Mnemonic.brhc: RewriteBranch(Registers.H, false); break;
            case Mnemonic.brhs: RewriteBranch(Registers.H, true); break;
            case Mnemonic.brid: RewriteBranch(Registers.I, false); break;
            case Mnemonic.brie: RewriteBranch(Registers.I, true); break;
            case Mnemonic.brlt: RewriteBranch(ConditionCode.LT, Registers.VN); break;
            case Mnemonic.brmi: RewriteBranch(ConditionCode.LT, Registers.N); break;
            case Mnemonic.brne: RewriteBranch(ConditionCode.NE, Registers.Z); break;
            case Mnemonic.brpl: RewriteBranch(ConditionCode.GE, Registers.N); break;
            case Mnemonic.brtc: RewriteBranch(Registers.T, false); break;
            case Mnemonic.brts: RewriteBranch(Registers.T, true); break;
            case Mnemonic.brvc: RewriteBranch(Registers.V, false); break;
            case Mnemonic.brvs: RewriteBranch(Registers.V, true); break;
            case Mnemonic.call: RewriteCall(); break;
            case Mnemonic.cli: RewriteCli(); break;
            case Mnemonic.cln: RewriteClearBit(Registers.N); break;
            case Mnemonic.com: RewriteUnary(Operator.Comp, Registers.SNZ, Registers.V, Registers.C); break;
            case Mnemonic.cp: RewriteCp(); break;
            case Mnemonic.cpi: RewriteCp(); break;
            case Mnemonic.cpc: RewriteCpc(); break;
            case Mnemonic.cpse: SkipIf(m.Eq); break;
            case Mnemonic.dec: RewriteIncDec(m.ISub); break;
            case Mnemonic.des: RewriteDes(); break;
            case Mnemonic.elpm: RewriteElpm(); break;
            case Mnemonic.eor: RewriteBinOp(m.Xor, LogicalFlags, Registers.V); break;
            case Mnemonic.icall: RewriteIcall(); break;
            case Mnemonic.@in: RewriteIn(); break;
            case Mnemonic.inc: RewriteIncDec(m.IAdd); break;
            case Mnemonic.ijmp: RewriteIjmp(); break;
            case Mnemonic.jmp: RewriteJmp(); break;
            case Mnemonic.ld: RewriteLd(); break;
            case Mnemonic.ldd: RewriteLd(); break;
            case Mnemonic.ldi: RewriteLdi(); break;
            case Mnemonic.lds: RewriteLds(); break;
            case Mnemonic.lpm: RewriteLpm(); break;
            case Mnemonic.lsr: RewriteLsr(); break;
            case Mnemonic.mov: RewriteMov(); break;
            case Mnemonic.movw: RewriteMovw(); break;
            case Mnemonic.muls: RewriteMuls(); break;
            case Mnemonic.neg: RewriteUnary(Operator.Neg, CmpFlags); break;
            case Mnemonic.@out: RewriteOut(); break;
            case Mnemonic.or: RewriteBinOp(m.Or, Registers.SNZV); break;
            case Mnemonic.ori: RewriteBinOp(m.Or, Registers.SNZV); break;
            case Mnemonic.pop: RewritePop(); break;
            case Mnemonic.push: RewritePush(); break;
            case Mnemonic.rcall: RewriteCall(); break;
            case Mnemonic.ror: RewriteRor(); break;
            case Mnemonic.ret: RewriteRet(); break;
            case Mnemonic.reti: RewriteRet(); break;  //$TODO: more to indicate interrupt return?
            case Mnemonic.rjmp: RewriteJmp(); break;
            case Mnemonic.sbc: RewriteAdcSbc(m.ISub); break;
            case Mnemonic.sbci: RewriteAdcSbc(m.ISub); break;
            case Mnemonic.sbis: RewriteSbis(); return; // We've already added ourself to clusters.
            case Mnemonic.sbiw: RewriteAddSubIW(m.ISub); break;
            case Mnemonic.sbrc: SkipIf(Sbrc); break;
            case Mnemonic.sbrs: SkipIf(Sbrs); break;
            case Mnemonic.sec: RewriteSetBit(Registers.C, true); break;
            case Mnemonic.sei: RewriteSei(); break;
            case Mnemonic.st: RewriteSt(); break;
            case Mnemonic.std: RewriteSt(); break;
            case Mnemonic.sts: RewriteSts(); break;
            case Mnemonic.sub: RewriteBinOp(m.ISub, CmpFlags); break;
            case Mnemonic.subi: RewriteBinOp(m.ISub, CmpFlags); break;
            case Mnemonic.swap: RewriteSwap(); break;
            default:
                host.Error(instr.Address, string.Format("AVR8 instruction '{0}' is not supported yet.", instr.Mnemonic));
                EmitUnitTest();
                m.Invalid();
                break;
            }
            clusters.Add(m.MakeCluster(instr.Address, instr.Length, iclass));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.RequireService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("Avr8_rw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private void EmitFlags(Expression e, FlagGroupStorage mod)
        {
            var grf = binder.EnsureFlagGroup(mod);
            m.Assign(grf, m.Cond(e));
        }

        private FlagGroupStorage CmpFlags => Registers.HSVNZC;
        private FlagGroupStorage ArithFlags => Registers.SVNZC;
        private FlagGroupStorage LogicalFlags => Registers.SNZC;
        private FlagGroupStorage IncDecFlags => Registers.SVNZ;

        private Identifier RegisterPair(MachineOperand operand)
        {
            var regN = (RegisterStorage) operand;
            if (regN.Number == Registers.z.Number)
            {
                return binder.EnsureRegister(Registers.z);
            }
            else
            {
                var regN1 = Registers.ByteRegs[regN.Number + 1];
                var regPair = binder.EnsureSequence(PrimitiveType.Word16, regN1, regN);
                return regPair;
            }
        }

        private void RewriteIO(int iRegOp, int iPortOp, bool read)
        {
            var reg = RewriteOp(iRegOp);
            var port = ((Constant)instr.Operands[iPortOp]).ToByte();
            if (port == 0x3F)
            {
                var psreg = binder.EnsureRegister(Registers.sreg);
                if (read)
                {
                    m.Assign(reg, psreg);
                }
                else
                {
                    m.Assign(psreg, reg);
                }
            }
            else if (read)
            {
                m.Assign(reg, m.Fn(in_intrinsic, Constant.Byte(port)));
            }
            else
            {
                m.SideEffect(m.Fn(out_intrinsic, Constant.Byte(port), reg));
            }
        }

        private void RewriteBinOp(
             Func<Expression, Expression, Expression> fn,
             FlagGroupStorage mod,
             FlagGroupStorage? clr = null)
        {
            var dst = RewriteOp(0);
            var src = RewriteOp(1);
            m.Assign(dst, fn(dst, src));
            EmitFlags(dst, mod);
            if (clr is not null)
            {
                m.Assign(binder.EnsureFlagGroup(clr), 0);
            }
        }

        private void RewriteUnary(
            UnaryOperator op,
             FlagGroupStorage mod,
             FlagGroupStorage? clr = null,
             FlagGroupStorage? set = null)
        {
            var reg = RewriteOp(0);
            m.Assign(reg, m.Unary(op, reg));
            EmitFlags(reg, mod);
            if (clr is not null)
            {
                m.Assign(binder.EnsureFlagGroup(clr), 0);
            }
            if (set is not null)
            {
                m.Assign(binder.EnsureFlagGroup(set), set.FlagGroupBits);
            }
        }

        private Expression RewriteOp(int iOp)
        {
            var op = instr.Operands[iOp];
            switch (op)
            {
            case RegisterStorage rop:
                return binder.EnsureRegister(rop);
            case Constant iop:
                return iop;
            case Address aop:
                return aop;
            }
            throw new NotImplementedException(string.Format("Rewriting {0}s not implemented yet.", op.GetType().Name));
        }

        private void RewriteMem(int iOp, Expression src, Action<Expression, Expression> write, Expression? seg)
        {
            var op = instr.Operands[iOp];
            var mop = (MemoryOperand)op;
            var baseReg = binder.EnsureRegister(mop.Base!);
            Expression ea = baseReg;
            if (mop.PreDecrement)
            {
                m.Assign(baseReg, m.ISubS(baseReg, 1));
            } else if (mop.Displacement != 0)
            {
                ea = m.IAddS(ea, mop.Displacement);
            }
            Expression val;
            if (seg is not null)
            {
                val = m.SegMem(mop.DataType, seg, ea);
            }
            else
            {
                val = m.Mem(mop.DataType, ea);
            }
            write(val, src);
            if (mop.PostIncrement)
            {
                m.Assign(baseReg, m.IAddS(baseReg, 1));
            }
        }

        public void RewriteAdcSbc(Func<Expression, Expression, Expression> opr)
        {
            // We do not take the trouble of widening the CF to the word size
            // to simplify code analysis in later stages. 
            var c = binder.EnsureFlagGroup(Registers.C);
            var dst = RewriteOp(0);
            var src = RewriteOp(1);
            m.Assign(
                dst,
                opr(
                    opr(dst, src),
                    c));
            EmitFlags(dst, CmpFlags);
        }

        private void RewriteAddSubIW(Func<Expression,Expression,Expression> fn)
        {
            var operand = instr.Operands[0];
            var regPair = RegisterPair(operand);
            var imm = (Constant)instr.Operands[1];
            m.Assign(regPair, fn(regPair, Constant.Word16(imm.ToUInt16())));
            EmitFlags(regPair, ArithFlags);
        }

        private void RewriteAsr()
        {
            var reg = RewriteOp(0);
            m.Assign(reg, m.Sar(reg, 1));
            EmitFlags(reg, ArithFlags);
        }

        private void RewriteBranch(ConditionCode cc, FlagGroupStorage flags)
        {
            var grf = binder.EnsureFlagGroup(flags);
            var target = (Address)RewriteOp(0);
            m.Branch(m.Test(cc, grf), target, InstrClass.ConditionalTransfer);
        }

        private void RewriteBranch(FlagGroupStorage grf, bool set)
        {
            Expression test = binder.EnsureFlagGroup(grf);
            if (!set)
                test = m.Not(test);
            var target = (Address)RewriteOp(0);
            m.Branch(test, target, InstrClass.ConditionalTransfer);
        }

        private void RewriteMov()
        {
            var dst = RewriteOp(0);
            var src = RewriteOp(1);
            m.Assign(dst, src);
        }

        private void RewriteMovw()
        {
            var pairDst = RegisterPair(instr.Operands[0]);
            var pairSrc = RegisterPair(instr.Operands[1]);
            m.Assign(pairDst, pairSrc);
        }

        private void RewriteCall()
        {
            m.Call(RewriteOp(0), 2);    //$TODO: 3-byte mode in architecture.
        }

        private void RewriteClearBit(FlagGroupStorage grf)
        {
            m.Assign(binder.EnsureFlagGroup(grf), Constant.False());
        }

        private void RewriteCli()
        {
            m.SideEffect(m.Fn(cli_intrinsic));
        }

        private void RewriteCp()
        {
            var left = RewriteOp(0);
            var right = RewriteOp(1);
            var flags = binder.EnsureFlagGroup(CmpFlags);
            m.Assign(flags, m.ISub(left, right));
        }

        private void RewriteCpc()
        {
            var left = RewriteOp(0);
            var right = RewriteOp(1);
            var c = binder.EnsureFlagGroup(Registers.C);
            var flags = binder.EnsureFlagGroup(CmpFlags);
            m.Assign(flags, m.ISub(m.ISub(left, right), c));
        }

        private void SkipIf(Func<Expression, Expression,Expression> cond)
        {
            if (!dasm.TryPeek(1, out var nextInstr))
            {
                m.Invalid();
                iclass = InstrClass.Invalid;
                return;
            }
            var left = RewriteOp(0);
            var right = RewriteOp(1);
            iclass = InstrClass.ConditionalTransfer;
            m.Branch(cond(left,right), nextInstr!.Address + nextInstr.Length, iclass);
        }

        private Expression Sbrc(Expression a, Expression b)
        {
            var imm = ((Constant)b).ToInt32();
            return m.Eq0(m.And(a, m.Byte((byte)(1 << imm))));
        }

        private Expression Sbrs(Expression a, Expression b)
        {
            var imm = ((Constant)b).ToInt32();
            return m.Ne0(m.And(a, m.Byte((byte)(1 << imm))));
        }

        private void RewriteDes()
        {
            var h = binder.EnsureFlagGroup(Registers.H);
            m.SideEffect(m.Fn(des_intrinsic, RewriteOp(0), h));
        }

        private void RewriteElpm()
        {
            Identifier dst;
            if (instr.Operands.Length == 0)
            {
                dst = binder.EnsureRegister(Registers.ByteRegs[0]);
            }
            else
            {
                dst = (Identifier) RewriteOp(0);
            }
            var rampz = binder.EnsureRegister(Registers.rampz);
            var z = binder.EnsureRegister(Registers.z);
            m.Assign(dst, m.Fn(elpmIntrinsic, rampz, z));
            if (instr.Operands.Length == 2 &&
                instr.Operands[1] is MemoryOperand mem &&
                mem.PostIncrement)
            {
                m.Assign(z, m.IAddS(z, 1));
            }
        }

        private void RewriteIcall()
        {
            iclass = InstrClass.Transfer | InstrClass.Call;
            var z = binder.EnsureRegister(Registers.z);
            m.Call(z, 2);
        }

        private void RewriteIn()
        {
            RewriteIO(0, 1, true);
        }

        private void RewriteIjmp()
        {
            iclass = InstrClass.Transfer;
            var z = binder.EnsureRegister(Registers.z);
            m.Goto(z);
        }

        private void RewriteIncDec(Func<Expression, Expression, Expression> fn)
        {
            var reg = RewriteOp(0);
            m.Assign(reg, fn(reg, Constant.SByte(1)));
            EmitFlags(reg, IncDecFlags);
        }

        private void RewriteLd()
        {
            var dst = RewriteOp(0);
            RewriteMem(1, dst, (d, s) => m.Assign(s, d), null);
        }

        private void RewriteLdi()
        {
            m.Assign(RewriteOp(0), RewriteOp(1));
        }

        private void RewriteLds()
        {
            m.Assign(RewriteOp(0), m.Mem8(RewriteOp(1)));
        }

        private void RewriteLpm()
        {
            var codeSel = binder.EnsureRegister(Registers.code);
            if (instr.Operands.Length == 0)
            {
                var z = binder.EnsureRegister(Registers.z);
                var r0 = binder.EnsureRegister(arch.GetRegister(0));
                m.Assign(r0, m.SegMem8(codeSel, z));
            }
            else
            {
                var dst = RewriteOp(0);
                RewriteMem(1, dst, (d, s) => m.Assign(s, d), codeSel);
            }
        }

        private void RewriteLsr()
        {
            var reg = RewriteOp(0);
            m.Assign(reg, m.Shr(reg, 1));
            EmitFlags(reg, Registers.SVZC);
            m.Assign(binder.EnsureFlagGroup(Registers.N), 0);
        }

        private void RewriteJmp()
        {
            iclass = InstrClass.Transfer;
            var op = RewriteOp(0);
            if (op is Constant c)
            {
                op = arch.MakeAddressFromConstant(c, true);
            }
            m.Goto(op);
        }

        private void RewriteMuls()
        {
            var r1_r0 = binder.EnsureSequence(PrimitiveType.Word16, Registers.ByteRegs[1], Registers.ByteRegs[0]);
            var op0 = RewriteOp(0);
            var op1 = RewriteOp(1);
            var c = binder.EnsureFlagGroup(Registers.C);
            var z = binder.EnsureFlagGroup(Registers.Z);
            var smul = m.SMul(op0, op1);
            smul.DataType = PrimitiveType.Int16;
            m.Assign(r1_r0, smul);
            m.Assign(c, m.Lt0(r1_r0));
            m.Assign(z, m.Eq0(r1_r0));
        }

        private void RewriteOut()
        {
            RewriteIO(1, 0, false);
        }

        private void RewritePop()
        {
            var sp = binder.EnsureRegister(Registers.StackRegister);
            m.Assign(RewriteOp(0), m.Mem8(sp));
            m.Assign(sp, m.IAddS(sp, 1));
        }

        private void RewritePush()
        {
            var sp = binder.EnsureRegister(Registers.StackRegister);
            m.Assign(sp, m.ISubS(sp, 1));
            m.Assign(m.Mem8(sp), RewriteOp(0));
        }

        private void RewriteRet()
        {
            m.Return(2, 0);
        }

        private void RewriteRor()
        {
            var c = binder.EnsureFlagGroup(Registers.C);
            var reg = RewriteOp(0);
            m.Assign(reg, m.Fn(CommonOps.RorC.MakeInstance(reg.DataType, PrimitiveType.Byte),
                reg, m.Byte(1), c));
            EmitFlags(reg, CmpFlags);
        }

        private void RewriteSbis()
        {
            var io = m.Fn(in_intrinsic, RewriteOp(0));
            var bis = m.Fn(bit_set_intrinsic, io, RewriteOp(1));
            if (!dasm.MoveNext())
            {
                m.Invalid();
                return;
            }
            var addrSkip = dasm.Current.Address + dasm.Current.Length;
            var branch = m.BranchInMiddleOfInstruction(bis, addrSkip, InstrClass.ConditionalTransfer);
            clusters.Add(m.MakeCluster(this.instr.Address, this.instr.Length, InstrClass.ConditionalTransfer));
            Rewrite(dasm.Current);
        }

        private void RewriteSei()
        {
            m.SideEffect(m.Fn(sei_intrinsic));
        }

        private void RewriteSetBit(FlagGroupStorage grf, bool value)
        {
            m.Assign(binder.EnsureFlagGroup(grf), Constant.Bool(value));
        }

        private void RewriteSt()
        {
            var src = RewriteOp(1);
            RewriteMem(0, src, (d, s) => m.Assign(d, s), null);
        }

        private void RewriteSts()
        {
            m.Assign(m.Mem8(RewriteOp(0)), RewriteOp(1));
        }

        private void RewriteSwap()
        {
            var reg = RewriteOp(0);
            m.Assign(reg, m.Fn(swap_intrinsic, reg));
        }

        static Avr8Rewriter()
        {
            bit_set_intrinsic = new IntrinsicBuilder("__bit_set", false)
                .Param(PrimitiveType.Byte)
                .Param(PrimitiveType.Byte)
                .Returns(PrimitiveType.Bool);

            cli_intrinsic = new IntrinsicBuilder("__cli", true)
                .Void();
            des_intrinsic = new IntrinsicBuilder("__des", true)
                .Param(PrimitiveType.Byte)
                .Param(PrimitiveType.Byte)
                .Void();
            in_intrinsic = new IntrinsicBuilder("__in", true)
                .Param(PrimitiveType.Byte)
                .Returns(PrimitiveType.Byte);
            out_intrinsic = new IntrinsicBuilder("__out", true)
                .Param(PrimitiveType.Byte)
                .Param(PrimitiveType.Byte)
                .Void();
            sei_intrinsic = new IntrinsicBuilder("__sei", true)
                .Void();
        }
        private static readonly IntrinsicProcedure elpmIntrinsic = new IntrinsicBuilder("__extended_load_program_memory", true)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Ptr16)
            .Returns(PrimitiveType.Byte);

        private static readonly IntrinsicProcedure bit_set_intrinsic;
        private static readonly IntrinsicProcedure cli_intrinsic;
        private static readonly IntrinsicProcedure des_intrinsic;
        private static readonly IntrinsicProcedure in_intrinsic;
        private static readonly IntrinsicProcedure out_intrinsic;
        private static readonly IntrinsicProcedure sei_intrinsic;
        private static readonly IntrinsicProcedure swap_intrinsic = IntrinsicBuilder.Unary("__swap_nybbles", PrimitiveType.Byte);
    }
}
