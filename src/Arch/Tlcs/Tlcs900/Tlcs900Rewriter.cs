#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Mnemonic = Reko.Arch.Tlcs.Tlcs900.Tlcs900Mnemonic;

namespace Reko.Arch.Tlcs.Tlcs900
{
    public partial class Tlcs900Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly Tlcs900Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<Tlcs900Instruction> dasm;
        private readonly List<RtlInstruction> instrs;
        private readonly RtlEmitter m;
        private InstrClass iclass;
        private Tlcs900Instruction instr;

        public Tlcs900Rewriter(Tlcs900Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new Tlcs900Disassembler(this.arch, rdr).GetEnumerator();
            this.instrs = new List<RtlInstruction>();
            this.m = new RtlEmitter(instrs);
            this.instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                iclass = InstrClass.Linear;
                this.instr = dasm.Current;
                switch (instr.Mnemonic)
                {
                default:
                    host.Warn(
                       instr.Address,
                       string.Format(
                           "TLCS-900 instruction '{0}' not supported yet.",
                           instr.Mnemonic));
                    EmitUnitTest();
                    Invalid();
                    break;
                case Mnemonic.invalid:
                    Invalid();
                    break;
                case Mnemonic.adc: RewriteAdcSbc(m.IAdd, "****0*"); break;
                case Mnemonic.add: RewriteBinOp(m.IAdd, "***V0*"); break;
                case Mnemonic.and: RewriteBinOp(m.And, "**1*00"); break;
                case Mnemonic.bit: RewriteBit(); break;
                case Mnemonic.bs1b: RewriteBs1b(); break;
                case Mnemonic.call: RewriteCall(); break;
                case Mnemonic.calr: RewriteCall(); break;
                case Mnemonic.ccf: RewriteCcf(); break;
                case Mnemonic.chg: RewriteChg(); break;
                case Mnemonic.cp: RewriteCp("SZHV1C"); break;
                case Mnemonic.daa: RewriteDaa("****-*"); break;
                case Mnemonic.dec: RewriteIncDec(m.ISub, "****1-"); break;
                case Mnemonic.decf: RewriteDecf(); break;
                case Mnemonic.div: RewriteDiv(m.UDiv, m.UMod, "---V--");break;
                case Mnemonic.divs: RewriteDiv(m.SDiv, m.SMod, "---V--");break;
                case Mnemonic.djnz: RewriteDjnz(); break;
                case Mnemonic.ei: RewriteEi(); break;
                case Mnemonic.ex: RewriteEx(); break;
                case Mnemonic.halt: RewriteHalt(); break;
                case Mnemonic.inc: RewriteIncDec(m.IAdd, "****0-"); break;
                case Mnemonic.incf: RewriteIncf(); break;
                case Mnemonic.lda: RewriteLda(); break;
                case Mnemonic.jp: RewriteJp(); break;
                case Mnemonic.jr: RewriteJp(); break;
                case Mnemonic.ld: RewriteLd(); break;
                case Mnemonic.ldf: RewriteLdf(); break;
                case Mnemonic.ldir: RewriteLdir(PrimitiveType.Byte, "--000-"); break;
                case Mnemonic.ldirw: RewriteLdir(PrimitiveType.Word16, "--000-"); break;
                case Mnemonic.mul: RewriteMul(Operator.UMul); break;
                case Mnemonic.muls: RewriteMul(Operator.SMul); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.or: RewriteBinOp(m.Or, "**0*00"); break;
                case Mnemonic.pop: RewritePop(); break;
                case Mnemonic.push: RewritePush(); break;
                case Mnemonic.rcf: RewriteRcf(); break;
                case Mnemonic.res: RewriteRes(); break;
                case Mnemonic.ret: RewriteRet(); break;
                case Mnemonic.retd: RewriteRetd(); break;
                case Mnemonic.reti: RewriteReti(); break;
                case Mnemonic.sbc: RewriteAdcSbc(m.ISub, "****1*"); break;
                case Mnemonic.scc: RewriteScc(); break;
                case Mnemonic.scf: RewriteScf(); break;
                case Mnemonic.set: RewriteSet(); break;
                case Mnemonic.sla: RewriteShift(m.Shl,"**0*0*"); break;
                case Mnemonic.sll: RewriteShift(m.Shl,"**0*0*"); break;
                case Mnemonic.srl: RewriteShift(m.Shr, "**0*0*"); break;
                case Mnemonic.sub: RewriteBinOp(m.ISub, "***V1*"); break;
                case Mnemonic.swi: RewriteSwi(); break;
                case Mnemonic.xor: RewriteBinOp(m.Xor, "**0*00"); break;
                case Mnemonic.zcf: RewriteZcf(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                this.instrs.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Invalid()
        {
            iclass = InstrClass.Invalid;
            m.Invalid();
        }

        private Expression RewriteSrc(MachineOperand op)
        {
            switch (op)
            {
            case RegisterStorage reg:
                return binder.EnsureRegister(reg);
            case Address addr:
                return addr;
            case Constant imm:
                return imm;
            case MemoryOperand mem:
                Expression ea = RewriteSrcEa(mem);
                var tmp = binder.CreateTemporary(mem.DataType);
                m.Assign(tmp, m.Mem(mem.DataType, ea));
                return tmp;
            }
            throw new NotImplementedException(op.GetType().Name);
        }

        /// <summary>
        /// Rewrites the effective address of a memory load.
        /// </summary>
        /// <param name="mem"></param>
        /// <returns></returns>
        private Expression RewriteSrcEa(MemoryOperand mem)
        {
            Expression ea;
            if (mem.Base is not null)
            {
                ea = binder.EnsureRegister(mem.Base);
                if (mem.Increment < 0)
                {
                    m.Assign(ea, m.AddSubSignedInt(ea, mem.Increment));
                }
                if (mem.Offset is not null)
                {
                    ea = m.IAdd(ea, mem.Offset);
                }
                if (mem.Increment > 0)
                {
                    var tmp = binder.CreateTemporary(ea.DataType);
                    m.Assign(tmp, ea);
                    m.Assign(ea, m.AddSubSignedInt(ea, mem.Increment));
                    ea = tmp;
                }
            }
            else
            {
                ea = arch.MakeAddressFromConstant(mem.Offset!, false);
            }
            return ea;
        }

        private Expression RewriteDst(MachineOperand op, Expression src, Func<Expression, Expression, Expression> fn)
        {
            switch (op)
            {
            case RegisterStorage reg:
                var id = binder.EnsureRegister(reg);
                m.Assign(id, fn(id, src));
                return id;
            case Address addr:
                return addr;
            case MemoryOperand mem:
                Expression ea;
                if (mem.Base is not null)
                {
                    ea = binder.EnsureRegister(mem.Base);
                }
                else
                {
                    ea = arch.MakeAddressFromConstant(mem.Offset!, false);
                }
                if (mem.Increment < 0)
                {
                    m.Assign(ea, m.ISub(ea, mem.DataType.Size));
                }
                var load = m.Mem(mem.DataType, ea);
                var tmp = binder.CreateTemporary(ea.DataType);
                m.Assign(tmp, fn(load, src));
                m.Assign(m.Mem(mem.DataType, ea), tmp);
                if (mem.Increment > 0)
                {
                    m.Assign(ea, m.IAdd(ea, mem.DataType.Size));
                }
                return tmp;
            }
            throw new NotImplementedException(op.GetType().Name);
        }

        public void EmitCc(Expression exp, string szhvnc) 
        {
            var mask = 1u << 5;
            uint grf = 0;
            foreach (var c in szhvnc)
            {
                switch (c)
                {
                case '*':
                case 'S':
                case 'Z':
                case 'H':
                case 'N':
                case 'C':
                case 'V':
                case 'P':
                    grf |= mask;
                    break;
                case '0':
                    m.Assign(
                        binder.EnsureFlagGroup(arch.GetFlagGroup(Tlcs900Registers.f,  mask)),
                        Constant.False());
                    break;
                case '1':
                    m.Assign(
                        binder.EnsureFlagGroup(arch.GetFlagGroup(Tlcs900Registers.f, mask)),
                        Constant.True());
                    break;
                }
                mask >>= 1;
            }
            if (grf != 0)
            {
                m.Assign(
                    binder.EnsureFlagGroup(arch.GetFlagGroup(Tlcs900Registers.f, grf)),
                    m.Cond(exp));
            }
        }

        private Expression GenerateTestExpression(ConditionOperand<CondCode> cOp, bool invert)
        {
            ConditionCode cc = ConditionCode.ALWAYS;
            string flags = "";
            switch (cOp.Condition)
            {
            case CondCode.F: return invert ? Constant.True() : Constant.False();
            case CondCode.LT: cc = invert ? ConditionCode.GE : ConditionCode.LT; flags = "SV"; break;
            case CondCode.LE: cc = invert ? ConditionCode.GT : ConditionCode.LE; flags = "SZV"; break;
            case CondCode.ULE: cc = invert ? ConditionCode.UGT : ConditionCode.ULE; flags = "ZC"; break;
            case CondCode.OV: cc = invert ? ConditionCode.NO : ConditionCode.OV; flags = "V"; break;
            case CondCode.MI: cc = invert ? ConditionCode.NS : ConditionCode.SG; flags = "S"; break;
            case CondCode.Z: cc = invert ? ConditionCode.NE : ConditionCode.EQ; flags = "Z"; break;
            case CondCode.C: cc = invert ? ConditionCode.UGE : ConditionCode.ULT; flags = "Z"; break;
            case CondCode.T: return invert ? Constant.False() : Constant.True();
            case CondCode.GE: cc = invert ? ConditionCode.LT : ConditionCode.GE; flags = "SV"; break;
            case CondCode.GT: cc = invert ? ConditionCode.LE : ConditionCode.GT; flags = "SZV"; break;
            case CondCode.UGT: cc = invert ? ConditionCode.ULE : ConditionCode.UGT; flags = "ZC"; break;
            case CondCode.NV: cc = invert ? ConditionCode.OV : ConditionCode.NO; flags = "V"; break;
            case CondCode.PL: cc = invert ? ConditionCode.SG : ConditionCode.NS; flags = "S"; break;
            case CondCode.NZ: cc = invert ? ConditionCode.EQ : ConditionCode.NE; flags = "Z"; break;
            case CondCode.NC: cc = invert ? ConditionCode.ULT : ConditionCode.UGE; flags = "Z"; break;
            }
            return m.Test(
                cc,
                binder.EnsureFlagGroup(arch.GetFlagGroup(flags)));
        }

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("Tlcs900_rw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private static readonly IntrinsicProcedure bs1b_intrinsic = IntrinsicBuilder.Unary("__bs1b", PrimitiveType.Byte);

        private static readonly IntrinsicProcedure daa_intrinsic = IntrinsicBuilder.Unary("__daa", PrimitiveType.Byte);
        private static readonly IntrinsicProcedure decf_intrinsic = IntrinsicBuilder.SideEffect("__decf")
            .Void();
        private static readonly IntrinsicProcedure incf_intrinsic = IntrinsicBuilder.SideEffect("__incf")
            .Void();
        private static readonly IntrinsicProcedure ldf_intrinsic = IntrinsicBuilder.SideEffect("__ldf")
            .Param(PrimitiveType.Byte)
            .Void();

        private static readonly IntrinsicProcedure ei_intrinsic = IntrinsicBuilder.SideEffect("__ei")
            .Param(PrimitiveType.Byte)
            .Void();
    }

}
