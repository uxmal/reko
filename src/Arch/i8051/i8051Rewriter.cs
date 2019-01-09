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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;

namespace Reko.Arch.i8051
{
    // http://www.keil.com/support/man/docs/is51/is51_instructions.htm
    public class i8051Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private i8051Architecture arch;
        private EndianImageReader rdr;
        private ProcessorState state;
        private IStorageBinder binder;
        private IRewriterHost host;
        private IEnumerator<i8051Instruction> dasm;
        private i8051Instruction instr;
        private RtlEmitter m;
        private InstrClass rtlc;

        public i8051Rewriter(i8051Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new i8051Disassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var rtls = new List<RtlInstruction>();
                this.m = new RtlEmitter(rtls);
                this.rtlc = InstrClass.Linear;
                switch (instr.Opcode)
                {
                default:
                    host.Warn(
                       instr.Address,
                       string.Format(
                           "i8051 instruction '{0}' not supported yet.",
                           instr.Opcode));
                    goto case Opcode.Invalid;
                case Opcode.Invalid:
                case Opcode.reserved:
                    Invalid();
                    break;
                case Opcode.acall: RewriteCall(); break;
                case Opcode.add: RewriteBinop(m.IAdd, FlagM.C | FlagM.AC | FlagM.OV | FlagM.P); break;
                case Opcode.addc: RewriteAddcSubb(m.IAdd); break;
                case Opcode.ajmp: RewriteJump(); break;
                case Opcode.anl: RewriteLogical(m.And); break;
                case Opcode.cjne: RewriteCjne(); break;
                case Opcode.clr: RewriteClr(); break;
                case Opcode.dec: RewriteIncDec(m.ISub); break;
                case Opcode.djnz: RewriteDjnz(); break;
                case Opcode.inc: RewriteIncDec(m.IAdd); break;
                case Opcode.jb: RewriteJb(m.Ne0); break;
                case Opcode.jbc: RewriteJbc(); break;
                case Opcode.jc: RewriteJc(e => e); break;
                case Opcode.jmp: RewriteJump(); break;
                case Opcode.jnb: RewriteJb(m.Eq0); break;
                case Opcode.jnc: RewriteJc(m.Not); break;
                case Opcode.jnz: RewriteJz(m.Ne0); break;
                case Opcode.jz: RewriteJz(m.Eq0); break;
                case Opcode.lcall: RewriteCall(); break;
                case Opcode.ljmp: RewriteJump(); break;
                case Opcode.mov: RewriteMov(); break;
                case Opcode.movc: RewriteMovc(); break;
                case Opcode.movx: RewriteMovx(); break;
                case Opcode.mul: RewriteMul(); break;
                case Opcode.nop: m.Nop(); break;
                case Opcode.orl: RewriteLogical(m.Or); break;
                case Opcode.pop: RewritePop(); break;
                case Opcode.push: RewritePush(); break;
                case Opcode.ret: RewriteRet(); break;
                case Opcode.reti: RewriteRet(); break;
                case Opcode.rl: RewriteRotate(PseudoProcedure.Rol); break;
                case Opcode.rr: RewriteRotate(PseudoProcedure.Ror); break;
                case Opcode.setb: RewriteSetb(); break;
                case Opcode.sjmp: RewriteJump(); break;
                case Opcode.subb: RewriteAddcSubb(m.ISub); break;
                case Opcode.swap: RewriteSwap(); break;
                case Opcode.xrl: RewriteLogical(m.Xor); break;
                case Opcode.xch: RewriteXch(); break;

                case Opcode.cpl:
                case Opcode.da:
                case Opcode.div:
                case Opcode.rrc:
                case Opcode.rlc:
                case Opcode.xchd:
                    EmitUnitTest();
                    Invalid();
                    break;
                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, rtls.ToArray())
                {
                    Class = rtlc
                };
            }
        }

     
        private void RewriteJump()
        {
            rtlc = InstrClass.Transfer;
            var dst = OpSrc(instr.Operand1);
            dst.DataType = PrimitiveType.Ptr16;
            m.Goto(dst);
        }

        private void RewriteBinop(Func<Expression, Expression, BinaryExpression> fn, FlagM grf)
        {
            var dst = OpSrc(instr.Operand1);
            var src = OpSrc(instr.Operand2);
            m.Assign(dst, fn(dst, src));
            if (grf != 0)
            {
                var flg = arch.GetFlagGroup((uint)grf);
                m.Assign(binder.EnsureFlagGroup(flg), m.Cond(dst));
            }
        }

        public void RewriteAddcSubb(Func<Expression, Expression, Expression> opr)
        {
            // We do not take the trouble of widening the CF to the word size
            // to simplify code analysis in later stages. 
            var c = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.C));
            var dst = OpSrc(instr.Operand1);
            var src = OpSrc(instr.Operand2);
            m.Assign(
                dst,
                opr(
                    opr(
                        dst,
                        src),
                    c));
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)(FlagM.C | FlagM.AC | FlagM.OV | FlagM.P)));
            m.Assign(grf, m.Cond(dst));
        }

        private void RewriteCall()
        {
            rtlc = InstrClass.Transfer | InstrClass.Call;
            var dst = OpSrc(instr.Operand1);
            dst.DataType = PrimitiveType.Ptr16;
            m.Call(dst, 2);
        }

        private void RewriteCjne()
        {
            rtlc = InstrClass.ConditionalTransfer;
            var a = OpSrc(instr.Operand1);
            var b = OpSrc(instr.Operand2);
            var addr = ((AddressOperand)instr.Operand3).Address;
            m.Branch(m.Ne(a, b), addr, rtlc);
        }

        private void RewriteClr()
        {
            var dst = OpSrc(instr.Operand1);
            WriteDst(instr.Operand1, Constant.Zero(dst.DataType));
            if (dst is Identifier id && id.Storage is RegisterStorage)
            {
                var flg = arch.GetFlagGroup((uint)FlagM.P);
                m.Assign(binder.EnsureFlagGroup(flg), m.Cond(dst));
            }
        }

        private void RewriteDjnz()
        {
            rtlc = InstrClass.ConditionalTransfer;
            var dst = OpSrc(instr.Operand1);
            var addr = ((AddressOperand)instr.Operand2).Address;
            m.Assign(dst, m.ISub(dst, 1));
            m.Branch(m.Ne0(dst), addr, InstrClass.ConditionalTransfer);
        }

        private void RewriteIncDec(Func<Expression, Expression, Expression> fn)
        {
            var dst = OpSrc(instr.Operand1);
            m.Assign(dst, fn(dst, m.Const(dst.DataType, 1)));
            if (dst is Identifier id && id.Storage is RegisterStorage)
            {
                var flg = arch.GetFlagGroup((uint)FlagM.P);
                m.Assign(binder.EnsureFlagGroup(flg), m.Cond(dst));
            }
        }

        private void RewriteJb(Func<Expression, Expression> cmp)
        {
            rtlc = InstrClass.ConditionalTransfer;
            var src = OpSrc(instr.Operand1);
            var addr = ((AddressOperand)instr.Operand2).Address;
            m.Branch(cmp(src), addr, InstrClass.ConditionalTransfer);
        }

        private void RewriteJc(Func<Expression, Expression> cmp)
        {
            rtlc = InstrClass.ConditionalTransfer;
            var src = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.C));
            var addr = ((AddressOperand)instr.Operand1).Address;
            m.Branch(cmp(src), addr, InstrClass.ConditionalTransfer);
        }

        private void RewriteJbc()
        {
            rtlc = InstrClass.ConditionalTransfer;
            var src = OpSrc(instr.Operand1);
            m.BranchInMiddleOfInstruction(m.Eq0(src), instr.Address + instr.Length, rtlc);
            WriteDst(instr.Operand1, Constant.Zero(src.DataType));
            m.Goto(OpSrc(instr.Operand2));
        }

        private void RewriteJz(Func<Expression, Expression> cmp)
        {
            rtlc = InstrClass.ConditionalTransfer;
            var src = binder.EnsureRegister(Registers.A);
            var addr = ((AddressOperand)instr.Operand1).Address;
            m.Branch(cmp(src), addr, InstrClass.ConditionalTransfer);
        }

        private void RewriteLogical(Func<Expression, Expression, Expression> fn)
        {
            var dst = OpSrc(instr.Operand1);
            var src = OpSrc(instr.Operand2);
            m.Assign(dst, fn(dst, src));
            if (dst is Identifier id && id.Storage is RegisterStorage)
            {
                var flg = arch.GetFlagGroup((uint)FlagM.P);
                m.Assign(binder.EnsureFlagGroup(flg), m.Cond(dst));
            }
        }

        private void RewriteMov()
        {
            var dst = OpSrc(instr.Operand1);
            var src = OpSrc(instr.Operand2);
            m.Assign(dst, src);
        }

        private void RewriteMovc()
        {
            var dst = binder.EnsureRegister(Registers.A);
            var src = OpSrc(instr.Operand1);
            m.Assign(dst, src);
        }

        private void RewriteMovx()
        {
            var dst = OpSrc(instr.Operand1);
            var src = OpSrc(instr.Operand2);
            m.Assign(dst, src);
        }

        private void RewriteMul()
        {
            var a = binder.EnsureRegister(Registers.A);
            var b = binder.EnsureRegister(Registers.B);
            var ab = binder.EnsureSequence(Registers.B, Registers.A, PrimitiveType.Word16);
            m.Assign(ab, m.UMul(a, b));
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.P)), m.Cond(ab));
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.OV)), m.Ugt(ab, m.Word16(0xFF)));
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.C)), Constant.False());
        }

        private void RewritePop()
        {
            var dst = OpSrc(instr.Operand1);
            var sp = binder.EnsureRegister(Registers.SP);
            m.Assign(dst, m.Mem(dst.DataType, sp));
            m.Assign(sp, m.ISub(sp, 1));
        }

        private void RewritePush() {
            var src = OpSrc(instr.Operand1);
            var sp = binder.EnsureRegister(Registers.SP);
            m.Assign(sp, m.IAdd(sp, 1));
            m.Assign(m.Mem8(sp), src);
        }

        private void RewriteRotate(string rot)
        {
            var dst = OpSrc(instr.Operand1);
            m.Assign(dst, host.PseudoProcedure(rot, dst.DataType, dst, m.Byte(1)));
        }

        private void RewriteRet()
        {
            rtlc = InstrClass.Transfer;
            m.Return(2, 0);
        }

        private void RewriteSetb()
        {
            WriteDst(instr.Operand1, Constant.True());
        }

        private void RewriteSwap()
        {
            var tmp = binder.CreateTemporary(Registers.A.DataType);
            var a = binder.EnsureRegister(Registers.A);
            m.Assign(tmp, m.Shl(a, 4));
            m.Assign(a, m.Shr(a, 4));
            m.Assign(a, m.Or(a, tmp));
        }

        private void RewriteXch()
        {
            var tmp = binder.CreateTemporary(Registers.A.DataType);
            var a = OpSrc(instr.Operand1);
            var b = OpSrc(instr.Operand2);
            m.Assign(tmp, a);
            m.Assign(a, b);
            m.Assign(b, tmp);
        }

        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            //if (seen.Contains(dasm.Current.Opcode))
            //    return;
            //seen.Add(dasm.Current.Opcode);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var bytes = r2.ReadBytes(dasm.Current.Length);
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void I8051_rw_" + dasm.Current.Opcode + "()");
            Debug.WriteLine("        {");
            Debug.Write("            BuildTest(");
            Debug.Write(string.Join(
                ", ",
                bytes.Select(b => string.Format("0x{0:X2}", (int)b))));
            Debug.WriteLine(");\t// " + dasm.Current.ToString());
            Debug.WriteLine("            AssertCode(");
            Debug.WriteLine("                \"0|L--|00100000(2): 1 instructions\",");
            Debug.WriteLine("                \"1|L--|@@@\");");
            Debug.WriteLine("        }");
            Debug.WriteLine("");
        }

        private void Invalid()
        {
            m.Invalid();
            rtlc = InstrClass.Invalid;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Expression OpSrc(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand reg:
                return binder.EnsureRegister(reg.Register);
            case FlagGroupOperand flg:
                return binder.EnsureFlagGroup(flg.FlagGroup);
            case ImmediateOperand imm:
                return imm.Value;
            case AddressOperand addr:
                return addr.Address;
            case MemoryOperand mem:
                Expression ea;
                if (mem.DirectAddress != null)
                {
                    if (mem.Index != null)
                    {
                        ea = m.IAdd(mem.DirectAddress, binder.EnsureRegister(mem.Index));
                    }
                    else
                    {
                        ea = mem.DirectAddress;
                    }
                }
                else if (mem.Register != null)
                {
                    if (mem.Index != null)
                    {
                        var idx = binder.EnsureRegister(mem.Index);
                        if (mem.Register == Registers.PC)
                        {
                            ea = m.IAdd(
                                instr.Address + instr.Length, idx);
                        }
                        else
                        {
                            ea = binder.EnsureIdentifier(mem.Register);
                            ea = m.IAdd(ea, idx);
                        }
                    }
                    else
                    {
                        ea = binder.EnsureIdentifier(mem.Register);
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
                return m.Mem(mem.Width, ea);
            case BitOperand bit:
                Expression e = binder.EnsureRegister(bit.Register);
                if (bit.Bit > 0)
                {
                    e = m.Shr(e, (byte)bit.Bit);
                }
                e = m.And(e, 1);
                if (bit.Negated)
                {
                    e = m.Not(e);
                }
                return e;
            case SequenceOperand seq:
                return binder.EnsureSequence(seq.Sequence.Name, seq.Sequence.Head, seq.Sequence.Tail, seq.Sequence.DataType);
            default:
                throw new NotImplementedException($"Not implemented {op.GetType().Name}.");
            }
        }


        private void WriteDst(MachineOperand op, Expression src)
        {
            switch (op)
            {
            case RegisterOperand reg:
                m.Assign(binder.EnsureRegister(reg.Register), src);
                break;
            case FlagGroupOperand flg:
                m.Assign(binder.EnsureFlagGroup(flg.FlagGroup), src);
                break;
            case MemoryOperand mem:
                Expression ea;
                if (mem.DirectAddress != null)
                {
                    if (mem.Index != null)
                    {
                        ea = m.IAdd(mem.DirectAddress, binder.EnsureRegister(mem.Index));
                    }
                    else
                    {
                        ea = mem.DirectAddress;
                    }
                }
                else if (mem.Register != null)
                {
                    if (mem.Index != null)
                    {
                        var idx = binder.EnsureRegister(mem.Index);
                        if (mem.Register == Registers.PC)
                        {
                            ea = m.IAdd(
                                instr.Address + instr.Length, idx);
                        }
                        else
                        {
                            ea = binder.EnsureIdentifier(mem.Register);
                            ea = m.IAdd(ea, idx);
                        }
                    }
                    else
                    {
                        ea = binder.EnsureIdentifier(mem.Register);
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
                m.Assign(m.Mem(mem.Width, ea), src);
                break;
            case BitOperand bit:
                if (bit.Bit > 0)
                {
                    src = m.Shl(src, (byte)bit.Bit);
                }
                var mask = (byte)~(1 << bit.Bit);
                var e = binder.EnsureRegister(bit.Register);
                m.Assign(e, m.Or(m.And(e, mask), src));
                break;
            default:
                throw new NotImplementedException($"Not implemented {op.GetType().Name}.");
            }
        }

    }
}
