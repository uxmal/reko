#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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
using Decompiler.Core.Rtl;
using Decompiler.Core.Lib;
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;
using IEnumerable = System.Collections.IEnumerable;
using IEnumerator = System.Collections.IEnumerator;

namespace Decompiler.Arch.X86
{
    /// <summary>
    /// Rewrites x86 instructions into a stream of low-level RTL-like instructions.
    /// </summary>
    public partial class X86Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private IRewriterHost host;
        private IntelArchitecture arch;
        private Frame frame;
        private LookaheadEnumerator<DisassembledInstruction> dasm;
        private RtlEmitter emitter;
        private OperandRewriter orw;
        private DisassembledInstruction di;
        private RtlInstructionCluster ric;
        private X86State state;

        public X86Rewriter(
            IntelArchitecture arch,
            IRewriterHost host,
            X86State state,
            ImageReader rdr, 
            Frame frame)
        {
            this.host = host;
            this.arch = arch;
            this.frame = frame;
            this.state = state;
            this.dasm = new LookaheadEnumerator<DisassembledInstruction>(CreateDisassemblyStream(rdr));
        }

        protected IEnumerable<DisassembledInstruction> CreateDisassemblyStream(ImageReader rdr)
        {
            var d = (X86Disassembler) arch.CreateDisassembler(rdr);
            while (rdr.IsValid)
            {
                var addr = d.Address;
                var instr = d.Disassemble();
                if (instr == null)
                    yield break;
                var length = (uint)(d.Address - addr);
                yield return new DisassembledInstruction(addr, instr, length);
            }
        }

        /// <summary>
        /// Iterator that yields one RtlIntructionCluster for each x86 instruction.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                di = dasm.Current;
                ric = new RtlInstructionCluster(di.Address, (byte)di.Length);
                emitter = new RtlEmitter(ric.Instructions);
                orw = new OperandRewriter(arch, frame, host);
                switch (di.Instruction.code)
                {
                default:
                    throw new AddressCorrelatedException(string.Format("Rewriting x86 opcode '{0}' is not supported yet.",
                        di.Instruction.code),
                        di.Address);
                case Opcode.aaa: RewriteAaa(); break;
                case Opcode.aam: RewriteAam(); break;
                case Opcode.adc: RewriteAdcSbb(BinaryOperator.Add); break;
                case Opcode.add: RewriteAddSub(BinaryOperator.Add); break;
                case Opcode.and: RewriteLogical(BinaryOperator.And); break;
                case Opcode.arpl: RewriteArpl(); break;
                case Opcode.bsr: RewriteBsr(); break;
                case Opcode.bswap: RewriteBswap(); break;
                case Opcode.bt: RewriteBt(); break;
                case Opcode.call: RewriteCall(di.Instruction.op1, di.Instruction.op1.Width); break;
                case Opcode.cbw: RewriteCbw(); break;
                case Opcode.clc: RewriteSetFlag(FlagM.CF, Constant.False()); break;
                case Opcode.cld: RewriteSetFlag(FlagM.DF, Constant.False()); break;
                case Opcode.cli: break; //$TODO
                case Opcode.cmc: emitter.Assign(orw.FlagGroup(FlagM.CF), emitter.Not(orw.FlagGroup(FlagM.CF))); break;
                case Opcode.cmp: RewriteCmp(); break;
                case Opcode.cmps: RewriteStringInstruction(); break;
                case Opcode.cmpsb: RewriteStringInstruction(); break;
                case Opcode.cwd: RewriteCwd(); break;
                case Opcode.daa: EmitDaaDas("__daa"); break;
                case Opcode.das: EmitDaaDas("__das"); break;
                case Opcode.dec: RewriteIncDec(-1); break;
                case Opcode.div: RewriteDivide(Operator.Divu, Domain.UnsignedInt); break;
                case Opcode.enter: RewriteEnter(); break;
                case Opcode.fadd: EmitCommonFpuInstruction(Operator.Add, false, false); break;
                case Opcode.faddp: EmitCommonFpuInstruction(Operator.Add, false, true); break;
                case Opcode.fchs: EmitFchs(); break;
                case Opcode.fclex: RewriteFclex(); break;
                case Opcode.fcom: RewriteFcom(0); break;
                case Opcode.fcomp: RewriteFcom(1); break;
                case Opcode.fcompp: RewriteFcom(2); break;
                case Opcode.fcos: RewriteFUnary("cos"); break;
                case Opcode.fdiv: EmitCommonFpuInstruction(Operator.Divs, false, false); break;
                case Opcode.fdivp: EmitCommonFpuInstruction(Operator.Divs, false, true); break;
                case Opcode.fiadd: EmitCommonFpuInstruction(Operator.Add, false, false, PrimitiveType.Real64); break;
                case Opcode.fimul: EmitCommonFpuInstruction(Operator.Mul, false, false, PrimitiveType.Real64); break;
                case Opcode.fisub: EmitCommonFpuInstruction(Operator.Sub, false, false, PrimitiveType.Real64); break;
                case Opcode.fisubr: EmitCommonFpuInstruction(Operator.Sub, true, false, PrimitiveType.Real64); break;
                case Opcode.fidiv: EmitCommonFpuInstruction(Operator.Divs, false, false, PrimitiveType.Real64); break;
                case Opcode.fdivr: EmitCommonFpuInstruction(Operator.Divs, true, false); break;
                case Opcode.fdivrp: EmitCommonFpuInstruction(Operator.Divs, true, true); break;
                case Opcode.fild: RewriteFild(); break;
                case Opcode.fistp: RewriteFistp(); break;
                case Opcode.fld: RewriteFld(); break;
                case Opcode.fld1: RewriteFldConst(1.0); break;
                case Opcode.fldcw: RewriteFldcw(); break;
                case Opcode.fldln2: RewriteFldConst(Constant.Ln2()); break;
                case Opcode.fldpi: RewriteFldConst(Constant.Pi()); break;
                case Opcode.fldz: RewriteFldConst(0.0); break;
                case Opcode.fmul: EmitCommonFpuInstruction(Operator.Muls, false, false); break;
                case Opcode.fmulp: EmitCommonFpuInstruction(Operator.Muls, false, true); break;
                case Opcode.fpatan: RewriteFpatan(); break;
                case Opcode.frndint: RewriteFUnary("__rndint"); break;
                case Opcode.fsin: RewriteFUnary("sin"); break;
                case Opcode.fsincos: RewriteFsincos(); break;
                case Opcode.fsqrt: RewriteFUnary("sqrt"); break;
                case Opcode.fst: RewriteFst(false); break;
                case Opcode.fstcw: RewriterFstcw(); break;
                case Opcode.fstp: RewriteFst(true); break;
                case Opcode.fstsw: RewriteFstsw(); break;
                case Opcode.fsub: EmitCommonFpuInstruction(Operator.Sub, false, false); break;
                case Opcode.fsubp: EmitCommonFpuInstruction(Operator.Sub, false, true); break;
                case Opcode.fsubr: EmitCommonFpuInstruction(Operator.Sub, true, false); break;
                case Opcode.fsubrp: EmitCommonFpuInstruction(Operator.Sub, true, true); break;
                case Opcode.ftst: RewriteFtst(); break;
                case Opcode.fxam: RewriteFxam(); break;
                case Opcode.fxch: RewriteExchange(); break;
                case Opcode.fyl2x: RewriteFyl2x(); break;
                case Opcode.hlt: RewriteHlt(); break;
                case Opcode.idiv: RewriteDivide(Operator.Divs, Domain.SignedInt); break;
                case Opcode.@in: RewriteIn(); break;
                case Opcode.imul: RewriteMultiply(Operator.Muls, Domain.SignedInt); break;
                case Opcode.inc: RewriteIncDec(1); break;
                case Opcode.ins: RewriteStringInstruction(); break;
                case Opcode.@int: RewriteInt(); break;
                case Opcode.iret: RewriteIret(); break;
                case Opcode.jmp: RewriteJmp(); break;
                case Opcode.ja: RewriteConditionalGoto(ConditionCode.UGT, di.Instruction.op1); break;
                case Opcode.jbe: RewriteConditionalGoto(ConditionCode.ULE, di.Instruction.op1); break;
                case Opcode.jc: RewriteConditionalGoto(ConditionCode.ULT, di.Instruction.op1); break;
                case Opcode.jcxz: RewriteJcxz(); break;
                case Opcode.jge: RewriteConditionalGoto(ConditionCode.GE, di.Instruction.op1); break;
                case Opcode.jg: RewriteConditionalGoto(ConditionCode.GT, di.Instruction.op1); break;
                case Opcode.jl: RewriteConditionalGoto(ConditionCode.LT, di.Instruction.op1); break;
                case Opcode.jle: RewriteConditionalGoto(ConditionCode.LE, di.Instruction.op1); break;
                case Opcode.jnc: RewriteConditionalGoto(ConditionCode.UGE, di.Instruction.op1); break;
                case Opcode.jno: RewriteConditionalGoto(ConditionCode.NO, di.Instruction.op1); break;
                case Opcode.jns: RewriteConditionalGoto(ConditionCode.NS, di.Instruction.op1); break;
                case Opcode.jnz: RewriteConditionalGoto(ConditionCode.NE, di.Instruction.op1); break;
                case Opcode.jo: RewriteConditionalGoto(ConditionCode.OV, di.Instruction.op1); break;
                case Opcode.jpe: RewriteConditionalGoto(ConditionCode.PE, di.Instruction.op1); break;
                case Opcode.jpo: RewriteConditionalGoto(ConditionCode.PO, di.Instruction.op1); break;
                case Opcode.js: RewriteConditionalGoto(ConditionCode.SG, di.Instruction.op1); break;
                case Opcode.jz: RewriteConditionalGoto(ConditionCode.EQ, di.Instruction.op1); break;
                case Opcode.lahf: RewriteLahf(); break;
                case Opcode.lds: RewriteLxs(Registers.ds); break;
                case Opcode.lea: RewriteLea(); break;
                case Opcode.leave: RewriteLeave(); break;
                case Opcode.les: RewriteLxs(Registers.es); break;
                case Opcode.lfs: RewriteLxs(Registers.fs); break;
                case Opcode.lgs: RewriteLxs(Registers.gs); break;
                case Opcode.lods: RewriteStringInstruction(); break;
                case Opcode.lodsb: RewriteStringInstruction(); break;
                case Opcode.loop: RewriteLoop(0, ConditionCode.EQ); break;
                case Opcode.loope: RewriteLoop(FlagM.ZF, ConditionCode.EQ); break;
                case Opcode.loopne: RewriteLoop(FlagM.ZF, ConditionCode.NE); break;
                case Opcode.lss: RewriteLxs(Registers.ss); break;
                case Opcode.mov: RewriteMov(); break;
                case Opcode.movs: RewriteStringInstruction(); break;
                case Opcode.movsb: RewriteStringInstruction(); break;
                case Opcode.movsx: EmitCopy(di.Instruction.op1, emitter.Cast(PrimitiveType.Create(Domain.SignedInt, di.Instruction.op1.Width.Size), SrcOp(di.Instruction.op2)), false); break;
                case Opcode.movzx: EmitCopy(di.Instruction.op1, emitter.Cast(di.Instruction.op1.Width, SrcOp(di.Instruction.op2)), false); break;
                case Opcode.mul: RewriteMultiply(Operator.Mulu, Domain.UnsignedInt); break;
                case Opcode.neg: RewriteNeg(); break;
                case Opcode.nop: continue;
                case Opcode.not: RewriteNot(); break;
                case Opcode.or: RewriteLogical(BinaryOperator.Or); break;
                case Opcode.@out: RewriteOut(); break;
                case Opcode.@outs: RewriteStringInstruction(); break;
                case Opcode.@outsb: RewriteStringInstruction(); break;
                case Opcode.pop: RewritePop(); break;
                case Opcode.popa: RewritePopa(); break;
                case Opcode.popf: RewritePopf(); break;
                case Opcode.push: RewritePush(); break;
                case Opcode.pusha: RewritePusha(); break;
                case Opcode.pushf: RewritePushf(); break;
                case Opcode.rcl: RewriteRotation("__rcl", true, true); break;
                case Opcode.rcr: RewriteRotation("__rcr", true, false); break;
                case Opcode.rol: RewriteRotation("__rol", false, true); break;
                case Opcode.ror: RewriteRotation("__ror", false, false); break;
                case Opcode.rep: RewriteRep(); break;
                case Opcode.repne: RewriteRep(); break;
                case Opcode.ret: RewriteRet(); break;
                case Opcode.retf: RewriteRet(); break;
                case Opcode.sahf: emitter.Assign(orw.FlagGroup(IntelInstruction.DefCc(di.Instruction.code)), orw.AluRegister(Registers.ah)); break;
                case Opcode.sar: RewriteBinOp(Operator.Sar); break;
                case Opcode.sbb: RewriteAdcSbb(BinaryOperator.Sub); break;
                case Opcode.scas: RewriteStringInstruction(); break;
                case Opcode.scasb: RewriteStringInstruction(); break;
                case Opcode.setg: RewriteSet(ConditionCode.GT); break;
                case Opcode.setge: RewriteSet(ConditionCode.GE); break;
                case Opcode.setl: RewriteSet(ConditionCode.LT); break;
                case Opcode.setle: RewriteSet(ConditionCode.LE); break;
                case Opcode.setnz: RewriteSet(ConditionCode.NE); break;
                case Opcode.sets: RewriteSet(ConditionCode.SG); break;
                case Opcode.setz: RewriteSet(ConditionCode.EQ); break;
                case Opcode.shl: RewriteBinOp(BinaryOperator.Shl); break;
                case Opcode.shld: RewriteShxd("__shld"); break;
                case Opcode.shr: RewriteBinOp(BinaryOperator.Shr); break;
                case Opcode.shrd: RewriteShxd("__shrd"); break;
                case Opcode.stc: RewriteSetFlag(FlagM.CF, Constant.True()); break;
                case Opcode.std: RewriteSetFlag(FlagM.DF, Constant.True()); break;
                case Opcode.sti: break; //$TODO:
                case Opcode.stos: RewriteStringInstruction(); break;
                case Opcode.stosb: RewriteStringInstruction(); break;
                case Opcode.sub: RewriteAddSub(BinaryOperator.Sub); break;
                case Opcode.test: RewriteTest(); break;
                case Opcode.wait: break;	// used to slow down FPU.
                case Opcode.xchg: RewriteExchange(); break;
                case Opcode.xlat: RewriteXlat(); break;
                case Opcode.xor: RewriteLogical(BinaryOperator.Xor); break;
                }
                yield return ric;
            }
        }

        public Expression PseudoProc(string name, PrimitiveType retType, params Expression[] args)
        {
            var ppp = host.EnsurePseudoProcedure(name, retType, args.Length);
            return PseudoProc(ppp, retType, args);
        }

        public Expression PseudoProc(PseudoProcedure ppp, PrimitiveType retType, params Expression[] args)
        {
            if (args.Length != ppp.Arity)
                throw new ArgumentOutOfRangeException(
                    string.Format("Pseudoprocedure {0} expected {1} arguments, but was passed {2}.",
                    ppp.Name,
                    ppp.Arity,
                    args.Length));

            return emitter.Fn(new ProcedureConstant(arch.PointerType, ppp), retType, args);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Breaks up very common case of x86:
        /// <code>
        ///		op [memaddr], reg
        /// </code>
        /// into the equivalent:
        /// <code>
        ///		tmp := [memaddr] op reg;
        ///		store([memaddr], tmp);
        /// </code>
        /// </summary>
        /// <param name="opDst"></param>
        /// <param name="src"></param>
        /// <param name="forceBreak">if true, forcibly splits the assignments in two if the destination is a memory store.</param>
        /// <returns></returns>
        public RtlAssignment EmitCopy(MachineOperand opDst, Expression src, bool forceBreak)
        {
            Expression dst = SrcOp(opDst);
            Identifier idDst = dst as Identifier;
            if (idDst != null || !forceBreak)
            {
                MemoryAccess acc = dst as MemoryAccess;
                if (acc != null)
                {
                    return emitter.Assign(acc, src);
                }
                else
                {
                    return emitter.Assign(idDst, src);
                }
            }
            else
            {
                Identifier tmp = frame.CreateTemporary(opDst.Width);
                emitter.Assign(tmp, src);
                var ea = orw.CreateMemoryAccess((MemoryOperand)opDst, state);
                return emitter.Assign(ea, tmp);
            }
        }

        private Expression SrcOp(MachineOperand opSrc)
        {
            return orw.Transform(opSrc, opSrc.Width, state);
        }

        private Expression SrcOp(MachineOperand opSrc, PrimitiveType dstWidth)
        {
            return orw.Transform(opSrc, dstWidth, state);
        }
    }
}
