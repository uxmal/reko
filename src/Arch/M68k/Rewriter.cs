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
using Reko.Core.Operators;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Reko.Core.Services;
using Reko.Core.Memory;

namespace Reko.Arch.M68k
{
    /// <summary>
    /// Rewrites <seealso cref="M68kInstruction"/>s to <see cref="RtlInstructionCluster"/>s.
    /// </summary>
    /// http://www.easy68k.com/paulrsm/doc/trick68k.htm
    public partial class Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private static readonly Dictionary<int, double> fpuRomConstants;

        // These fields are internal so that the OperandRewriter can use them.
        private readonly M68kArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly IStorageBinder binder;
        private readonly M68kState state;
        private readonly IRewriterHost host;
        private readonly IEnumerator<M68kInstruction> dasm;
        private M68kInstruction instr;
        private RtlEmitter m;
        private List<RtlInstruction> rtlInstructions;
        private InstrClass iclass;
        private OperandRewriter orw;

        public Rewriter(M68kArchitecture m68kArchitecture, EndianImageReader rdr, M68kState m68kState, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = m68kArchitecture;
            this.state = m68kState;
            this.binder = binder;
            this.host = host;
            this.rdr = rdr;
            this.dasm = arch.CreateDisassemblerImpl(rdr).GetEnumerator();
            this.instr = default!;
            this.m = default!;
            this.orw = default!;
            this.rtlInstructions = new List<RtlInstruction>();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                instr = dasm.Current;
                var addr = instr.Address;
                var len = instr.Length;
                iclass = instr.InstructionClass;
                m = new RtlEmitter(rtlInstructions);
                orw = new OperandRewriter(arch, this.m, this.binder, instr.DataWidth!);
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    host.Warn(
                        instr.Address,
                        "M68k instruction '{0}' is not supported yet.",
                        instr.Mnemonic);
                    m.Invalid();
                    iclass = InstrClass.Invalid;
                    break;
                case Mnemonic.illegal: RewriteIllegal(); break;
                case Mnemonic.abcd: RewriteAbcd(); break;
                case Mnemonic.add: RewriteBinOp((s, d) => m.IAdd(d, s), Registers.CVZNX); break;
                case Mnemonic.adda: RewriteBinOp((s, d) => m.IAdd(d, s)); break;
                case Mnemonic.addi: RewriteArithmetic((s, d) => m.IAdd(d, s)); break;
                case Mnemonic.addq: RewriteAddSubq((s, d) => m.IAdd(d, s)); break;
                case Mnemonic.addx: RewriteAddSubx(m.IAdd); break;
                
                case Mnemonic.and: RewriteLogical((s, d) => m.And(d, s)); break;
                case Mnemonic.andi: RewriteLogical((s, d) => m.And(d, s)); break;
                case Mnemonic.asl: RewriteShift((s, d) => m.Shl(d, s)); break;
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
                case Mnemonic.bcc: RewriteBcc(ConditionCode.UGE, Registers.C); break;
                case Mnemonic.bcs: RewriteBcc(ConditionCode.ULT, Registers.C); break;
                case Mnemonic.beq: RewriteBcc(ConditionCode.EQ, Registers.Z); break;
                case Mnemonic.bge: RewriteBcc(ConditionCode.GE, Registers.VN); break;
                case Mnemonic.bgt: RewriteBcc(ConditionCode.GT, Registers.VZN); break;
                case Mnemonic.bhi: RewriteBcc(ConditionCode.UGT, Registers.CZ); break;
                case Mnemonic.ble: RewriteBcc(ConditionCode.LE, Registers.VZN); break;
                case Mnemonic.bls: RewriteBcc(ConditionCode.ULE, Registers.CZ); break;
                case Mnemonic.blt: RewriteBcc(ConditionCode.LT, Registers.VN); break;
                case Mnemonic.bmi: RewriteBcc(ConditionCode.LT, Registers.N); break;
                case Mnemonic.bne: RewriteBcc(ConditionCode.NE, Registers.Z); break;
                case Mnemonic.bpl: RewriteBcc(ConditionCode.GT, Registers.N); break;
                case Mnemonic.bvc: RewriteBcc(ConditionCode.NO, Registers.V); break;
                case Mnemonic.bvs: RewriteBcc(ConditionCode.OV, Registers.V); break;
                case Mnemonic.bchg: RewriteBchg(); break;
                case Mnemonic.bfchg: RewriteBfchg(); break;
                case Mnemonic.bkpt: RewriteBkpt(); break;
                case Mnemonic.bra: RewriteBra(); break;
                case Mnemonic.bset: RewriteBclrBset("__bset"); break;
                case Mnemonic.bsr: RewriteBsr(); break;
                case Mnemonic.btst: RewriteBtst(); break;
                case Mnemonic.callm: RewriteCallm(); break;
                case Mnemonic.cas: RewriteCas(); break;
                case Mnemonic.cinva: RewriteCinva(); break;
                case Mnemonic.cinvl: RewriteCinvl(); break;
                case Mnemonic.cinvp: RewriteCinvp(); break;
                case Mnemonic.clr: RewriteClr(); break;
                case Mnemonic.chk: RewriteChk(); break;
                case Mnemonic.chk2: RewriteChk2(); break;
                case Mnemonic.cmp: RewriteCmp(); break;
                case Mnemonic.cmp2: RewriteCmp2(); break;
                case Mnemonic.cmpa: RewriteCmp(); break;
                case Mnemonic.cmpi: RewriteCmp(); break;
                case Mnemonic.cmpm: RewriteCmp(); break;

                case Mnemonic.dbcc: RewriteDbcc(ConditionCode.UGE, Registers.C); break;
                case Mnemonic.dbcs: RewriteDbcc(ConditionCode.ULT, Registers.C); break;
                case Mnemonic.dbeq: RewriteDbcc(ConditionCode.EQ, Registers.Z); break;
                case Mnemonic.dbge: RewriteDbcc(ConditionCode.GE, Registers.VN); break;
                case Mnemonic.dbgt: RewriteDbcc(ConditionCode.GT, Registers.VZN); break;
                case Mnemonic.dbhi: RewriteDbcc(ConditionCode.UGT, Registers.CZ); break;
                case Mnemonic.dble: RewriteDbcc(ConditionCode.LE, Registers.VZN); break;
                case Mnemonic.dbls: RewriteDbcc(ConditionCode.ULE, Registers.CZ); break;
                case Mnemonic.dblt: RewriteDbcc(ConditionCode.LT, Registers.VN); break;
                case Mnemonic.dbmi: RewriteDbcc(ConditionCode.LT, Registers.N); break;
                case Mnemonic.dbne: RewriteDbcc(ConditionCode.NE, Registers.Z); break;
                case Mnemonic.dbpl: RewriteDbcc(ConditionCode.GT, Registers.N); break;
                case Mnemonic.dbt: RewriteDbcc(ConditionCode.ALWAYS, null); break;
                case Mnemonic.dbvc: RewriteDbcc(ConditionCode.NO, Registers.V); break;
                case Mnemonic.dbvs: RewriteDbcc(ConditionCode.OV, Registers.V); break;
                case Mnemonic.dbra: RewriteDbcc(ConditionCode.None, null); break;
                case Mnemonic.divs: RewriteDiv(m.SDiv, PrimitiveType.Int16); break;
                case Mnemonic.divsl: RewriteDiv(m.SDiv, PrimitiveType.Int32); break;
                case Mnemonic.divu: RewriteDiv(m.UDiv, PrimitiveType.UInt16); break;
                case Mnemonic.divul: RewriteDiv(m.UDiv, PrimitiveType.UInt32); break;
                case Mnemonic.eor: RewriteLogical((s, d) => m.Xor(d, s)); break;
                case Mnemonic.eori: RewriteLogical((s, d) => m.Xor(d, s)); break;
                case Mnemonic.exg: RewriteExg(); break;
                case Mnemonic.ext: RewriteExt(); break;
                case Mnemonic.extb: RewriteExtb(); break;
                case Mnemonic.fabs: RewriteFabs(); break;
                case Mnemonic.fadd: RewriteFBinOp((s, d) => m.FAdd(d, s)); break;
                case Mnemonic.facos: RewriteFUnaryIntrinsic("acos"); break;
                //$REVIEW: the following don't respect NaN, but NaN typically doesn't exist in HLLs.
                case Mnemonic.fatan: RewriteFUnaryIntrinsic("atan"); break;
                case Mnemonic.fatanh: RewriteFUnaryIntrinsic("atanh"); break;
                case Mnemonic.fbf: m.Nop(); break;
                case Mnemonic.fblt: RewriteFbcc(ConditionCode.LT); break;
                case Mnemonic.fbgl: RewriteFbcc(ConditionCode.NE); break;
                case Mnemonic.fbeq: RewriteFbcc(ConditionCode.EQ); break;
                case Mnemonic.fbgt: RewriteFbcc(ConditionCode.GT); break;
                case Mnemonic.fbge: RewriteFbcc(ConditionCode.GE); break;
                case Mnemonic.fbgle: RewriteFbcc(ConditionCode.NE); break;    //$BUG: should be !is_nan
                case Mnemonic.fble: RewriteFbcc(ConditionCode.LE); break;
                case Mnemonic.fbne: RewriteFbcc(ConditionCode.NE); break;
                case Mnemonic.fbnge: RewriteFbcc(ConditionCode.LT); break;
                case Mnemonic.fbngl: RewriteFbcc(ConditionCode.EQ); break;
                case Mnemonic.fbngt: RewriteFbcc(ConditionCode.LE); break;
                case Mnemonic.fbngle: RewriteFbcc(ConditionCode.EQ); break;   //$BUG: should be is_nan
                case Mnemonic.fbnlt: RewriteFbcc(ConditionCode.GE); break;
                case Mnemonic.fbnle: RewriteFbcc(ConditionCode.GT); break;
                case Mnemonic.fboge: RewriteFbcc(ConditionCode.GE); break;
                case Mnemonic.fbogl: RewriteFbcc(ConditionCode.NE); break;
                case Mnemonic.fbole: RewriteFbcc(ConditionCode.LE); break;
                case Mnemonic.fbolt: RewriteFbcc(ConditionCode.LT); break;
                case Mnemonic.fbogt: RewriteFbcc(ConditionCode.GT); break;
                case Mnemonic.fbor: RewriteFbcc(ConditionCode.EQ); break;     //$REVIEW: is this correct?
                case Mnemonic.fbseq: RewriteFbcc(ConditionCode.EQ); break;
                case Mnemonic.fbsf: RewriteFbcc(ConditionCode.NEVER); break;
                case Mnemonic.fbsne: RewriteFbcc(ConditionCode.NE); break;
                case Mnemonic.fbst: RewriteFbcc(ConditionCode.ALWAYS); break;
                case Mnemonic.fbt: RewriteBra(); break;
                case Mnemonic.fbueq: RewriteFbcc(ConditionCode.EQ); break;
                case Mnemonic.fbuge: RewriteFbcc(ConditionCode.GE); break;
                case Mnemonic.fbugt: RewriteFbcc(ConditionCode.GT); break;
                case Mnemonic.fbule: RewriteFbcc(ConditionCode.LE); break;
                case Mnemonic.fbult: RewriteFbcc(ConditionCode.LT); break;
                case Mnemonic.fbun: RewriteFbcc(ConditionCode.IS_NAN); break;
                case Mnemonic.fasin: RewriteFasin(); break;
                case Mnemonic.fintrz: RewriteFintrz(); break;
                case Mnemonic.fcmp: RewriteFcmp(); break;
                case Mnemonic.fcos: RewriteFUnaryIntrinsic("cos"); break;
                case Mnemonic.fcosh: RewriteFUnaryIntrinsic("cosh"); break;
                case Mnemonic.fdiv: RewriteFBinOp((s, d) => m.FDiv(d, s)); break;
                case Mnemonic.fdsqrt: RewriteFUnaryIntrinsic("sqrt"); break;
                case Mnemonic.fetox: RewriteFUnaryIntrinsic("exp"); break;
                case Mnemonic.fetoxm1: RewriteFUnaryIntrinsic("exp", e => m.FSub(e, Constant.Real64(1.0))); break;
                case Mnemonic.fgetexp: RewriteFUnaryIntrinsic("fgetexp"); break;
                case Mnemonic.fgetman: RewriteFUnaryIntrinsic("fgetman"); break;
                case Mnemonic.fint: RewriteFUnaryIntrinsic("trunc"); break;
                case Mnemonic.flog10: RewriteFUnaryIntrinsic("log10"); break;
                case Mnemonic.flog2: RewriteFUnaryIntrinsic("log2"); break;
                case Mnemonic.flogn: RewriteFUnaryIntrinsic("log"); break;
                case Mnemonic.flognp1: RewriteFUnaryIntrinsic(e => m.FAdd(e, Constant.Real64(1.0)), "log"); break;
                case Mnemonic.fmod: RewriteFBinIntrinsic("fmod"); break;
                case Mnemonic.fmove: RewriteFmove(); break;
                case Mnemonic.fmovecr: RewriteFmovecr(); break;
                case Mnemonic.fmovem: RewriteMovem(i => Registers.GetRegister(i+Registers.fp0.Number)); break;
                case Mnemonic.fmul: RewriteFBinOp((s, d) => m.FMul(d,s)); break;
                case Mnemonic.fneg: RewriteFUnaryOp(m.Neg); break;
                case Mnemonic.frem: RewriteFUnaryIntrinsic("frem"); break;
                case Mnemonic.fsabs: RewriteFUnaryIntrinsic("fabsf"); break;
                case Mnemonic.fsin: RewriteFUnaryIntrinsic("fsin"); break;
                case Mnemonic.fsincos: RewriteFSinCos(); break;
                case Mnemonic.fsqrt: RewriteFsqrt(); break;
                case Mnemonic.fsub: RewriteFBinOp((s, d) => m.FSub(d, s)); break;
                case Mnemonic.ftan: RewriteFtan(); break;
                case Mnemonic.ftanh1: RewriteFUnaryIntrinsic("tanh"); break;
                case Mnemonic.jmp: RewriteJmp(); break;
                case Mnemonic.jsr: RewriteJsr(); break;
                case Mnemonic.lea: RewriteLea(); break;
                case Mnemonic.link: RewriteLink(); break;
                case Mnemonic.lsl: RewriteShift((s, d) => m.Shl(d, s)); break;
                case Mnemonic.lsr: RewriteShift((s, d) => m.Shr(d, s)); break;
                case Mnemonic.move: RewriteMove(true); break;
                case Mnemonic.move16: RewriteMove16(); break;
                case Mnemonic.movea: RewriteMove(false); break;
                case Mnemonic.movec: RewriteMovec(); break;
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
                case Mnemonic.pload: RewritePload(); break;
                case Mnemonic.ptest: RewritePtest(); break;
                case Mnemonic.reset: RewriteReset(); break;
                case Mnemonic.rol: RewriteRotation(IntrinsicProcedure.Rol); break;
                case Mnemonic.ror: RewriteRotation(IntrinsicProcedure.Ror);  break;
                case Mnemonic.roxl: RewriteRotationX(IntrinsicProcedure.RolC);  break;
                case Mnemonic.roxr: RewriteRotationX(IntrinsicProcedure.RorC);  break;
                case Mnemonic.rtd: RewriteRtd(); break;
                case Mnemonic.rte: RewriteRte(); break;
                case Mnemonic.rtm: RewriteRtm(); break;
                case Mnemonic.rtr: RewriteRtr(); break;
                case Mnemonic.rts: RewriteRts(); break;
                case Mnemonic.sbcd: RewriteSbcd(); break;
                case Mnemonic.scc: RewriteScc(ConditionCode.UGE, Registers.C); break;
                case Mnemonic.scs: RewriteScc(ConditionCode.ULT, Registers.C); break;
                case Mnemonic.seq: RewriteScc(ConditionCode.EQ, Registers.Z); break;
                case Mnemonic.sge: RewriteScc(ConditionCode.GE, Registers.VN); break;
                case Mnemonic.sgt: RewriteScc(ConditionCode.GT, Registers.VZN); break;
                case Mnemonic.shi: RewriteScc(ConditionCode.UGT, Registers.CZ); break;
                case Mnemonic.sle: RewriteScc(ConditionCode.LE,  Registers.VZN); break;
                case Mnemonic.sls: RewriteScc(ConditionCode.ULE, Registers.CZ); break;
                case Mnemonic.slt: RewriteScc(ConditionCode.LT, Registers.VN); break;
                case Mnemonic.smi: RewriteScc(ConditionCode.LT, Registers.N); break;
                case Mnemonic.sne: RewriteScc(ConditionCode.NE, Registers.Z); break;
                case Mnemonic.spl: RewriteScc(ConditionCode.GT, Registers.N); break;
                case Mnemonic.svc: RewriteScc(ConditionCode.NO, Registers.V); break;
                case Mnemonic.svs: RewriteScc(ConditionCode.OV, Registers.V); break;
                case Mnemonic.st: orw.RewriteMoveDst(instr.Operands[0], instr.Address, PrimitiveType.Bool, Constant.True()); break;
                case Mnemonic.sf: orw.RewriteMoveDst(instr.Operands[0], instr.Address, PrimitiveType.Bool, Constant.False()); break;
                case Mnemonic.stop: RewriteStop(); break;
                case Mnemonic.sub: RewriteArithmetic((s, d) => m.ISub(d, s)); break;
                case Mnemonic.suba: RewriteBinOp((s, d) => m.ISub(d, s)); break;
                case Mnemonic.subi: RewriteArithmetic((s, d) => m.ISub(d, s)); break;
                case Mnemonic.subq: RewriteAddSubq((s, d) => m.ISub(d, s)); break;
                case Mnemonic.subx: RewriteArithmetic((s, d) => m.ISub(m.ISub(d, s), binder.EnsureFlagGroup(Registers.X))); break;
                case Mnemonic.swap: RewriteSwap(); break;
                case Mnemonic.trap: RewriteTrap(); break;
                case Mnemonic.trapcc: RewriteTrapCc(ConditionCode.UGE, Registers.C); break;
                case Mnemonic.trapcs: RewriteTrapCc(ConditionCode.ULT, Registers.C); break;
                case Mnemonic.trapeq: RewriteTrapCc(ConditionCode.EQ, Registers.Z); break;
                case Mnemonic.trapf: RewriteTrapCc(ConditionCode.NEVER, null); break;
                case Mnemonic.trapge: RewriteTrapCc(ConditionCode.GE, Registers.VN); break;
                case Mnemonic.trapgt: RewriteTrapCc(ConditionCode.GT, Registers.VZN); break;
                case Mnemonic.traphi: RewriteTrapCc(ConditionCode.UGT, Registers.CZ); break;
                case Mnemonic.traple: RewriteTrapCc(ConditionCode.LE, Registers.VZN); break;
                case Mnemonic.trapls: RewriteTrapCc(ConditionCode.ULE, Registers.CZ); break;
                case Mnemonic.traplt: RewriteTrapCc(ConditionCode.LT, Registers.VN); break;
                case Mnemonic.trapmi: RewriteTrapCc(ConditionCode.LT, Registers.N); break;
                case Mnemonic.trapne: RewriteTrapCc(ConditionCode.NE, Registers.Z); break;
                case Mnemonic.trappl: RewriteTrapCc(ConditionCode.GT, Registers.N); break;
                case Mnemonic.trapt: RewriteTrapCc(ConditionCode.ALWAYS , null); break;
                case Mnemonic.trapv: RewriteTrapCc(ConditionCode.OV, Registers.V); break;
                case Mnemonic.trapvc: RewriteTrapCc(ConditionCode.NO, Registers.V); break;
                case Mnemonic.trapvs: RewriteTrapCc(ConditionCode.OV, Registers.V); break;

                case Mnemonic.tas: RewriteTas(); break;
                case Mnemonic.tst: RewriteTst(); break;
                case Mnemonic.unlk: RewriteUnlk(); break;
                case Mnemonic.unpk: RewriteUnpk(); break;
                }
                yield return m.MakeCluster(addr, len, iclass);
                rtlInstructions.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("M68krw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private RegisterStorage? GetRegister(MachineOperand op)
        {
            var rOp = op as RegisterOperand;
            return rOp?.Register;
        }

        private void EmitInvalid()
        {
            rtlInstructions.Clear();
            iclass = InstrClass.Invalid;
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
