#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Arch.CompactRisc
{
    internal class Cr16Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly Cr16Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<Cr16Instruction> dasm;
        private readonly List<RtlInstruction> rtls;
        private readonly RtlEmitter m;
        private Cr16Instruction instr;
        private InstrClass iclass;

        public Cr16Rewriter(Cr16Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new Cr16cDisassembler(arch, rdr).GetEnumerator();
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
                    host.Warn(instr.Address, "Cr16 instruction {0} not supported yet.", this.instr);
                    goto case Mnemonic.Invalid;
                case Mnemonic.Invalid: m.Invalid(); iclass = InstrClass.Invalid; break;
                case Mnemonic.movb: RewriteMovsx(PrimitiveType.SByte); break;
                case Mnemonic.movd: RewriteMovd(); break;
                case Mnemonic.push: RewritePush(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, this.iclass);
                rtls.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("Cr16Rw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private Expression Operand(int iop)
        {
            var op = instr.Operands[iop];
            switch (op)
            {
            case RegisterStorage reg:
                return binder.EnsureRegister(reg);
            case ImmediateOperand imm:
                return imm.Value;
            default:
                throw new AddressCorrelatedException(instr.Address, $"Unimplemented address mode {op.GetType().Name}.");
            }
        }

        private Expression MaybeSlice(int iop, PrimitiveType dt)
        {
            var exp = Operand(iop);
            if (exp.DataType.BitSize > dt.BitSize)
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, m.Slice(exp, dt));
                return tmp;
            }
            return exp;
        }

        private void RewriteMovd()
        {
            var src = Operand(0);
            var dst = Operand(1);
            m.Assign(dst, src);
        }

        private void RewriteMovsx(PrimitiveType dtSrc)
        {
            var dtDst = PrimitiveType.Create(Domain.SignedInt, instr.Operands[1].Width.BitSize);
            var src = MaybeSlice(0, dtSrc);
            var dst = Operand(1);
            m.Assign(dst, m.Convert(src, dtSrc, dtDst));
        }

        public void RewritePush()
        {
            //$TODO: operand 0 is a count
            var reg = Operand(1);
            var sp = binder.EnsureRegister(arch.StackRegister);
            m.Assign(sp, m.ISubS(sp, reg.DataType.Size));
            m.Assign(m.Mem(reg.DataType, sp), reg);
        }
    }
}