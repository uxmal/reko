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
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;

namespace Reko.Arch.Blackfin
{
    internal class BlackfinRewriter : IEnumerable<RtlInstructionCluster>
    {
        private static HashSet<Mnemonic> opcode_seen = new HashSet<Mnemonic>();

        private readonly BlackfinArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<BlackfinInstruction> dasm;
        private RtlEmitter m;
        private InstrClass iclass;
        private BlackfinInstruction instr;

        public BlackfinRewriter(BlackfinArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new BlackfinDisassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var rtls = new List<RtlInstruction>();
                this.m = new RtlEmitter(rtls);
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest(instr);
                    iclass = InstrClass.Invalid;
                    goto case Mnemonic.invalid;
                case Mnemonic.invalid:
                    this.iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.CLI: RewriteCli(); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.mov_x: RewriteMovx(); break;
                case Mnemonic.mov_xb: RewriteMovxb(); break;
                case Mnemonic.mov_zb: RewriteMovzb(); break;
                case Mnemonic.mul: RewriteMul(); break;
                case Mnemonic.JUMP_L: RewriteJump(); break;
                case Mnemonic.RTN: RewriteRtn(); break;
                case Mnemonic.RTS: RewriteRts(); break;
                case Mnemonic.sub3: RewriteBinop(m.ISub); break;
                case Mnemonic.xor3: RewriteBinop(m.Xor); break;
                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, rtls.ToArray())
                {
                    Class = iclass
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void EmitUnitTest(BlackfinInstruction instr)
        {
            if (opcode_seen.Contains(instr.Mnemonic))
                return;
            opcode_seen.Add(instr.Mnemonic);

            var r2 = rdr.Clone();
            r2.Offset -= instr.Length;

            Console.WriteLine("        [Test]");
            Console.WriteLine("        public void BlackfinRw_{0}()", instr.Mnemonic);
            Console.WriteLine("        {");

            if (instr.Length > 2)
            {
                var wInstr = r2.ReadBeUInt32();
                Console.WriteLine($"            RewriteCode(\"{wInstr:X8}\");  // {instr}");
            }
            else
            {
                var wInstr = r2.ReadBeUInt16();
                Console.WriteLine($"            RewriteCode(\"{wInstr:X4}\");  // {instr}");
            }
            Console.WriteLine("            AssertCode(");
            Console.WriteLine($"                \"0|L--|00100000({instr.Length}): 1 instructions\",");
            Console.WriteLine($"                \"1|L--|@@@\");");
            Console.WriteLine("        }");
        }

        private Address Addr(int iOperand)
        {
            return ((AddressOperand) instr.Operands[iOperand]).Address;
        }

        private Expression Operand(int iOperand)
        {
            switch (instr.Operands[iOperand])
            {
            case RegisterOperand rop:
                return binder.EnsureRegister(rop.Register);
            case ImmediateOperand imm:
                return imm.Value;
            default:
                throw new NotImplementedException($"Operand type {instr.Operands[iOperand].GetType().Name}.");
            }
        }

        private Identifier Reg(int iOperand)
        {
            return binder.EnsureRegister(((RegisterOperand) instr.Operands[iOperand]).Register);
        }

        private void RewriteCli()
        {
            m.SideEffect(host.PseudoProcedure("__cli", VoidType.Instance));
        }

        private void RewriteJump()
        {
            var addrDst = Addr(0);
            m.Goto(addrDst);
        }

        private void RewriteRts()
        {
            // A more accurate rewriter would assign PC = RETS
            m.Return(0, 0);
        }

        private void RewriteMov()
        {
            m.Assign(Reg(0), Operand(1));
        }

        private void RewriteMovx()
        {
            var src = Operand(1);
            m.Assign(Reg(0), m.Cast(PrimitiveType.Word32, src));
        }

        private void RewriteMovxb()
        {
            m.Assign(Reg(0), m.Cast(PrimitiveType.Int32, m.Slice(PrimitiveType.SByte, Reg(1), 0)));
        }

        private void RewriteMovzb()
        {
            m.Assign(Reg(0), m.Cast(PrimitiveType.Word32, m.Slice(PrimitiveType.Byte, Reg(1), 0)));
        }

        private void RewriteMul()
        {
            Debug.Assert(instr.Operands.Length == 2);
            var dst = Reg(0);
            var src = Reg(1);
            m.Assign(dst, m.IMul(dst, src));
        }

        private void RewriteRtn()
        {
            // A more accurate rewriter would assign PC = RETN
            m.Return(0, 0);
        }

        private void RewriteBinop(Func<Expression,Expression,Expression> binop)
        {
            var src1 = Reg(1);
            var src2 = Reg(2);
            var dst = Reg(0);
            m.Assign(dst, binop(src1, src2));
        }
    }
}