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
using System.Diagnostics;
using System.Linq;

namespace Reko.Arch.Rl78
{
    public class Rl78Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly Rl78Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly LookaheadEnumerator<Rl78Instruction> dasm;
        private RtlEmitter m;
        private InstrClass iclass;
        private Rl78Instruction instr;

        public Rl78Rewriter(Rl78Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new LookaheadEnumerator<Rl78Instruction>(new Rl78Disassembler(arch, rdr));
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var instrs = new List<RtlInstruction>();
                this.m = new RtlEmitter(instrs);
                this.iclass = dasm.Current.InstructionClass;
                switch (instr.Mnemonic)
                {
                case Mnemonic.invalid:
                    m.Invalid(); break;
                case Mnemonic.brk:
                case Mnemonic.cmps:
                case Mnemonic.movs:
                case Mnemonic.retb:
                case Mnemonic.stop:
                default:
                    EmitUnitTest();
                    this.iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.add: RewriteAdd(); break;
                case Mnemonic.addc: RewriteAddcSubc(m.IAdd); break;
                case Mnemonic.addw: RewriteAddw(); break;
                case Mnemonic.and: RewriteLogical(m.And); break;
                case Mnemonic.and1: RewriteLogical1(m.And); break;
                case Mnemonic.bc: RewriteBranch(ConditionCode.ULT, C()); break;
                case Mnemonic.bf: RewriteBf(); break;
                case Mnemonic.bh: RewriteBranch(ConditionCode.UGT, CZ()); break;
                case Mnemonic.bnc: RewriteBranch(ConditionCode.UGE, C()); break;
                case Mnemonic.bnh: RewriteBranch(ConditionCode.ULE, CZ()); break;
                case Mnemonic.bnz: RewriteBranch(ConditionCode.NE, Z()); break;
                case Mnemonic.br: RewriteBr(); break;
                case Mnemonic.bt: RewriteBt(); break;
                case Mnemonic.btclr: RewriteBtclr(); break;
                case Mnemonic.bz: RewriteBranch(ConditionCode.EQ, Z()); break;
                case Mnemonic.call: RewriteCall(); break;
                case Mnemonic.callt: RewriteCall(); break;
                case Mnemonic.clr1: RewriteClr1(); break;
                case Mnemonic.clrb: RewriteClr(PrimitiveType.Byte); break;
                case Mnemonic.clrw: RewriteClr(PrimitiveType.Word16); break;
                case Mnemonic.cmp: RewriteCmp(); break;
                case Mnemonic.cmp0: RewriteCmp0(); break;
                case Mnemonic.cmpw: RewriteCmp(); break;
                case Mnemonic.dec: RewriteIncDec((a, b) => m.ISubS(a, 1)); break;
                case Mnemonic.decw: RewriteIncwDecw((a, b) => m.ISubS(a, 1)); break;
                case Mnemonic.halt: RewriteHalt(); break;
                case Mnemonic.inc: RewriteIncDec((a, b) => m.IAddS(a, 1)); break;
                case Mnemonic.incw: RewriteIncwDecw((a, b) => m.IAddS(a, 1)); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.mov1: RewriteMov1(); break;
                case Mnemonic.movw: RewriteMov(); break;
                case Mnemonic.mulu: RewriteMulu(); break;
                case Mnemonic.oneb: RewriteOne(PrimitiveType.Byte); break;
                case Mnemonic.onew: RewriteOne(PrimitiveType.Word16); break;
                case Mnemonic.or: RewriteLogical(m.Or); break;
                case Mnemonic.or1: RewriteLogical1(m.Or); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.not1: RewriteNot1(); break;
                case Mnemonic.pop: RewritePop(); break;
                case Mnemonic.push: RewritePush(); break;
                case Mnemonic.ret: RewriteRet(); break;
                case Mnemonic.reti: RewriteRet(); break;
                case Mnemonic.rol: RewriteRotate(PseudoProcedure.Rol); break;
                case Mnemonic.rolc: RewriteRotateC(PseudoProcedure.RolC); break;
                case Mnemonic.rolwc: RewriteRotateC(PseudoProcedure.RolC); break;
                case Mnemonic.ror: RewriteRotate(PseudoProcedure.Ror); break;
                case Mnemonic.rorc: RewriteRotate(PseudoProcedure.RorC); break;
                case Mnemonic.sar: RewriteShift(m.Sar); break;
                case Mnemonic.sarw: RewriteShiftw(m.Sar); break;
                case Mnemonic.sel: RewriteSel(); break;
                case Mnemonic.set1: RewriteSet1(); break;
                case Mnemonic.shl: RewriteShift(m.Shl); break;
                case Mnemonic.shlw: RewriteShiftw(m.Shl); break;
                case Mnemonic.shr: RewriteShift(m.Shr); break;
                case Mnemonic.shrw: RewriteShiftw(m.Shr); break;
                case Mnemonic.skc: RewriteSkip(ConditionCode.ULT, C()); break;
                case Mnemonic.skh: RewriteSkip(ConditionCode.UGT, CZ()); break;
                case Mnemonic.sknc: RewriteSkip(ConditionCode.UGE, C()); break;
                case Mnemonic.sknh: RewriteSkip(ConditionCode.ULE, CZ()); break;
                case Mnemonic.sknz: RewriteSkip(ConditionCode.NE, Z()); break;
                case Mnemonic.skz: RewriteSkip(ConditionCode.EQ, Z()); break;
                case Mnemonic.sub: RewriteSub(); break;
                case Mnemonic.subc: RewriteAddcSubc(m.ISub); break;
                case Mnemonic.subw: RewriteSubw(); break;
                case Mnemonic.xch: RewriteXch(); break;
                case Mnemonic.xchw: RewriteXch(); break;
                case Mnemonic.xor: RewriteLogical(m.Xor); break;
                case Mnemonic.xor1: RewriteLogical1(m.Xor); break;
                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, instrs.ToArray())
                {
                    Class = this.iclass
                };
            }
        }

         private static HashSet<Mnemonic> seen = new HashSet<Mnemonic>();

        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            if (seen.Contains(dasm.Current.Mnemonic))
                return;
            seen.Add(dasm.Current.Mnemonic);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var bytes = r2.ReadBytes(dasm.Current.Length);
            Console.WriteLine("        [Test]");
            Console.WriteLine("        public void Rl78Rw_" + dasm.Current.Mnemonic + "()");
            Console.WriteLine("        {");
            Console.Write("            RunTest(\"");
            Console.Write(string.Join(
                " ",
                bytes.Select(b => string.Format("{0:X2}", (int) b))));
            Console.WriteLine("\");\t// " + dasm.Current.ToString());
            Console.WriteLine("            AssertCode(");
            Console.WriteLine("                \"0|L--|{0}({1}): 1 instructions\",", dasm.Current.Address, dasm.Current.Length);
            Console.WriteLine("                \"1|L--|@@@\");");
            Console.WriteLine("        }");
            Console.WriteLine("");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Identifier C()
        {
            var c = arch.GetFlagGroup(Registers.psw, (uint) FlagM.CF);
            return binder.EnsureFlagGroup(c);
        }

        private Identifier CZ()
        {
            var cz = arch.GetFlagGroup(Registers.psw, (uint) (FlagM.CF | FlagM.ZF));
            return binder.EnsureFlagGroup(cz);
        }

        private Identifier Z()
        {
            var z = arch.GetFlagGroup(Registers.psw, (uint) FlagM.ZF);
            return binder.EnsureFlagGroup(z);
        }

        private Expression RewriteSrc(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand rop:
                return binder.EnsureRegister(rop.Register);
            case ImmediateOperand iop:
                return iop.Value;
            case AddressOperand aop:
                return aop.Address;
            case MemoryOperand mop:
                Expression ea;
                if (mop.Base != null)
                {
                    ea = binder.EnsureRegister(mop.Base);
                    if (mop.Offset != 0)
                    {
                        ea = m.IAddS(ea, mop.Offset);
                    }
                }
                else
                {
                    ea = m.Word32(mop.Offset);
                }
                if (mop.Index != null)
                {
                    var idx = binder.EnsureRegister(mop.Index);
                    ea = m.IAdd(ea, idx);
                }
                return m.Mem(op.Width, ea);
            case BitOperand bit:
                var bitSrc = RewriteSrc(bit.Operand);
                return host.PseudoProcedure(
                    "__bit",
                    PrimitiveType.Bool,
                    bitSrc,
                    Constant.Byte((byte) bit.BitPosition));
            case FlagGroupOperand fop:
                return binder.EnsureFlagGroup(fop.FlagGroup);
            default:
                throw new NotImplementedException($"Rl87Rewriter: operand type {op.GetType().Name} not implemented yet.");
            }
        }

        private Expression RewriteDst(MachineOperand op, Expression src, Func<Expression, Expression, Expression> fn)
        {
            switch (op)
            {
            case RegisterOperand rop:
                var rDst = binder.EnsureRegister(rop.Register);
                m.Assign(rDst, fn(rDst, src));
                return rDst;
            case FlagGroupOperand fop:
                var grfDst = binder.EnsureFlagGroup(fop.FlagGroup);
                m.Assign(grfDst, fn(grfDst, src));
                return grfDst;
            case MemoryOperand mop:
                Expression ea;
                if (mop.Base != null)
                {
                    ea = binder.EnsureRegister(mop.Base);
                    if (mop.Offset != 0)
                    {
                        ea = m.IAddS(ea, mop.Offset);
                    }
                }
                else
                {
                    ea = m.Word32(mop.Offset);
                }
                if (mop.Index != null)
                {
                    var idx = binder.EnsureRegister(mop.Index);
                    ea = m.IAdd(ea, idx);
                }
                Expression tmp = binder.CreateTemporary(op.Width);
                m.Assign(tmp, fn(m.Mem(op.Width, ea), src));
                m.Assign(m.Mem(op.Width, ea), tmp);
                return tmp;
            case BitOperand bit:
                var left = RewriteSrc(bit.Operand);
                m.SideEffect(host.PseudoProcedure(
                    "__set_bit",
                    VoidType.Instance,
                    left,
                    Constant.Byte((byte) bit.BitPosition),
                    fn(left, src)));
                return left;
            default:
                throw new NotImplementedException($"Rl87Rewriter: operand type {op.GetType().Name} not implemented yet.");
            }
        }

        private void EmitCond(Expression e, Identifier grf)
        {
            m.Assign(grf, m.Cond(e));
        }

        private void RewriteAdd()
        {
            var src = RewriteSrc(instr.Operands[1]);
            var dst = RewriteDst(instr.Operands[0], src, m.IAdd);
            EmitCond(dst, CZ());
        }

        private void RewriteAddcSubc(Func<Expression, Expression, Expression> fn)
        {
            var src = RewriteSrc(instr.Operands[1]);
            var dst = RewriteDst(instr.Operands[0], src, (a, b) => fn(fn(a, b), C()));
            EmitCond(dst, CZ());
        }

        private void RewriteAddw()
        {
            var src = RewriteSrc(instr.Operands[1]);
            Func<Expression,Expression,Expression> fn;
            if (src.DataType.Size < instr.Operands[0].Width.Size)
            {
                if (src is Constant c)
                {
                    fn = (a, b) => m.IAddS(a, c.ToInt32());
                }
                else
                {
                    fn = (a, b) => m.IAdd(a, m.Cast(PrimitiveType.Word16, b));
                }
            }
            else
            {
                fn = m.IAdd;
            }
            var dst = RewriteDst(instr.Operands[0], src, fn);
            EmitCond(dst, CZ());
        }

        private void RewriteBf()
        {
            var cond = RewriteSrc(instr.Operands[0]);
            var target = (Address) RewriteSrc(instr.Operands[1]);
            m.Branch(m.Not(cond), target, InstrClass.ConditionalTransfer);
        }

        private void RewriteBr()
        {
            var target = RewriteSrc(instr.Operands[0]);
            m.Goto(target);
        }

        private void RewriteBranch(ConditionCode cc, Identifier grf)
        {
            var target = (Address) RewriteSrc(instr.Operands[0]);
            m.Branch(m.Test(cc, grf), target, InstrClass.ConditionalTransfer);
        }

        private void RewriteBt()
        {
            var cond = RewriteSrc(instr.Operands[0]);
            var target = (Address) RewriteSrc(instr.Operands[1]);
            m.Branch(cond, target, InstrClass.ConditionalTransfer);
        }

        private void RewriteBtclr()
        {
            var cond = RewriteSrc(instr.Operands[0]);
            m.BranchInMiddleOfInstruction(m.Not(cond), instr.Address + instr.Length, InstrClass.ConditionalTransfer);
            RewriteDst(instr.Operands[0], Constant.False(), (a, b) => b);
            m.Goto(((AddressOperand) instr.Operands[1]).Address);
        }

        private void RewriteCall()
        {
            var target = RewriteSrc(instr.Operands[0]);
            m.Call(target, 4);
        }

        private void RewriteClr(PrimitiveType dt)
        {
            m.Assign(RewriteSrc(instr.Operands[0]), Constant.Zero(dt));
        }

        private void RewriteClr1()
        {
            RewriteDst(instr.Operands[0], Constant.Bool(false), (a, b) => b);
        }

        private void RewriteCmp()
        {
            var left = RewriteSrc(instr.Operands[0]);
            var right = RewriteSrc(instr.Operands[1]);
            m.Assign(CZ(), m.Cond(m.ISub(left, right)));
        }

        private void RewriteCmp0()
        {
            var left = RewriteSrc(instr.Operands[0]);
            var right = Constant.Zero(left.DataType);
            m.Assign(CZ(), m.Cond(m.ISub(left, right)));
        }

        private void RewriteHalt()
        {
            m.SideEffect(host.PseudoProcedure("__halt", VoidType.Instance));
        }

        private void RewriteIncDec(Func<Expression, Expression, Expression> fn)
        {
            var dst = RewriteDst(instr.Operands[0], null, fn);
            EmitCond(dst, Z());
        }

        private void RewriteIncwDecw(Func<Expression,Expression,Expression> fn)
        {
            RewriteDst(instr.Operands[0], null, fn);
        }

        private void RewriteLogical(Func<Expression,Expression,Expression> fn)
        {
            var src = RewriteSrc(instr.Operands[1]);
            var dst = RewriteDst(instr.Operands[0], src, fn);
            EmitCond(dst, Z());
        }

        private void RewriteLogical1(Func<Expression, Expression, Expression> fn)
        {
            var src = RewriteSrc(instr.Operands[1]);
            var dst = RewriteDst(instr.Operands[0], src, fn);
        }

        private void RewriteMov()
        {
            var src = RewriteSrc(instr.Operands[1]);
            var dst = RewriteSrc(instr.Operands[0]);
            m.Assign(dst, src);
        }

        private void RewriteMov1()
        {
            var src = RewriteSrc(instr.Operands[1]);
            var dst = RewriteDst(instr.Operands[0], src, (a, b) => b);
        }

        private void RewriteMulu()
        {
            var src = RewriteSrc(instr.Operands[0]);
            var a = binder.EnsureRegister(Registers.a);
            var dst = binder.EnsureRegister(Registers.ax);
            m.Assign(dst, m.UMul(a, src));
        }

        private void RewriteNot1()
        {
            var src = RewriteSrc(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[0], src, (a, b) => m.Not(b));
        }

        private void RewriteOne(PrimitiveType dt)
        {
            m.Assign(RewriteSrc(instr.Operands[0]), Constant.Create(dt, 1));
        }

        private void RewritePop()
        {
            var sp = binder.EnsureRegister(Registers.sp);
            var dst = RewriteSrc(instr.Operands[0]);
            Expression val = m.Mem16(sp);
            if (dst.DataType.BitSize < 16)
            {
                val = m.Cast(dst.DataType, val);
            }
            m.Assign(dst, val);
            m.Assign(sp, m.IAddS(sp, 2));
        }

        private void RewritePush()
        {
            var sp = binder.EnsureRegister(Registers.sp);
            m.Assign(sp, m.ISubS(sp, 2));
            var src = RewriteSrc(instr.Operands[0]);
            if (src.DataType.BitSize < 16)
            {
                src = m.Cast(PrimitiveType.Word16, src);
            }
            m.Assign(m.Mem16(sp), src);
        }

        private void RewriteRet()
        {
            m.Return(4, 0);
        }

        private void RewriteRotate(string intrinsic)
        {
            var src = RewriteSrc(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[0], src, (a, b) =>
                host.PseudoProcedure(
                    intrinsic,
                    b.DataType,
                    b,
                    RewriteSrc(instr.Operands[1])));
            EmitCond(dst, C());
        }

        private void RewriteRotateC(string intrinsic)
        {
            var src = RewriteSrc(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[0], src, (a, b) =>
                host.PseudoProcedure(
                    intrinsic,
                    b.DataType,
                    b,
                    C(),
                    RewriteSrc(instr.Operands[1])));
            EmitCond(dst, C());
        }

        private void RewriteSel()
        {
            var bank = (RegisterBankOperand) instr.Operands[0];
            m.SideEffect(host.PseudoProcedure("__select_register_bank", VoidType.Instance, Constant.Byte((byte) bank.Bank)));
        }

        private void RewriteSet1()
        {
            RewriteDst(instr.Operands[0], Constant.Bool(true), (a, b) => b);
        }

        private void RewriteShift(Func<Expression, Expression, Expression> fn)
        {
            var src = RewriteSrc(instr.Operands[1]);
            var dst = RewriteDst(instr.Operands[0], src, fn);
            EmitCond(dst, C());
        }

        private void RewriteShiftw(Func<Expression,Expression,Expression> fn)
        {
            var src = RewriteSrc(instr.Operands[1]);
            var dst = RewriteDst(instr.Operands[0], src, fn);
            EmitCond(dst, C());
        }

        private void RewriteSkip(ConditionCode cc, Expression flagGroup)
        {
            var instrNext = dasm.Peek(1);
            var addrSkip = instrNext.Address + instrNext.Length;
            m.Branch(m.Test(cc, flagGroup), addrSkip, InstrClass.ConditionalTransfer);
        }

        private void RewriteSub()
        {
            var src = RewriteSrc(instr.Operands[1]);
            var dst = RewriteDst(instr.Operands[0], src, m.ISub);
            EmitCond(dst, CZ());
        }

        private void RewriteSubw()
        {
            var src = RewriteSrc(instr.Operands[1]);
            Func<Expression, Expression, Expression> fn;
            if (src.DataType.Size < instr.Operands[0].Width.Size)
            {
                if (src is Constant c)
                {
                    fn = (a, b) => m.ISubS(a, c.ToInt32());
                }
                else
                {
                    fn = (a, b) => m.ISub(a, m.Cast(PrimitiveType.Word16, b));
                }
            }
            else
            {
                fn = m.IAdd;
            }
            var dst = RewriteDst(instr.Operands[0], src, fn);
            EmitCond(dst, CZ());
        }

        private void RewriteXch()
        {
            var op1 = RewriteSrc(instr.Operands[0]);
            var op2 = RewriteSrc(instr.Operands[1]);
            var tmp = binder.CreateTemporary(op1.DataType);

            m.Assign(tmp, op1);
            m.Assign(op1, op2);
            m.Assign(op2, tmp);
        }
    }
}
