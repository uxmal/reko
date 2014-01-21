#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using Decompiler.Core.Operators;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.M68k
{
    /// <summary>
    /// Rewrites <seealso cref="M68kInstruntion"/>s to <see cref="RtlInstructionCluster"/>s.
    /// </summary>
    public partial class Rewriter : IEnumerable<RtlInstructionCluster>
    {
        // These field are internal so that the OperandRewriter can use them.
        internal M68kArchitecture arch;
        internal Frame frame;
        internal DisassembledInstruction di;
        internal RtlEmitter emitter;
        private M68kState state;
        private IRewriterHost host;
        private IEnumerator<DisassembledInstruction> dasm;
        private RtlInstructionCluster ric;
        private OperandRewriter orw;

        public Rewriter(M68kArchitecture m68kArchitecture, ImageReader rdr, M68kState m68kState, Frame frame, IRewriterHost host)
        {
            this.arch = m68kArchitecture;
            this.state = m68kState;
            this.frame = frame;
            this.host = host;
            this.dasm = CreateDisassemblyStream(rdr);
        }

        protected IEnumerator<DisassembledInstruction> CreateDisassemblyStream(ImageReader rdr)
        {
            var d = (M68kDisassembler) arch.CreateDisassembler(rdr);
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

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                di = dasm.Current;
                ric = new RtlInstructionCluster(di.Address, (byte)di.Length);
                emitter = new RtlEmitter(ric.Instructions);
                orw = new OperandRewriter(this, di.Instruction.dataWidth);
                switch (di.Instruction.code)
                {
                case Opcode.and: RewriteLogical((s, d) => emitter.And(d, s)); break;
                case Opcode.andi: RewriteLogical((s, d) => emitter.And(d, s)); break;
                case Opcode.asl: RewriteArithmetic((s, d) => emitter.Shl(d, s)); break;
                case Opcode.asr: RewriteShift((s, d) => emitter.Sar(d, s)); break;
                case Opcode.bchg: RewriteBchg(); break;
                case Opcode.adda: RewriteBinOp((s,d)=>emitter.IAdd(d, s)); break;
                case Opcode.clr: RewriteClr(); break;
                case Opcode.cmp: RewriteCmp(); break;
                case Opcode.cmpa: RewriteCmp(); break;
                case Opcode.cmpi: RewriteCmp(); break;
                case Opcode.dble: RewriteDbcc(ConditionCode.GT, FlagM.NF | FlagM.VF | FlagM.ZF); break;
                case Opcode.dbra: RewriteDbcc(ConditionCode.None, 0); break;
                case Opcode.eor: RewriteLogical((s, d) => emitter.Xor(d, s)); break;
                case Opcode.ext: RewriteExt(); break;
                case Opcode.extb: RewriteExtb(); break;
                case Opcode.jsr: RewriteJsr(); break;
                case Opcode.link: RewriteLink(); break;
                case Opcode.lsl: RewriteShift((s, d) => emitter.Shl(d, s)); break;
                case Opcode.lsr: RewriteShift((s, d) => emitter.Shr(d, s)); break;
                case Opcode.move: RewriteMove(true); break;
                case Opcode.movea: RewriteMove(false); break;
                case Opcode.muls: RewriteMul((s, d) => emitter.SMul(d, s)); break;
                case Opcode.mulu: RewriteMul((s, d) => emitter.UMul(d, s)); break;
                case Opcode.neg: RewriteUnary(s => emitter.Neg(s), AllConditions); break;
                case Opcode.negx: RewriteUnary(RewriteNegx, AllConditions); break;
                case Opcode.not: RewriteUnary(s => emitter.Comp(s), LogicalConditions); break;
                case Opcode.or: RewriteLogical((s, d) => emitter.Or(d, s)); break;
                case Opcode.rts: emitter.Return(4, 0); break;
                case Opcode.sub: RewriteArithmetic((s, d) => emitter.ISub(d, s)); break;
                case Opcode.suba: RewriteArithmetic((s, d) => emitter.ISub(d, s)); break;
                case Opcode.subi: RewriteArithmetic((s, d) => emitter.ISub(d, s)); break;
                case Opcode.subq: RewriteArithmetic((s, d) => emitter.ISub(d, s)); break;
                case Opcode.subx: RewriteArithmetic((s, d) => emitter.ISub(emitter.ISub(d, s), frame.EnsureFlagGroup((uint)FlagM.XF, "X", PrimitiveType.Bool))); break;
                case Opcode.unlk: RewriteUnlk(); break;
                default:
                    throw new AddressCorrelatedException(
                        di.Address,
                        "Rewriting M68k opcode '{0}' is not supported yet.",
                        di.Instruction.code);
                }
                yield return ric;
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
