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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Pascal;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Arch.Avr.Avr32
{
    public class Avr32Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private static FlagGroupStorage VNZC;

        private readonly Avr32Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<Avr32Instruction> dasm;
        private Avr32Instruction instr;
        private InstrClass iclass;
        private RtlEmitter m;


        public Avr32Rewriter(Avr32Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new Avr32Disassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = this.instr.InstructionClass;
                var instrs = new List<RtlInstruction>();
                this.m = new RtlEmitter(instrs);
                switch (this.instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.invalid:
                    m.Invalid();
                    break;
                case Mnemonic.cp_w: RewriteCp_w(); break;
                case Mnemonic.ld_w: RewriteLd(PrimitiveType.Word32); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.pushm: RewritePushm(); break;
                case Mnemonic.sub: RewriteSub(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
            }
        }

#if DEBUG
        private static readonly HashSet<Mnemonic> seen = new HashSet<Mnemonic>();

        private void EmitUnitTest()
        {
            if (seen.Contains(dasm.Current.Mnemonic))
                return;
            seen.Add(dasm.Current.Mnemonic);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var hexInstr = string.Join("", r2.ReadBytes(dasm.Current.Length)
                .Select(b => $"{b:X2}"));
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void Avr32Rw_" + dasm.Current.Mnemonic + "()");
            Debug.WriteLine("        {");
            Debug.Write("            Given_Instruction(");
            Debug.Write($"\"{hexInstr}\"");
            Debug.WriteLine(");\t// " + dasm.Current.ToString());
            Debug.WriteLine("            AssertCode(");
            Debug.WriteLine($"                \"0|L--|{dasm.Current.Address}({dasm.Current.Length}): 1 instructions\",");
            Debug.WriteLine("                \"1|L--|@@@\");");
            Debug.WriteLine("        }");
            Debug.WriteLine("");
        }
#else
        [Conditional("DEBUG")]
        public void EmitUnitTest()
        {
        }
#endif

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Push(RegisterStorage reg)
        {
            var sp = binder.EnsureRegister(Registers.sp);
            m.Assign(sp, m.ISubS(sp, reg.DataType.Size));
            m.Assign(m.Mem(reg.DataType, sp), binder.EnsureRegister(reg));
        }

        private Expression RewriteMemoryOperand(MemoryOperand mem)
        {
            var baseReg = binder.EnsureRegister(mem.Base);
            if (mem.PostIncrement)
            {
                var tmp = binder.CreateTemporary(mem.Width);
                m.Assign(tmp, m.Mem(mem.Width, baseReg));
                m.Assign(baseReg, m.IAddS(baseReg, mem.Width.Size));
                return tmp;
            }
            throw new NotImplementedException($"NYI: {mem}");
        }

        private Expression RewriteOp(int iOp)
        {
            switch (instr.Operands[iOp])
            {
            case RegisterOperand reg:
                if (reg.Register == Registers.pc)
                    return instr.Address;
                else
                    return binder.EnsureRegister(reg.Register);
            case ImmediateOperand imm:
                return imm.Value;
            case MemoryOperand mem:
                return RewriteMemoryOperand(mem);
            }
            throw new NotImplementedException($"AVR32 operand type {instr.Operands[iOp].GetType()} not implemented yet.");
        }

        private Expression RewriteOpDst(int iOp)
        {
            switch (instr.Operands[iOp])
            {
            case RegisterOperand reg:
                if (reg.Register == Registers.pc)
                    return instr.Address;
                else
                    return binder.EnsureRegister(reg.Register);
            }
            throw new NotImplementedException($"AVR32 operand type {instr.Operands[iOp].GetType()} not implemented yet.");
        }

        private void RewriteCp_w()
        {
            var op1 = RewriteOp(0);
            var op2 = RewriteOp(1);
            var grf = binder.EnsureFlagGroup(VNZC);
            m.Assign(grf, m.Cond(m.ISub(op1, op2)));
        }

        private void RewriteLd(PrimitiveType dtCast)
        {
            var src = RewriteOp(1);
            var dst = RewriteOpDst(0);
            if (src.DataType.BitSize < dst.DataType.BitSize)
            {
                src = m.Cast(dtCast, src);
            }
            m.Assign(dst, src);
        }

        private void RewriteMov()
        {
            var src = RewriteOp(1);
            var dst = RewriteOpDst(0);
            m.Assign(dst, src);
        }

        private void RewritePushm()
        {
            foreach (var op in instr.Operands)
            {
                switch (op)
                {
                case RegisterOperand reg:
                    Push(reg.Register);
                    break;
                case RegisterRange range:
                    for (int i = 0; i < range.Count; ++i)
                    {
                        Push(range.Registers[range.RegisterIndex + i]);
                    }
                    break;
                default:
                    throw new InvalidOperationException($"Invalid operand type {op.GetType().Name}.");
                }
            }
        }

        private void RewriteSub()
        {
            Expression src;
            if (instr.Operands.Length == 3)
            {
                var left = RewriteOp(1);
                var right = RewriteOp(2);
                if (right is Constant c)
                {
                    if (c.IsZero)
                    {
                        src = left;
                    }
                    else
                    {
                        var value = c.ToInt32();
                        if (value < 0)
                        {
                            src = m.IAdd(left, m.Word32(-value));
                        }
                        else
                        {
                            src = m.ISub(left, c);
                        }
                    }
                }
                else
                {
                    src = m.ISub(left, right);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
            var dst = RewriteOpDst(0);
            m.Assign(dst, src);
        }

        static Avr32Rewriter()
        {
            VNZC = new FlagGroupStorage(Registers.sr, (uint)(FlagM.VF | FlagM.NF | FlagM.CF), "VNZC", PrimitiveType.Byte);
        }
    }
}