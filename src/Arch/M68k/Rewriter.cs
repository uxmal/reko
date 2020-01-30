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
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.M68k
{
    /// <summary>
    /// Rewrites <seealso cref="M68kInstruction"/>s to <see cref="RtlInstructionCluster"/>s.
    /// </summary>
    /// http://www.easy68k.com/paulrsm/doc/trick68k.htm
    public partial class Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private static Dictionary<int, double> fpuRomConstants;

        // These fields are internal so that the OperandRewriter can use them.
        private readonly M68kArchitecture arch;
        private readonly IStorageBinder binder;
        private readonly M68kState state;
        private readonly IRewriterHost host;
        private readonly IEnumerator<M68kInstruction> dasm;
        private M68kInstruction instr;
        private RtlEmitter m;
        private List<RtlInstruction> rtlInstructions;
        private InstrClass rtlc;
        private OperandRewriter orw;

        public Rewriter(M68kArchitecture m68kArchitecture, EndianImageReader rdr, M68kState m68kState, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = m68kArchitecture;
            this.state = m68kState;
            this.binder = binder;
            this.host = host;
            this.dasm = arch.CreateDisassemblerImpl(rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                instr = dasm.Current;
                var addr = instr.Address;
                var len = instr.Length;
                rtlInstructions = new List<RtlInstruction>();
                rtlc = instr.InstructionClass;
                m = new RtlEmitter(rtlInstructions);
                orw = new OperandRewriter(arch, this.m, this.binder, instr.DataWidth);
                switch (instr.Mnemonic)
                {
                default:
                    host.Warn(
                        instr.Address,
                        "M68k instruction '{0}' is not supported yet.",
                        instr.Mnemonic);
                    m.Invalid();
                    break;
                case Mnemonic.illegal: RewriteIllegal(); break;
                case Mnemonic.abcd: RewriteAbcd(); break;
                case Mnemonic.add: RewriteBinOp((s, d) => m.IAdd(d, s), FlagM.CVZNX); break;
                case Mnemonic.adda: RewriteBinOp((s, d) => m.IAdd(d, s)); break;
                case Mnemonic.addi: RewriteArithmetic((s, d) => m.IAdd(d, s)); break;
                case Mnemonic.addq: RewriteAddSubq((s, d) => m.IAdd(d, s)); break;
                case Mnemonic.addx: RewriteAddSubx(m.IAdd); break;
                
                case Mnemonic.and: RewriteLogical((s, d) => m.And(d, s)); break;
                case Mnemonic.andi: RewriteLogical((s, d) => m.And(d, s)); break;
                case Mnemonic.asl: RewriteArithmetic((s, d) => m.Shl(d, s)); break;
                case Mnemonic.asr: RewriteShift((s, d) => m.Sar(d, s)); break;
/*
 * 
 * Mnemonic Condition Encoding Test
T* True 0000 1
F* False 0001 0
HI High 0010 C L Z
LS Low or Same 0011 C V Z
VC Overflow Clear 1000 V
VS Overflow Set 1001 V
 */
                case Mnemonic.bclr: RewriteBclrBset("__bclr"); break;
                case Mnemonic.bcc: RewriteBcc(ConditionCode.UGE, FlagM.CF); break;
                case Mnemonic.bcs: RewriteBcc(ConditionCode.ULT, FlagM.CF); break;
                case Mnemonic.beq: RewriteBcc(ConditionCode.EQ, FlagM.ZF); break;
                case Mnemonic.bge: RewriteBcc(ConditionCode.GE, FlagM.NF | FlagM.VF); break;
                case Mnemonic.bgt: RewriteBcc(ConditionCode.GT, FlagM.NF | FlagM.VF | FlagM.ZF); break;
                case Mnemonic.bhi: RewriteBcc(ConditionCode.UGT, FlagM.CF | FlagM.ZF); break;
                case Mnemonic.ble: RewriteBcc(ConditionCode.LE, FlagM.NF | FlagM.VF | FlagM.ZF); break;
                case Mnemonic.bls: RewriteBcc(ConditionCode.ULE, FlagM.CF | FlagM.ZF); break;
                case Mnemonic.blt: RewriteBcc(ConditionCode.LT, FlagM.NF | FlagM.VF); break;
                case Mnemonic.bmi: RewriteBcc(ConditionCode.LT, FlagM.NF); break;
                case Mnemonic.bne: RewriteBcc(ConditionCode.NE, FlagM.ZF); break;
                case Mnemonic.bpl: RewriteBcc(ConditionCode.GT, FlagM.NF); break;
                case Mnemonic.bvc: RewriteBcc(ConditionCode.NO, FlagM.VF); break;
                case Mnemonic.bvs: RewriteBcc(ConditionCode.OV, FlagM.VF); break;
                case Mnemonic.bchg: RewriteBchg(); break;
                case Mnemonic.bkpt: RewriteBkpt(); break;
                case Mnemonic.bra: RewriteBra(); break;
                case Mnemonic.bset: RewriteBclrBset("__bset"); break;
                case Mnemonic.bsr: RewriteBsr(); break;
                case Mnemonic.btst: RewriteBtst(); break;
                case Mnemonic.callm: RewriteCallm(); break;
                case Mnemonic.cas: RewriteCas(); break;
                case Mnemonic.clr: RewriteClr(); break;
                case Mnemonic.chk: RewriteChk(); break;
                case Mnemonic.chk2: RewriteChk2(); break;
                case Mnemonic.cmp: RewriteCmp(); break;
                case Mnemonic.cmp2: RewriteCmp2(); break;
                case Mnemonic.cmpa: RewriteCmp(); break;
                case Mnemonic.cmpi: RewriteCmp(); break;
                case Mnemonic.cmpm: RewriteCmp(); break;

                case Mnemonic.dbcc: RewriteDbcc(ConditionCode.UGE, FlagM.CF); break;
                case Mnemonic.dbcs: RewriteDbcc(ConditionCode.ULT, FlagM.CF); break;
                case Mnemonic.dbeq: RewriteDbcc(ConditionCode.EQ, FlagM.ZF); break;
                case Mnemonic.dbge: RewriteDbcc(ConditionCode.GE, FlagM.NF | FlagM.VF); break;
                case Mnemonic.dbgt: RewriteDbcc(ConditionCode.GE, FlagM.NF | FlagM.VF | FlagM.ZF); break;
                case Mnemonic.dbhi: RewriteDbcc(ConditionCode.UGT, FlagM.CF | FlagM.ZF); break;
                case Mnemonic.dble: RewriteDbcc(ConditionCode.GT, FlagM.NF | FlagM.VF | FlagM.ZF); break;
                case Mnemonic.dbls: RewriteDbcc(ConditionCode.ULE, FlagM.CF | FlagM.ZF ); break;
                case Mnemonic.dblt: RewriteDbcc(ConditionCode.LT, FlagM.NF | FlagM.VF); break;
                case Mnemonic.dbmi: RewriteDbcc(ConditionCode.LT, FlagM.NF); break;
                case Mnemonic.dbne: RewriteDbcc(ConditionCode.NE, FlagM.ZF); break;
                case Mnemonic.dbpl: RewriteDbcc(ConditionCode.GT, FlagM.NF); break;
                case Mnemonic.dbt: RewriteDbcc(ConditionCode.ALWAYS, 0); break;
                case Mnemonic.dbra: RewriteDbcc(ConditionCode.None, 0); break;
                case Mnemonic.divs: RewriteDiv(m.SDiv, PrimitiveType.Int16); break;
                case Mnemonic.divsl: RewriteDiv(m.SDiv, PrimitiveType.Int32); break;
                case Mnemonic.divu: RewriteDiv(m.UDiv, PrimitiveType.UInt16); break;
                case Mnemonic.divul: RewriteDiv(m.UDiv, PrimitiveType.UInt32); break;
                case Mnemonic.eor: RewriteLogical((s, d) => m.Xor(d, s)); break;
                case Mnemonic.eori: RewriteLogical((s, d) => m.Xor(d, s)); break;
                case Mnemonic.exg: RewriteExg(); break;
                case Mnemonic.ext: RewriteExt(); break;
                case Mnemonic.extb: RewriteExtb(); break;
                case Mnemonic.fadd: RewriteFBinOp((s, d) => m.FAdd(d, s)); break;
                    //$REVIEW: the following don't respect NaN, but NaN typically doesn't exist in HLLs.
                case Mnemonic.fbf: m.Nop(); break;
                case Mnemonic.fblt: RewriteFbcc(ConditionCode.LT); break;
                case Mnemonic.fbgl: RewriteFbcc(ConditionCode.NE); break;
                case Mnemonic.fbgt: RewriteFbcc(ConditionCode.GT); break;
                case Mnemonic.fbgle: RewriteFbcc(ConditionCode.NE); break;    //$BUG: should be !is_nan
                case Mnemonic.fbne: RewriteFbcc(ConditionCode.NE); break;
                case Mnemonic.fbnge: RewriteFbcc(ConditionCode.LT); break;
                case Mnemonic.fbngl: RewriteFbcc(ConditionCode.EQ); break;
                case Mnemonic.fbngle: RewriteFbcc(ConditionCode.EQ); break;   //$BUG: should be is_nan
                case Mnemonic.fbnlt: RewriteFbcc(ConditionCode.GE); break;
                case Mnemonic.fbnle: RewriteFbcc(ConditionCode.GT); break;
                case Mnemonic.fbogl: RewriteFbcc(ConditionCode.NE); break;
                case Mnemonic.fbole: RewriteFbcc(ConditionCode.LE); break;
                case Mnemonic.fbolt: RewriteFbcc(ConditionCode.LT); break;
                case Mnemonic.fbogt: RewriteFbcc(ConditionCode.GT); break;
                case Mnemonic.fbor: RewriteFbcc(ConditionCode.EQ); break;     //$REVIEW: is this correct?
                case Mnemonic.fbseq: RewriteFbcc(ConditionCode.EQ); break;
                case Mnemonic.fbsf: RewriteFbcc(ConditionCode.NEVER); break;
                case Mnemonic.fbsne: RewriteFbcc(ConditionCode.NE); break;
                case Mnemonic.fbst: RewriteFbcc(ConditionCode.ALWAYS); break;
                case Mnemonic.fbuge: RewriteFbcc(ConditionCode.GE); break;
                case Mnemonic.fbugt: RewriteFbcc(ConditionCode.GT); break;
                case Mnemonic.fbult: RewriteFbcc(ConditionCode.LT); break;
                case Mnemonic.fbun: RewriteFbcc(ConditionCode.IS_NAN); break;
                case Mnemonic.fasin: RewriteFasin(); break;
                case Mnemonic.fintrz: RewriteFintrz(); break;
                case Mnemonic.fcmp: RewriteFcmp(); break;
                case Mnemonic.fdiv: RewriteFBinOp((s, d) => m.FDiv(d, s)); break;
                case Mnemonic.fmove: RewriteFmove(); break;
                case Mnemonic.fmovecr: RewriteFmovecr(); break;
                case Mnemonic.fmovem: RewriteMovem(i => Registers.GetRegister(i+Registers.fp0.Number)); break;
                case Mnemonic.fmul: RewriteFBinOp((s, d) => m.FMul(d,s)); break;
                case Mnemonic.fneg: RewriteFUnaryOp(m.Neg); break;
                case Mnemonic.fsqrt: RewriteFsqrt(); break;
                case Mnemonic.fsub: RewriteFBinOp((s, d) => m.FSub(d, s)); break;
                case Mnemonic.ftan: RewriteFtan(); break;
                case Mnemonic.jmp: RewriteJmp(); break;
                case Mnemonic.jsr: RewriteJsr(); break;
                case Mnemonic.lea: RewriteLea(); break;
                case Mnemonic.link: RewriteLink(); break;
                case Mnemonic.lsl: RewriteShift((s, d) => m.Shl(d, s)); break;
                case Mnemonic.lsr: RewriteShift((s, d) => m.Shr(d, s)); break;
                case Mnemonic.move: RewriteMove(true); break;
                case Mnemonic.move16: RewriteMove16(); break;
                case Mnemonic.movea: RewriteMove(false); break;
                case Mnemonic.movep: RewriteMovep(); break;
                case Mnemonic.moveq: RewriteMoveq(); break;
                case Mnemonic.moves: RewriteMoves(); break;
                case Mnemonic.movem: RewriteMovem(Registers.GetRegister); break;
                case Mnemonic.muls: RewriteMul((s, d) => m.SMul(d, s)); break;
                case Mnemonic.mulu: RewriteMul((s, d) => m.UMul(d, s)); break;
                case Mnemonic.nbcd: RewriteNbcd(); break;
                case Mnemonic.neg: RewriteUnary(s => m.Neg(s), AllConditions); break;
                case Mnemonic.negx: RewriteUnary(RewriteNegx, AllConditions); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.not: RewriteUnary(s => m.Comp(s), LogicalConditions); break;
                case Mnemonic.or: RewriteLogical((s, d) => m.Or(d, s)); break;
                case Mnemonic.ori: RewriteLogical((s, d) => m.Or(d, s)); break;
                case Mnemonic.pack: RewritePack(); break;
                case Mnemonic.pea: RewritePea(); break;
                case Mnemonic.pflushr: RewritePflushr(); break;
                case Mnemonic.ptest: RewritePtest(); break;
                case Mnemonic.rol: RewriteRotation(PseudoProcedure.Rol); break;
                case Mnemonic.ror: RewriteRotation(PseudoProcedure.Ror);  break;
                case Mnemonic.roxl: RewriteRotationX(PseudoProcedure.RolC);  break;
                case Mnemonic.roxr: RewriteRotationX(PseudoProcedure.RorC);  break;
                case Mnemonic.rtd: RewriteRtd(); break;
                case Mnemonic.rte: RewriteRte(); break;
                case Mnemonic.rtm: RewriteRtm(); break;
                case Mnemonic.rts: RewriteRts(); break;
                case Mnemonic.sbcd: RewriteSbcd(); break;
                case Mnemonic.scc: RewriteScc(ConditionCode.UGE, FlagM.CF); break;
                case Mnemonic.scs: RewriteScc(ConditionCode.ULT, FlagM.CF); break;
                case Mnemonic.seq: RewriteScc(ConditionCode.EQ, FlagM.ZF); break;
                case Mnemonic.sge: RewriteScc(ConditionCode.GE, FlagM.NF | FlagM.VF); break;
                case Mnemonic.sgt: RewriteScc(ConditionCode.GT, FlagM.NF | FlagM.VF | FlagM.ZF); break;
                case Mnemonic.shi: RewriteScc(ConditionCode.UGT, FlagM.CF | FlagM.ZF); break;
                case Mnemonic.sle: RewriteScc(ConditionCode.LE, FlagM.NF | FlagM.VF | FlagM.ZF); break;
                case Mnemonic.sls: RewriteScc(ConditionCode.ULE, FlagM.CF | FlagM.ZF); break;
                case Mnemonic.slt: RewriteScc(ConditionCode.LT, FlagM.NF | FlagM.ZF); break;
                case Mnemonic.smi: RewriteScc(ConditionCode.LT, FlagM.NF); break;
                case Mnemonic.sne: RewriteScc(ConditionCode.NE, FlagM.ZF); break;
                case Mnemonic.spl: RewriteScc(ConditionCode.GT, FlagM.NF); break;
                case Mnemonic.svc: RewriteScc(ConditionCode.NO, FlagM.VF); break;
                case Mnemonic.svs: RewriteScc(ConditionCode.OV, FlagM.VF); break;
                case Mnemonic.st: orw.RewriteMoveDst(instr.Operands[0], instr.Address, PrimitiveType.Bool, Constant.True()); break;
                case Mnemonic.sf: orw.RewriteMoveDst(instr.Operands[0], instr.Address, PrimitiveType.Bool, Constant.False()); break;
                case Mnemonic.stop: RewriteStop(); break;
                case Mnemonic.sub: RewriteArithmetic((s, d) => m.ISub(d, s)); break;
                case Mnemonic.suba: RewriteBinOp((s, d) => m.ISub(d, s)); break;
                case Mnemonic.subi: RewriteArithmetic((s, d) => m.ISub(d, s)); break;
                case Mnemonic.subq: RewriteAddSubq((s, d) => m.ISub(d, s)); break;
                case Mnemonic.subx: RewriteArithmetic((s, d) => m.ISub(m.ISub(d, s), binder.EnsureFlagGroup(Registers.ccr, (uint)FlagM.XF, "X", PrimitiveType.Bool))); break;
                case Mnemonic.swap: RewriteSwap(); break;
                case Mnemonic.trap: RewriteTrap(); break;
                case Mnemonic.trapcc: RewriteTrapCc(ConditionCode.UGE, FlagM.CF); break;
                case Mnemonic.trapcs: RewriteTrapCc(ConditionCode.ULT, FlagM.CF); break;
                case Mnemonic.trapeq: RewriteTrapCc(ConditionCode.EQ, FlagM.ZF); break;
                case Mnemonic.trapf: RewriteTrapCc(ConditionCode.NEVER, 0); break;
                case Mnemonic.trapge: RewriteTrapCc(ConditionCode.GE, FlagM.NF | FlagM.VF); break;
                case Mnemonic.trapgt: RewriteTrapCc(ConditionCode.GT, FlagM.NF | FlagM.VF | FlagM.ZF); break;
                case Mnemonic.traphi: RewriteTrapCc(ConditionCode.UGT, FlagM.CF | FlagM.ZF); break;
                case Mnemonic.traple: RewriteTrapCc(ConditionCode.LE, FlagM.NF | FlagM.VF | FlagM.ZF); break;
                case Mnemonic.traplt: RewriteTrapCc(ConditionCode.LT, FlagM.CF | FlagM.VF); break;
                case Mnemonic.trapls: RewriteTrapCc(ConditionCode.ULE, FlagM.VF | FlagM.ZF); break;
                case Mnemonic.trapmi: RewriteTrapCc(ConditionCode.LT, FlagM.NF); break;
                case Mnemonic.trapne: RewriteTrapCc(ConditionCode.NE, FlagM.ZF); break;
                case Mnemonic.trappl: RewriteTrapCc(ConditionCode.GT, FlagM.NF); break;
                case Mnemonic.trapvc: RewriteTrapCc(ConditionCode.NO, FlagM.VF); break;
                case Mnemonic.trapvs: RewriteTrapCc(ConditionCode.OV, FlagM.VF); break;

                case Mnemonic.tas: RewriteTas(); break;
                case Mnemonic.tst: RewriteTst(); break;
                case Mnemonic.unlk: RewriteUnlk(); break;
                case Mnemonic.unpk: RewriteUnpk(); break;
                }
                yield return new RtlInstructionCluster(
                    addr,
                    len,
                    rtlInstructions.ToArray())
                {
                    Class = rtlc
                };
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private RegisterStorage GetRegister(MachineOperand op)
        {
            var rOp = op as RegisterOperand;
            return rOp?.Register;
        }

        private void EmitInvalid()
        {
            rtlInstructions.Clear();
            rtlc = InstrClass.Invalid;
            m.Invalid();
        }

        static Rewriter()
        {
            fpuRomConstants = new Dictionary<int, double>
            {
                { 0x00, Math.PI } ,
                { 0x0B, Math.Log10(2) } ,
                { 0x0C, Math.E } ,
                { 0x0D, 1.0 / Math.Log(2) } ,   // Log2(E)
                { 0x0E, Math.Log10(Math.E) } ,
                { 0x0F, 0.0 } ,
                { 0x30, Math.Log(2) } ,
                { 0x31, Math.Log(10) } ,
                { 0x32, 100 } ,
                { 0x33, 1e1 } ,
                { 0x34, 1e2 } ,
                { 0x35, 1e4 } ,
                { 0x36, 1e8 } ,
                { 0x37, 1e16 } ,
                { 0x38, 1e32 } ,
                { 0x39, 1e64 } ,
                { 0x3A, 1e128 } ,
                { 0x3B, 1e256 } ,
                // These cannot be represented in a 64-bit IEEE constant,
                // which is the limit of C#.
                //{ 0x3C, 1e512 } ,
                //{ 0x3D, 1e1024 } ,
                //{ 0x3E, 1e2048 } ,
                //{ 0x3F, 1e4096 } }  ,
            };
        }
    }
}
