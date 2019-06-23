#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Rtl;
using System.Collections;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;
using Reko.Core.Types;

namespace Reko.Arch.Xtensa
{
    public partial class XtensaRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly XtensaArchitecture arch;
        private readonly IEnumerator<XtensaInstruction> dasm;
        private XtensaInstruction instr;
        private InstrClass rtlc;
        private RtlEmitter m;

        public XtensaRewriter(XtensaArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new XtensaDisassembler(this.arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                var addr = dasm.Current.Address;
                var len = dasm.Current.Length;
                var rtlInstructions = new List<RtlInstruction>();
                rtlc = InstrClass.Linear;
                m = new RtlEmitter(rtlInstructions);
                this.instr = dasm.Current;
                switch (instr.Opcode)
                {
                default:
                    throw new AddressCorrelatedException(
                       instr.Address,
                       "Rewriting of Xtensa instruction '{0}' not implemented yet.",
                       instr.Opcode);
                case Opcodes.abs: RewritePseudoFn("abs"); break;
                case Opcodes.add:
                case Opcodes.add_n: RewriteBinOp(m.IAdd); break;
                case Opcodes.add_s: RewriteBinOp(m.FAdd); break;
                case Opcodes.addi: RewriteAddi(); break;
                case Opcodes.addi_n: RewriteAddi(); break;
                case Opcodes.addmi: RewriteBinOp(m.IAdd); break;
                case Opcodes.addx2: RewriteAddx(2); break;
                case Opcodes.addx4: RewriteAddx(4); break;
                case Opcodes.addx8: RewriteAddx(8); break;
                case Opcodes.and: RewriteBinOp(m.And); break;
                case Opcodes.andb: RewriteBinOp(m.And); break;
                case Opcodes.andbc: RewriteBinOp((a, b) => m.And(a, m.Not(b))); break;
                case Opcodes.ball: RewriteBall(); break;
                case Opcodes.bany: RewriteBany(); break;
                case Opcodes.bbc: 
                case Opcodes.bbci: RewriteBbx(m.Eq0); break;
                case Opcodes.bbs:
                case Opcodes.bbsi: RewriteBbx(m.Ne0); break;
                case Opcodes.beq:
                case Opcodes.beqi: RewriteBranch(m.Eq); break;
                case Opcodes.beqz:
                case Opcodes.beqz_n: RewriteBranchZ(m.Eq0); break;
                case Opcodes.bge:
                case Opcodes.bgei: RewriteBranch(m.Ge); break;
                case Opcodes.bgeu:
                case Opcodes.bgeui: RewriteBranch(m.Uge); break;
                case Opcodes.bgez: RewriteBranchZ(m.Ge0); break;
                case Opcodes.blt: RewriteBranch(m.Lt); break;
                case Opcodes.blti: RewriteBranch(m.Lt); break;
                case Opcodes.bltu:
                case Opcodes.bltui: RewriteBranch(m.Ult); break;
                case Opcodes.bltz: RewriteBranchZ(m.Lt0); break;
                case Opcodes.bnall: RewriteBnall(); break;
                case Opcodes.bne: RewriteBranch(m.Ne); break;
                case Opcodes.bnei: RewriteBranch(m.Ne); break;
                case Opcodes.bnez:
                case Opcodes.bnez_n: RewriteBranchZ(m.Ne0); break;
                case Opcodes.bnone: RewriteBnone(); break;
                case Opcodes.@break: RewriteBreak(); break;
                case Opcodes.call0:
                case Opcodes.callx0: RewriteCall0(); break;
                case Opcodes.extui: RewriteExtui(); break;
                case Opcodes.floor_s: RewritePseudoFn("__floor"); break;
                case Opcodes.isync: RewritePseudoProc("__isync"); break;
                case Opcodes.j:
                case Opcodes.jx: RewriteJ(); break;
                case Opcodes.ill: RewriteIll(); break;
                case Opcodes.l16si: RewriteLsi(PrimitiveType.Int16); break;
                case Opcodes.l16ui: RewriteLui(PrimitiveType.UInt16); break;
                case Opcodes.l32i: RewriteL32i(); break;
                case Opcodes.l32e: RewriteL32e(); break;
                case Opcodes.l32i_n: RewriteL32i(); break;
                case Opcodes.l32r: RewriteCopy(); break;
                case Opcodes.l8ui: RewriteLui(PrimitiveType.Byte); break;
                case Opcodes.ldpte: RewritePseudoProc("__ldpte"); break;
                case Opcodes.lsiu: RewriteLsiu(); break;
                case Opcodes.memw: RewriteNop(); break; /// memory sync barriers?
                case Opcodes.mov_n: RewriteCopy(); break;
                case Opcodes.movi: RewriteCopy(); break;
                case Opcodes.movi_n: RewriteMovi_n(); break;
                case Opcodes.moveqz:
                case Opcodes.moveqz_s: RewriteMovcc(m.Eq); break;
                case Opcodes.movltz: RewriteMovcc(m.Lt); break;
                case Opcodes.movgez: RewriteMovcc(m.Ge); break;
                case Opcodes.movnez: RewriteMovcc(m.Ne); break;
                case Opcodes.mul_s: RewriteBinOp(m.FMul); break;
                case Opcodes.mul16s: RewriteMul16(m.SMul, Domain.SignedInt); break;
                case Opcodes.mul16u: RewriteMul16(m.UMul, Domain.UnsignedInt); break;
                case Opcodes.mull: RewriteBinOp(m.IMul); break;
                case Opcodes.neg: RewriteUnaryOp(m.Neg); break;
                case Opcodes.nsa: RewritePseudoFn("__nsa"); break;
                case Opcodes.nsau: RewritePseudoFn("__nsau"); break;
                case Opcodes.or: RewriteOr(); break;
                case Opcodes.orbc: RewriteBinOp((a, b) => m.Or(a, m.Not(b))); break;
                case Opcodes.quos: RewriteBinOp(m.SDiv); break;
                case Opcodes.quou: RewriteBinOp(m.UDiv); break;
                case Opcodes.rems: RewriteBinOp(m.Mod); break;
                case Opcodes.remu: RewriteBinOp(m.Mod); break;
                case Opcodes.reserved: RewriteReserved(); break;
                case Opcodes.ret:
                case Opcodes.ret_n: RewriteRet(); break;
                case Opcodes.rfe: RewriteRet(); break;      //$REVIEW: emit some hint this is a return from exception?
                case Opcodes.rfi: RewriteRet(); break;      //$REVIEW: emit some hint this is a return from interrupt?
                case Opcodes.rsil: RewritePseudoFn("__rsil"); break;
                case Opcodes.rsr: RewriteCopy(); break;
                case Opcodes.s16i: RewriteSi(PrimitiveType.Word16); break;
                case Opcodes.s32e: RewriteS32e(); break;
                case Opcodes.s32i:
                case Opcodes.s32i_n: RewriteSi(PrimitiveType.Word32); break;
                case Opcodes.s32ri: RewriteSi(PrimitiveType.Word32); break; //$REVIEW: what about concurrency semantics
                case Opcodes.s8i: RewriteSi(PrimitiveType.Byte); break;
                case Opcodes.sll: RewriteShift(m.Shl); break;
                case Opcodes.slli: RewriteShiftI(m.Shl); break;
                case Opcodes.sra: RewriteShift(m.Sar); break;
                case Opcodes.srai: RewriteShiftI(m.Sar); break;
                case Opcodes.src: RewriteSrc(); break;
                case Opcodes.srl: RewriteShift(m.Sar); break;
                case Opcodes.srli: RewriteShiftI(m.Shr); break;
                case Opcodes.ssa8l: RewriteSsa8l(); break;
                case Opcodes.ssi: RewriteSi(PrimitiveType.Real32); break;
                case Opcodes.ssl: RewriteSsl(); break;
                case Opcodes.ssr:
                case Opcodes.ssai: RewriteSsa(); break;
                case Opcodes.sub: RewriteBinOp(m.ISub); break;
                case Opcodes.sub_s: RewriteBinOp(m.FSub); break;
                case Opcodes.subx2: RewriteSubx(2); break;
                case Opcodes.subx4: RewriteSubx(4); break;
                case Opcodes.subx8: RewriteSubx(8); break;
                case Opcodes.ueq_s: RewriteBinOp(m.Eq); break;
                case Opcodes.wsr: RewriteWsr(); break;
                case Opcodes.xor: RewriteBinOp(m.Xor); break;

                }
                yield return new RtlInstructionCluster(addr, len, rtlInstructions.ToArray())
                {
                    Class = rtlc,
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Expression RewriteOp(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand rOp:
                return binder.EnsureRegister(rOp.Register);
            case AddressOperand aOp:
                return aOp.Address;
            case ImmediateOperand iOp:
                return iOp.Value;
            }
            throw new NotImplementedException(op.GetType().FullName);
        }

        // Sign-extend an operand known to be signed immediate.
        private Constant RewriteSimm(MachineOperand op)
        {
            var iOp = (ImmediateOperand)op;
            return Constant.Int32(iOp.Value.ToInt32());
        }

        // Zero-extend an operand known to be unsigned immediate.
        private Constant RewriteUimm(MachineOperand op)
        {
            var iOp = (ImmediateOperand)op;
            return Constant.UInt32(iOp.Value.ToUInt32());
        }
    }
}
