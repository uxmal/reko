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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Sparc
{
    public partial class SparcRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly SparcArchitecture arch;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly LookaheadEnumerator<SparcInstruction> dasm;
        private readonly EndianImageReader rdr;
        private RtlEmitter m;
        private SparcInstruction instrCur;
        private InstrClass rtlc;

        public SparcRewriter(SparcArchitecture arch, EndianImageReader rdr, SparcProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.binder = binder;
            this.host = host;
            this.rdr = rdr;
            this.dasm = new LookaheadEnumerator<SparcInstruction>(CreateDisassemblyStream(rdr));
        }

        public SparcRewriter(SparcArchitecture arch, IEnumerator<SparcInstruction> instrs, SparcProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.binder = binder;
            this.host = host;
            this.dasm = new LookaheadEnumerator<SparcInstruction>(instrs);
        }

        private IEnumerable<SparcInstruction> CreateDisassemblyStream(EndianImageReader rdr)
        {
            return new SparcDisassembler(arch, rdr);
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                instrCur = dasm.Current;
                var addr = instrCur.Address;
                var rtlInstructions = new List<RtlInstruction>();
                rtlc = InstrClass.Linear;
                m = new RtlEmitter(rtlInstructions);
                switch (instrCur.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    host.Warn(
                        instrCur.Address,
                        "SPARC instruction '{0}' is not supported yet.",
                        instrCur.Mnemonic);
                    goto case Mnemonic.illegal;
                case Mnemonic.illegal:
                    rtlc = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.add: RewriteAlu(m.IAdd, false); break;
                case Mnemonic.addcc: RewriteAluCc(m.IAdd, false); break;
                case Mnemonic.addx: RewriteAddxSubx(m.IAdd, false); break;
                case Mnemonic.addxcc: RewriteAddxSubx(m.IAdd, true); break;
                case Mnemonic.and: RewriteAlu(m.And, false); break;
                case Mnemonic.andcc: RewriteAluCc(m.And, false); break;
                case Mnemonic.andn: RewriteAlu(m.And, true); break;
                case Mnemonic.ba: RewriteBranch(Constant.True()); break;
                case Mnemonic.bn: RewriteBranch(Constant.False()); break;
                case Mnemonic.bne: RewriteBranch(m.Test(ConditionCode.NE, Grf(FlagM.ZF))); break;
                case Mnemonic.be: RewriteBranch(m.Test(ConditionCode.EQ, Grf(FlagM.ZF))); break;
                case Mnemonic.bg: RewriteBranch(m.Test(ConditionCode.GT, Grf(FlagM.ZF | FlagM.NF | FlagM.VF))); break;
                case Mnemonic.bge: RewriteBranch(m.Test(ConditionCode.GE, Grf(FlagM.NF | FlagM.VF))); break;
                case Mnemonic.bgu: RewriteBranch(m.Test(ConditionCode.UGE, Grf(FlagM.CF | FlagM.ZF))); break;
                case Mnemonic.bl: RewriteBranch(m.Test(ConditionCode.LT, Grf(FlagM.ZF | FlagM.NF | FlagM.VF))); break;
                case Mnemonic.ble: RewriteBranch(m.Test(ConditionCode.LE, Grf(FlagM.ZF | FlagM.NF | FlagM.VF))); break;
                case Mnemonic.bleu: RewriteBranch(m.Test(ConditionCode.ULE, Grf(FlagM.CF | FlagM.ZF))); break;
                case Mnemonic.bcc: RewriteBranch(m.Test(ConditionCode.UGE, Grf(FlagM.CF))); break;
                case Mnemonic.bcs: RewriteBranch(m.Test(ConditionCode.ULT, Grf(FlagM.CF))); break;
                case Mnemonic.bneg: RewriteBranch(m.Test(ConditionCode.LT, Grf(FlagM.NF))); break;
                case Mnemonic.bpos: RewriteBranch(m.Test(ConditionCode.GE, Grf(FlagM.NF))); break;
                //                    Z
                //case Opcode.bgu  not (C or Z)
                //case Opcode.bleu (C or Z)
                //case Opcode.bcc  not C
                //case Opcode.bcs   C
                //case Opcode.bpos not N
                //case Opcode.bneg N
                //case Opcode.bvc  not V
                //case Opcode.bvs  V

                case Mnemonic.call: RewriteCall(); break;
                case Mnemonic.fabss: RewriteFabss(); break;
                case Mnemonic.fadds: RewriteFadds(); break;
                case Mnemonic.fbne: RewriteBranch(m.Test(ConditionCode.NE, Grf(FlagM.LF | FlagM.GF))); break;
                case Mnemonic.fba: RewriteBranch(Constant.True()); break;
                case Mnemonic.fbn: RewriteBranch(Constant.False()); break;

                case Mnemonic.fbu   : RewriteBranch(m.Test(ConditionCode.NE, Grf(FlagM.UF))); break;
                case Mnemonic.fbg   : RewriteBranch(m.Test(ConditionCode.GT, Grf(FlagM.GF))); break;
                case Mnemonic.fbug  : RewriteBranch(m.Test(ConditionCode.GT, Grf(FlagM.GF | FlagM.UF))); break;
                //case Opcode.fbug  : on Unordered or Greater G or U
                //case Opcode.fbl   : on Less L
                case Mnemonic.fbul: RewriteBranch(m.Test(ConditionCode.LT, Grf(FlagM.GF|FlagM.UF))); break;
                //case Opcode.fbul  : on Unordered or Less L or U
                //case Opcode.fblg  : on Less or Greater L or G
                //case Opcode.fbne  : on Not Equal L or G or U
                //case Opcode.fbe   : on Equal E
                case Mnemonic.fbue : RewriteBranch(m.Test(ConditionCode.EQ, Grf(FlagM.EF | FlagM.UF))); break;
                case Mnemonic.fbuge: RewriteBranch(m.Test(ConditionCode.GE, Grf(FlagM.EF | FlagM.GF | FlagM.UF))); break;

                //case Opcode.fble  : on Less or Equal E or L
                //case Opcode.fbule : on Unordered or Less or Equal E or L or U
                case Mnemonic.fbule: RewriteBranch(m.Test(ConditionCode.LE, Grf(FlagM.EF | FlagM.LF | FlagM.UF))); break;
                case Mnemonic.fbge: RewriteBranch(m.Test(ConditionCode.GE, Grf(FlagM.EF | FlagM.GF))); break;
                //                case Opcode.FBO   : on Ordered E or L or G


                case Mnemonic.fcmpes: RewriteFcmpes(); break;
                case Mnemonic.fcmpd: RewriteFcmpd(); break;
                case Mnemonic.fcmpq: RewriteFcmpq(); break;
                case Mnemonic.fcmps: RewriteFcmps(); break;
                case Mnemonic.fdivd: RewriteFdivs(); break;
                case Mnemonic.fdivs: RewriteFdivd(); break;
                case Mnemonic.fdtos: RewriteFdtos(); break;
                case Mnemonic.fitod: RewriteFitod(); break;
                case Mnemonic.fitoq: RewriteFitoq(); break;
                case Mnemonic.fitos: RewriteFitos(); break;
                case Mnemonic.fmovs: RewriteFmovs(); break;
                case Mnemonic.fmuls: RewriteFmuls(); break;
                case Mnemonic.fnegs: RewriteFmovs(); break;
                case Mnemonic.fstod: RewriteFstod(); break;
                case Mnemonic.fsubs: RewriteFsubs(); break;
                case Mnemonic.jmpl: RewriteJmpl(); break;
                case Mnemonic.ld: RewriteLoad(PrimitiveType.Word32); break;
                case Mnemonic.lddf: RewriteLoad(PrimitiveType.Real64); break;
                case Mnemonic.ldf: RewriteLoad(PrimitiveType.Real32); break;
                case Mnemonic.ldd: RewriteLoad(PrimitiveType.Word64); break;
                case Mnemonic.ldsb: RewriteLoad(PrimitiveType.SByte); break;
                case Mnemonic.ldsh: RewriteLoad(PrimitiveType.Int16); break;
                case Mnemonic.ldstub: RewriteLdstub(); break;
                case Mnemonic.ldub: RewriteLoad(PrimitiveType.Byte); break;
                case Mnemonic.lduh: RewriteLoad(PrimitiveType.Word16); break;
                case Mnemonic.ldfsr: RewriteLoad(PrimitiveType.Word32); break;
                case Mnemonic.mulscc: RewriteMulscc(); break;
                case Mnemonic.or: RewriteAlu(m.Or, false); break;
                case Mnemonic.orcc: RewriteAluCc(m.Or, false); break;
                case Mnemonic.restore: RewriteRestore(); break;
                case Mnemonic.rett: RewriteRett(); break;
                case Mnemonic.save: RewriteSave(); break;
                case Mnemonic.sethi: RewriteSethi(); break;
                case Mnemonic.sdiv: RewriteAlu(m.SDiv, false); break;
                case Mnemonic.sdivcc: RewriteAlu(m.SDiv, false); break;
                case Mnemonic.sll: RewriteAlu(m.Shl, false); break;
                case Mnemonic.smul: RewriteAlu(m.SMul, false); break;
                case Mnemonic.smulcc: RewriteAluCc(m.SMul, false); break;
                case Mnemonic.sra: RewriteAlu(m.Sar, false); break;
                case Mnemonic.srl: RewriteAlu(m.Shr, false); break;
                case Mnemonic.st: RewriteStore(PrimitiveType.Word32); break;
                case Mnemonic.stb: RewriteStore(PrimitiveType.Byte); break;
                case Mnemonic.std: RewriteStore(PrimitiveType.Word64); break;
                case Mnemonic.stdf: RewriteStore(PrimitiveType.Real64); break;
                case Mnemonic.stf: RewriteStore(PrimitiveType.Real32); break;
                case Mnemonic.sth: RewriteStore(PrimitiveType.Word16); break;
                case Mnemonic.stfsr: RewriteStore(PrimitiveType.Word32); break;
                case Mnemonic.sub: RewriteAlu(m.ISub, false); break;
                case Mnemonic.subcc: RewriteAluCc(m.ISub, false); break;
                case Mnemonic.subx: RewriteAddxSubx(m.ISub, false); break;
                case Mnemonic.subxcc: RewriteAddxSubx(m.ISub, true); break;
                case Mnemonic.ta: RewriteTrap(Constant.True()); break;
                case Mnemonic.tn: RewriteTrap(Constant.False()); break;
                case Mnemonic.tne: RewriteTrap(m.Test(ConditionCode.NE, Grf(FlagM.ZF))); break;
                case Mnemonic.te: RewriteTrap(m.Test(ConditionCode.EQ, Grf(FlagM.ZF))); break;

                case Mnemonic.udiv: RewriteAlu(m.UDiv, false); break;
                case Mnemonic.udivcc: RewriteAluCc(m.UDiv, false); break;
                case Mnemonic.umul: RewriteAlu(m.UMul, false); break;
                case Mnemonic.umulcc: RewriteAluCc(m.UMul, false); break;
                case Mnemonic.unimp: m.Invalid(); break;
                case Mnemonic.xor: RewriteAlu(m.Xor, false); break;
                case Mnemonic.xorcc: RewriteAlu(m.Xor, true); break;
                case Mnemonic.xnor: RewriteAlu(XNor, false); break;
                case Mnemonic.xnorcc: RewriteAlu(XNor, true); break;

                }
                yield return new RtlInstructionCluster(addr, 4, rtlInstructions.ToArray())
                {
                    Class = rtlc
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static HashSet<Mnemonic> seen = new HashSet<Mnemonic>();

        //[Conditional("DEBUG")]
        public void EmitUnitTest()
        {
            if (seen.Contains(instrCur.Mnemonic))
                return;
            seen.Add(instrCur.Mnemonic);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var wInstr = r2.ReadUInt32();
            Console.WriteLine("        [Test]");
            Console.WriteLine("        public void SparcRw_" + dasm.Current.Mnemonic + "()");
            Console.WriteLine("        {");
            Console.Write($"            BuildTest(0x{wInstr:X8}");
            Console.WriteLine(");\t// " + dasm.Current.ToString());
            Console.WriteLine("            AssertCode(");
            Console.WriteLine("                \"0|L--|{0}({1}): 1 instructions\",", instrCur.Address, instrCur.Length);
            Console.WriteLine("                \"1|L--|@@@\");");
            Console.WriteLine("        }");
            Console.WriteLine("");
        }

        private void EmitCc(Expression dst)
        {
            m.Assign(
                binder.EnsureFlagGroup(
                    Registers.psr,
                    0xF, "NZVC",
                    PrimitiveType.Byte),
                m.Cond(dst));
        }

        private Expression RewriteOp(MachineOperand op)
        {
            return RewriteOp(op, false);
        }

        private Expression RewriteOp(MachineOperand op, bool g0_becomes_null)
        {
            if (op is RegisterOperand r)
            {
                if (r.Register == Registers.g0)
                {
                    if (g0_becomes_null)
                        return null;
                    else
                        return Constant.Zero(PrimitiveType.Word32);
                }
                else
                    return binder.EnsureRegister(r.Register);
            }
            if (op is ImmediateOperand imm)
                return imm.Value;
            throw new NotImplementedException(string.Format("Unsupported operand {0} ({1})", op, op.GetType().Name));
        }

        private Expression RewriteRegister(MachineOperand op)
        {
            return binder.EnsureRegister(((RegisterOperand)op).Register);
        }

        private Expression RewriteDoubleRegister(MachineOperand op)
        {
            var reg = ((RegisterOperand)op).Register;
            var iReg = reg.Number - Registers.FloatRegisters[0].Number;
            var regLo = Registers.FloatRegisters[iReg + 1];
            return binder.EnsureSequence(PrimitiveType.Word64, reg, regLo);
        }

        private Expression RewriteQuadRegister(MachineOperand op)
        {
            var reg3 = ((RegisterOperand) op).Register;
            var iReg = reg3.Number - Registers.FloatRegisters[0].Number;
            var reg2 = Registers.FloatRegisters[iReg + 1];
            var reg1 = Registers.FloatRegisters[iReg + 2];
            var reg0 = Registers.FloatRegisters[iReg + 3];
            return binder.EnsureSequence(PrimitiveType.Word128, reg3, reg2, reg1, reg0);
        }


        private Expression RewriteMemOp(MachineOperand op, PrimitiveType size)
        {
            Expression baseReg;
            Expression offset;
            if (op is MemoryOperand mem)
            {
                baseReg = mem.Base == Registers.g0 ? null : binder.EnsureRegister(mem.Base);
                offset = mem.Offset.IsIntegerZero ? null : mem.Offset;
            }
            else
            {
                if (op is IndexedMemoryOperand i)
                {
                    baseReg = i.Base == Registers.g0 ? null : binder.EnsureRegister(i.Base);
                    offset = i.Index == Registers.g0 ? null : binder.EnsureRegister(i.Index);
                }
                else
                    throw new NotImplementedException(string.Format("Unknown memory operand {0} ({1})", op, op.GetType().Name));
            }
            return m.Mem(size, SimplifySum(baseReg, offset));
        }

        private Expression SimplifySum(Expression srcLeft, Expression srcRight)
        {
            if (srcLeft == null && srcRight == null)
                return Constant.Zero(PrimitiveType.Ptr32);
            else if (srcLeft == null)
                return srcRight;
            else if (srcRight == null)
                return srcLeft;
            else
                return m.IAdd(srcLeft, srcRight);
        }

        private Expression XNor(Expression left, Expression right)
        {
            return m.Comp(m.Xor(left, right));
        }
    }
}
