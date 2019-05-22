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

using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Reko.Core;
using System.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;

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
        private Tlcs90Instruction instr;
        private InstrClass rtlc;
        private RtlEmitter m;

        public Tlcs90Rewriter(Tlcs90Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new Tlcs90Disassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                rtlc = instr.InstructionClass;
                var instrs = new List<RtlInstruction>();
                this.m = new RtlEmitter(instrs);
                switch (instr.Opcode)
                {
                default:
                    EmitUnitTest();
                    Invalid();
                    host.Warn(
                       instr.Address,
                       string.Format(
                           "TLCS-90 instruction '{0}' not supported yet.",
                           instr.Opcode));

                    break;
                case Opcode.invalid: m.Invalid(); break;
                case Opcode.adc: RewriteAdcSbc(m.IAdd, "**-**V0*"); break;
                case Opcode.add: RewriteBinOp(m.IAdd, "**-***0*"); break;
                case Opcode.and: RewriteBinOp(m.And,  "**-10*00"); break;
                case Opcode.bit: RewriteBit("*--I**0-"); break;
                case Opcode.call: RewriteCall(); break;
                case Opcode.callr: RewriteCall(); break;
                case Opcode.ccf: RewriteCcf(); break;
                case Opcode.cp: RewriteCp(); break;
                case Opcode.cpl: RewriteCpl("---1--1-"); break;
                case Opcode.daa: RewriteDaa(); break;
                case Opcode.dec: RewriteIncDec(m.ISub, false); break;
                case Opcode.decx: RewriteIncDec(m.ISub, true); break;
                case Opcode.di: RewriteDi(); break;
                case Opcode.div: RewriteDiv(); break;
                case Opcode.djnz: RewriteDjnz(); break;
                case Opcode.ei: RewriteEi(); break;
                case Opcode.ex: RewriteEx(); break;
                case Opcode.exx: RewriteExx(); break;
                case Opcode.halt: RewriteHalt(); break;
                case Opcode.inc: RewriteIncDec(m.IAdd, false); break;
                case Opcode.incw: RewriteIncwDecw(m.IAdd); break;
                case Opcode.incx: RewriteIncDec(m.IAdd, true); break;
                case Opcode.jp: RewriteJp(); break;
                case Opcode.jr: RewriteJp(); break;
                case Opcode.ld: RewriteLd(); break;
                case Opcode.ldar: RewriteLdar(); break;
                case Opcode.ldir: RewriteLdir("---0-00-"); break;
                case Opcode.ldw: RewriteLd(); break;
                case Opcode.mul: RewriteMul(); break;
                case Opcode.neg: RewriteNeg(); break;
                case Opcode.nop: m.Nop(); break;
                case Opcode.or: RewriteBinOp(m.Or, "**-00*00"); break;
                case Opcode.pop: RewritePop(); break;
                case Opcode.push: RewritePush(); break;
                case Opcode.rcf: RewriteRcf(); break;
                case Opcode.res: RewriteSetRes(false); break;
                case Opcode.ret: RewriteRet(); break;
                case Opcode.reti: RewriteReti(); break;
                case Opcode.rl: RewriteRotation(PseudoProcedure.RolC, true); break;
                case Opcode.rlc: RewriteRotation(PseudoProcedure.Rol, false); break;
                case Opcode.rr: RewriteRotation(PseudoProcedure.RorC, true); break;
                case Opcode.rrc: RewriteRotation(PseudoProcedure.Ror, false); break;
                case Opcode.sbc: RewriteAdcSbc(m.ISub, "**-**V1*"); break;
                case Opcode.scf: RewriteScf(); break;
                case Opcode.set: RewriteSetRes(true); break;
                case Opcode.tset: RewriteTset("*--I**0-"); break;
                case Opcode.sla: RewriteShift(m.Shl); break;
                case Opcode.sll: RewriteShift(m.Shl); break;
                case Opcode.sra: RewriteShift(m.Sar); break;
                case Opcode.srl: RewriteShift(m.Shr); break;
                case Opcode.sub: RewriteBinOp(m.ISub, "**-**V1*"); break;
                case Opcode.swi: RewriteSwi(); break;
                case Opcode.xor: RewriteBinOp(m.Xor, "**-10*00"); break;
                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, instrs.ToArray())
                {
                    Class = rtlc,
                };
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
                   instr.Opcode));
            rtlc = InstrClass.Invalid;
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
                        binder.EnsureFlagGroup(arch.GetFlagGroup(mask)),
                        Constant.False());
                    break;
                case '1':
                    m.Assign(
                        binder.EnsureFlagGroup(arch.GetFlagGroup(mask)),
                        Constant.True());
                    break;
                }
                mask >>= 1;
            }
            if (grf != 0)
            {
                m.Assign(
                    binder.EnsureFlagGroup(arch.GetFlagGroup(grf)),
                    m.Cond(exp));
            }
        }

        private Expression RewriteSrc(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand reg:
                return binder.EnsureRegister(reg.Register);
            case AddressOperand addr:
                return addr.Address;
            case ImmediateOperand imm:
                return imm.Value;
            case MemoryOperand mem:
                Expression ea;
                if (mem.Base != null)
                {
                    ea = binder.EnsureRegister(mem.Base);
                    if (mem.Index != null)
                    {
                        var idx = binder.EnsureRegister(mem.Index);
                        ea = m.IAdd(
                            ea,
                            m.Cast(PrimitiveType.Int16, idx));
                    }
                    else if (mem.Offset != null)
                    {
                        ea = m.IAdd(
                            ea,
                            m.Int16((sbyte) mem.Offset.ToByte()));
                    }
                }
                else
                {
                    ea = arch.MakeAddressFromConstant(mem.Offset);
                }
                var tmp = binder.CreateTemporary(mem.Width);
                m.Assign(tmp, m.Mem(mem.Width, ea));
                return tmp;
            }
            throw new NotImplementedException(op.GetType().Name);
        }

        private Expression RewriteDst(MachineOperand op, Expression src, Func<Expression, Expression, Expression> fn)
        {
            var reg = op as RegisterOperand;
            if (reg != null)
            {
                var id = binder.EnsureRegister(reg.Register);
                m.Assign(id, fn(id, src));
                return id;
            }
            var addr = op as AddressOperand;
            if (addr != null)
            {
                return addr.Address;
            }
            var mem = op as MemoryOperand;
            if (mem != null)
            {
                Expression ea;
                if (mem.Base != null)
                {
                    ea = binder.EnsureRegister(mem.Base);
                    if (mem.Index != null)
                    {
                        var idx = binder.EnsureRegister(mem.Index);
                        ea = m.IAdd(
                            ea,
                            m.Cast(PrimitiveType.Int16, idx));
                    }
                    else if (mem.Offset != null)
                    {
                        ea = m.IAdd(
                            ea,
                            m.Int16((sbyte)mem.Offset.ToByte()));
                    }
                }
                else
                {
                    ea = arch.MakeAddressFromConstant(mem.Offset);
                }
                var load = m.Mem(mem.Width, ea);
                src = fn(load, src);
                if (src is Identifier || src is Constant)
                {
                    m.Assign(m.Mem(mem.Width, ea), src);
                    return src;
                }
                else
                {
                    var tmp = binder.CreateTemporary(ea.DataType);
                    m.Assign(tmp, src);
                    m.Assign(m.Mem(mem.Width, ea), tmp);
                    return tmp;
                }
            }
            throw new NotImplementedException(op.GetType().Name);
        }

        private Expression RewriteCondition(ConditionOperand c)
        {
            switch (c.Code)
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


        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            EmitUnitTest("Tlcs90_rw_");
        }

        [Conditional("DEBUG")]
        private void EmitUnitTest(string prefix)
        {
            //if (seen.Contains(dasm.Current.Opcode))
            //    return;
            //seen.Add(dasm.Current.Opcode);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var bytes = r2.ReadBytes(dasm.Current.Length);
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void {0}{1}()", prefix, dasm.Current.Opcode);
            Debug.WriteLine("        {");
            Debug.Print("            RewriteCode(\"{0}\");  // {1}",
                string.Join("", bytes.Select(b => string.Format("{0:X2}", (int)b))),
                dasm.Current);
            Debug.WriteLine("            AssertCode(");
            Debug.WriteLine("                \"0|L--|{0}({1}): 1 instructions\",", instr.Address, bytes.Length);
            Debug.WriteLine("                \"1|L--|@@@\");");
            Debug.WriteLine("        }");
            Debug.WriteLine("");
        }
    }
}
