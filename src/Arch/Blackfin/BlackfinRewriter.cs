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
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Arch.Blackfin
{
    public class BlackfinRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly BlackfinArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<BlackfinInstruction> dasm;
        private readonly List<RtlInstruction> rtls;
        private readonly RtlEmitter m;
        private InstrClass iclass;
        private BlackfinInstruction instr;

        public BlackfinRewriter(BlackfinArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new BlackfinDisassembler(arch, rdr).GetEnumerator();
            this.rtls = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtls);
            this.instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest(instr);
                    iclass = InstrClass.Invalid;
                    goto case Mnemonic.invalid;
                case Mnemonic.invalid:
                    this.iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.add: RewriteArithmetic(m.IAdd); break;
                case Mnemonic.add3: RewriteArithmetic3(m.IAdd); break;
                case Mnemonic.and3: RewriteLogical3(m.And); break;
                case Mnemonic.asr: RewriteShift(m.Sar); break;
                case Mnemonic.asr3: RewriteShift3(m.Sar); break;
                case Mnemonic.bitclr: RewriteBitclrset(CommonOps.ClearBit); break;
                case Mnemonic.bitset: RewriteBitclrset(CommonOps.SetBit); break;
                case Mnemonic.CALL: RewriteCall(); break;
                case Mnemonic.CLI: RewriteCli(); break;
                case Mnemonic.CSYNC: RewriteSync(csync_intrinsic); break;
                case Mnemonic.EXCPT: RewriteExcpt(); break;
                case Mnemonic.JUMP: RewriteJump(); break;
                case Mnemonic.JUMP_L: RewriteJump(); break;
                case Mnemonic.JUMP_S: RewriteJump(); break;
                case Mnemonic.LINK: RewriteLink(); break;
                case Mnemonic.lsl: RewriteShift(m.Shl); break;
                case Mnemonic.lsl3: RewriteShift3(m.Shl); break;
                case Mnemonic.lsr: RewriteShift(m.Shr); break;
                case Mnemonic.lsr3: RewriteShift3(m.Shr); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.mov_cc_lt: RewriteCompareDataRegister(m.Lt); break;
                case Mnemonic.mov_cc_ule: RewriteCompareDataRegister(m.Ule); break;
                case Mnemonic.mov_x: RewriteMovx(); break;
                case Mnemonic.mov_xb: RewriteMovxb(); break;
                case Mnemonic.mov_z: RewriteMovz(); break;
                case Mnemonic.mov_zb: RewriteMovz(PrimitiveType.Byte); break;
                case Mnemonic.mov_zl: RewriteMovz(PrimitiveType.Word16); break;
                case Mnemonic.mul: RewriteMul(); break;
                case Mnemonic.neg: RewriteNeg(); break;
                case Mnemonic.NOP: m.Nop(); break;
                case Mnemonic.RTN: RewriteRtn(); break;
                case Mnemonic.RTS: RewriteRts(); break;
                case Mnemonic.SSYNC: RewriteSync(ssync_intrinsic); break;
                case Mnemonic.sub3: RewriteArithmetic3(m.ISub); break;
                case Mnemonic.UNLINK: RewriteUnlink(); break;
                case Mnemonic.xor3: RewriteLogical3(m.Xor); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                rtls.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EmitUnitTest(BlackfinInstruction instr)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("BlackfinRw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private Address Addr(int iOperand)
        {
            return (Address) instr.Operands[iOperand];
        }

        private void EmitCc(FlagGroupStorage grf, Expression e)
        {
            m.Assign(binder.EnsureFlagGroup(grf), e);
        }

        private void EmitCc(FlagGroupStorage grf, int n)
        {
            var cc = binder.EnsureFlagGroup(grf);
            var value = Constant.Create(cc.DataType, n);
            m.Assign(cc, value);
        }

        private Expression SrcOperand(int iOperand)
        {
            switch (instr.Operands[iOperand])
            {
            case RegisterStorage rop:
                return binder.EnsureRegister(rop);
            case Constant imm:
                return imm;
            case Address addr:
                return addr;
            case MemoryOperand mem:
                var ea = EffectiveAddress(mem);
                return m.Mem(mem.DataType, ea);
            case RegisterRange range:
                return ExtendedRegister(range);
            default:
                throw new NotImplementedException($"Operand type {instr.Operands[iOperand].GetType().Name}.");
            }
        }

        private Expression DstOperand(int iOperand, Expression src)
        {
            switch (instr.Operands[iOperand])
            {
            case RegisterStorage rop:
                var dst = binder.EnsureRegister(rop);
                m.Assign(dst, src);
                return dst;
            case Constant imm:
                return imm;
            case MemoryOperand mem:
                var ea = EffectiveAddress(mem);
                m.Assign(m.Mem(mem.DataType, ea), src);
                return src;
            case RegisterRange range:
                var extReg = ExtendedRegister(range);
                m.Assign(extReg, src);
                return extReg;
            default:
                throw new NotImplementedException($"Operand type {instr.Operands[iOperand].GetType().Name}.");
            }
        }

        private Expression EffectiveAddress(MemoryOperand mem)
        {
            Expression ea;
            if (mem.Base is not null)
            {
                ea = binder.EnsureRegister(mem.Base);
                if (mem.Index is not null)
                    throw new NotImplementedException();
                if (mem.Offset != 0)
                {
                    ea = m.AddSubSignedInt(ea, mem.Offset);
                }
                if (mem.PostDecrement || mem.PostIncrement)
                {
                    var tmp = binder.CreateTemporary(ea.DataType);
                    m.Assign(tmp, ea);
                    var size = mem.DataType.Size;
                    m.Assign(ea, m.AddSubSignedInt(ea, mem.PostIncrement
                        ? size
                        : -size));
                    ea = tmp;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
            return ea;
        }

        private Identifier ExtendedRegister(RegisterRange range)
        {
            int nRegs = range.MaxRegister - range.MinRegister + 1;
            var regSequence = new Storage[nRegs];
            int bitsize = 0;
            for (int i = 0; i < nRegs; ++i)
            {
                var reg = range.Registers[range.MaxRegister - i];
                regSequence[i] = reg;
                bitsize += reg.DataType.BitSize; 
            }
            var dt = PrimitiveType.CreateWord(bitsize);
            return binder.EnsureSequence(dt, regSequence);
        }

        private Identifier Reg(int iOperand)
        {
            return binder.EnsureRegister((RegisterStorage) instr.Operands[iOperand]);
        }

        private void RewriteCall()
        {
            m.Call(SrcOperand(0), 0);
        }

        private void RewriteCli()
        {
            m.SideEffect(m.Fn(cli_intrinsic));
        }

        private void RewriteSync(IntrinsicProcedure intrinsic)
        {
            m.SideEffect(m.Fn(intrinsic));
        }

        private void RewriteExcpt()
        {
            m.SideEffect(m.Fn(excpt_intrinsic, SrcOperand(0)));
        }

        private void RewriteJump()
        {
            var addrDst = SrcOperand(0);
            m.Goto(addrDst);
        }

        private void RewriteArithmetic(Func<Expression, Expression, Expression> fn)
        {
            var src1 = SrcOperand(0);
            var src2 = SrcOperand(1);
            var dst = DstOperand(0, fn(src1, src2));
            EmitCc(Registers.NZVC, m.Cond(dst));
        }

        private void RewriteArithmetic3(Func<Expression, Expression, Expression> fn)
        {
            var src1 = SrcOperand(1);
            var src2 = SrcOperand(2);
            var dst = DstOperand(0, fn(src1, src2));
            EmitCc(Registers.NZVC, m.Cond(dst));
        }

        private void RewriteBitclrset(IntrinsicProcedure intrinsic)
        {
            var left = SrcOperand(0);
            var right = SrcOperand(1);
            m.Assign(left, m.Fn(
                intrinsic.MakeInstance(left.DataType, right.DataType),
                left, right));
            EmitCc(Registers.AN, m.Cond(left));
            EmitCc(Registers.AZ, 0);
            EmitCc(Registers.V, 0);
            EmitCc(Registers.AC0, 0);
        }

        private void RewriteCompareDataRegister(Func<Expression, Expression, Expression> fn)
        {
            var src1 = SrcOperand(0);
            var src2 = SrcOperand(1);
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, fn(src1, src2));
            EmitCc(Registers.NZVC, fn(src1, src2));
        }

        private void RewriteLink()
        {
            var localVars = ((Constant) instr.Operands[0]).ToInt32();
            var sp = binder.EnsureRegister(Registers.SP);
            var fp = binder.EnsureRegister(Registers.FP);
            // Allocate slots for RETS and old FP.
            //$TODO: actually save rets and use a special %cont continuation register.
            m.Assign(sp, m.ISubS(sp, 8));
            m.Assign(m.Mem32(sp), fp);
            if (localVars != 0)
            {
                m.Assign(sp, m.ISubS(sp, localVars));
            }
        }

        private void RewriteLogical3(Func<Expression, Expression, Expression> fn)
        {
            var src1 = SrcOperand(1);
            var src2 = SrcOperand(2);
            var dst = DstOperand(0, fn(src1, src2));
            EmitCc(Registers.NZ, m.Cond(dst));
            EmitCc(Registers.V, 0);
            EmitCc(Registers.AC0, 0);
        }

        private void RewriteMov()
        {
            var src = SrcOperand(1);
            DstOperand(0, src);
        }

        private void RewriteMovx()
        {
            var src = SrcOperand(1);
            var from = PrimitiveType.Create(Domain.SignedInt, src.DataType.BitSize);
            m.Assign(Reg(0), m.Convert(src, from, PrimitiveType.Int32));
        }

        private void RewriteMovxb()
        {
            var src = SrcOperand(1);
            m.Assign(Reg(0), m.Convert(m.Slice(src, PrimitiveType.SByte), PrimitiveType.SByte, PrimitiveType.Int32));
        }

        private void RewriteMovz()
        {
            var src = SrcOperand(1);
            m.Assign(Reg(0), m.Convert(src, src.DataType, PrimitiveType.Word32));
        }

        private void RewriteMovz(PrimitiveType dt)
        {
            var src = SrcOperand(1);
            var tmp = binder.CreateTemporary(dt);
            m.Assign(tmp, m.Slice(src, dt));
            m.Assign(Reg(0), m.Convert(tmp, dt, PrimitiveType.Word32));
        }

        private void RewriteMul()
        {
            Debug.Assert(instr.Operands.Length == 2);
            var dst = Reg(0);
            var src = Reg(1);
            m.Assign(dst, m.IMul(dst, src));
        }

        private void RewriteNeg()
        {
            var src = SrcOperand(1);
            var dst = DstOperand(0, m.Neg(src));
            EmitCc(Registers.NZV, m.Cond(dst));
            EmitCc(Registers.AC0, m.Eq0(dst));
        }

        private void RewriteRtn()
        {
            // A more accurate rewriter would assign PC = RETN
            m.Return(0, 0);
        }

        private void RewriteRts()
        {
            // A more accurate rewriter would assign PC = RETS
            m.Return(0, 0);
        }

        private void RewriteShift(Func<Expression, Expression, Expression> fn)
        {
            var src1 = SrcOperand(0);
            var src2 = SrcOperand(1);
            var dst = DstOperand(0, fn(src1, src2));
            EmitCc(Registers.NZV, m.Cond(dst));
        }

        private void RewriteShift3(Func<Expression, Expression, Expression> fn)
        {
            var src1 = SrcOperand(1);
            var src2 = SrcOperand(2);
            var dst = DstOperand(0, fn(src1, src2));
            EmitCc(Registers.NZV, m.Cond(dst));
        }

        private void RewriteUnlink()
        {
            var sp = binder.EnsureRegister(Registers.SP);
            var fp = binder.EnsureRegister(Registers.FP);
            // Restore slots for RETS and old FP.
            //$TODO: actually use RETS and a special %cont continuation register.
            m.Assign(sp, fp);
            m.Assign(fp, m.Mem32(sp));
            m.Assign(sp, m.IAddS(sp, 8));
        }

        private static readonly IntrinsicProcedure cli_intrinsic = new IntrinsicBuilder("__cli", true)
            .Void();
        private static readonly IntrinsicProcedure csync_intrinsic = new IntrinsicBuilder("__core_synchronize", true)
            .Void();
        private static readonly IntrinsicProcedure excpt_intrinsic = new IntrinsicBuilder("__force_exception", true)
            .Param(PrimitiveType.Byte)
            .Void();
        private static readonly IntrinsicProcedure ssync_intrinsic = new IntrinsicBuilder("__system_synchronize", true)
            .Void();
    }
}