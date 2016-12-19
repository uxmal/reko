#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opcode = Reko.Arch.Tlcs.Tlcs900Opcode;

namespace Reko.Arch.Tlcs
{
    public partial class Tlcs900Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private Tlcs900Architecture arch;
        private ImageReader rdr;
        private ProcessorState state;
        private Frame frame;
        private IRewriterHost host;
        private IEnumerator<Tlcs900Instruction> dasm;
        private RtlInstructionCluster rtlc;
        private RtlEmitter m;
        private Tlcs900Instruction instr;

        public Tlcs900Rewriter(Tlcs900Architecture arch, ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.frame = frame;
            this.host = host;
            this.dasm = new Tlcs900Disassembler(this.arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                rtlc = new RtlInstructionCluster(dasm.Current.Address, dasm.Current.Length);
                rtlc.Class = RtlClass.Linear;
                m = new RtlEmitter(rtlc.Instructions);
                this.instr = dasm.Current;
                switch (instr.Opcode)
                {
                default:
                    throw new AddressCorrelatedException(
                       instr.Address,
                       "Rewriting of TLCS-900 instruction '{0}' not implemented yet.",
                       instr.Opcode);
                case Opcode.add: RewriteBinOp(m.IAdd); break;
                case Opcode.ld: RewriteLd(); break;
                }
                yield return rtlc;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Expression RewriteSrc(MachineOperand op)
        {
            var imm = op as ImmediateOperand;
            if (imm != null)
            {
                return imm.Value;
            }
            var mem = op as MemoryOperand;
            if (mem != null)
            {
                Expression ea;
                if (mem.Base != null)
                {
                    if (mem.Increment < 0)
                        throw new NotImplementedException("predec");
                    ea = frame.EnsureRegister(mem.Base);
                    if (mem.Offset != null)
                    {
                        ea = m.IAdd(ea, mem.Offset);
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
                var tmp = frame.CreateTemporary(mem.Width);
                m.Assign(tmp, m.Load(mem.Width, ea));
                return tmp;
            }

            throw new NotImplementedException(op.GetType().Name);
        }

        private Expression RewriteDst(MachineOperand op, Expression src, Func<Expression, Expression, Expression> fn)
        {
            var reg = op as RegisterOperand;
            if(reg != null)
            {
                var id = frame.EnsureIdentifier(reg.Register);
                m.Assign(id, fn(id, src));
                return id;
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
                case 'V':
                case 'P':
                    grf |= mask;
                    break;
                case '0':
                    m.Assign(
                        frame.EnsureFlagGroup(arch.GetFlagGroup(mask)),
                        Constant.False());
                    break;
                case '1':
                    m.Assign(
                        frame.EnsureFlagGroup(arch.GetFlagGroup(mask)),
                        Constant.True());
                    break;
                }
                mask >>= 1;
            }
            if (grf != 0)
            {
                m.Assign(
                    frame.EnsureFlagGroup(arch.GetFlagGroup(grf)),
                    m.Cond(exp));
            }
        }
    }
}
