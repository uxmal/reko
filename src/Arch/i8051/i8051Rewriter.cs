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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;

namespace Reko.Arch.i8051
{
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
        private RtlClass rtlc;

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
                this.rtlc = RtlClass.Linear;
                switch (instr.Opcode)
                {
                case Opcode.Invalid:
                case Opcode.reserved:
                    Invalid();
                    break;
                case Opcode.acall:
                case Opcode.add:
                case Opcode.addc:
                case Opcode.ajmp:
                case Opcode.anl:
                case Opcode.cjne:
                case Opcode.clr:
                case Opcode.cpl:
                case Opcode.da:
                case Opcode.dec:
                case Opcode.div:
                case Opcode.djnz:
                case Opcode.inc:
                case Opcode.jb:
                case Opcode.jbc:
                case Opcode.jc:
                case Opcode.jmp:
                case Opcode.jnb:
                case Opcode.jnc:
                case Opcode.jnz:
                case Opcode.jz:
                case Opcode.lcall:
                case Opcode.ljmp:
                case Opcode.mov: RewriteMov(); break;
                case Opcode.movc:
                case Opcode.movx:
                case Opcode.mul:
                case Opcode.nop:
                case Opcode.orl:
                case Opcode.pop:
                case Opcode.push:
                case Opcode.ret:
                case Opcode.rl:
                case Opcode.rr:
                case Opcode.rrc:
                case Opcode.reti:
                case Opcode.rlc:
                case Opcode.sjmp:
                case Opcode.subb:
                case Opcode.setb:
                case Opcode.swap:
                case Opcode.xch:
                case Opcode.xchd:
                case Opcode.xrl:
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
            Debug.WriteLine("        public void SHRw_" + dasm.Current.Opcode + "()");
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
            rtlc = RtlClass.Invalid;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Expression Op(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand reg:
                return binder.EnsureRegister(reg.Register);
            case ImmediateOperand imm:
                return imm.Value;
            default:
                throw new NotImplementedException($"Not implemented {op.GetType().Name}.");
            }
        }

        private void RewriteMov()
        {
            var dst = Op(instr.Operand1);
            var src = Op(instr.Operand2);
            m.Assign(dst, src);
        }

     
    }
}
