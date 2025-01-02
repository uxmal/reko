#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;

namespace Reko.Arch.XCore
{
    public class XCore200Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly XCore200Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<XCoreInstruction> dasm;
        private readonly List<RtlInstruction> rtls;
        private readonly RtlEmitter m;
        private XCoreInstruction instr;
        private InstrClass iclass;

        public XCore200Rewriter(XCore200Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new XCore200Disassembler(arch, rdr).GetEnumerator();
            this.rtls = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtls);
            this.instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    break;
                case Mnemonic.Invalid: m.Invalid(); break;
                case Mnemonic.add: RewriteBinOp(m.IAdd); break;
                case Mnemonic.addi: RewriteBinOp(m.IAdd); break;
                case Mnemonic.and: RewriteBinOp(m.And); break;
                case Mnemonic.andnot: RewriteAndNot(); break;
                case Mnemonic.bau: RewriteBau(); break;
                case Mnemonic.bla: RewriteBla(); break;
                case Mnemonic.eq: RewriteBinOp(m.Eq); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                rtls.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EmitUnitTest()
        {
            m.Invalid();
            iclass = InstrClass.Invalid;
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("XCore200Rw", instr, instr.Mnemonic.ToString(), rdr, "");
            //var r2 = rdr.Clone();
            //r2.Offset -= instr.Length;
            //var hexString = string.Join("",
            //    r2.ReadBytes(instr.Length)
            //    .Select(b => string.Format("{0:X2}", b)));

            //var sb = new StringBuilder();
            //sb.AppendLine($"        [Test]");
            //sb.AppendLine($"        public void XCore200Rw_{instr.Mnemonic}()");
            //sb.AppendLine("        {");

            //sb.AppendLine($"            Given_HexString(\"{hexString}\"); // {instr}");
            //sb.AppendLine("            AssertCode(");
            //sb.AppendLine($"                \"0|L--|00100000({instr.Length}): 1 instructions\",");
            //sb.AppendLine($"                \"1|L--|@@@\");");
            //sb.AppendLine("        }");
            //sb.AppendLine();
            //Console.Write(sb);
            //Debug.WriteLine(sb);
        }

        private Expression Operand(MachineOperand op)
        {
            switch (op)
            {
            case RegisterStorage rop:
                return binder.EnsureRegister(rop);
            case ImmediateOperand imm:
                return imm.Value;
            default:
                throw new NotImplementedException($"'{op.GetType().Name} not implemented.");
            }
        }

        private void RewriteAndNot()
        {
            var src1 = Operand(instr.Operands[1]);
            var dst = Operand(instr.Operands[0]);
            m.Assign(dst, m.And(dst, m.Comp(src1)));
        }

        private void RewriteBau()
        {
            var dst = Operand(instr.Operands[0]);
            m.Goto(dst);
        }

        private void RewriteBinOp(Func<Expression, Expression, BinaryExpression> fn)
        {
            var src1 = Operand(instr.Operands[1]);
            var src2 = Operand(instr.Operands[2]);
            var dst = Operand(instr.Operands[0]);
            m.Assign(dst, fn(src1, src2));
        }

        private void RewriteBla()
        {
            var dst = Operand(instr.Operands[0]);
            m.Call(dst, 0);
        }
    }
}