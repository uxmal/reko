#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
                    EmitUnitTest();
                    Invalid();
                    break;
                case Opcode.add: RewriteBinop(m.IAdd, "V-----NZC"); break;
                case Opcode.and: RewriteBinop(m.And,  "0-----NZC"); break;
                }
                var rtlc = new RtlInstructionCluster(instr.Address, instr.Length, instrs.ToArray())
                {
                    Class = this.rtlc,
                };
                yield return rtlc;
            }
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
                    m.Assign(tmp, m.Load(op.Width, ea));
                    m.Assign(ea, m.IAdd(ea, m.Int16((short)op.Width.Size)));
                    return tmp;
                } else if (mop.Offset != 0)
                {
                    var tmp = binder.CreateTemporary(op.Width);
                    m.Assign(tmp, m.Load(op.Width, m.IAdd(ea, m.Int16(mop.Offset))));
                    return tmp;
                }
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
                m.Assign(tmp, m.Load(tmp.DataType, ea));
                m.Assign(tmp, fn(tmp, src));
                m.Assign(m.Load(tmp.DataType, ea.CloneExpression()), tmp);
                return tmp;
            }
            throw new NotImplementedException(op.ToString());
        }

        private void EmitCc(Expression exp, string vnzc)
        {

            // SZIH XVNC
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

        private void RewriteBinop(Func<Expression,Expression,Expression> fn, string vnzc)
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDst(instr.op2, src, fn);
            EmitCc(dst, vnzc);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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