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
using Reko.Core;
using Reko.Core.Rtl;
using System.Diagnostics;
using System.Linq;
using Reko.Core.Machine;
using Reko.Core.Expressions;

namespace Reko.Arch.Msp430
{
    internal class Msp430Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private IStorageBinder binder;
        private IRewriterHost host;
        private Msp430Architecture arch;
        private ProcessorState state;
        private EndianImageReader rdr;
        private IEnumerator<Msp430Instruction> dasm;
        private Msp430Instruction instr;
        private RtlEmitter m;
        private RtlClass rtlc;

        public Msp430Rewriter(Msp430Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.rdr = rdr;
            this.dasm = new Msp430Disassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var instrs = new List<RtlInstruction>();
                this.m = new RtlEmitter(instrs);
                this.rtlc = RtlClass.Linear;
                switch (instr.opcode)
                {
                case Opcode.invalid: Invalid(); break;
                default:
                    Invalid();
                    break;
                case Opcode.addc: RewriteAdcSbc(m.IAdd); break;
                case Opcode.add: RewriteBinop(m.IAdd, "V-----NZC"); break;
                case Opcode.and: RewriteBinop(m.And,  "0-----NZC"); break;
                case Opcode.bic: RewriteBinop(Bis,    "---------"); break;
                case Opcode.bis: RewriteBinop(m.Or,   "---------"); break;
                case Opcode.bit: RewriteBit(); break;
                case Opcode.call: RewriteCall(); break;
                case Opcode.cmp: RewriteCmp(); break;
                case Opcode.dadd: RewriteBinop(Dadd,  "------NZC"); break;

                case Opcode.jc:  RewriteBranch(ConditionCode.ULT, FlagM.CF); break;
                case Opcode.jge: RewriteBranch(ConditionCode.GE, FlagM.VF|FlagM.NF); break;
                case Opcode.jl:  RewriteBranch(ConditionCode.LT, FlagM.VF | FlagM.NF); break;
                case Opcode.jmp: RewriteGoto(); break;
                case Opcode.jn:  RewriteBranch(ConditionCode.SG, FlagM.NF); break;
                case Opcode.jnc: RewriteBranch(ConditionCode.UGE, FlagM.CF); break;
                case Opcode.jnz: RewriteBranch(ConditionCode.NE, FlagM.ZF); break;
                case Opcode.jz:  RewriteBranch(ConditionCode.EQ, FlagM.ZF); break;

                case Opcode.mov: RewriteBinop((a, b) => b, ""); break;
                case Opcode.popm: RewritePopm(); break;
                case Opcode.pushm: RewritePushm(); break;
                case Opcode.sub: RewriteBinop(m.ISub, "V-----NZC"); break;
                case Opcode.subc: RewriteAdcSbc(m.ISub); break;
                case Opcode.xor: RewriteBinop(m.Xor,  "V-----NZC"); break;
                }
                var rtlc = new RtlInstructionCluster(instr.Address, instr.Length, instrs.ToArray())
                {
                    Class = this.rtlc,
                };
                yield return rtlc;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Expression Bis(Expression a, Expression b)
        {
            return m.And(a, m.Comp(b));
        }

        private Expression Dadd(Expression a, Expression b)
        {
            return host.PseudoProcedure("__dadd", a.DataType, a, b);
        }

        private Expression RewriteOp(MachineOperand op)
        {
            var rop = op as RegisterOperand;
            if (rop != null)
            {
                return binder.EnsureRegister(rop.Register);
            }
            var mop = op as MemoryOperand;
            if (mop != null)
            {
                Expression ea = binder.EnsureRegister(mop.Base);
                if (mop.PostIncrement)
                {
                    var tmp = binder.CreateTemporary(op.Width);
                    m.Assign(tmp, m.Mem(op.Width, ea));
                    m.Assign(ea, m.IAdd(ea, m.Int16((short)op.Width.Size)));
                    return tmp;
                }
                else if (mop.Offset != 0)
                {
                    var tmp = binder.CreateTemporary(op.Width);
                    m.Assign(tmp, m.Mem(op.Width, m.IAdd(ea, m.Int16(mop.Offset))));
                    return tmp;
                }
                else
                {
                    var tmp = binder.CreateTemporary(op.Width);
                    m.Assign(tmp, m.Mem(op.Width, ea));
                    return tmp;
                }
            }
            var iop = op as ImmediateOperand;
            if (iop != null)
            {
                return iop.Value;
            }
            var aop = op as AddressOperand;
            if (aop != null)
            {
                return aop.Address;
            }
            throw new NotImplementedException(op.ToString());
        }

        private Expression RewriteDst(MachineOperand op, Expression src, Func<Expression,Expression,Expression> fn)
        {
            var rop = op as RegisterOperand;
            if (rop != null)
            {
                var dst = binder.EnsureRegister(rop.Register);
                m.Assign(dst, fn(dst, src));
                return dst;
            }
            var mop = op as MemoryOperand;
            if (mop != null)
            {
                Expression ea = binder.EnsureRegister(mop.Base);
                if (mop.Offset != 0)
                {
                    ea = m.IAdd(ea, m.Int16(mop.Offset));
                }
                var tmp = binder.CreateTemporary(mop.Width);
                m.Assign(tmp, m.Mem(tmp.DataType, ea));
                m.Assign(tmp, fn(tmp, src));
                m.Assign(m.Mem(tmp.DataType, ea.CloneExpression()), tmp);
                return tmp;
            }
            var aop = op as AddressOperand;
            if (aop != null)
            {
                var mem = m.Mem(op.Width, aop.Address);
                m.Assign(mem, fn(mem, src));
                return mem;
            }
            throw new NotImplementedException(op.ToString());
        }

        private void EmitCc(Expression exp, string vnzc)
        {
            var mask = 1u << 8;
            uint grf = 0;
            foreach (var c in vnzc)
            {
                switch (c)
                {
                case '*':
                case 'V':
                case 'N':
                case 'Z':
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

        private void RewriteAdcSbc(Func<Expression, Expression, Expression> fn)
        {
            var c = binder.EnsureFlagGroup(this.arch.GetFlagGroup((uint)FlagM.CF));
            var src = RewriteOp(instr.op1);
            var dst = RewriteDst(instr.op2, src, (a, b) => fn(fn(a, b), c));
            EmitCc(dst, "V-----NZC");
        }

        private void RewriteBinop(Func<Expression,Expression,Expression> fn, string vnzc)
        {
            var src = RewriteOp(instr.op1);
            var rop = instr.op2 as RegisterOperand;
            if (rop != null && rop.Register == Registers.pc)
            {
                var mop = instr.op1 as MemoryOperand;
                if (mop != null && mop.PostIncrement && mop.Base == Registers.sp)
                {
                    m.Return(2, 0);
                    return;
                }
            }
            var dst = RewriteDst(instr.op2, src, fn);
            EmitCc(dst, vnzc);
        }

        private void RewriteBit()
        {
            var left = RewriteOp(instr.op2);
            var right = RewriteOp(instr.op1);
            var tmp = binder.CreateTemporary(instr.op1.Width);
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)(FlagM.NF | FlagM.ZF)));
            var c = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.CF));
            var v = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.VF));
            m.Assign(tmp, m.And(left, right));
            m.Assign(grf, m.Cond(tmp));
            m.Assign(c, m.Test(ConditionCode.NE, tmp));
            m.Assign(v, Constant.Bool(false));
        }

        private void RewriteBranch(ConditionCode cc, FlagM flags)
        {
            rtlc = RtlClass.ConditionalTransfer;
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)flags));
            m.Branch(m.Test(cc, grf), ((AddressOperand)instr.op1).Address, RtlClass.ConditionalTransfer);
        }

        private void RewriteCall()
        {
            rtlc = RtlClass.Transfer | RtlClass.Call;
            m.Call(RewriteOp(instr.op1), 2);
        }

        private void RewriteCmp()
        {
            var right = RewriteOp(instr.op1);
            var left = RewriteOp(instr.op2);
            EmitCc(m.ISub(left, right), "V-----NZC");
        }

        private void RewriteGoto()
        {
            rtlc = RtlClass.Transfer;
            m.Goto(RewriteOp(instr.op1));
        }

        private void RewritePopm()
        {
            int c = ((ImmediateOperand)instr.op1).Value.ToInt32();
            int iReg = ((RegisterOperand)instr.op2).Register.Number - c + 1;
            if (iReg < 0)
            {
                Invalid();
                return;
            }
            var sp = binder.EnsureRegister(Registers.sp);
            while (c > 0)
            {
                m.Assign(binder.EnsureRegister(Registers.GpRegisters[iReg]), m.Mem16(sp));
                m.Assign(sp, m.IAdd(sp, m.Int32(2)));
                ++iReg;
                --c;
            }
        }

        private void RewritePushm()
        {
            int c = ((ImmediateOperand)instr.op1).Value.ToInt32();
            var sp = binder.EnsureRegister(Registers.sp);
            int iReg = ((RegisterOperand)instr.op2).Register.Number;
            if (iReg < c)
            {
                Invalid();
                return;
            }
            while (c > 0)
            {
                m.Assign(sp, m.ISub(sp, m.Int32(2)));
                m.Assign(m.Mem16(sp), binder.EnsureRegister(Registers.GpRegisters[iReg]));
                --iReg;
                --c;
            }
        }

        private void Invalid()
        {
            m.Invalid();
            rtlc = RtlClass.Invalid;
        }

#if DEBUG
        private static HashSet<Opcode> seen = new HashSet<Opcode>();
        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            if (seen.Contains(instr.opcode))
                return;
            seen.Add(instr.opcode);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var bytes = r2.ReadBytes(dasm.Current.Length);
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void Msp430Rw_" + instr.opcode + "()");
            Debug.WriteLine("        {");
            Debug.Write("            BuildTest(");
            Debug.Write(string.Join(
                ", ",
                bytes.Select(b => string.Format("0x{0:X2}", (int)b))));
            Debug.WriteLine(");\t// " + dasm.Current.ToString());
            Debug.WriteLine("            AssertCode(");
            Debug.WriteLine("                \"0|L--|0100(2): 1 instructions\",");
            Debug.WriteLine("                \"1|L--|@@@\");");
            Debug.WriteLine("        }");
            Debug.WriteLine("");
        }
#endif
    }
}