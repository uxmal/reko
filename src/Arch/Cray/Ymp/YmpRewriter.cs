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
using System.Diagnostics;
using System.Text;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;

namespace Reko.Arch.Cray.Ymp
{
    public class YmpRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly CrayYmpArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<CrayInstruction> dasm;
        private CrayInstruction instrCur;
        private InstrClass iclass;
        private RtlEmitter m;

        public YmpRewriter(CrayYmpArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new YmpDisassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instrCur = dasm.Current;
                this.iclass = instrCur.InstructionClass;
                var instrs = new List<RtlInstruction>();
                this.m = new RtlEmitter(instrs);
                switch (instrCur.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    this.iclass = InstrClass.Invalid;
                    goto case Mnemonic.Invalid;
                case Mnemonic.Invalid:
                    m.Invalid();
                    break;
                case Mnemonic.j: RewriteJ(); break;
                case Mnemonic._and: Rewrite3(m.And); break;
                case Mnemonic._mov: RewriteMov(); break;
                case Mnemonic._fmul: Rewrite3(m.FMul); break;
                }
                yield return new RtlInstructionCluster(instrCur.Address, instrCur.Length, instrs.ToArray())
                {
                    Class = this.iclass
                };
            }
        }



        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private static readonly HashSet<Mnemonic> seen = new HashSet<Mnemonic>();

        private void EmitUnitTest()
        {
            if (rdr == null || seen.Contains(dasm.Current.Mnemonic))
                return;
            seen.Add(dasm.Current.Mnemonic);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var sb = new StringBuilder();
            while (r2.Offset < rdr.Offset)
            {
                var parcel = r2.ReadBeUInt16();
                sb.Append(Convert.ToString(parcel, 8).PadLeft(6, '0'));
            }
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void YmpRw_{0}()", dasm.Current.Mnemonic);
            Debug.WriteLine("        {");
            Debug.WriteLine("            RewriteCode(\"{0}\");   // {1}", sb.ToString(), dasm.Current);
            Debug.WriteLine("            AssertCode(");
            Debug.WriteLine("                \"0|L--|00100000({0}): 1 instructions\",", dasm.Current.Length);
            Debug.WriteLine("                \"1|L--|@@@\");");
            Debug.WriteLine("        }");
            Debug.WriteLine("");
        }

        private Expression Rewrite(MachineOperand mop)
        {
            switch (mop)
            {
            case RegisterOperand rop:
                var reg = this.binder.EnsureRegister(rop.Register);
                return reg;
            default:
                throw new NotImplementedException($"Unimplemented Cray operand {mop.GetType().Name}.");
            }
        }

        private void Rewrite3(Func<Expression, Expression, Expression> fn)
        {
            var dst = Rewrite(instrCur.Operands[0]);
            var src1 = Rewrite(instrCur.Operands[1]);
            var src2 = Rewrite(instrCur.Operands[2]);
            m.Assign(dst, fn(src1, src2));
        }

        private void RewriteJ()
        {
            var dst = Rewrite(instrCur.Operands[0]);
            m.Goto(dst);
        }

        private void RewriteMov()
        {
            var dst = Rewrite(instrCur.Operands[0]);
            var src1 = Rewrite(instrCur.Operands[1]);
            if (instrCur.Operands.Length == 2)
            {
                m.Assign(dst, src1);
            }
            else
            {
                var src2 = Rewrite(instrCur.Operands[2]);
                m.Assign(dst, m.ARef(PrimitiveType.Word64, src1, src2));
            }
        }
    }
}
