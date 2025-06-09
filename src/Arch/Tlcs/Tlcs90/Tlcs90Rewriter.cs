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
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Tlcs.Tlcs90
{
    public partial class Tlcs90Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly Tlcs90Architecture arch;
        private readonly IEnumerator<Tlcs90Instruction> dasm;
        private readonly List<RtlInstruction> instrs;
        private readonly RtlEmitter m;
        private Tlcs90Instruction instr;
        private InstrClass iclass;

        public Tlcs90Rewriter(Tlcs90Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new Tlcs90Disassembler(arch, rdr).GetEnumerator();
            this.instrs = new List<RtlInstruction>();
            this.m = new RtlEmitter(instrs);
            this.instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    Invalid();
                    host.Warn(
                       instr.Address,
                       string.Format(
                           "TLCS-90 instruction '{0}' not supported yet.",
                           instr.Mnemonic));

                    break;
                case Mnemonic.invalid: m.Invalid(); break;
                case Mnemonic.adc: RewriteAdcSbc(m.IAdd, "**-**V0*"); break;
                case Mnemonic.add: RewriteBinOp(m.IAdd, "**-***0*"); break;
                case Mnemonic.and: RewriteBinOp(m.And,  "**-10*00"); break;
                case Mnemonic.bit: RewriteBit("*--I**0-"); break;
                case Mnemonic.call: RewriteCall(); break;
                case Mnemonic.callr: RewriteCall(); break;
                case Mnemonic.ccf: RewriteCcf(); break;
                case Mnemonic.cp: RewriteCp(); break;
                case Mnemonic.cpl: RewriteCpl("---1--1-"); break;
                case Mnemonic.daa: RewriteDaa(); break;
                case Mnemonic.dec: RewriteIncDec(m.ISub, false); break;
                case Mnemonic.decx: RewriteIncDec(m.ISub, true); break;
                case Mnemonic.di: RewriteDi(); break;
                case Mnemonic.div: RewriteDiv(); break;
                case Mnemonic.djnz: RewriteDjnz(); break;
                case Mnemonic.ei: RewriteEi(); break;
                case Mnemonic.ex: RewriteEx(); break;
                case Mnemonic.exx: RewriteExx(); break;
                case Mnemonic.halt: RewriteHalt(); break;
                case Mnemonic.inc: RewriteIncDec(m.IAdd, false); break;
                case Mnemonic.incw: RewriteIncwDecw(m.IAdd); break;
                case Mnemonic.incx: RewriteIncDec(m.IAdd, true); break;
                case Mnemonic.jp: RewriteJp(); break;
                case Mnemonic.jr: RewriteJp(); break;
                case Mnemonic.ld: RewriteLd(); break;
                case Mnemonic.ldar: RewriteLdar(); break;
                case Mnemonic.ldir: RewriteLdir("---0-00-"); break;
                case Mnemonic.ldw: RewriteLd(); break;
                case Mnemonic.mul: RewriteMul(); break;
                case Mnemonic.neg: RewriteNeg(); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.or: RewriteBinOp(m.Or, "**-00*00"); break;
                case Mnemonic.pop: RewritePop(); break;
                case Mnemonic.push: RewritePush(); break;
                case Mnemonic.rcf: RewriteRcf(); break;
                case Mnemonic.res: RewriteSetRes(false); break;
                case Mnemonic.ret: RewriteRet(); break;
                case Mnemonic.reti: RewriteReti(); break;
                case Mnemonic.rl: RewriteRotation(CommonOps.RolC, true); break;
                case Mnemonic.rlc: RewriteRotation(CommonOps.Ror, false); break;
                case Mnemonic.rr: RewriteRotation(CommonOps.RorC, true); break;
                case Mnemonic.rrc: RewriteRotation(CommonOps.Ror, false); break;
                case Mnemonic.sbc: RewriteAdcSbc(m.ISub, "**-**V1*"); break;
                case Mnemonic.scf: RewriteScf(); break;
                case Mnemonic.set: RewriteSetRes(true); break;
                case Mnemonic.tset: RewriteTset("*--I**0-"); break;
                case Mnemonic.sla: RewriteShift(m.Shl); break;
                case Mnemonic.sll: RewriteShift(m.Shl); break;
                case Mnemonic.sra: RewriteShift(m.Sar); break;
                case Mnemonic.srl: RewriteShift(m.Shr); break;
                case Mnemonic.sub: RewriteBinOp(m.ISub, "**-**V1*"); break;
                case Mnemonic.swi: RewriteSwi(); break;
                case Mnemonic.xor: RewriteBinOp(m.Xor, "**-10*00"); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                instrs.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Invalid()
        {
            host.Error(
               instr.Address,
               string.Format(
                   "Rewriting of TLCS-90 instruction '{0}' not implemented yet.",
                   instr.Mnemonic));
            iclass = InstrClass.Invalid;
            m.Invalid();
        }

        public void EmitCc(Expression exp, string szhvnc)
        {
            // SZIH XVNC
            var mask = 1u << 7;
            uint grf = 0;
            foreach (var c in szhvnc)
            {
                switch (c)
                {
                case '*':
                case 'S':
                case 'Z':
                case 'I':
                case 'H':
                case 'X':
                case 'V':
                case 'N':
                case 'C':
                    grf |= mask;
                    break;
                case '0':
                    m.Assign(
                        binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.f, mask)),
                        Constant.False());
                    break;
                case '1':
                    m.Assign(
                        binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.f, mask)),
                        Constant.True());
                    break;
                }
                mask >>= 1;
            }
            if (grf != 0)
            {
                m.Assign(
                    binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.f, grf)),
                    m.Cond(exp));
            }
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
                Expression ea;
                if (mem.Base is not null)
                {
                    ea = binder.EnsureRegister(mem.Base);
                    if (mem.Index is not null)
                    {
                        var idx = binder.EnsureRegister(mem.Index);
                        ea = m.IAdd(
                            ea,
                            m.Convert(idx, idx.DataType, PrimitiveType.Int16));
                    }
                    else if (mem.Offset is not null)
                    {
                        ea = m.IAdd(
                            ea,
                            m.Int16((sbyte) mem.Offset.ToByte()));
                    }
                }
                else
                {
                    ea = arch.MakeAddressFromConstant(mem.Offset!, false);
                }
                var tmp = binder.CreateTemporary(mem.DataType);
                m.Assign(tmp, m.Mem(mem.DataType, ea));
                return tmp;
            }
            throw new NotImplementedException(op.GetType().Name);
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
                    if (mem.Index is not null)
                    {
                        var idx = binder.EnsureRegister(mem.Index);
                        ea = m.IAdd(
                            ea,
                            m.Convert(idx, idx.DataType, PrimitiveType.Int16));
                    }
                    else if (mem.Offset is not null)
                    {
                        ea = m.IAdd(
                            ea,
                            m.Int16((sbyte)mem.Offset.ToByte()));
                    }
                }
                else
                {
                    ea = arch.MakeAddressFromConstant(mem.Offset!, false);
                }
                var load = m.Mem(mem.DataType, ea);
                src = fn(load, src);
                if (src is Identifier || src is Constant)
                {
                    m.Assign(m.Mem(mem.DataType, ea), src);
                    return src;
                }
                else
                {
                    var tmp = binder.CreateTemporary(ea.DataType);
                    m.Assign(tmp, src);
                    m.Assign(m.Mem(mem.DataType, ea), tmp);
                    return tmp;
                }
            }
            throw new NotImplementedException(op.GetType().Name);
        }

        private Expression RewriteCondition(ConditionOperand<CondCode> c)
        {
            switch (c.Condition)
            {
            case CondCode.F: return Constant.False();
            case CondCode.LT: return m.Test(ConditionCode.LT, binder.EnsureFlagGroup(arch.GetFlagGroup("SV")));
            case CondCode.LE: return m.Test(ConditionCode.LE, binder.EnsureFlagGroup(arch.GetFlagGroup("SZV")));
            case CondCode.ULE: return m.Test(ConditionCode.ULE, binder.EnsureFlagGroup(arch.GetFlagGroup("ZC")));
            case CondCode.OV: return m.Test(ConditionCode.OV, binder.EnsureFlagGroup(arch.GetFlagGroup("V")));
            case CondCode.M: return m.Test(ConditionCode.LT, binder.EnsureFlagGroup(arch.GetFlagGroup("S")));
            case CondCode.Z: return m.Test(ConditionCode.EQ, binder.EnsureFlagGroup(arch.GetFlagGroup("Z")));
            case CondCode.C: return m.Test(ConditionCode.ULT, binder.EnsureFlagGroup(arch.GetFlagGroup("C")));
            case CondCode.T: return Constant.True();
            case CondCode.GE: return m.Test(ConditionCode.GE, binder.EnsureFlagGroup(arch.GetFlagGroup("SV")));
            case CondCode.GT: return m.Test(ConditionCode.GT, binder.EnsureFlagGroup(arch.GetFlagGroup("SZV")));
            case CondCode.UGT: return m.Test(ConditionCode.UGT, binder.EnsureFlagGroup(arch.GetFlagGroup("ZC")));
            case CondCode.NV: return m.Test(ConditionCode.NO, binder.EnsureFlagGroup(arch.GetFlagGroup("V")));
            case CondCode.P: return m.Test(ConditionCode.GE, binder.EnsureFlagGroup(arch.GetFlagGroup("S")));
            case CondCode.NZ: return m.Test(ConditionCode.NE, binder.EnsureFlagGroup(arch.GetFlagGroup("Z")));
            case CondCode.NC: return m.Test(ConditionCode.UGE, binder.EnsureFlagGroup(arch.GetFlagGroup("C")));
            }
            throw new ArgumentException();
        }

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("Tlcs90_rw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private static readonly IntrinsicProcedure di_intrinsic = IntrinsicBuilder.SideEffect("__disable_interrupts")
            .Void();
        private static readonly IntrinsicProcedure ei_intrinsic = IntrinsicBuilder.SideEffect("__enable_interrupts")
            .Void();
    }
}
