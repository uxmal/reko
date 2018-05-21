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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Tms7000
{
    public class Tms7000Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private Tms7000Architecture arch;
        private EndianImageReader rdr;
        private IRewriterHost host;
        private IStorageBinder binder;
        private IEnumerator<Tms7000Instruction> dasm;
        private Tms7000Instruction instr;
        private RtlClass rtlc;
        private RtlEmitter m;

        public Tms7000Rewriter(Tms7000Architecture arch, EndianImageReader rdr, Tms7000State state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.host = host;
            this.binder = binder;
            this.dasm = new Tms7000Disassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.rtlc = RtlClass.Linear;
                var rtls = new List<RtlInstruction>();
                this.m = new RtlEmitter(rtls);
                switch (instr.Opcode)
                {
                default:
                    host.Error(instr.Address, "Rewriting x86 opcode '{0}' is not supported yet.", instr);
                    rtlc = RtlClass.Invalid;
                    break;
                case Opcode.add: RewriteArithmetic(m.IAdd); break;
                case Opcode.and: RewriteLogical(m.And); break;
                case Opcode.andp: RewriteLogical(m.And); break;
                case Opcode.btjo: RewriteBtj(a => a); break;
                case Opcode.btjop: RewriteBtj(a => a); break;
                case Opcode.btjz: RewriteBtj(m.Comp); break;
                case Opcode.btjzp: RewriteBtj(m.Comp); break;
                case Opcode.br: RewriteBr(); break;
                case Opcode.nop: m.Nop(); break;
                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, rtls.ToArray())
                {
                    Class = rtlc,
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        private Expression Operand(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand rop:
                return binder.EnsureRegister(rop.Register);
            case ImmediateOperand imm:
                return imm.Value;
            case MemoryOperand mem:
                Expression ea;
                if (mem.Address != null)
                {
                    if (mem.Register != null)
                    {
                        ea = m.IAdd(
                            mem.Address,
                            m.Cast(PrimitiveType.UInt16, binder.EnsureRegister(mem.Register)));
                    }
                    else
                    {
                        ea = mem.Address;
                    }
                }
                else
                {
                    ea = binder.EnsureSequence(
                        mem.Register,
                        arch.GpRegs[mem.Register.Number - 1],
                        PrimitiveType.Word32);
                }
                return m.Mem(mem.Width, ea);
            default:
                throw new NotImplementedException(op.GetType().Name);
            }
        }

        private void CNZ(Expression e)
        {
            var cnz = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.CNZ));
            m.Assign(cnz, m.Cond(e));
        }

        private void NZ0(Expression e)
        {
            var nz = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.NZ));
            m.Assign(nz, m.Cond(e));
            var c = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.CF));
            m.Assign(c, Constant.False());
        }

        private void RewriteArithmetic(Func<Expression, Expression, Expression> fn)
        {
            var src = Operand(instr.op1);
            var dst = Operand(instr.op2);
            m.Assign(dst, fn(dst, src));
            CNZ(dst);
        }

        private void RewriteBtj(Func<Expression, Expression> fn)
        {
            this.rtlc = RtlClass.ConditionalTransfer;
            var opLeft = Operand(instr.op2);
            var opRight = Operand(instr.op1);
            NZ0(m.And(opLeft, fn(opRight)));
            var z = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.ZF));
            m.Branch(
                m.Test(ConditionCode.NE, z),
                ((AddressOperand)instr.op3).Address,
                RtlClass.ConditionalTransfer);
        }

        private void RewriteBr()
        {
            rtlc = RtlClass.Transfer;
            var dst = Operand(instr.op1);
            var ea = ((MemoryAccess)dst).EffectiveAddress;
            m.Goto(ea);
        }

        private void RewriteLogical(Func<Expression, Expression, Expression> fn)
        {
            var src = Operand(instr.op1);
            var dst = Operand(instr.op2);

            m.Assign(dst, fn(dst, src));
            NZ0(dst);
        }
    }
}
