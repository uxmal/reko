#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;

namespace Reko.Arch.Arm.AArch64
{
    public partial class A64Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly Arm64Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private IEnumerator<AArch64Instruction> dasm;
        private AArch64Instruction instr;
        private RtlEmitter m;
        private RtlClass rtlc;

        public A64Rewriter(Arm64Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new AArch64Disassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var cluster = new List<RtlInstruction>();
                m = new RtlEmitter(cluster);
                rtlc = RtlClass.Linear;
                switch (instr.opcode)
                {
                default:
                    EmitUnitTest();
                    goto case Opcode.Invalid;
                case Opcode.Invalid:
                    rtlc = RtlClass.Invalid;
                    m.Invalid();
                    break;
                case Opcode.add: RewriteBinary(m.IAdd); break;
                case Opcode.adds: RewriteBinary(m.IAdd, this.NZCV); break;
                case Opcode.adr: RewriteUnary(n => n); break;
                case Opcode.asrv: RewriteBinary(m.Sar); break;
                case Opcode.bic: RewriteBinary((a, b) => m.And(a, m.Comp(b))); break;
                case Opcode.adrp: RewriteAdrp(); break;
                case Opcode.and: RewriteBinary(m.And); break;
                case Opcode.ands: RewriteBinary(m.And, this.NZ00); break;
                case Opcode.b: RewriteB(); break;
                case Opcode.bl: RewriteBl(); break;
                case Opcode.blr: RewriteBlr(); break;
                case Opcode.br: RewriteBr(); break;
                case Opcode.cbnz: RewriteCb(m.Ne0); break;
                case Opcode.cbz: RewriteCb(m.Eq0); break;
                case Opcode.ccmp: RewriteCcmp(); break;
                case Opcode.cmp: RewriteCmp(); break;
                case Opcode.eor: RewriteBinary(m.Xor); break;
                case Opcode.csinc: RewriteCsinc(); break;
                case Opcode.ldp: RewriteLoadStorePair(true); break;
                case Opcode.ldr: RewriteLdr(null); break;
                case Opcode.ldrb: RewriteLdr(PrimitiveType.Byte); break;
                case Opcode.ldrh: RewriteLdr(PrimitiveType.Word16); break;
                case Opcode.ldrsb: RewriteLdr(PrimitiveType.SByte); break;
                case Opcode.ldrsh: RewriteLdr(PrimitiveType.Int16); break;
                case Opcode.ldrsw: RewriteLdr(PrimitiveType.Int32); break;
                case Opcode.lslv: RewriteBinary(m.Shl); break;
                case Opcode.lsrv: RewriteBinary(m.Shr); break;
                case Opcode.ldur: RewriteLdr(null); break;
                case Opcode.ldurb: RewriteLdr(PrimitiveType.Byte); break;
                case Opcode.ldurh: RewriteLdr(PrimitiveType.Word16); break;
                case Opcode.ldursb: RewriteLdr(PrimitiveType.SByte); break;
                case Opcode.ldursh: RewriteLdr(PrimitiveType.Int16); break;
                case Opcode.ldursw: RewriteLdr(PrimitiveType.Int32); break;
                case Opcode.madd: RewriteMaddSub(m.IAdd); break;
                case Opcode.mneg: RewriteBinary((a, b) => m.Neg(m.IMul(a, b))); break;
                case Opcode.msub: RewriteMaddSub(m.ISub); break;
                case Opcode.mov: RewriteUnary(n => n); break;
                case Opcode.movk: RewriteMovk(); break;
                case Opcode.movn: RewriteMovn(); break;
                case Opcode.movz: RewriteMovz(); break;
                case Opcode.mul: RewriteBinary(m.IMul); break;
                case Opcode.mvn: RewriteUnary(m.Comp); break;
                case Opcode.nop: m.Nop(); break;
                case Opcode.orr: RewriteBinary(m.Or);break;
                case Opcode.orn: RewriteBinary((a, b) => m.Or(a, m.Comp(b))); break;
                case Opcode.ret: RewriteRet(); break;
                case Opcode.scvtf: RewriteScvtf(); break;
                case Opcode.sdiv: RewriteBinary(m.SDiv); break;
                case Opcode.smaddl: RewriteMaddl(PrimitiveType.Int64, m.SMul); break;
                case Opcode.smull: RewriteMull(PrimitiveType.Int64, m.SMul); break;
                case Opcode.stp: RewriteLoadStorePair(false); break;
                case Opcode.str: RewriteStr(null); break;
                case Opcode.strb: RewriteStr(PrimitiveType.Byte); break;
                case Opcode.strh: RewriteStr(PrimitiveType.Word16); break;
                case Opcode.stur: RewriteStr(null); break;
                case Opcode.sturb: RewriteStr(PrimitiveType.Byte); break;
                case Opcode.sturh: RewriteStr(PrimitiveType.Word16); break;
                case Opcode.sub: RewriteBinary(m.ISub); break;
                case Opcode.subs: RewriteBinary(m.ISub, NZCV); break;
                case Opcode.tbnz: RewriteTb(m.Ne0); break;
                case Opcode.tbz: RewriteTb(m.Eq0); break;
                case Opcode.test: RewriteTest(); break;
                case Opcode.umaddl: RewriteMaddl(PrimitiveType.UInt64, m.UMul); break;
                case Opcode.umull: RewriteMull(PrimitiveType.UInt64, m.UMul); break;
                case Opcode.umulh: RewriteMulh(PrimitiveType.UInt64, m.UMul); break;

                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, cluster.ToArray())
                {
                    Class = rtlc,
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void NotImplementedYet()
        {
            uint wInstr;
            wInstr = rdr.PeekLeUInt32(-4);
            host.Error(instr.Address, "Rewriting A64 opcode '{0}' ({1:X4}) is not supported yet.", instr.opcode, wInstr);
            EmitUnitTest();
            m.Invalid();
        }

        private static HashSet<Opcode> seen = new HashSet<Opcode>();

        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            if (seen.Contains(dasm.Current.opcode))
                return;
            seen.Add(dasm.Current.opcode);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var wInstr = r2.ReadInt32();
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void A64Rw_" + dasm.Current.opcode + "()");
            Debug.WriteLine("        {");
            Debug.Write("            BuildTest(");
            Debug.Write($"0x{wInstr}");
            Debug.WriteLine(");\t// " + dasm.Current.ToString());
            Debug.WriteLine("            AssertCode(");
            Debug.WriteLine($"                \"0|L--|{dasm.Current.Address}({dasm.Current.Length}): 1 instructions\")");
            Debug.WriteLine( "                \"1|L--|@@@\");");
            Debug.WriteLine("        }");
            Debug.WriteLine("");
        }

        private Expression RewriteOp(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand regOp:
                return binder.EnsureRegister(regOp.Register);
            case ImmediateOperand immOp:
                return immOp.Value;
            case AddressOperand addrOp:
                return addrOp.Address;
            default:
                throw new NotImplementedException($"Rewriting {op.GetType().Name} not implemented yet.");
            }
        }

        private Expression MaybeZeroRegister(RegisterStorage reg, PrimitiveType dt)
        {
            if (reg == Registers.GpRegs32[31] ||
                reg == Registers.GpRegs64[31])
            {
                return Constant.Zero(dt);
            }
            else
            {
                return binder.EnsureRegister(reg);
            }
        }
        private Identifier NZCV()
        {
            var nzcv = arch.GetFlagGroup((uint)(FlagM.NF | FlagM.ZF | FlagM.CF | FlagM.VF));
            return binder.EnsureFlagGroup(nzcv);
        }

        private void NZCV(Expression test)
        {
            var nzcv = NZCV();
            m.Assign(nzcv, test);
        }

        private void NZ00(Expression test)
        {
            var nz = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)(FlagM.NF | FlagM.ZF)));
            var c = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.CF));
            var v = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.VF));
            m.Assign(nz, test);
            m.Assign(c, Constant.False());
            m.Assign(v, Constant.False());
        }

        Identifier FlagGroup(FlagM bits, string name, PrimitiveType type)
        {
            var grf = arch.GetFlagGroup((uint)bits);
            return binder.EnsureFlagGroup(grf);
        }

        protected Expression TestCond(ArmCondition cond)
        {
            switch (cond)
            {
            //default:
            //	throw new NotImplementedException(string.Format("ARM condition code {0} not implemented.", cond));
            case ArmCondition.HS:
                return m.Test(ConditionCode.UGE, FlagGroup(FlagM.CF, "C", PrimitiveType.Byte));
            case ArmCondition.LO:
                return m.Test(ConditionCode.ULT, FlagGroup(FlagM.CF, "C", PrimitiveType.Byte));
            case ArmCondition.EQ:
                return m.Test(ConditionCode.EQ, FlagGroup(FlagM.ZF, "Z", PrimitiveType.Byte));
            case ArmCondition.GE:
                return m.Test(ConditionCode.GE, FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF, "NZV", PrimitiveType.Byte));
            case ArmCondition.GT:
                return m.Test(ConditionCode.GT, FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF, "NZV", PrimitiveType.Byte));
            case ArmCondition.HI:
                return m.Test(ConditionCode.UGT, FlagGroup(FlagM.ZF | FlagM.CF, "ZC", PrimitiveType.Byte));
            case ArmCondition.LE:
                return m.Test(ConditionCode.LE, FlagGroup(FlagM.ZF | FlagM.CF | FlagM.VF, "NZV", PrimitiveType.Byte));
            case ArmCondition.LS:
                return m.Test(ConditionCode.ULE, FlagGroup(FlagM.ZF | FlagM.CF, "ZC", PrimitiveType.Byte));
            case ArmCondition.LT:
                return m.Test(ConditionCode.LT, FlagGroup(FlagM.NF | FlagM.VF, "NV", PrimitiveType.Byte));
            case ArmCondition.MI:
                return m.Test(ConditionCode.LT, FlagGroup(FlagM.NF, "N", PrimitiveType.Byte));
            case ArmCondition.PL:
                return m.Test(ConditionCode.GE, FlagGroup(FlagM.NF, "N", PrimitiveType.Byte));
            case ArmCondition.NE:
                return m.Test(ConditionCode.NE, FlagGroup(FlagM.ZF, "Z", PrimitiveType.Byte));
            case ArmCondition.VC:
                return m.Test(ConditionCode.NO, FlagGroup(FlagM.VF, "V", PrimitiveType.Byte));
            case ArmCondition.VS:
                return m.Test(ConditionCode.OV, FlagGroup(FlagM.VF, "V", PrimitiveType.Byte));
            }
            return null;
        }

        protected ArmCondition Invert(ArmCondition cc)
        {
            switch (cc)
            {
            case ArmCondition.EQ: return ArmCondition.NE;
            case ArmCondition.NE: return ArmCondition.EQ;
            case ArmCondition.HS: return ArmCondition.LO;
            case ArmCondition.LO: return ArmCondition.HS;
            case ArmCondition.MI: return ArmCondition.PL;
            case ArmCondition.PL: return ArmCondition.MI;
            case ArmCondition.VS: return ArmCondition.VC;
            case ArmCondition.VC: return ArmCondition.VS;
            case ArmCondition.HI: return ArmCondition.LS;
            case ArmCondition.LS: return ArmCondition.HI;
            case ArmCondition.GE: return ArmCondition.LT;
            case ArmCondition.LT: return ArmCondition.GE;
            case ArmCondition.GT: return ArmCondition.LE;
            case ArmCondition.LE: return ArmCondition.GT;
            case ArmCondition.AL: return ArmCondition.Invalid;
            }
            return ArmCondition.Invalid;
        }

    }
}