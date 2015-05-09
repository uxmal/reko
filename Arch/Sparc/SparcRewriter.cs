#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Arch.Sparc;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Sparc
{
    public partial class SparcRewriter : IEnumerable<RtlInstructionCluster>
    {
        private SparcArchitecture arch;
        private SparcProcessorState state;
        private Frame frame;
        private IRewriterHost host;
        private LookaheadEnumerator<SparcInstruction> dasm;
        private SparcInstruction instrCur;
        private RtlEmitter emitter;
        private RtlInstructionCluster ric;

        public SparcRewriter(SparcArchitecture arch, ImageReader rdr, SparcProcessorState state, Frame frame, IRewriterHost host)
        {
            this.arch = arch;
            this.state = state;
            this.frame = frame;
            this.host = host;
            this.dasm = new LookaheadEnumerator<SparcInstruction>(CreateDisassemblyStream(rdr));
        }

        public SparcRewriter(SparcArchitecture arch, IEnumerator<SparcInstruction> instrs, SparcProcessorState state, Frame frame, IRewriterHost host)
        {
            this.arch = arch;
            this.state = state;
            this.frame = frame;
            this.host = host;
            this.dasm = new LookaheadEnumerator<SparcInstruction>(instrs);
        }

        private IEnumerable<SparcInstruction> CreateDisassemblyStream(ImageReader rdr)
        {
            return new SparcDisassembler(arch, rdr);
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                instrCur = dasm.Current;
                ric = new RtlInstructionCluster(instrCur.Address, 4);
                emitter = new RtlEmitter(ric.Instructions);
                switch (instrCur.Opcode)
                {
                default: 
                    throw new AddressCorrelatedException(
                        instrCur.Address,
                        "Rewriting SPARC opcode '{0}' is not supported yet.",
                        instrCur.Opcode);
                case Opcode.add: RewriteAlu(Operator.IAdd); break;
                case Opcode.addcc: RewriteAluCc(Operator.IAdd); break;
                case Opcode.and: RewriteAlu(Operator.And); break;
                case Opcode.andcc: RewriteAluCc(Operator.And); break;
                case Opcode.ba: RewriteBranch(Constant.True()); break;
                case Opcode.bn: RewriteBranch(Constant.False()); break;
                case Opcode.bne: RewriteBranch(emitter.Test(ConditionCode.NE, Grf(FlagM.ZF))); break;
                case Opcode.be: RewriteBranch(emitter.Test(ConditionCode.EQ, Grf(FlagM.ZF))); break;
//                    Z
//case Opcode.bg   not (Z or (N xor V))
//case Opcode.ble  Z or (N xor V)
//case Opcode.bge  not (N xor V)
//case Opcode.bl   N xor V
//case Opcode.bgu  not (C or Z)
//case Opcode.bleu (C or Z)
//case Opcode.bcc  not C
//case Opcode.bcs   C
//case Opcode.bpos not N
//case Opcode.bneg N
//case Opcode.bvc  not V
//case Opcode.bvs  V

                case Opcode.call: RewriteCall(); break;
                case Opcode.fbne: RewriteBranch(emitter.Test(ConditionCode.NE, Grf(FlagM.LF | FlagM.GF))); break;
                case Opcode.fba: RewriteBranch(Constant.True()); break;
                case Opcode.fbn: RewriteBranch(Constant.False()); break;
//case Opcode.fbu   : on Unordered U
//case Opcode.fbg   : RewriteBranch(emitter.Test(ConditionCode.GT, Grf(FlagM.GF))); break;
//case Opcode.fbug  : on Unordered or Greater G or U
//case Opcode.fbl   : on Less L
//case Opcode.fbul  : on Unordered or Less L or U
//case Opcode.fblg  : on Less or Greater L or G
//case Opcode.fbne  : on Not Equal L or G or U
//case Opcode.fbe   : on Equal E
//case Opcode.fbue  : on Unordered or Equal E or U
//case Opcode.fbge  : on Greater or Equal E or G
//case Opcode.fbuge : on Unordered or Greater or Equal E or G or U
//case Opcode.fble  : on Less or Equal E or L
//case Opcode.fbule : on Unordered or Less or Equal E or L or U
//                case Opcode.FBO   : on Ordered E or L or G

                case Opcode.fitod: RewriteFitod(); break;
                case Opcode.fitoq: RewriteFitoq(); break;
                case Opcode.fitos: RewriteFitos(); break;
                case Opcode.jmpl: RewriteJmpl(); break;
                case Opcode.ldsb: RewriteLoad(PrimitiveType.SByte); break;
                case Opcode.mulscc: RewriteMulscc(); break;
                case Opcode.or: RewriteAlu(Operator.Or); break;
                case Opcode.orcc: RewriteAluCc(Operator.Or); break;
                case Opcode.rett: RewriteRett(); break;
                case Opcode.sethi: RewriteSethi(); break;
                case Opcode.sdiv: RewriteAlu(Operator.SDiv); break;
                case Opcode.sdivcc: RewriteAlu(Operator.SDiv); break;
                case Opcode.sll: RewriteAlu(Operator.Shl); break;
                case Opcode.smul: RewriteAlu(Operator.SMul); break;
                case Opcode.smulcc: RewriteAlu(Operator.SMul); break;
                case Opcode.sth: RewriteStore(PrimitiveType.Word16); break;
                case Opcode.ta: RewriteTrap(Constant.True()); break;
                case Opcode.tn: RewriteTrap(Constant.False()); break;
                case Opcode.tne: RewriteTrap(emitter.Test(ConditionCode.NE, Grf(FlagM.ZF))); break;
                case Opcode.te: RewriteTrap(emitter.Test(ConditionCode.EQ, Grf(FlagM.ZF))); break;

                case Opcode.udiv: RewriteAlu(Operator.UDiv); break;
                case Opcode.udivcc: RewriteAluCc(Operator.UDiv); break;
                case Opcode.umul: RewriteAlu(Operator.UMul); break;
                case Opcode.umulcc: RewriteAluCc(Operator.UMul); break;
                }
                yield return ric;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EmitCc(Expression dst)
        {
            emitter.Assign(
                frame.EnsureFlagGroup(0xF, "NZVC", PrimitiveType.Byte),
                emitter.Cond(dst));
        }

        private Application PseudoProc(string name, DataType ret, params Expression[] exprs)
        {
            var ppp = host.EnsurePseudoProcedure(name, ret, exprs.Length);
            var fn = emitter.Fn(ppp, exprs);
            return fn;
        }

        private Expression RewriteOp(MachineOperand op)
        {
            return RewriteOp(op, false);
        }

        private Expression RewriteOp(MachineOperand op, bool g0_becomes_null)
        {
            var r = op as RegisterOperand;
            if (r != null)
            {
                if (r.Register == Registers.g0)
                {
                    if (g0_becomes_null)
                        return null;
                    else 
                        return Constant.Zero(PrimitiveType.Word32);
                }
                else
                    return frame.EnsureRegister(r.Register);
            }
            var imm = op as ImmediateOperand;
            if (imm != null)
                return imm.Value;
            throw new NotImplementedException(string.Format("Unsupported operand {0} ({1})", op, op.GetType().Name));
        }

        private Expression RewriteMemOp(MachineOperand op, PrimitiveType size)
        {
            var m = op as MemoryOperand;
            Expression baseReg;
            Expression offset;
            if (m != null)
            {
                baseReg = m.Base == Registers.g0 ? null : frame.EnsureRegister(m.Base);
                offset = m.Offset.IsIntegerZero ? null : m.Offset;
            }
            else
            {
                var i = op as IndexedMemoryOperand;
                if (i != null)
                {
                    baseReg = i.Base == Registers.g0 ? null : frame.EnsureRegister(i.Base);
                    offset = i.Index == Registers.g0 ? null : frame.EnsureRegister(i.Index);
                }
                else
                    throw new NotImplementedException(string.Format("Unknown memory operand {0} ({1})", op, op.GetType().Name));
            }
            return new MemoryAccess(SimplifySum(baseReg, offset), size);
        }

        private Expression SimplifySum(Expression srcLeft, Expression srcRight)
        {
            if (srcLeft == null && srcRight == null)
                return Constant.Zero(PrimitiveType.Pointer32);
            else if (srcLeft == null)
                return srcRight;
            else if (srcRight == null)
                return srcLeft;
            else
                return emitter.IAdd(srcLeft, srcRight);
        }
    }
}
