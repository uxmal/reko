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

using System;
using System.Collections;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Rtl;
using System.Diagnostics;
using System.Linq;
using Reko.Core.Machine;
using Reko.Core.Expressions;
using Reko.Core.Types;

namespace Reko.Arch.Msp430
{
    internal class Msp430Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly Msp430Architecture arch;
        private readonly ProcessorState state;
        private readonly EndianImageReader rdr;
        private readonly IEnumerator<Msp430Instruction> dasm;
        private Msp430Instruction instr;
        private RtlEmitter m;
        private InstrClass rtlc;

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
                this.rtlc = InstrClass.Linear;
                switch (instr.Mnemonic)
                {
                case Mnemonics.invalid: Invalid(); break;
                default:
                    EmitUnitTest();
                    Invalid();
                    break;
                case Mnemonics.addc: RewriteAdcSbc(m.IAdd); break;
                case Mnemonics.add: RewriteBinop(m.IAdd, "V-----NZC"); break;
                case Mnemonics.and: RewriteBinop(m.And,  "0-----NZC"); break;
                case Mnemonics.bic: RewriteBinop(Bis,    "---------"); break;
                case Mnemonics.bis: RewriteBinop(m.Or,   "---------"); break;
                case Mnemonics.bit: RewriteBit(); break;
                case Mnemonics.br: RewriteBr(); break;
                case Mnemonics.call: RewriteCall(); break;
                case Mnemonics.cmp: RewriteCmp(); break;
                case Mnemonics.dadd: RewriteBinop(Dadd,  "------NZC"); break;

                case Mnemonics.jc:  RewriteBranch(ConditionCode.ULT, FlagM.CF); break;
                case Mnemonics.jge: RewriteBranch(ConditionCode.GE, FlagM.VF|FlagM.NF); break;
                case Mnemonics.jl:  RewriteBranch(ConditionCode.LT, FlagM.VF | FlagM.NF); break;
                case Mnemonics.jmp: RewriteGoto(); break;
                case Mnemonics.jn:  RewriteBranch(ConditionCode.SG, FlagM.NF); break;
                case Mnemonics.jnc: RewriteBranch(ConditionCode.UGE, FlagM.CF); break;
                case Mnemonics.jnz: RewriteBranch(ConditionCode.NE, FlagM.ZF); break;
                case Mnemonics.jz:  RewriteBranch(ConditionCode.EQ, FlagM.ZF); break;

                case Mnemonics.mov: RewriteBinop((a, b) => b, ""); break;
                case Mnemonics.mova: RewriteBinop((a, b) => b, ""); break;
                case Mnemonics.popm: RewritePopm(); break;
                case Mnemonics.push: RewritePush(); break;
                case Mnemonics.pushm: RewritePushm(); break;
                case Mnemonics.ret: RewriteRet(); break;
                case Mnemonics.reti: RewriteReti(); break;
                case Mnemonics.rra: RewriteRra(  "0-----NZC"); break;
                case Mnemonics.rrax: RewriteRrax("0-----NZC"); break;
                case Mnemonics.rrc: RewriteRrc(  "0-----NZC"); break;
                case Mnemonics.rrum: RewriteRrum("0-----NZC"); break;
                case Mnemonics.sub: RewriteBinop(m.ISub, "V-----NZC"); break;
                case Mnemonics.subc: RewriteAdcSbc(m.ISub); break;
                case Mnemonics.swpb: RewriteSwpb(); break;
                case Mnemonics.sxt: RewriteSxt("0-----NZC"); break;
                case Mnemonics.xor: RewriteBinop(m.Xor,  "V-----NZC"); break;
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
            switch (op)
            {
            case RegisterOperand rop:
                return binder.EnsureRegister(rop.Register);
            case MemoryOperand mop:
                Expression ea = binder.EnsureRegister(mop.Base);
                if (mop.PostIncrement)
                {
                    var tmp = binder.CreateTemporary(op.Width);
                    m.Assign(tmp, m.Mem(op.Width, ea));
                    m.Assign(ea, m.IAdd(ea, m.Int16((short) op.Width.Size)));
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
            case ImmediateOperand iop:
                return iop.Value;
            case AddressOperand aop:
                return aop.Address;
            }
            throw new NotImplementedException(op.ToString());
        }

        private Expression RewriteDst(MachineOperand op, Expression src, Func<Expression,Expression,Expression> fn)
        {
            switch (op)
            {
            case RegisterOperand rop:
                var dst = binder.EnsureRegister(rop.Register);
                var ev = fn(dst, src);
                if (dst.Storage == Registers.sp && ev is Constant)
                {
                    m.SideEffect(host.PseudoProcedure("__set_stackpointer", VoidType.Instance, src));
                }
                else
                {
                    m.Assign(dst, ev);
                }
                return dst;
            case MemoryOperand mop:
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
            case AddressOperand aop:
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
                        binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.sr, mask)),
                        Constant.False());
                    break;
                case '1':
                    m.Assign(
                        binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.sr, mask)),
                        Constant.True());
                    break;
                }
                mask >>= 1;
            }
            if (grf != 0)
            {
                m.Assign(
                    binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.sr, grf)),
                    m.Cond(exp));
            }
        }

        private void RewriteAdcSbc(Func<Expression, Expression, Expression> fn)
        {
            var c = binder.EnsureFlagGroup(this.arch.GetFlagGroup(Registers.sr, (uint)FlagM.CF));
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[1], src, (a, b) => fn(fn(a, b), c));
            EmitCc(dst, "V-----NZC");
        }

        private void RewriteBinop(Func<Expression,Expression,Expression> fn, string vnzc)
        {
            var src = RewriteOp(instr.Operands[0]);
            if (instr.Operands[1] is RegisterOperand rop &&
                rop.Register == Registers.pc)
            {
                if (instr.Operands[0] is MemoryOperand mop &&
                    mop.PostIncrement &&
                    mop.Base == Registers.sp)
                {
                    m.Return(2, 0);
                    return;
                }
            }
            var dst = RewriteDst(instr.Operands[1], src, fn);
            EmitCc(dst, vnzc);
        }

        private void RewriteBit()
        {
            var left = RewriteOp(instr.Operands[1]);
            var right = RewriteOp(instr.Operands[0]);
            var tmp = binder.CreateTemporary(instr.Operands[0].Width);
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.sr, (uint)(FlagM.NF | FlagM.ZF)));
            var c = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.sr, (uint)FlagM.CF));
            var v = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.sr, (uint)FlagM.VF));
            m.Assign(tmp, m.And(left, right));
            m.Assign(grf, m.Cond(tmp));
            m.Assign(c, m.Test(ConditionCode.NE, tmp));
            m.Assign(v, Constant.Bool(false));
        }

        private void RewriteBr()
        {
            rtlc = InstrClass.Transfer;
            m.Goto(RewriteOp(instr.Operands[0]));
        }

        private void RewriteBranch(ConditionCode cc, FlagM flags)
        {
            rtlc = InstrClass.ConditionalTransfer;
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.sr, (uint)flags));
            m.Branch(m.Test(cc, grf), ((AddressOperand)instr.Operands[0]).Address, InstrClass.ConditionalTransfer);
        }

        private void RewriteCall()
        {
            rtlc = InstrClass.Transfer | InstrClass.Call;
            m.Call(RewriteOp(instr.Operands[0]), 2);
        }

        private void RewriteCmp()
        {
            var right = RewriteOp(instr.Operands[0]);
            var left = RewriteOp(instr.Operands[1]);
            EmitCc(m.ISub(left, right), "V-----NZC");
        }

        private void RewriteGoto()
        {
            rtlc = InstrClass.Transfer;
            m.Goto(RewriteOp(instr.Operands[0]));
        }

        private void RewritePopm()
        {
            int c = ((ImmediateOperand)instr.Operands[0]).Value.ToInt32();
            int iReg = ((RegisterOperand)instr.Operands[1]).Register.Number - c + 1;
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

        private void RewritePush()
        {
            var src = RewriteOp(instr.Operands[0]);
            var sp = binder.EnsureRegister(Registers.sp);
            m.Assign(sp, m.ISub(sp, m.Int32(2)));
            m.Assign(m.Mem16(sp), src);
        }

        private void RewritePushm()
        {
            int c = ((ImmediateOperand)instr.Operands[0]).Value.ToInt32();
            var sp = binder.EnsureRegister(Registers.sp);
            int iReg = ((RegisterOperand)instr.Operands[1]).Register.Number;
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

        private void RewriteRet()
        {
            rtlc = InstrClass.Transfer;
            m.Return(2, 0);
        }

        private void RewriteReti()
        {
            rtlc = InstrClass.Transfer;
            m.Return(2, 0);
        }

        private void RewriteRra(string flags)
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[0], src, (a, b) => m.Sar(a, m.Byte(1)));
            EmitCc(dst, flags);
        }

        private void RewriteRrax(string flags)
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[0], src, (a, b) => m.Sar(a, Repeat()));
            EmitCc(dst, flags);
        }

        private void RewriteRrc(string flags)
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteDst(
                instr.Operands[0],
                src, 
                (a, b) => host.PseudoProcedure(
                    PseudoProcedure.RorC, 
                    a.DataType, 
                    a, 
                    m.Byte(1),
                    binder.EnsureFlagGroup(this.arch.GetFlagGroup(Registers.sr, (uint) FlagM.CF))));
            EmitCc(dst, flags);
        }

        private Expression Repeat()
        {
            if (instr.repeatImm != 0)
            {
                return Constant.Byte((byte)instr.repeatImm);
            }
            else
            {
                return binder.EnsureRegister(instr.repeatReg);
            }
        }

        private void RewriteRrum(string flags)
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[1], src, (a, b) => m.Shr(a, b));
            EmitCc(dst, flags);
        }

        private void RewriteSwpb()
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[0],
                src,
                (a, b) => host.PseudoProcedure(
                    "__swpb",
                    PrimitiveType.Word16,
                    b));
        }

        private void RewriteSxt(string flags)
        {
            var src = RewriteOp(instr.Operands[0]);
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(tmp, m.Slice(PrimitiveType.Byte, src, 0));
            var dst = RewriteDst(instr.Operands[0], tmp, (a, b) => m.Cast(PrimitiveType.Int16, b));
            EmitCc(dst, flags);
        }

        private void Invalid()
        {
            m.Invalid();
            rtlc = InstrClass.Invalid;
        }

        private static HashSet<Mnemonics> seen = new HashSet<Mnemonics>();

        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            if (seen.Contains(instr.Mnemonic))
                return;
            seen.Add(instr.Mnemonic);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var bytes = r2.ReadBytes(dasm.Current.Length);
            Console.WriteLine("        [Test]");
            Console.WriteLine("        public void Msp430Rw_" + instr.Mnemonic + "()");
            Console.WriteLine("        {");
            Console.Write("            BuildTest(");
            Console.Write(string.Join(
                ", ",
                bytes.Select(b => string.Format("0x{0:X2}", (int)b))));
            Console.WriteLine(");\t// " + dasm.Current.ToString());
            Console.WriteLine("            AssertCode(");
            Console.WriteLine("                \"0|L--|0100(2): 1 instructions\",");
            Console.WriteLine("                \"1|L--|@@@\");");
            Console.WriteLine("        }");
            Console.WriteLine("");
        }
    }
}