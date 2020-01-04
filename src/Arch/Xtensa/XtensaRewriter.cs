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
                switch (instr.Mnemonic)
                {
                default:
                    throw new AddressCorrelatedException(
                       instr.Address,
                       "Rewriting of Xtensa instruction '{0}' not implemented yet.",
                       instr.Mnemonic);
                case Mnemonic.abs: RewritePseudoFn("abs"); break;
                case Mnemonic.add:
                case Mnemonic.add_n: RewriteBinOp(m.IAdd); break;
                case Mnemonic.add_s: RewriteBinOp(m.FAdd); break;
                case Mnemonic.addi: RewriteAddi(); break;
                case Mnemonic.addi_n: RewriteAddi(); break;
                case Mnemonic.addmi: RewriteBinOp(m.IAdd); break;
                case Mnemonic.addx2: RewriteAddx(2); break;
                case Mnemonic.addx4: RewriteAddx(4); break;
                case Mnemonic.addx8: RewriteAddx(8); break;
                case Mnemonic.and: RewriteBinOp(m.And); break;
                case Mnemonic.andb: RewriteBinOp(m.And); break;
                case Mnemonic.andbc: RewriteBinOp((a, b) => m.And(a, m.Not(b))); break;
                case Mnemonic.ball: RewriteBall(); break;
                case Mnemonic.bany: RewriteBany(); break;
                case Mnemonic.bbc: 
                case Mnemonic.bbci: RewriteBbx(m.Eq0); break;
                case Mnemonic.bbs:
                case Mnemonic.bbsi: RewriteBbx(m.Ne0); break;
                case Mnemonic.beq:
                case Mnemonic.beqi: RewriteBranch(m.Eq); break;
                case Mnemonic.beqz:
                case Mnemonic.beqz_n: RewriteBranchZ(m.Eq0); break;
                case Mnemonic.bge:
                case Mnemonic.bgei: RewriteBranch(m.Ge); break;
                case Mnemonic.bgeu:
                case Mnemonic.bgeui: RewriteBranch(m.Uge); break;
                case Mnemonic.bgez: RewriteBranchZ(m.Ge0); break;
                case Mnemonic.blt: RewriteBranch(m.Lt); break;
                case Mnemonic.blti: RewriteBranch(m.Lt); break;
                case Mnemonic.bltu:
                case Mnemonic.bltui: RewriteBranch(m.Ult); break;
                case Mnemonic.bltz: RewriteBranchZ(m.Lt0); break;
                case Mnemonic.bnall: RewriteBnall(); break;
                case Mnemonic.bne: RewriteBranch(m.Ne); break;
                case Mnemonic.bnei: RewriteBranch(m.Ne); break;
                case Mnemonic.bnez:
                case Mnemonic.bnez_n: RewriteBranchZ(m.Ne0); break;
                case Mnemonic.bnone: RewriteBnone(); break;
                case Mnemonic.@break: RewriteBreak(); break;
                case Mnemonic.call0:
                case Mnemonic.callx0: RewriteCall0(); break;
                case Mnemonic.extui: RewriteExtui(); break;
                case Mnemonic.floor_s: RewritePseudoFn("__floor"); break;
                case Mnemonic.isync: RewritePseudoProc("__isync"); break;
                case Mnemonic.j:
                case Mnemonic.jx: RewriteJ(); break;
                case Mnemonic.ill: RewriteIll(); break;
                case Mnemonic.l16si: RewriteLsi(PrimitiveType.Int16); break;
                case Mnemonic.l16ui: RewriteLui(PrimitiveType.UInt16); break;
                case Mnemonic.l32i: RewriteL32i(); break;
                case Mnemonic.l32e: RewriteL32e(); break;
                case Mnemonic.l32i_n: RewriteL32i(); break;
                case Mnemonic.l32r: RewriteCopy(); break;
                case Mnemonic.l8ui: RewriteLui(PrimitiveType.Byte); break;
                case Mnemonic.ldpte: RewritePseudoProc("__ldpte"); break;
                case Mnemonic.lsiu: RewriteLsiu(); break;
                case Mnemonic.memw: RewriteNop(); break; /// memory sync barriers?
                case Mnemonic.mov_n: RewriteCopy(); break;
                case Mnemonic.movi: RewriteCopy(); break;
                case Mnemonic.movi_n: RewriteMovi_n(); break;
                case Mnemonic.moveqz:
                case Mnemonic.moveqz_s: RewriteMovcc(m.Eq); break;
                case Mnemonic.movltz: RewriteMovcc(m.Lt); break;
                case Mnemonic.movgez: RewriteMovcc(m.Ge); break;
                case Mnemonic.movnez: RewriteMovcc(m.Ne); break;
                case Mnemonic.mul_s: RewriteBinOp(m.FMul); break;
                case Mnemonic.mul16s: RewriteMul16(m.SMul, Domain.SignedInt); break;
                case Mnemonic.mul16u: RewriteMul16(m.UMul, Domain.UnsignedInt); break;
                case Mnemonic.mull: RewriteBinOp(m.IMul); break;
                case Mnemonic.neg: RewriteUnaryOp(m.Neg); break;
                case Mnemonic.nsa: RewritePseudoFn("__nsa"); break;
                case Mnemonic.nsau: RewritePseudoFn("__nsau"); break;
                case Mnemonic.or: RewriteOr(); break;
                case Mnemonic.orbc: RewriteBinOp((a, b) => m.Or(a, m.Not(b))); break;
                case Mnemonic.quos: RewriteBinOp(m.SDiv); break;
                case Mnemonic.quou: RewriteBinOp(m.UDiv); break;
                case Mnemonic.rems: RewriteBinOp(m.Mod); break;
                case Mnemonic.remu: RewriteBinOp(m.Mod); break;
                case Mnemonic.reserved: RewriteReserved(); break;
                case Mnemonic.ret:
                case Mnemonic.ret_n: RewriteRet(); break;
                case Mnemonic.rfe: RewriteRet(); break;      //$REVIEW: emit some hint this is a return from exception?
                case Mnemonic.rfi: RewriteRet(); break;      //$REVIEW: emit some hint this is a return from interrupt?
                case Mnemonic.rsil: RewritePseudoFn("__rsil"); break;
                case Mnemonic.rsr: RewriteCopy(); break;
                case Mnemonic.s16i: RewriteSi(PrimitiveType.Word16); break;
                case Mnemonic.s32e: RewriteS32e(); break;
                case Mnemonic.s32i:
                case Mnemonic.s32i_n: RewriteSi(PrimitiveType.Word32); break;
                case Mnemonic.s32ri: RewriteSi(PrimitiveType.Word32); break; //$REVIEW: what about concurrency semantics
                case Mnemonic.s8i: RewriteSi(PrimitiveType.Byte); break;
                case Mnemonic.sll: RewriteShift(m.Shl); break;
                case Mnemonic.slli: RewriteShiftI(m.Shl); break;
                case Mnemonic.sra: RewriteShift(m.Sar); break;
                case Mnemonic.srai: RewriteShiftI(m.Sar); break;
                case Mnemonic.src: RewriteSrc(); break;
                case Mnemonic.srl: RewriteShift(m.Sar); break;
                case Mnemonic.srli: RewriteShiftI(m.Shr); break;
                case Mnemonic.ssa8l: RewriteSsa8l(); break;
                case Mnemonic.ssi: RewriteSi(PrimitiveType.Real32); break;
                case Mnemonic.ssl: RewriteSsl(); break;
                case Mnemonic.ssr:
                case Mnemonic.ssai: RewriteSsa(); break;
                case Mnemonic.sub: RewriteBinOp(m.ISub); break;
                case Mnemonic.sub_s: RewriteBinOp(m.FSub); break;
                case Mnemonic.subx2: RewriteSubx(2); break;
                case Mnemonic.subx4: RewriteSubx(4); break;
                case Mnemonic.subx8: RewriteSubx(8); break;
                case Mnemonic.ueq_s: RewriteBinOp(m.Eq); break;
                case Mnemonic.wsr: RewriteWsr(); break;
                case Mnemonic.xor: RewriteBinOp(m.Xor); break;

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
