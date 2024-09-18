#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.CSky
{
    public class CSkyRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly CSkyArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<CSkyInstruction> dasm;
        private readonly List<RtlInstruction> rtls;
        private readonly RtlEmitter m;
        private CSkyInstruction instr;
        private InstrClass iclass;
        private int conditionalLeft;
        private uint conditionalExecutionMask;

        public CSkyRewriter(CSkyArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new CSkyDisassembler(arch, rdr).GetEnumerator();
            this.instr = default!;
            this.rtls = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtls);
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
                    goto case Mnemonic.invalid;
                case Mnemonic.invalid:
                    m.Invalid();
                    iclass = InstrClass.Invalid;
                    break;
                case Mnemonic.abs: RewriteAbs(); break;
                case Mnemonic.addc: RewriteAddc(); break;
                case Mnemonic.addi: RewriteBinary(Operator.IAdd); break;
                case Mnemonic.addu: RewriteBinary(Operator.IAdd); break;
                case Mnemonic.and: RewriteBinary(Operator.And); break;
                case Mnemonic.andi: RewriteBinary(Operator.And); break;
                case Mnemonic.andn: RewriteAndn(); break;
                case Mnemonic.andni: RewriteAndn(); break;
                case Mnemonic.asr: RewriteBinary(Operator.Sar); break;
                case Mnemonic.asri: RewriteBinary(Operator.Sar); break;
                case Mnemonic.asrc: RewriteShiftRightC(Operator.Sar); break;
                case Mnemonic.bclri: RewriteBitop(CommonOps.ClearBit); break;
                case Mnemonic.bez: RewriteBranch(m.Eq0); break;
                case Mnemonic.bhsz: RewriteBranch(m.Ge0); break;
                case Mnemonic.bhz: RewriteBranch(m.Gt0); break;
                case Mnemonic.blsz: RewriteBranch(m.Le0); break;
                case Mnemonic.blz: RewriteBranch(m.Lt0); break;
                case Mnemonic.bf: RewriteBc(false); break;
                case Mnemonic.bgenr: RewriteBgenr(); break;
                case Mnemonic.bkpt: RewriteBkpt(); break;
                case Mnemonic.bmaski: RewriteBmaski(); break;
                case Mnemonic.bmclr: RewriteBmclr(); break;
                case Mnemonic.bmset: RewriteBmset(); break;
                case Mnemonic.bnez: RewriteBranch(m.Ne0); break;
                case Mnemonic.br: RewriteBr(); break;
                case Mnemonic.brev: RewriteBrev(); break;
                case Mnemonic.bseti: RewriteBitop(CommonOps.SetBit); break;
                case Mnemonic.bsr: RewriteBsr(); break;
                case Mnemonic.bt: RewriteBc(true); break;
                case Mnemonic.btsti: RewriteBtsti(); break;
                case Mnemonic.clrf: RewriteClr(false); break;
                case Mnemonic.clrt: RewriteClr(true); break;
                case Mnemonic.cmphs: RewriteCmp(Operator.Uge); break;
                case Mnemonic.cmphsi: RewriteCmp(Operator.Uge); break;
                case Mnemonic.cmplt: RewriteCmp(Operator.Lt); break;
                case Mnemonic.cmplti: RewriteCmp(Operator.Lt); break;
                case Mnemonic.cmpne: RewriteCmp(Operator.Ne); break;
                case Mnemonic.cmpnei: RewriteCmp(Operator.Ne); break;
                case Mnemonic.decf: RewriteIncDec(Operator.ISub, false); break;
                case Mnemonic.decgt: RewriteDec(Operator.Gt); break;
                case Mnemonic.declt: RewriteDec(Operator.Lt); break;
                case Mnemonic.decne: RewriteDec(Operator.Ne); break;
                case Mnemonic.dect: RewriteIncDec(Operator.ISub, true); break;
                case Mnemonic.divs: RewriteBinary(Operator.SDiv); break;
                case Mnemonic.divu: RewriteBinary(Operator.UDiv); break;
                case Mnemonic.doze: RewriteDoze(); break;
                case Mnemonic.ff0: RewriteFastFind(ff0_intrinsic); break;
                case Mnemonic.ff1: RewriteFastFind(ff1_intrinsic); break;
                case Mnemonic.grs: RewriteMov(); break;
                case Mnemonic.idly: RewriteIdly(); break;
                case Mnemonic.incf: RewriteIncDec(Operator.IAdd, false); break;
                case Mnemonic.inct: RewriteIncDec(Operator.IAdd, true); break;
                case Mnemonic.ins: RewriteIns(); break;
                case Mnemonic.ixd: RewriteIndexExpression(3); break;
                case Mnemonic.ixh: RewriteIndexExpression(1); break;
                case Mnemonic.ixw: RewriteIndexExpression(2); break;
                case Mnemonic.jmp: RewriteJmp(); break;
                case Mnemonic.jmpi: RewriteJmpi(); break;
                case Mnemonic.jmpix: RewriteJmpix(); break;
                case Mnemonic.jsr: RewriteJsr(); break;
                case Mnemonic.jsri: RewriteJsri(); break;
                case Mnemonic.ld_b: RewriteLoad(PrimitiveType.Word32); break;
                case Mnemonic.ld_bs: RewriteLoad(PrimitiveType.Int32); break;
                case Mnemonic.ld_d: RewriteLoad64(); break;
                case Mnemonic.ld_h: RewriteLoad(PrimitiveType.Word32); break;
                case Mnemonic.ld_hs: RewriteLoad(PrimitiveType.Int32); break;
                case Mnemonic.ld_w: RewriteLoad(PrimitiveType.Word32); break;
                case Mnemonic.ldex_w: RewriteLdex(); break;
                case Mnemonic.ldr_b: RewriteLoad(PrimitiveType.Word32); break;
                case Mnemonic.ldr_bs: RewriteLoad(PrimitiveType.Int32); break;
                case Mnemonic.ldr_h: RewriteLoad(PrimitiveType.Word32); break;
                case Mnemonic.ldr_hs: RewriteLoad(PrimitiveType.Int32); break;
                case Mnemonic.ldr_w: RewriteLoad(PrimitiveType.Word32); break;
                case Mnemonic.lrs_b: RewriteLrs(PrimitiveType.Byte); break;
                case Mnemonic.lrs_h: RewriteLrs(PrimitiveType.Word16); break;
                case Mnemonic.lrs_w: RewriteLrs(PrimitiveType.Word32); break;
                case Mnemonic.lrw: RewriteLrw(); break;
                case Mnemonic.lsl: RewriteBinary(Operator.Shl); break;
                case Mnemonic.lslc: RewriteShiftLeftC(Operator.Shr); break;
                case Mnemonic.lsli: RewriteBinary(Operator.Shl); break;
                case Mnemonic.lsr: RewriteBinary(Operator.Shr); break;
                case Mnemonic.lsri: RewriteBinary(Operator.Shr); break;
                case Mnemonic.lsrc: RewriteShiftRightC(Operator.Shr); break;
                case Mnemonic.mfcr: RewriteMfcr(); break;
                case Mnemonic.mfhi: RewriteMov(0, Registers.Hi); break;
                case Mnemonic.mfhis: RewriteMfhilos(mfhis_intrinsic); break;
                case Mnemonic.mflo: RewriteMov(0, Registers.Lo); break;
                case Mnemonic.mflos: RewriteMfhilos(mflos_intrinsic); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.movi: RewriteMov(); break;
                case Mnemonic.movih: RewriteMovih(); break;
                case Mnemonic.mtcr: RewriteMtcr(); break;
                case Mnemonic.mthi: RewriteMov(Registers.Hi, 0); break;
                case Mnemonic.mtlo: RewriteMov(Registers.Lo, 0); break;
                case Mnemonic.muls: RewriteMac(Operator.SMul, null); break;
                case Mnemonic.mulsa: RewriteMac(Operator.SMul, Operator.IAdd); break;
                case Mnemonic.mulss: RewriteMac(Operator.SMul, Operator.ISub); break;
                case Mnemonic.mulsh: RewriteMach(Operator.SMul, null); break;
                case Mnemonic.mulsha: RewriteMach(Operator.SMul, Operator.IAdd); break;
                case Mnemonic.mulshs: RewriteMach(Operator.SMul, Operator.ISub); break;
                case Mnemonic.mulsw: RewriteMacw(Operator.SMul, null); break;
                case Mnemonic.mulswa: RewriteMacw(Operator.SMul, Operator.IAdd); break;
                case Mnemonic.mulsws: RewriteMacw(Operator.SMul, Operator.ISub); break;
                case Mnemonic.mult: RewriteBinary(Operator.IMul); break;
                case Mnemonic.mulu: RewriteMac(Operator.UMul, null); break;
                case Mnemonic.mulua: RewriteMac(Operator.UMul, Operator.IAdd); break;
                case Mnemonic.mulus: RewriteMac(Operator.UMul, Operator.ISub); break;
                case Mnemonic.mvc: RewriteMvc(); break;
                case Mnemonic.mvcv: RewriteMvcv(); break;
                case Mnemonic.mvtc: RewriteMvtc(); break;
                case Mnemonic.nie: RewriteNie(); break;
                case Mnemonic.nir: RewriteNir(); break;
                case Mnemonic.nor: RewriteNor(); break;
                case Mnemonic.or: RewriteBinary(Operator.Or); break;
                case Mnemonic.ori: RewriteBinary(Operator.Or); break;
                case Mnemonic.pldr: RewritePrefetch(pldr_intrinsic); break;
                case Mnemonic.pldrw: RewritePrefetch(pldrw_intrinsic); break;
                case Mnemonic.pop: RewritePop(); break;
                case Mnemonic.push: RewritePush(); break;
                case Mnemonic.revb: RewriteReverse(CommonOps.ReverseBytes); break;
                case Mnemonic.revh: RewriteReverse(CommonOps.ReverseHalfwords); break;
                case Mnemonic.rfi: RewriteRfi(); break;
                case Mnemonic.rotl: RewriteBinary(CommonOps.Rol); break;
                case Mnemonic.rotli: RewriteBinary(CommonOps.Rol); break;
                case Mnemonic.rte: RewriteRte(); break;
                case Mnemonic.sce: RewriteSce(); break;
                case Mnemonic.se: RewriteSe(); break;
                case Mnemonic.sext: RewriteSext(); break;
                case Mnemonic.sextb: RewriteSext(PrimitiveType.Byte); break;
                case Mnemonic.sexth: RewriteSext(PrimitiveType.Word16); break;
                case Mnemonic.srs_b: RewriteSrs(PrimitiveType.Byte); break;
                case Mnemonic.srs_h: RewriteSrs(PrimitiveType.Word16); break;
                case Mnemonic.srs_w: RewriteSrs(PrimitiveType.Word32); break;
                case Mnemonic.srte: RewriteSrte(); break;
                case Mnemonic.st_b: RewriteStore(); break;
                case Mnemonic.st_d: RewriteStore64(); break;
                case Mnemonic.st_h: RewriteStore(); break;
                case Mnemonic.st_w: RewriteStore(); break;
                case Mnemonic.stex_w: RewriteStex(); break;
                case Mnemonic.stop: RewriteStop(); break;
                case Mnemonic.str_b: RewriteStore(); break;
                case Mnemonic.str_h: RewriteStore(); break;
                case Mnemonic.str_w: RewriteStore(); break;
                case Mnemonic.strap: RewriteStrap(); break;
                case Mnemonic.subc: RewriteSubc(); break;
                case Mnemonic.subi: RewriteBinary(Operator.ISub); break;
                case Mnemonic.subu: RewriteBinary(Operator.ISub); break;
                case Mnemonic.sync: RewriteSync(); break;
                case Mnemonic.trap: RewriteTrap(); break;
                case Mnemonic.tst: RewriteTst(); break;
                case Mnemonic.tstnbz: RewriteTstnbz(); break;
                case Mnemonic.vmulsh: RewriteVmacsh(null); break;
                case Mnemonic.vmulsha: RewriteVmacsh(Operator.IAdd); break;
                case Mnemonic.vmulshs: RewriteVmacsh(Operator.ISub); break;
                case Mnemonic.vmulsw: RewriteVmacsw(null); break;
                case Mnemonic.vmulswa: RewriteVmacsw(Operator.IAdd); break;
                case Mnemonic.vmulsws: RewriteVmacsw(Operator.ISub); break;
                case Mnemonic.wait: RewriteWait(); break;
                case Mnemonic.we: RewriteWe(); break;
                case Mnemonic.xor: RewriteBinary(Operator.Xor); break;
                case Mnemonic.xori: RewriteBinary(Operator.Xor); break;
                case Mnemonic.xsr: RewriteBinary(xsr_intrinsic); break;
                case Mnemonic.xtb0: RewriteXtb(0); break;
                case Mnemonic.xtb1: RewriteXtb(8); break;
                case Mnemonic.xtb2: RewriteXtb(16); break;
                case Mnemonic.xtb3: RewriteXtb(24); break;
                case Mnemonic.zext: RewriteZext(); break;
                case Mnemonic.zextb: RewriteZext(PrimitiveType.Byte); break;
                case Mnemonic.zexth: RewriteZext(PrimitiveType.Word16); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                rtls.Clear();
                if (conditionalLeft > 0)
                    --conditionalLeft;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Instructions that can be set by sce instruction only include arithmetic 
        /// operation instruction, multiply-divide instructions, and byte, half-word
        /// and word load/store instructions of immediate operand addressing
        /// mode; besides, these instructions should not affect condition bit C.
        /// 
        /// Instructions that cannot be set by sce instruction include but are not
        /// limited to: branch jump instruction, load/store instruction of register
        /// addressing mode, load/store instruction of double word, load/store
        /// instruction of multiword, co-processor instruction, privileged
        /// instruction, special function instruction, floating point instruction, and
        /// vector multimedia instructions.
        /// </remarks>
        private void MaybeConditionalExecution()
        {
            if (this.conditionalLeft <= 0)
                return;
            Expression c = binder.EnsureFlagGroup(Registers.C);
            if ((this.conditionalExecutionMask & 1) == 0)
            {
                c = m.Not(c);
            }
            m.BranchInMiddleOfInstruction(c, instr.Address + instr.Length, InstrClass.ConditionalTransfer);
            this.conditionalExecutionMask >>= 1;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void EmitUnitTest(CSkyInstruction instr, string message = "")
        {
            var testgenSvc = arch.Services.GetService<ITestGenerationService>();
            testgenSvc?.ReportMissingRewriter("CSkyRw", instr, instr.Mnemonic.ToString(), rdr, message);
        }

        private Expression Operand(int iop)
        {
            var op = instr.Operands[iop];
            switch (op)
            {
            case RegisterStorage reg:
                return binder.EnsureRegister(reg);
            case ImmediateOperand imm:
                return imm.Value;
            case Address addr:
                return addr;
            case MemoryOperand mem:
                return m.Mem(mem.Width, EffectiveAddress(mem));
            default:
                throw new NotImplementedException($"CSky operand type {op.GetType().Name}");
            }
        }

        private Expression EffectiveAddress(MemoryOperand mem)
        {
            if (mem.Base is null)
            {
                return Address.Ptr32((uint) mem.Offset);
            }
            Expression ea = binder.EnsureRegister(mem.Base);
            if (mem.Offset == 0)
            {
                if (mem.Index is null)
                    return ea;
                Expression idx = binder.EnsureRegister(mem.Index);
                if (mem.Shift > 0)
                {
                    idx = m.IMul(idx, 1 << mem.Shift);
                }
                return m.IAdd(ea, idx);
            }
            return m.AddSubSignedInt(ea, mem.Offset);
        }

        private void MaybeZeroExtend(Expression dst, Expression src)
        {
            if (dst.DataType.BitSize > src.DataType.BitSize)
            {
                var tmp = binder.CreateTemporary(src.DataType);
                m.Assign(tmp, src);
                src = m.ExtendZ(tmp, dst.DataType);
            }
            m.Assign(dst, src);
        }

        private Expression MaybeSlice(Expression e, DataType dt)
        {
            if (e.DataType.BitSize > dt.BitSize)
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, m.Slice(e, dt));
                e = tmp;
            }
            return e;
        }

        private (Expression, Expression, Expression) Rewrite2or3operands()
        {
            Expression left;
            Expression right;
            Expression dst;
            if (instr.Operands.Length == 3)
            {
                left = Operand(1);
                right = Operand(2);
                dst = Operand(0);
            }
            else
            {
                left = Operand(0);
                right = Operand(1);
                dst = left;
            }
            return (dst, left, right);
        }

        private void RewriteAbs()
        {
            MaybeConditionalExecution();
            var exp = Operand(1);
            var dst = Operand(0);
            m.Assign(dst, m.Fn(CommonOps.Abs.MakeInstance(PrimitiveType.Int32), exp));
        }

        private void RewriteAddc()
        {
            MaybeConditionalExecution();
            Identifier c = binder.EnsureFlagGroup(Registers.C);
            var (dst, left, right) = Rewrite2or3operands();
            m.Assign(dst, m.IAddC(left, right, c));
            m.Assign(c, m.Cond(dst));
        }

        private void RewriteAndn()
        {
            MaybeConditionalExecution();
            var (dst, left, right) = Rewrite2or3operands();
            m.Assign(dst, m.And(left, m.Comp(right)));
        }


        private void RewriteShiftLeftC(BinaryOperator op)
        {
            MaybeConditionalExecution();
            Identifier c = binder.EnsureFlagGroup(Registers.C);
            var (dst, left, right) = Rewrite2or3operands();
            var cRight = (Constant) right;
            var bitmask = Constant.Create(PrimitiveType.Word32, 1u << (cRight.ToInt32() - 1));
            m.Assign(c, m.Ne0(m.And(left, bitmask)));
            m.Assign(dst, m.Bin(op, left, right));
        }

        private void RewriteShiftRightC(BinaryOperator op)
        {
            MaybeConditionalExecution();
            Identifier c = binder.EnsureFlagGroup(Registers.C);
            var (dst, left, right) = Rewrite2or3operands();
            var cRight = (Constant) right;
            var bitmask = Constant.Create(PrimitiveType.Word32, 1u << (cRight.ToInt32() - 1));
            m.Assign(c, m.Ne0(m.And(left, bitmask)));
            m.Assign(dst, m.Bin(op, left, right));
        }

        private void RewriteBinary(BinaryOperator op)
        {
            MaybeConditionalExecution();
            var (dst, left, right) = Rewrite2or3operands();
            m.Assign(dst, m.Bin(op, left, right));
        }

        private void RewriteBinary(IntrinsicProcedure proc)
        {
            MaybeConditionalExecution();
            var (dst, left, right) = Rewrite2or3operands();
            m.Assign(dst, m.Fn(proc, left, right));
        }

        private void RewriteBitop(IntrinsicProcedure op)
        {
            MaybeConditionalExecution();
            var (dst, left, right) = Rewrite2or3operands();
            m.Assign(dst, m.Fn(op, left, right));
        }

        private void RewriteBkpt()
        {
            m.SideEffect(m.Fn(bkpt_intrinsic));
        }

        private void RewriteBmaski()
        {
            MaybeConditionalExecution();
            var src = ((ImmediateOperand) instr.Operands[1]).Value.ToInt32();
            var dst = binder.EnsureRegister((RegisterStorage) instr.Operands[0]);
            var value = (1u << src) - 1;
            m.Assign(dst, (int)value);
        }

        private void RewriteBmclr()
        {
            m.SideEffect(m.Fn(bmclr_intrinsic));
        }


        private void RewriteBmset()
        {
            m.SideEffect(m.Fn(bmset_intrinsic));
        }

        private void RewriteBr()
        {
            var target = (Address) instr.Operands[0];
            m.Goto(target);
        }

        private void RewriteBranch(Func<Expression, Expression> cmp)
        {
            var reg = Operand(0);
            var target = (Address) instr.Operands[1];
            m.Branch(cmp(reg), target);
        }

        private void RewriteBrev()
        {
            MaybeConditionalExecution();
            var src = Operand(1);
            var dst = Operand(0);
            m.Assign(dst, m.Fn(CommonOps.ReverseBits, src));
        }

        private void RewriteBc(bool branchIfTrue)
        {
            Expression condition = binder.EnsureFlagGroup(Registers.C);
            if (!branchIfTrue)
            {
                condition = m.Not(condition);
            }
            var target = (Address)instr.Operands[0];
            m.Branch(condition, target);
        }

        private void RewriteBgenr()
        {
            MaybeConditionalExecution();
            var src = Operand(1);
            var dst = Operand(0);
            m.Assign(dst, m.Shl(m.Word32(1), src));
        }

        private void RewriteBsr()
        {
            var target = (Address) instr.Operands[0];
            m.Call(target, 0);
        }

        private void RewriteBtsti()
        {
            var src = Operand(0);
            var pos = Operand(1);
            var c = binder.EnsureFlagGroup(Registers.C);
            m.Assign(c, m.Fn(CommonOps.Bit, src, pos));
        }

        private void RewriteCmp(BinaryOperator cmp)
        {
            var left = Operand(0);
            var right = Operand(1);
            var c = binder.EnsureFlagGroup(Registers.C);
            m.Assign(c, m.Bin(cmp, left, right));
        }

        private void RewriteClr(bool flag)
        {
            var dst = Operand(0);
            Expression c = binder.EnsureFlagGroup(Registers.C);
            if (!flag)
            {
                c = m.Not(c);
            }
            m.Assign(dst, m.Conditional(dst.DataType, c, m.Word32(0), dst));
        }

        private void RewriteIncDec(BinaryOperator op, bool flag)
        {
            var src = Operand(1);
            var dec = Operand(2);
            var dst = Operand(0);
            Expression c = binder.EnsureFlagGroup(Registers.C);
            if (!flag)
            {
                c = m.Not(c);
            }
            m.Assign(dst, m.Conditional(dst.DataType, c,
                m.Bin(op, src, dec),
                dst));
        }

        private void RewriteDec(ConditionalOperator op)
        {
            var src = Operand(1);
            var dec = Operand(2);
            var dst = Operand(0);
            Expression c = binder.EnsureFlagGroup(Registers.C);
            m.Assign(dst, m.ISub(src, dec));
            m.Assign(c, m.Bin(op, PrimitiveType.Bool, dst, m.Word32(0)));
        }

        private void RewriteDoze()
        {
            m.SideEffect(m.Fn(doze_intrinsic));
        }

        private void RewriteFastFind(IntrinsicProcedure proc)
        {
            MaybeConditionalExecution();
            var src = Operand(1);
            var dst = Operand(0);
            m.Assign(dst, m.Fn(proc, src));
        }

        private void RewriteIdly()
        {
            m.SideEffect(m.Fn(idly_intrinsic, Operand(0)));
        }

        private void RewriteIns()
        {
            MaybeConditionalExecution();
            var src = Operand(1);
            var msb = ((ImmediateOperand) instr.Operands[2]).Value.ToInt32();
            var lsb = ((ImmediateOperand) instr.Operands[3]).Value.ToInt32();
            var dst = Operand(0);
            int bits = (msb - lsb + 1);
            if (bits <= 0)
            {
                m.Invalid();
                return;
            }
            var dt = PrimitiveType.CreateWord(bits);
            var tmp = binder.CreateTemporary(dt);
            m.Assign(tmp, m.Slice(src, dt, lsb));
            m.Assign(dst, m.Dpb(dst, tmp, lsb));
        }

        private void RewriteIndexExpression(int shift)
        {
            MaybeConditionalExecution();
            var src = Operand(1);
            var idx = Operand(2);
            var dst = Operand(0);
            m.Assign(dst, m.IAdd(src, m.Shl(idx, shift)));
        }

        private void RewriteJmp()
        {
            var src = Operand(0);
            if (src is Identifier id &&
                id.Storage == Registers.GpRegs[15])
            {
                m.Return(0, 0);
            }
            m.Goto(src);
        }

        private void RewriteJmpi()
        {
            var src = Operand(0);
            m.Goto(m.Mem(PrimitiveType.Word32, src));
        }

        private void RewriteJmpix()
        {
            var idx = Operand(0);
            var scale = Operand(1);
            var vbr = binder.EnsureRegister(Registers.CodesToControlRegisters[1]);
            m.Goto(m.Mem(arch.PointerType, m.IAdd(vbr, m.IMul(idx, scale))));
        }

        private void RewriteJsr()
        {
            var target = Operand(0);
            m.Call(target, 0);
        }

        private void RewriteJsri()
        {
            var target = Operand(0);
            m.Call(m.Mem(arch.PointerType, target), 0);
        }

        private void RewriteLdex()
        {
            var src = Operand(1);
            var dst = Operand(0);
            m.Assign(dst, m.Fn(ldex_intrinsic.MakeInstance(32, src.DataType), m.AddrOf(arch.PointerType, src)));
        }

        private void RewriteLoad(PrimitiveType dtDst)
        {
            MaybeConditionalExecution();
            var src = Operand(1);
            var dst = Operand(0);
            if (dst.DataType.BitSize > src.DataType.BitSize)
            {
                var tmp = binder.CreateTemporary(src.DataType);
                m.Assign(tmp, src);
                src = m.Convert(tmp, src.DataType, dtDst);
            }
            m.Assign(dst, src);
        }

        private void RewriteLoad64()
        {
            var src = Operand(1);
            var rdst = (RegisterStorage) instr.Operands[0];
            var rdstp1 = Registers.GpRegs[(rdst.Number + 1) % Registers.GpRegs.Length];
            var dst = binder.EnsureSequence(PrimitiveType.Word32, rdstp1, rdst);
            m.Assign(dst, src);
        }

        private void RewriteLrs(PrimitiveType dt)
        {
            MaybeConditionalExecution();
            var gp = Operand(1);
            var offset = Operand(2);
            var dst = Operand(0);
            MaybeZeroExtend(dst, m.Mem(dt, m.IAdd(gp, offset)));
        }

        private void RewriteLrw()
        {
            MaybeConditionalExecution();
            var src = Operand(1);
            var dst = Operand(0);
            m.Assign(dst, src);
        }

        private void RewriteMac(BinaryOperator mul, BinaryOperator? acc)
        {
            MaybeConditionalExecution();
            var left = Operand(0);
            var right = Operand(1);
            var dst = binder.EnsureRegister(Registers.Macc);
            if (acc is null)
            {
                m.Assign(dst, m.Bin(mul, dst.DataType, left, right));
            }
            else
            {
                var tmp = binder.CreateTemporary(dst.DataType);
                m.Assign(tmp, m.Bin(mul, dst.DataType, left, right));
                m.Assign(dst, m.Bin(acc, dst, tmp));
            }
        }

        private void RewriteMach(BinaryOperator mul, BinaryOperator? acc)
        {
            MaybeConditionalExecution();
            var (dst, l, r) = Rewrite2or3operands();
            var left = binder.CreateTemporary(PrimitiveType.Word16);
            var right = binder.CreateTemporary(PrimitiveType.Word16);
            m.Assign(left, m.Slice(l, PrimitiveType.Word16));
            m.Assign(right, m.Slice(r, PrimitiveType.Word16));
            if (acc is null)
            {
                m.Assign(dst, m.Bin(mul, dst.DataType, left, right));
            }
            else
            {
                var tmp = binder.CreateTemporary(dst.DataType);
                m.Assign(tmp, m.Bin(mul, dst.DataType, left, right));
                m.Assign(dst, m.Bin(acc, dst, tmp));
            }
        }

        private void RewriteMacw(BinaryOperator mul, BinaryOperator? acc)
        {
            // All mulsw* instructions are 16x32 multiplications.
            MaybeConditionalExecution();
            var (dst, l, r) = Rewrite2or3operands();
            var left = binder.CreateTemporary(PrimitiveType.Word16);
            var right = binder.CreateTemporary(PrimitiveType.Word16);
            m.Assign(left, m.Slice(l, PrimitiveType.Word16));
            m.Assign(right, m.Slice(r, PrimitiveType.Word16));
            if (acc is null)
            {
                m.Assign(dst, m.Slice(m.Bin(mul, word48, left, right), PrimitiveType.Word32, 16));
            }
            else
            {
                var tmp = binder.CreateTemporary(dst.DataType);
                m.Assign(tmp, m.Slice(m.Bin(mul, word48, left, right), PrimitiveType.Word32, 16));
                m.Assign(dst, m.Bin(acc, dst, tmp));
            }
        }

        private void RewriteMfcr()
        {
            var src = Operand(1);
            var dst = Operand(0);
            m.Assign(dst, m.Fn(mfcr_intrinsic, src));
        }

        private void RewriteMfhilos(IntrinsicProcedure proc)
        {
            var dst = Operand(0);
            m.Assign(dst, m.Fn(proc));
        }

        private void RewriteMov()
        {
            MaybeConditionalExecution();
            var src = Operand(1);
            var dst = Operand(0);
            m.Assign(dst, src);
        }

        private void RewriteMov(int iOp, RegisterStorage reg)
        {
            var src = binder.EnsureRegister(reg);
            var dst = Operand(iOp);
            m.Assign(dst, src);
        }

        private void RewriteMov(RegisterStorage reg, int iOp)
        {
            var src = Operand(iOp);
            var dst = binder.EnsureRegister(reg);
            m.Assign(dst, src);
        }

        private void RewriteMovih()
        {
            MaybeConditionalExecution();
            var src = ((ImmediateOperand) instr.Operands[1]).Value.ToUInt32();
            m.Assign(Operand(0), m.Word32(src << 16));
        }

        private void RewriteMtcr()
        {
            var src = Operand(1);
            var dst = Operand(0);
            m.SideEffect(m.Fn(mtcr_intrinsic, src, dst));
        }

        private void RewriteMvc()
        {
            var c = binder.EnsureFlagGroup(Registers.C);
            var dst = Operand(0);
            m.Assign(dst, m.ExtendZ(c, dst.DataType));
        }

        private void RewriteMvcv()
        {
            var c = binder.EnsureFlagGroup(Registers.C);
            var dst = Operand(0);
            m.Assign(dst, m.ExtendZ(m.Not(c), dst.DataType));
        }

        private void RewriteMvtc()
        {
            var v = binder.EnsureFlagGroup(Registers.V);
            var c = binder.EnsureFlagGroup(Registers.C);
            m.Assign(c, v);
        }

        private void RewriteNie()
        {
            m.SideEffect(m.Fn(nie_intrinsic));
        }

        private void RewriteNir()
        {
            m.SideEffect(m.Fn(nir_intrinsic));
            m.Return(0, 0);
        }

        private void RewriteNor()
        {
            MaybeConditionalExecution();
            var (dst, l, r) = Rewrite2or3operands();
            m.Assign(dst, m.Comp(m.Or(l, r)));
        }

        private void RewritePrefetch(IntrinsicProcedure proc)
        {
            var src = Operand(0);
            m.SideEffect(m.Fn(proc, m.AddrOf(PrimitiveType.Ptr32, src)));
        }

        private void RewritePop()
        {
            var regs = (RegisterListOperand) instr.Operands[0];
            var sp = binder.EnsureRegister(arch.StackRegister);
            var r15popped = false;
            foreach (var reg in regs.RegisterList)
            {
                var id = binder.EnsureRegister(reg);
                m.Assign(id, m.Mem(reg.Width, sp));
                m.Assign(sp, m.IAddS(sp, id.DataType.Size));
                r15popped |= reg.Number == 15;
            }
            if (r15popped)
                m.Return(0, 0);
        }

        private void RewritePush()
        {
            var regs = (RegisterListOperand) instr.Operands[0];
            var sp = binder.EnsureRegister(arch.StackRegister);
            foreach (var reg in regs.RegisterList.Reverse())
            {
                var id = binder.EnsureRegister(reg);
                m.Assign(sp, m.ISubS(sp, id.DataType.Size));
                m.Assign(m.Mem(reg.Width, sp), id);
            }
        }

        private void RewriteReverse(IntrinsicProcedure proc)
        {
            MaybeConditionalExecution();
            var src = Operand(1);
            var dst = Operand(0);
            m.Assign(dst, m.Fn(proc, src));
        }

        private void RewriteRfi()
        {
            m.SideEffect(m.Fn(rfi_intrinsic));
            m.Return(0, 0);
        }

        private void RewriteRte()
        {
            m.SideEffect(m.Fn(rte_intrinsic));
            m.Return(0, 0);
        }

        private void RewriteSce()
        {
            this.conditionalLeft = 4 + 1;
            this.conditionalExecutionMask = ((ImmediateOperand) instr.Operands[0]).Value.ToUInt32();
            m.Nop();
        }

        private void RewriteSe()
        {
            m.SideEffect(m.Fn(se_intrinsic));
        }

        private void RewriteSext()
        {
            MaybeConditionalExecution();
            var lsb = ((ImmediateOperand) instr.Operands[2]).Value.ToInt32();
            var msb = ((ImmediateOperand) instr.Operands[3]).Value.ToInt32();
            var bits = 1 + msb - lsb;
            if (bits <= 0)
            {
                iclass = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            var dt = PrimitiveType.CreateWord(bits);
            var src = Operand(1);
            var dst = Operand(0);
            var tmp = binder.CreateTemporary(dt);
            m.Assign(tmp, m.Slice(src, dt));
            m.Assign(dst, m.ExtendS(tmp, dst.DataType));
        }

        private void RewriteSext(PrimitiveType dt)
        {
            MaybeConditionalExecution();
            var src = Operand(1);
            var dst = Operand(0);
            var tmp = binder.CreateTemporary(dt);
            m.Assign(tmp, m.Slice(src, dt));
            m.Assign(dst, m.ExtendS(tmp, dst.DataType));
        }

        private void RewriteSrs(PrimitiveType dt)
        {
            MaybeConditionalExecution();
            var gp = Operand(1);
            var offset = Operand(2);
            var src = MaybeSlice(Operand(0), dt);
            m.Assign(m.Mem(dt, m.IAdd(gp, offset)), src);
        }

        private void RewriteSrte()
        {
            m.SideEffect(m.Fn(srte_intrinsic));
            m.Return(0, 0);
        }

        private void RewriteStex()
        {
            var src = Operand(0);
            var dst = Operand(1);
            m.SideEffect(m.Fn(stex_intrinsic.MakeInstance(32, src.DataType), src, m.AddrOf(arch.PointerType, dst)));
        }

        private void RewriteStop()
        {
            m.SideEffect(m.Fn(stop_intrinsic));
        }

        private void RewriteStore()
        {
            MaybeConditionalExecution();
            var src = Operand(0);
            var dst = Operand(1);
            m.Assign(dst, MaybeSlice(src, dst.DataType));
        }

        private void RewriteStore64()
        {
            var dst = Operand(1);
            var rsrc = (RegisterStorage) instr.Operands[0];
            var rsrcp1 = Registers.GpRegs[(rsrc.Number + 1) % Registers.GpRegs.Length];
            var src = binder.EnsureSequence(PrimitiveType.Word32, rsrcp1, rsrc);
            m.Assign(dst, src);
        }

        private void RewriteStrap()
        {
            m.SideEffect(m.Fn(strap_intrinsic));
        }

        private void RewriteSubc()
        {
            MaybeConditionalExecution();
            Identifier c = binder.EnsureFlagGroup(Registers.C);
            var (dst, left, right) = Rewrite2or3operands();
            m.Assign(dst, m.ISubC(left, right, c));
            m.Assign(c, m.Cond(dst));
        }

        private void RewriteSync()
        {
            m.SideEffect(m.Fn(sync_intrinsic));
        }

        private void RewriteTrap()
        {
            var trapNo = Operand(0);
            m.SideEffect(m.Fn(CommonOps.Syscall_1, trapNo));
        }

        private void RewriteTst()
        {
            var left = Operand(0);
            var right = Operand(1);
            var c = binder.EnsureFlagGroup(Registers.C);
            m.Assign(c, m.Ne0(m.And(left, right)));
        }

        private void RewriteTstnbz()
        {
            var src = Operand(0);
            var c = binder.EnsureFlagGroup(Registers.C);
            m.Assign(c, m.Fn(tstnbz_intrinsic, src));
        }

        private void RewriteVmacsh(BinaryOperator? acc)
        {
            MaybeConditionalExecution();
            var left = Operand(0);
            var right = Operand(1);
            var leftHi = binder.CreateTemporary(PrimitiveType.Word16);
            var leftLo = binder.CreateTemporary(PrimitiveType.Word16);
            var rightHi = binder.CreateTemporary(PrimitiveType.Word16);
            var rightLo = binder.CreateTemporary(PrimitiveType.Word16);
            var hi = binder.EnsureRegister(Registers.Hi);
            var lo = binder.EnsureRegister(Registers.Lo);
            m.Assign(leftHi, m.Slice(left, PrimitiveType.Word16, 16));
            m.Assign(leftLo, m.Slice(left, PrimitiveType.Word16, 0));
            m.Assign(rightHi, m.Slice(right, PrimitiveType.Word16, 16));
            m.Assign(rightLo, m.Slice(right, PrimitiveType.Word16, 0));
            var hiproduct = m.SMul(PrimitiveType.Int32, leftHi, rightHi);
            var loproduct = m.SMul(PrimitiveType.Int32, leftLo, rightLo);
            if (acc is null)
            {
                m.Assign(hi, hiproduct);
                m.Assign(lo, loproduct);
            }
            else
            {
                m.Assign(hi, m.Bin(acc, hi, hiproduct));
                m.Assign(lo, m.Bin(acc, lo, loproduct));
            }
        }

        private void RewriteVmacsw(BinaryOperator? acc)
        {
            MaybeConditionalExecution();
            var left = Operand(0);
            var right = Operand(1);
            var leftHi = binder.CreateTemporary(PrimitiveType.Word16);
            var leftLo = binder.CreateTemporary(PrimitiveType.Word16);
            var rightHi = binder.CreateTemporary(PrimitiveType.Word16);
            var rightLo = binder.CreateTemporary(PrimitiveType.Word16);
            var hi = binder.EnsureRegister(Registers.Hi);
            var lo = binder.EnsureRegister(Registers.Lo);
            m.Assign(leftHi, m.Slice(left, PrimitiveType.Word16, 16));
            m.Assign(leftLo, m.Slice(left, PrimitiveType.Word16, 0));
            m.Assign(rightHi, m.Slice(right, PrimitiveType.Word16, 16));
            m.Assign(rightLo, m.Slice(right, PrimitiveType.Word16, 0));
            var word48 = PrimitiveType.CreateWord(48);
            var hiproduct = m.Slice(m.SMul(word48, leftHi, rightHi), PrimitiveType.Word32, 16);
            var loproduct = m.Slice(m.SMul(word48, leftLo, rightLo), PrimitiveType.Word32, 16);
            if (acc is null)
            {
                m.Assign(hi, hiproduct);
                m.Assign(lo, loproduct);
            }
            else
            {
                m.Assign(hi, m.Bin(acc, hi, hiproduct));
                m.Assign(lo, m.Bin(acc, lo, loproduct));
            }
        }

        private void RewriteWait()
        {
            m.SideEffect(m.Fn(wait_intrinsic));
        }

        private void RewriteWe()
        {
            m.SideEffect(m.Fn(we_intrinsic));
        }

        private void RewriteXtb(int lsb)
        {
            MaybeConditionalExecution();
            var slice = m.Slice(Operand(1), PrimitiveType.Byte, lsb);
            var tmp = binder.CreateTemporary(slice.DataType);
            m.Assign(tmp, slice);
            MaybeZeroExtend(Operand(0), tmp);
        }

        private void RewriteZext()
        {
            MaybeConditionalExecution();
            var lsb = ((ImmediateOperand) instr.Operands[2]).Value.ToInt32();
            var msb = ((ImmediateOperand) instr.Operands[3]).Value.ToInt32();
            var bits = 1 + msb - lsb;
            if (bits <= 0)
            {
                iclass = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            var dt = PrimitiveType.CreateWord(bits);
            var src = Operand(1);
            var dst = Operand(0);
            var tmp = binder.CreateTemporary(dt);
            m.Assign(tmp, m.Slice(src, dt));
            m.Assign(dst, m.ExtendZ(tmp, dst.DataType));
        }

        private void RewriteZext(PrimitiveType dt)
        {
            MaybeConditionalExecution();
            var src = Operand(1);
            var dst = Operand(0);
            var tmp = binder.CreateTemporary(dt);
            m.Assign(tmp, m.Slice(src, dt));
            m.Assign(dst, m.ExtendZ(tmp, dst.DataType));
        }

        private static readonly IntrinsicProcedure bkpt_intrinsic = new IntrinsicBuilder("__bkpt", true).Void();
        private static readonly IntrinsicProcedure bmclr_intrinsic = new IntrinsicBuilder("__bmclr", true).Void();
        private static readonly IntrinsicProcedure bmset_intrinsic = new IntrinsicBuilder("__bmset", true).Void();
        private static readonly IntrinsicProcedure bpop_h_intrinsic = new IntrinsicBuilder("__bpop_h", true).Void();
        private static readonly IntrinsicProcedure bpop_w_intrinsic = new IntrinsicBuilder("__bpop_w", true).Void();
        private static readonly IntrinsicProcedure bpush_h_intrinsic = new IntrinsicBuilder("__bpush_h", true).Void();
        private static readonly IntrinsicProcedure bpush_w_intrinsic = new IntrinsicBuilder("__bpush_w", true).Void();
        private static readonly IntrinsicProcedure doze_intrinsic = new IntrinsicBuilder("__doze", true).Void();
        private static readonly IntrinsicProcedure ff0_intrinsic = new IntrinsicBuilder("__ff0", true)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Int32);
        private static readonly IntrinsicProcedure ff1_intrinsic = new IntrinsicBuilder("__ff1", true)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Int32);
        private static readonly IntrinsicProcedure idly_intrinsic = new IntrinsicBuilder("__interrupt_delay", true)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure ipop_intrinsic = new IntrinsicBuilder("__ipop", true).Void();
        private static readonly IntrinsicProcedure ipush_intrinsic = new IntrinsicBuilder("__ipush", true).Void();
        private static readonly IntrinsicProcedure ldex_intrinsic = new IntrinsicBuilder("__ldex", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns("T");
        private static readonly IntrinsicProcedure mfcr_intrinsic = new IntrinsicBuilder("__read_control_register", true)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure mfhis_intrinsic = new IntrinsicBuilder("__mfhis", true).Void();
        private static readonly IntrinsicProcedure mflos_intrinsic = new IntrinsicBuilder("__mflos", true).Void();
        private static readonly IntrinsicProcedure mtcr_intrinsic = new IntrinsicBuilder("__write_control_register", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure mvtc_intrinsic = new IntrinsicBuilder("__mvtc", true).Void();
        private static readonly IntrinsicProcedure nie_intrinsic = new IntrinsicBuilder("__nie", true).Void();
        private static readonly IntrinsicProcedure nir_intrinsic = new IntrinsicBuilder("__interrup_nesting_return", true).Void();
        private static readonly IntrinsicProcedure pldr_intrinsic = new IntrinsicBuilder("__pldr", true)
            .Param(PrimitiveType.Ptr32)
            .Void();
        private static readonly IntrinsicProcedure pldrw_intrinsic = new IntrinsicBuilder("__pldrw", true)
            .Param(PrimitiveType.Ptr32)
            .Void();
        private static readonly IntrinsicProcedure rfi_intrinsic = new IntrinsicBuilder("__rfi", true).Void();
        private static readonly IntrinsicProcedure rte_intrinsic = new IntrinsicBuilder("__rte", true).Void();
        private static readonly IntrinsicProcedure se_intrinsic = new IntrinsicBuilder("__se", true).Void();
        private static readonly IntrinsicProcedure srte_intrinsic = new IntrinsicBuilder("__srte", true).Void();
        private static readonly IntrinsicProcedure stex_intrinsic = new IntrinsicBuilder("__stex_w", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Param("T")
            .Void();
        private static readonly IntrinsicProcedure stop_intrinsic = new IntrinsicBuilder("__stop", true).Void();
        private static readonly IntrinsicProcedure strap_intrinsic = new IntrinsicBuilder("__strap", true).Void();
        private static readonly IntrinsicProcedure sync_intrinsic = new IntrinsicBuilder("__sync", true).Void();
        private static readonly IntrinsicProcedure tstnbz_intrinsic = new IntrinsicBuilder("__tstnbz", false)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Bool);
        private static readonly IntrinsicProcedure wait_intrinsic = new IntrinsicBuilder("__wait", true).Void();
        private static readonly IntrinsicProcedure we_intrinsic = new IntrinsicBuilder("__we", true).Void();
        private static readonly IntrinsicProcedure xsr_intrinsic = new IntrinsicBuilder("__xsr", false)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Int32)
            .Returns(PrimitiveType.Word32);
        private static readonly PrimitiveType word48 = PrimitiveType.CreateWord(48);


    }
}
