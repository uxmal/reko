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
                case Opcode.add: RewriteBinOp(m.IAdd, "***V0*"); break;
                case Opcode.call: RewriteCall(); break;
                case Opcode.djnz: RewriteDjnz(); break;
                case Opcode.inc: RewriteIncDec(m.IAdd, "****0-"); break;
                case Opcode.jp: RewriteJp(); break;
                case Opcode.ld: RewriteLd(); break;
                case Opcode.sub: RewriteBinOp(m.ISub, "***V1*"); break;
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
            var reg = op as RegisterOperand;
            if (reg != null)
            {
                return frame.EnsureRegister(reg.Register);
            }
            var addr = op as AddressOperand;
            if (addr != null)
            {
                return addr.Address;
            }
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
                    ea = frame.EnsureRegister(mem.Base);
                    if (mem.Increment < 0)
                    {
                        m.Assign(ea, m.ISub(ea, mem.Width.Size));
                    }
                    var load = m.Load(mem.Width, ea);
                    var tmp = frame.CreateTemporary(ea.DataType);
                    m.Assign(tmp, fn(load, src));
                    m.Assign(m.Load(mem.Width, ea), tmp);
                    if (mem.Increment > 0)
                    {
                        m.Assign(ea, m.IAdd(ea, mem.Width.Size));
                    }
                    return tmp;
                }
                else
                {
                    throw new NotImplementedException(op.ToString());
                }

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

        private Expression GenerateTestExpression(ConditionOperand cOp, bool invert)
        {
            ConditionCode cc = ConditionCode.ALWAYS;
            string flags = "";
            switch (cOp.Code)
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
                frame.EnsureFlagGroup(arch.GetFlagGroup(flags)));
        }
    }
}
