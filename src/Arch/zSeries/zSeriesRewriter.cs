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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;

namespace Reko.Arch.zSeries
{
    public partial class zSeriesRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly zSeriesArchitecture arch;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly EndianImageReader rdr;
        private readonly IEnumerator<zSeriesInstruction> dasm;
        private zSeriesInstruction instr;
        private RtlEmitter m;
        private RtlClass rtlc;

        public zSeriesRewriter(zSeriesArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new zSeriesDisassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.rtlc = RtlClass.Linear;
                var instrs = new List<RtlInstruction>();
                this.m = new RtlEmitter(instrs);
                switch (instr.Opcode)
                {
                default:
                    EmitUnitTest();
                    goto case Opcode.invalid;
                case Opcode.invalid:
                    rtlc = RtlClass.Invalid;
                    m.Invalid();
                    break;
                case Opcode.br: RewriteBr(); break;
                case Opcode.larl: RewriteLarl(); break;
                case Opcode.stmg: RewriteStmg(); break;
                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, instrs.ToArray())
                {
                    Class = rtlc
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

#if DEBUG
        private static HashSet<Opcode> seen = new HashSet<Opcode>();

        private void EmitUnitTest()
        {
            if (rdr == null || seen.Contains(dasm.Current.Opcode))
                return;
            seen.Add(dasm.Current.Opcode);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var bytes = r2.ReadBytes(dasm.Current.Length);

            Debug.Print("        [Test]");
            Debug.Print("        public void zSeriesRw_{0}()", dasm.Current.Opcode);
            Debug.Print("        {");
            Debug.Print("            Given_MachineCode(\"{0}\");", string.Join("", bytes.Select(b => b.ToString("X2"))));
            Debug.Print("            AssertCode(     // {0}", dasm.Current);
            Debug.Print("                \"0|L--|00100000({0}): 1 instructions\",", dasm.Current.Length);
            Debug.Print("                \"1|L--|@@@\");");
            Debug.Print("        }");
            Debug.Print("");
        }
#else
        private void EmitUnitTest() { }
#endif

        private Address Addr(MachineOperand op)
        {
            return ((AddressOperand)op).Address;
        }

        private Expression EffectiveAddress(MachineOperand op)
        {
            var mem = (MemoryOperand)op;
            if (mem.Base == null || mem.Base.Number == 0)
            {
                // Must be abs address.
                return Address.Ptr32((uint)mem.Offset);
            }
            Expression ea = binder.EnsureRegister(mem.Base);
            if (mem.Index != null && mem.Index.Number > 0)
            {
                var idx = binder.EnsureRegister(mem.Index);
                ea = m.IAdd(ea, idx);
            }
            if (mem.Offset != 0)
            {
                var off = Constant.Int(mem.Base.DataType, mem.Offset);
                ea = m.IAdd(ea, off);
            }
            return ea;

        }
        private Identifier Reg(MachineOperand op)
        {
            return binder.EnsureRegister(((RegisterOperand)op).Register);
        }
    }
}