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

namespace Reko.Arch.SuperH
{
    public class SuperHRewriter : IEnumerable<RtlInstructionCluster>
    {
        private SuperHArchitecture arch;
        private IStorageBinder binder;
        private IRewriterHost host;
        private SuperHState state;
        private EndianImageReader rdr;
        private IEnumerator<SuperHInstruction> dasm;
        private SuperHInstruction instr;
        private RtlEmitter m;
        private RtlInstructionCluster rtlc;

        public SuperHRewriter(SuperHArchitecture arch, EndianImageReader rdr, SuperHState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new SuperHDisassembler(rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.rtlc = new RtlInstructionCluster(instr.Address, instr.Length);
                this.m = new RtlEmitter(rtlc.Instructions);
                switch (instr.Opcode)
                {
                case Opcode.invalid:
                default:
                    Invalid();
                    break;
                case Opcode.add: RewriteBinOp(m.IAdd, n => (sbyte)n); break;
                }
                yield return rtlc;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Invalid()
        {
            EmitUnitTest();
            host.Error(
                dasm.Current.Address,
                string.Format(
                    "Rewriting of SuperH instruction {0} not implemented yet.",
                dasm.Current.Opcode));

            rtlc = new RtlInstructionCluster(this.rtlc.Address, this.rtlc.Length);
            rtlc.Instructions.Add(new RtlInvalid());
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

        private Expression SrcOp(MachineOperand op, Func<int, int> immediateFn)
        {
            var immOp = op as ImmediateOperand;
            if (immOp != null)
            {
                return Constant.Word32(immediateFn(immOp.Value.ToInt32()));
            }
            throw new NotImplementedException();
        }

        private Expression DstOp(MachineOperand op, Expression src, Func<Expression, Expression, Expression> fn)
        {
            var regOp = op as RegisterOperand;
            if (regOp != null)
            {
                var id = binder.EnsureRegister(regOp.Register);
                m.Assign(id, fn(id, src));
                return id;
            }
            throw new NotImplementedException();
        }

        private void RewriteBinOp(
            Func<Expression, Expression, Expression> fn,
            Func<int, int> immediateFn)
        {
            rtlc.Class = RtlClass.Linear;
            var src = SrcOp(instr.op1, immediateFn);
            var dst = DstOp(instr.op2, src, fn);
        }

     
    }
}
