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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;

namespace Reko.Arch.Mips
{
    /* MIPS32® Architecture for Programmers
Volume IV-a: The MIPS16e™ ApplicationSpecific Extension to the MIPS32®
Architecture */
    public class Mips16eRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly MipsProcessorArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly IEnumerator<MipsInstruction> dasm;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private InstrClass iclass;
        private RtlEmitter m;

        public Mips16eRewriter(
            MipsProcessorArchitecture arch,
            EndianImageReader rdr,
            IEnumerable<MipsInstruction> instrs,
            IStorageBinder binder,
            IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.dasm = instrs.GetEnumerator();
            this.binder = binder;
            this.host = host;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                var instr = dasm.Current;
                var rtlInstructions = new List<RtlInstruction>();
                this.iclass = instr.InstructionClass;
                this.m = new RtlEmitter(rtlInstructions);
                switch (instr.Mnemonic)
                {
                default:
                    host.Error(
                        instr.Address,
                        string.Format("MIPS instruction '{0}' is not supported yet.", instr));
                    EmitUnitTest();
                    goto case Mnemonic.illegal;
                case Mnemonic.illegal:
                    iclass = InstrClass.Invalid; m.Invalid(); break;
                case Mnemonic.addi: RewriteBinary(instr, m.IAdd); break;
                case Mnemonic.addiu: RewriteBinary(instr, m.IAdd); break;
                case Mnemonic.addu: RewriteBinary(instr, m.IAdd); break;
                case Mnemonic.b: RewriteB(instr); break;
                case Mnemonic.beqz: RewriteBeqz(instr); break;
                case Mnemonic.bnez: RewriteBnez(instr); break;
                case Mnemonic.cmpi: RewriteCmp(instr); break;
                case Mnemonic.jal: RewriteJal(instr); break;
                case Mnemonic.jalx: RewriteJalx(instr); break;
                case Mnemonic.lb: RewriteLoad(instr, PrimitiveType.SByte); break;
                case Mnemonic.lbu: RewriteLoad(instr, PrimitiveType.Byte); break;
                case Mnemonic.lh: RewriteLoad(instr, PrimitiveType.Int16); break;
                case Mnemonic.lhu: RewriteLoad(instr, PrimitiveType.UInt16); break;
                case Mnemonic.li: RewriteMove(instr); break;
                case Mnemonic.lw: RewriteLoad(instr, PrimitiveType.Word32); break;
                case Mnemonic.mfhi: RewriteMf(instr, arch.hi); break;
                case Mnemonic.mflo: RewriteMf(instr, arch.lo); break;
                case Mnemonic.move: RewriteMove(instr); break;
                case Mnemonic.neg: RewriteUnary(instr, m.Neg); break;
                case Mnemonic.save: RewriteSave(instr); break;
                case Mnemonic.sb: RewriteStore(instr); break;
                case Mnemonic.sh: RewriteStore(instr); break;
                case Mnemonic.sll: RewriteBinary(instr, m.Shl); break;
                case Mnemonic.sllv: RewriteBinary(instr, m.Shl); break;
                case Mnemonic.slt: RewriteScc(instr, m.Lt); break;
                case Mnemonic.slti: RewriteScc(instr, m.Lt); break;
                case Mnemonic.sltiu: RewriteScc(instr, m.Ult); break;
                case Mnemonic.sra: RewriteBinary(instr, m.Sar); break;
                case Mnemonic.srav: RewriteBinary(instr, m.Sar); break;
                case Mnemonic.srl: RewriteBinary(instr, m.Shr); break;
                case Mnemonic.srlv: RewriteBinary(instr, m.Shr); break;
                case Mnemonic.subu: RewriteBinary(instr, m.ISub); break;
                case Mnemonic.sw: RewriteStore(instr); break;
                case Mnemonic.xor: RewriteBinary(instr, m.Xor); break;
                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, rtlInstructions.ToArray())
                {
                    Class = iclass,
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

#if DEBUG
        private static readonly HashSet<Mnemonic> seen = new HashSet<Mnemonic>();

        protected void EmitUnitTest()
        {
            if (rdr == null || seen.Contains(dasm.Current.Mnemonic))
                return;
            seen.Add(dasm.Current.Mnemonic);

            var r2 = rdr.Clone();
            int cbInstr = dasm.Current.Length;
            r2.Offset -= cbInstr;
            var uInstr = cbInstr == 2 ? r2.ReadUInt16() : r2.ReadUInt32();
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void Mips16eRw_{0}()", dasm.Current.Mnemonic);
            Debug.WriteLine("        {");
            Debug.WriteLine("            AssertCode(0x{0:X8},   // {1}", uInstr, dasm.Current);
            Debug.WriteLine("                \"0|L--|00100000({0}): 1 instructions\",", cbInstr);
            Debug.WriteLine("                \"1|L--|@@@\");");
            Debug.WriteLine("        }");
            Debug.WriteLine("");
        }
#else
        private void EmitUnitTest() { }
#endif

        private Expression Rewrite(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand rop:
                return binder.EnsureRegister(rop.Register);
            case ImmediateOperand imm:
                return imm.Value;
            case IndirectOperand mem:
                Expression ea;
                if (mem.Base == arch.pc)
                {
                    ea = dasm.Current.Address + mem.Offset;
                }
                else
                {
                    ea = binder.EnsureRegister(mem.Base);
                    ea = m.AddSubSignedInt(ea, mem.Offset);
                }
                return m.Mem(op.Width, ea);
            }
            throw new NotImplementedException($"Mips16e operand {op.GetType()} not implemented yet.");
        }

        private void RewriteB(MipsInstruction instr)
        {
            var dst = ((AddressOperand) instr.Operands[0]).Address;
            m.Goto(dst);    // No jump delay slot
        }

        private void RewriteBeqz(MipsInstruction instr)
        {
            var reg = binder.EnsureRegister(((RegisterOperand) instr.Operands[0]).Register);
            var dst = ((AddressOperand) instr.Operands[1]).Address;
            m.Branch(m.Eq0(reg), dst);    // No jump delay slot
        }

        private void RewriteBnez(MipsInstruction instr)
        {
            var reg = binder.EnsureRegister(((RegisterOperand) instr.Operands[0]).Register);
            var dst = ((AddressOperand) instr.Operands[1]).Address;
            m.Branch(m.Ne0(reg), dst);    // No jump delay slot
        }

        private void RewriteBinary(MipsInstruction instr, Func<Expression, Expression, Expression> fn)
        {
            Expression dst, left, right;
            if (instr.Operands.Length == 2)
            {
                dst = Rewrite(instr.Operands[0]);
                left = dst;
                right = Rewrite(instr.Operands[1]);
            }
            else
            {
                dst = Rewrite(instr.Operands[0]);
                left = Rewrite(instr.Operands[1]);
                right = Rewrite(instr.Operands[2]);
            }
            m.Assign(dst, fn(left, right));
        }

        private void RewriteCmp(MipsInstruction instr)
        {
            var t = binder.EnsureRegister(arch.GeneralRegs[24]);
            var src1 = Rewrite(instr.Operands[0]);
            var src2 = Rewrite(instr.Operands[1]);
            m.Assign(t, m.Xor(src1, src2)); // [sic] -- see the manual!
        }

        private void RewriteJal(MipsInstruction instr)
        {
            //$TODO: if we want explicit representation of the continuation of call
            // use the line below
            //emitter.Assign( frame.EnsureRegister(Registers.ra), instr.Address + 8);
            var dst = ((AddressOperand) instr.Operands[0]).Address;
            m.CallD(dst, 0);
        }

        private void RewriteJalx(MipsInstruction instr)
        {
            //$TODO: if we want explicit representation of the continuation of call
            // use the line below
            //emitter.Assign( frame.EnsureRegister(Registers.ra), instr.Address + 8);
            var dst = ((AddressOperand) instr.Operands[0]).Address;
            m.CallXD(dst, 0, arch);
        }

        private void RewriteLoad(MipsInstruction instr, PrimitiveType dt)
        {
            var src = Rewrite(instr.Operands[1]);
            var dst = Rewrite(instr.Operands[0]);
            src.DataType = dt;
            if (dst.DataType.Size != src.DataType.Size)
            {
                // If the source is smaller than the destination register,
                // perform a sign/zero extension/conversion.
                src = m.Cast(arch.WordWidth, src);
            }
            m.Assign(dst, src);
        }

        private void RewriteMove(MipsInstruction instr)
        {
            var src = Rewrite(instr.Operands[1]);
            var dst = Rewrite(instr.Operands[0]);
            m.Assign(dst, src);
        }

        private void RewriteMf(MipsInstruction instr, RegisterStorage reg)
        {
            var opDst = Rewrite(instr.Operands[0]);
            m.Assign(opDst, binder.EnsureRegister(reg));
        }

        private static readonly int [] saveRegs = new [] { 31, 17, 16 };

        private void RewriteSave(MipsInstruction instr)
        {
            var sp = binder.EnsureRegister(arch.StackRegister);
            var mop = (MultiRegisterOperand) instr.Operands[0];
            int decrement = 0;

            foreach (var iReg in saveRegs)
            {
                var reg = arch.GeneralRegs[iReg];
                if (Bits.IsBitSet(mop.Bitmask, iReg))
                {
                    decrement -= reg.DataType.Size;
                    m.Assign(m.Mem(reg.DataType, m.AddSubSignedInt(sp, decrement)), binder.EnsureRegister(reg));
                }
            }
            if (decrement != 0)
            {
                m.Assign(sp, m.AddSubSignedInt(sp, decrement));
            }
        }

        private void RewriteScc(MipsInstruction instr, Func<Expression, Expression, Expression> fn)
        {
            var left = Rewrite(instr.Operands[0]);
            var right = Rewrite(instr.Operands[1]);
            var t = binder.EnsureRegister(arch.GeneralRegs[24]);
            m.Assign(t, fn(left, right));
        }

        private void RewriteStore(MipsInstruction instr)
        {
            var src = Rewrite(instr.Operands[0]);
            var dst = Rewrite(instr.Operands[1]);
            if (dst.DataType.Size < src.DataType.Size)
                src = m.Cast(dst.DataType, src);
            m.Assign(dst, src);
        }

        private void RewriteUnary(MipsInstruction instr, Func<Expression, Expression> fn)
        {
            var src = Rewrite(instr.Operands[1]);
            var dst = Rewrite(instr.Operands[0]);
            m.Assign(dst, fn(src));
        }
    }
}
