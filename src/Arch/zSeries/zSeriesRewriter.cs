#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Types;

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
        private readonly ExpressionValueComparer cmp;
        private zSeriesInstruction instr;
        private RtlEmitter m;
        private InstrClass rtlc;

        public zSeriesRewriter(zSeriesArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new zSeriesDisassembler(arch, rdr).GetEnumerator();
            this.cmp = new ExpressionValueComparer();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.rtlc = InstrClass.Linear;
                var instrs = new List<RtlInstruction>();
                this.m = new RtlEmitter(instrs);
                switch (instr.Opcode)
                {
                default:
                    EmitUnitTest();
                    goto case Opcode.invalid;
                case Opcode.invalid:
                    rtlc = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Opcode.aghi: RewriteAhi(PrimitiveType.Word64); break;
                case Opcode.ahi: RewriteAhi(PrimitiveType.Word32); break;
                case Opcode.agr: RewriteAgr(); break;
                case Opcode.ar: RewriteAr(); break;
                case Opcode.basr: RewriteBasr(); break;
                case Opcode.ber: RewriteBranch(ConditionCode.EQ); break;
                case Opcode.bler: RewriteBranch(ConditionCode.LE); break;
                case Opcode.br: RewriteBr(); break;
                case Opcode.brasl: RewriteBrasl(); break;
                case Opcode.brctg: RewriteBrctg(); break;
                case Opcode.chi: RewriteChi(); break;
                case Opcode.clc: RewriteClc(); break;
                case Opcode.clg: RewriteClg(); break;
                case Opcode.cli: RewriteCli(); break;
                case Opcode.j: RewriteJ(); break;
                case Opcode.je: RewriteJcc(ConditionCode.EQ); break;
                case Opcode.jg: RewriteJcc(ConditionCode.GT); break;
                case Opcode.jh: RewriteJcc(ConditionCode.UGT); break;
                case Opcode.jne: RewriteJcc(ConditionCode.NE); break;
                case Opcode.la: RewriteLa(); break;
                case Opcode.larl: RewriteLarl(); break;
                case Opcode.l: RewriteL(PrimitiveType.Word32); break;
                case Opcode.lg: RewriteL(PrimitiveType.Word64); break;
                case Opcode.lgf: RewriteLgf(); break;
                case Opcode.lgfr: RewriteLgfr(); break;
                case Opcode.lghi: RewriteLghi(); break;
                case Opcode.lgr: RewriteLgr(); break;
                case Opcode.lhi: RewriteLhi(); break;
                case Opcode.lmg: RewriteLmg(); break;
                case Opcode.lr: RewriteLr(); break;
                case Opcode.ltgr: RewriteLtgr(); break;
                case Opcode.mvi: RewriteMvi(); break;
                case Opcode.mvz: RewriteMvz(); break;
                case Opcode.nc: RewriteNc(); break;
                case Opcode.ngr: RewriteNgr(); break;
                case Opcode.nopr: m.Nop(); break;
                case Opcode.sgr: RewriteSgr(); break;
                case Opcode.srag: RewriteSrag(); break;
                case Opcode.srlg: RewriteSrlg(); break;
                case Opcode.st: RewriteSt(PrimitiveType.Word32); break;
                case Opcode.stg: RewriteSt(PrimitiveType.Word64); break;
                case Opcode.stmg: RewriteStmg(); break;
                case Opcode.xc: RewriteXc(); break;
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

        private Constant Const(MachineOperand op)
        {
            return ((ImmediateOperand)op).Value;
        }

        private Expression EffectiveAddress(MachineOperand op)
        {
            if (op is AddressOperand aOp)
                return aOp.Address;
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