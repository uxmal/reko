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

using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Types;
using System.Diagnostics;
using Reko.Core.Lib;

namespace Reko.Arch.SuperH
{
    // http://www.shared-ptr.com/sh_insns.html

    public class SuperHDisassembler : DisassemblerBase<SuperHInstruction>
    {
        private readonly EndianImageReader rdr;
        private readonly DasmState state;
        private Address addr;

        public SuperHDisassembler(EndianImageReader rdr)
        {
            this.rdr = rdr;
            this.state = new DasmState();
        }

        public override SuperHInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt16(out ushort uInstr))
                return null;
            var instr = decoders[uInstr >> 12].Decode(this, uInstr);
            state.Clear();
            instr.Address = addr;
            instr.Length = 2;
            return instr;
        }

        private SuperHInstruction Invalid()
        {
            return new SuperHInstruction
            {
                Opcode = Opcode.invalid
            };
        }

        public class DasmState
        {
            public List<MachineOperand> ops = new List<MachineOperand>();
            public Opcode opcode;
            public InstrClass iclass;
            public RegisterStorage reg;

            public void Clear()
            {
                ops.Clear();
            }

            internal SuperHInstruction MakeInstruction()
            {
                var instr = new SuperHInstruction
                {
                    Opcode = this.opcode,
                    InstructionClass = iclass,
                };
                if (ops.Count > 0)
                {
                    instr.op1 = ops[0];
                    if (ops.Count > 1)
                    {
                        instr.op2 = ops[1];
                        if (ops.Count > 2)
                        {
                            instr.op3 = ops[2];
                        }
                    }
                }
                return instr;
            }
        }

        private SuperHInstruction NotYetImplemented(ushort wInstr)
        {
            var instrHex = $"{wInstr & 0xFF:X2}{wInstr >> 8:X2}";
            base.EmitUnitTest("SuperH", instrHex, "", "ShDis", this.addr, w =>
            {
                w.WriteLine($"    AssertCode(\"@@@\", \"{instrHex}\");");
            });
            return Invalid();
        }

        // Mutators

        private static bool r1(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = (uInstr >> 8) & 0xF;
            dasm.state.ops.Add(new RegisterOperand(Registers.gpregs[reg]));
            return true;
        }

        private static bool r2(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = (uInstr >> 4) & 0xF;
            dasm.state.ops.Add(new RegisterOperand(Registers.gpregs[reg]));
            return true;
        }

        private static bool r3(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = uInstr & 0xF;
            dasm.state.ops.Add(new RegisterOperand(Registers.gpregs[reg]));
            return true;
        }

        private static bool R0(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.r0));
            return true;
        }
        private static bool RBank2_3bit(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = (uInstr >> 4) & 0b0111;
            dasm.state.ops.Add(new RegisterOperand(Registers.rbank[reg]));
            return true;
        }


        private static bool f1(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = (uInstr >> 8) & 0xF;
            dasm.state.ops.Add(new RegisterOperand(Registers.fpregs[reg]));
            return true;
        }
        private static bool f2(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = (uInstr >> 4) & 0xF;
            dasm.state.ops.Add(new RegisterOperand(Registers.fpregs[reg]));
            return true;
        }
        private static bool F0(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.fr0));
            return true;
        }

        private static bool d1(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = (uInstr >> (1 + 8)) & 0x7;
            dasm.state.ops.Add(new RegisterOperand(Registers.dfpregs[reg]));
            return true;
        }
        private static bool d2(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = (uInstr >> (1 + 4)) & 0x7;
            dasm.state.ops.Add(new RegisterOperand(Registers.dfpregs[reg]));
            return true;
        }

        private static bool v1(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = (uInstr >> 10) & 0x3;
            dasm.state.ops.Add(new RegisterOperand(Registers.vfpregs[reg]));
            return true;
        }
        private static bool v2(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = (uInstr >> 8) & 0x3;
            dasm.state.ops.Add(new RegisterOperand(Registers.vfpregs[reg]));
            return true;
        }

        private static bool pr(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.pr));
            return true;
        }

        private static bool sr(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.sr));
            return true;
        }

        private static bool gbr(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.gbr));
            return true;
        }

        private static bool RK(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.spc));
            return true;
        }

        private static bool tbr(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.tbr));
            return true;
        }

        private static bool RV(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.vbr));
            return true;
        }

        private static bool mod(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.mod));
            return true;
        }

        private static bool ssr(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.ssr));
            return true;
        }

        private static bool rs(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.rs));
            return true;
        }

        private static bool spc(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.spc));
            return true;
        }

        private static bool mh(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.mach));
            return true;
        }
        private static bool ml(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.macl));
            return true;
        }
        private static bool fpul(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.fpul));
            return true;
        }

        private static bool xmtrx(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.xmtrx));
            return true;
        }
        private static bool dsr(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.dsr));
            return true;
        }
        private static bool dbr(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.dbr));
            return true;
        }
        private static bool sgr(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.sgr));
            return true;
        }


        private static bool I(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(ImmediateOperand.Byte((byte)uInstr));
            return true;
        }

        private static bool j(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(AddressOperand.Create(dasm.rdr.Address + (2 + 2 * (sbyte)uInstr)));
            return true;
        }
        private static bool J(uint uInstr, SuperHDisassembler dasm)
        {
            int offset = ((int)uInstr << 20) >> 19;
            dasm.state.ops.Add(AddressOperand.Create(dasm.rdr.Address + (2 + offset)));
            return true;
        }


        private static bool Ind1b(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 8) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.Indirect(PrimitiveType.Byte, reg));
            return true;
        }
        private static bool Ind1w(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 8) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.Indirect(PrimitiveType.Word16, reg));
            return true;
        }
        private static bool Ind1l(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 8) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.Indirect(PrimitiveType.Word32, reg));
            return true;
        }
        private static bool Ind1d(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 8) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.Indirect(PrimitiveType.Word64, reg));
            return true;
        }

        private static bool Ind2b(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 4) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.Indirect(PrimitiveType.Byte, reg));
            return true;
        }
        private static bool Ind2w(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 4) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.Indirect(PrimitiveType.Word16, reg));
            return true;
        }
        private static bool Ind2l(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 4) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.Indirect(PrimitiveType.Word32, reg));
            return true;
        }
        private static bool Pre1b(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 8) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.IndirectPreDecr(PrimitiveType.Byte, reg));
            return true;
        }

        private static bool Pre1w(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 8) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.IndirectPreDecr(PrimitiveType.Word16, reg));
            return true;
        }

        private static bool Pre1l(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 8) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.IndirectPreDecr(PrimitiveType.Word32, reg));
            return true;
        }

        private static bool Pre15l(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.r15;
            dasm.state.ops.Add(MemoryOperand.IndirectPreDecr(PrimitiveType.Word32, reg));
            return true;
        }

        private static bool Post1w(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 8) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.IndirectPostIncr(PrimitiveType.Word16, reg));
            return true;
        }
        private static bool Post1l(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 8) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.IndirectPostIncr(PrimitiveType.Word32, reg));
            return true;
        }
        private static bool Post2b(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 4) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.IndirectPostIncr(PrimitiveType.Byte, reg));
            return true;
        }
        private static bool Post2w(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 4) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.IndirectPostIncr(PrimitiveType.Word16, reg));
            return true;
        }
        private static bool Post2l(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 4) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.IndirectPostIncr(PrimitiveType.Word32, reg));
            return true;
        }
        private static bool Post15l(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.r15;
            dasm.state.ops.Add(MemoryOperand.IndirectPostIncr(PrimitiveType.Word32, reg));
            return true;
        }
        // Indirect indexed
        private static bool X1b(uint uInstr, SuperHDisassembler dasm)
        {
            var iReg = (uInstr >> 8) & 0xF;
            dasm.state.ops.Add(MemoryOperand.IndexedIndirect(PrimitiveType.Byte, Registers.gpregs[iReg]));
            return true;
        }
        private static bool X1w(uint uInstr, SuperHDisassembler dasm)
        {
            var iReg = (uInstr >> 8) & 0xF;
            dasm.state.ops.Add(MemoryOperand.IndexedIndirect(PrimitiveType.Word16, Registers.gpregs[iReg]));
            return true;
        }
        private static bool X1l(uint uInstr, SuperHDisassembler dasm)
        {
            var iReg = (uInstr >> 8) & 0xF;
            dasm.state.ops.Add(MemoryOperand.IndexedIndirect(PrimitiveType.Word32, Registers.gpregs[iReg]));
            return true;
        }
        private static bool X1d(uint uInstr, SuperHDisassembler dasm)
        {
            var iReg = (uInstr >> 8) & 0xF;
            dasm.state.ops.Add(MemoryOperand.IndexedIndirect(PrimitiveType.Word64, Registers.gpregs[iReg]));
            return true;
        }
        private static bool X2b(uint uInstr, SuperHDisassembler dasm)
        {
            var iReg = (uInstr >> 4) & 0xF;
            dasm.state.ops.Add(MemoryOperand.IndexedIndirect(PrimitiveType.Byte, Registers.gpregs[iReg]));
            return true;
        }
        private static bool X2w(uint uInstr, SuperHDisassembler dasm)
        {
            var iReg = (uInstr >> 4) & 0xF;
            dasm.state.ops.Add(MemoryOperand.IndexedIndirect(PrimitiveType.Word16, Registers.gpregs[iReg]));
            return true;
        }
        private static bool X2l(uint uInstr, SuperHDisassembler dasm)
        {
            var iReg = (uInstr >> 4) & 0xF;
            dasm.state.ops.Add(MemoryOperand.IndexedIndirect(PrimitiveType.Word32, Registers.gpregs[iReg]));
            return true;
        }


        // indirect with displacement
        private static bool D1l(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 8) & 0xF];
            var width = PrimitiveType.Word32;
            dasm.state.ops.Add(MemoryOperand.IndirectDisplacement(width, reg, (int)(uInstr & 0xF) * width.Size));
            return true;
        }
        private static bool D2b(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 4) & 0xF];
            var width = PrimitiveType.Byte;
            dasm.state.ops.Add(MemoryOperand.IndirectDisplacement(width, reg, (int)(uInstr & 0xF) * width.Size));
            return true;
        }
        private static bool D2w(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 4) & 0xF];
            var width = PrimitiveType.Word16;
            dasm.state.ops.Add(MemoryOperand.IndirectDisplacement(width, reg, (int)(uInstr & 0xF) * width.Size));
            return true;
        }
        private static bool D2l(uint uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 4) & 0xF];
            var width = PrimitiveType.Word32;
            dasm.state.ops.Add(MemoryOperand.IndirectDisplacement(width, reg, (int)(uInstr & 0xF) * width.Size));
            return true;
        }



        // PC-relative with displacement
        private static bool Pw(uint uInstr, SuperHDisassembler dasm)
        {
            var width = PrimitiveType.Word16;
            dasm.state.ops.Add(MemoryOperand.PcRelativeDisplacement(width, width.Size * (byte)uInstr));
            return true;
        }
        private static bool Pl(uint uInstr, SuperHDisassembler dasm)
        {
            var width = PrimitiveType.Word32;
            dasm.state.ops.Add(MemoryOperand.PcRelativeDisplacement(width, width.Size * (byte)uInstr));
            return true;
        }


        private static bool Gb(uint uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(MemoryOperand.GbrIndexedIndirect(PrimitiveType.Byte));
            return true;
        }

        // Factory methods

        private static InstrDecoder Instr(Opcode opcode, params Mutator<SuperHDisassembler>[] mutators)
        {
            return new InstrDecoder(opcode, InstrClass.Linear, mutators);
        }

        private static InstrDecoder Instr(Opcode opcode, InstrClass iclass, params Mutator<SuperHDisassembler>[] mutators)
        {
            return new InstrDecoder(opcode, iclass, mutators);
        }


        private static FieldDecoder Mask(int pos, int length, params Decoder[] decoders)
        {
            return new FieldDecoder(pos, length, decoders);
        }

        private static FieldDecoder Sparse(int pos, int length, params (int, Decoder)[] sparseDecoders)
        {
            var decoders = Enumerable.Range(0, 1 << length)
                .Select(n => (Decoder)new NyiDecoder())
                .ToArray();
            foreach (var decoder in sparseDecoders)
            {
                decoders[decoder.Item1] = decoder.Item2;
            }
            return new FieldDecoder(pos, length, decoders);
        }
        private static ConditionalDecoder Cond(int pos, int length, Predicate<uint> pred, Decoder trueDecoder, Decoder falseDecoder)
        {
            return new ConditionalDecoder(pos, length, pred, trueDecoder, falseDecoder);
        }

        private static FieldDecoder Sparse(int pos, int length, Dictionary<int, Decoder> sparseDecoders)
        {
            var decoders = new Decoder[1 << length];
            for (int i = 0; i < decoders.Length; ++i)
            {
                if (sparseDecoders.TryGetValue(i, out var decoder))
                {
                    decoders[i] = decoder;
                }
                else
                {
                    decoders[i] = new NyiDecoder();
                }
            }
            return new FieldDecoder(pos, length, decoders);
        }

        // Predicates

        private static bool Ne0(uint n)
        {
            return n != 0;
        }

        // Decoders

        private abstract class Decoder
        {
            public abstract SuperHInstruction Decode(SuperHDisassembler dasm, ushort uInstr);
        }

        private class InstrDecoder : Decoder
        {
            private readonly Opcode opcode;
            private readonly InstrClass iclass;
            private readonly Mutator<SuperHDisassembler>[] mutators;

            public InstrDecoder(Opcode opcode, InstrClass iclass, Mutator<SuperHDisassembler>[] mutators)
            {
                this.opcode = opcode;
                this.iclass = iclass;
                this.mutators = mutators;
            }

            public override SuperHInstruction Decode(SuperHDisassembler dasm, ushort uInstr)
            {
                dasm.state.opcode = this.opcode;
                dasm.state.iclass = this.iclass;
                foreach (var mutator in this.mutators)
                {
                    if (!mutator(uInstr, dasm))
                        return dasm.Invalid();
                }
                return dasm.state.MakeInstruction();
            }
        }

        private class NyiDecoder : Decoder
        {
            public NyiDecoder()
            {

            }

            public override SuperHInstruction Decode(SuperHDisassembler dasm, ushort uInstr)
            {
                return dasm.NotYetImplemented(uInstr);
            }
        }

        private class Oprec4Bits : Decoder
        {
            private readonly int shift;
            private readonly Decoder[] oprecs;

            public Oprec4Bits(int shift, Decoder[] oprecs)
            {
                this.shift = shift;
                this.oprecs = oprecs;
            }

            public override SuperHInstruction Decode(SuperHDisassembler dasm, ushort uInstr)
            {
                return oprecs[(uInstr >> shift) & 0xF].Decode(dasm, uInstr);
            }
        }

        private class FieldDecoder : Decoder
        {
            private readonly int shift;
            private readonly int bitcount;
            private readonly Decoder[] decoders;

            public FieldDecoder(int shift, int bitcount, Decoder[] decoders)
            {
                this.shift = shift;
                this.bitcount = bitcount;
                this.decoders = decoders;
            }

            public override SuperHInstruction Decode(SuperHDisassembler dasm, ushort uInstr)
            {
                var mask = (1 << bitcount) - 1;
                var code = (uInstr >> shift) & mask;
                return decoders[code].Decode(dasm, uInstr);
            }
        }

        private class ConditionalDecoder : Decoder
        {
            private readonly int pos;
            private readonly uint mask;
            private readonly Predicate<uint> pred;
            private readonly Decoder trueDecoder;
            private readonly Decoder falseDecoder;

            public ConditionalDecoder(int pos, int length, Predicate<uint> pred, Decoder trueDecoder, Decoder falseDecoder)
            {
                this.pos = pos;
                this.mask = (1u << length) - 1u;
                this.pred = pred;
                this.trueDecoder = trueDecoder;
                this.falseDecoder = falseDecoder;
            }

            public override SuperHInstruction Decode(SuperHDisassembler dasm, ushort uInstr)
            {
                var bits = (uInstr >> pos) & mask;
                var decoder = (pred((uint)bits)) ? trueDecoder : falseDecoder;
                return decoder.Decode(dasm, uInstr);
            }
        }

        private static readonly Decoder invalid = Instr(Opcode.invalid);

        private static readonly FieldDecoder decode_FxFD = Mask(8, 4,
            Instr(Opcode.fsca, fpul, f1),
            Instr(Opcode.ftrv, xmtrx, v2),
            Instr(Opcode.fsca, fpul, f1),
            Instr(Opcode.fschg),

            Instr(Opcode.fsca, fpul, f1),
            Instr(Opcode.ftrv, xmtrx, v2),
            Instr(Opcode.fsca, fpul, f1),
            invalid,

            Instr(Opcode.fsca, fpul, f1),
            Instr(Opcode.ftrv, xmtrx, v2),
            Instr(Opcode.fsca, fpul, f1),
            Instr(Opcode.frchg),

            Instr(Opcode.fsca, fpul, f1),
            Instr(Opcode.ftrv, xmtrx, v2),
            Instr(Opcode.fsca, fpul, f1),
            invalid);

        private static readonly FieldDecoder decode_FxxD = Sparse(4, 5,
            (0x01, Instr(Opcode.flds, d1, fpul)),
            (0x02, Instr(Opcode.@float, fpul, d1)),
            (0x03, Instr(Opcode.ftrc, d1, fpul)),
            (0x04, Instr(Opcode.fneg, d1)),
            (0x05, Instr(Opcode.fabs, d1)),
            (0x06, Instr(Opcode.fsqrt, f1)),
            (0x08, Instr(Opcode.fldi0, f1)),
            (0x09, Instr(Opcode.fldi1, f1)),
            (0x0A, Instr(Opcode.fcnvsd, fpul, d1)),
            (0x0B, Instr(Opcode.fcnvds, d1, fpul)),
            (0x0E, Instr(Opcode.fipr, v2, v1)),
            (0x0F, decode_FxFD),

            (0x11, Instr(Opcode.flds, f1, fpul)),
            (0x15, Instr(Opcode.fabs, f1)),
            (0x18, Instr(Opcode.fldi0, f1)),
            (0x19, Instr(Opcode.fldi1, f1)),
            (0x1E, Instr(Opcode.fipr, v2, v1)),
            (0x1F, decode_FxFD));

        private static readonly Decoder[] decoders = new Decoder[]
        {
            // 0...
            Sparse(0, 4, new Dictionary<int, Decoder>
            {
                { 0x0, Instr(Opcode.invalid, InstrClass.Invalid|InstrClass.Zero) },
                { 0x1, invalid },
                { 0x02, Mask(7, 1,
                    Mask(4, 3,
                        Instr(Opcode.stc, sr,r1),
                        Instr(Opcode.stc, gbr,r1),
                        Instr(Opcode.stc, gbr,r1),
                        Instr(Opcode.stc, ssr,r1),
                        Instr(Opcode.stc, RK,r1),
                        Instr(Opcode.stc, mod,r1),
                        Instr(Opcode.stc, rs,r1),
                        invalid),
                    Instr(Opcode.stc, RBank2_3bit,r1))
                },
                { 0x03, Sparse(4, 4,    // 0..3
                        ( 0x0, Instr(Opcode.bsrf, r1)),
                        ( 0x1, invalid),
                        ( 0x2, Instr(Opcode.braf, r1)),
                        ( 0x3, invalid),
                        ( 0x7, Instr(Opcode.movco_l, R0,Ind1l)),
                        ( 0x8, Instr(Opcode.pref, Ind1l)),
                        ( 0x9, Instr(Opcode.ocbi, Ind1b)),
                        ( 0xA, Instr(Opcode.ocbp, Ind1b)),
                        ( 0xC, Instr(Opcode.movca_l, R0,Ind1l))
                        )
                },
                { 0x4, Instr(Opcode.mov_b, r2,X1b) },
                { 0x5, Instr(Opcode.mov_w, r2,X1w) },
                { 0x6, Instr(Opcode.mov_l, r2,X1l) },
                { 0x7, Instr(Opcode.mul_l, r2,r1) },
                { 0x8, Cond(8, 4, Ne0,
                    invalid,
                    Mask(4, 4,
                        Instr(Opcode.clrt),
                        Instr(Opcode.sett),
                        Instr(Opcode.clrmac),
                        Instr(Opcode.ldtlb),

                        Instr(Opcode.clrs),
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid))
                },
                { 0x9, Sparse(4, 4,
                        ( 0x0, Cond(8,4,Ne0,invalid, Instr(Opcode.nop))),
                        ( 0x1, Cond(8,4,Ne0,invalid, Instr(Opcode.div0u))),
                        ( 0x2, Instr(Opcode.movt, r1)),
                        ( 0x5, invalid),
                        ( 0xD, invalid),
                        ( 0xF, invalid))
                },
                { 0xA, Sparse(4, 4,
                        ( 0x0, Instr(Opcode.sts, mh,r1) ),
                        ( 0x1, Instr(Opcode.sts, ml,r1) ),
                        ( 0x2, Instr(Opcode.sts, pr,r1) ),
                        ( 0x4, Instr(Opcode.sts, tbr,r1) ),
                        ( 0x5, Instr(Opcode.sts, fpul,r1) ),
                        ( 0x6, Instr(Opcode.sts, dsr,r1) ),
                        ( 0x8, invalid ),   // DSP:sts X0,r1
                        ( 0x9, invalid ),   // DSP:sts X1,r1
                        ( 0xA, invalid ),   // DSP:lds	Rm,Y1
                        ( 0xF, Instr(Opcode.stc, dbr,r1))
                    )
                },
                { 0xB, Cond(8, 4, Ne0,
                    invalid,
                    Sparse(4, 4,
                        (0x0, Instr(Opcode.rts)),
                        (0x1, Instr(Opcode.sleep)),
                        (0x2, Instr(Opcode.rte)),
                        (0x3, Instr(Opcode.brk))))
                },
                { 0xC, Instr(Opcode.mov_b, X2b,r1) },
                { 0xD, Instr(Opcode.mov_w, X2w,r1) },
                { 0xE, Instr(Opcode.mov_l, X2l,r1) },
                { 0xF, Instr(Opcode.mac_l, Post2l,Post1l) }
            }),
            Instr(Opcode.mov_l, r2,D1l),
            // 2...
            new Oprec4Bits(0, new Decoder[]
            {
                Instr(Opcode.mov_b, r2,Ind1b),
                Instr(Opcode.mov_w, r2,Ind1w),
                Instr(Opcode.mov_l, r2,Ind1l),
                invalid,

                Instr(Opcode.mov_b, r2,Pre1b),
                Instr(Opcode.mov_w, r2,Pre1w),
                Instr(Opcode.mov_l, r2,Pre1l),
                Instr(Opcode.div0s, r2,r1),

                Instr(Opcode.tst, r2,r1),
                Instr(Opcode.and, r2,r1),
                Instr(Opcode.xor, r2,r1),
                Instr(Opcode.or, r2,r1),

                Instr(Opcode.cmp_str, r2,r1),
                Instr(Opcode.xtrct, r2,r1),
                Instr(Opcode.mulu_w, r2,r1),
                Instr(Opcode.muls_w, r2,r1),
            }),
            // 3...
            new Oprec4Bits(0, new Decoder[]
            {
                Instr(Opcode.cmp_eq, r2,r1),
                invalid,
                Instr(Opcode.cmp_hs, r2,r1),
                Instr(Opcode.cmp_ge, r2,r1),

                Instr(Opcode.div1, r2,r1),
                Instr(Opcode.dmulu_l, r2,r1),
                Instr(Opcode.cmp_hi, r2,r1),
                Instr(Opcode.cmp_gt, r2,r1),

                Instr(Opcode.sub, r2,r1),
                invalid,
                Instr(Opcode.subc, r2,r1),
                Instr(Opcode.subv, r2,r1),

                Instr(Opcode.add, r2,r1),
                Instr(Opcode.dmuls_l, r2,r1),
                Instr(Opcode.addc, r2,r1),
                Instr(Opcode.addv, r2,r1),
            }),

            // 4...
            Sparse(0, 8, new Dictionary<int, Decoder>
            {
                { 0x00, Instr(Opcode.shll, r1) },
                { 0x01, Instr(Opcode.shlr, r1) },
                { 0x04, Instr(Opcode.rotl, r1) },
                { 0x05, Instr(Opcode.rotr, r1) },
                { 0x06, Instr(Opcode.lds_l, Post1l,mh) },
                { 0x08, Instr(Opcode.shll2, r1) },
                { 0x09, Instr(Opcode.shlr, r1) },
                { 0x0B, Instr(Opcode.jsr, Ind1l) },
                { 0x0C, Instr(Opcode.shad, r2,r1) },
                { 0x0E, Instr(Opcode.ldc, r1,sr) },
                { 0x13, Instr(Opcode.stc_l, gbr,Pre1l) },
                { 0x14, invalid },  // DSP setrc r1
                { 0x1B, Instr(Opcode.tas_b, Ind1b) },
                { 0x1C, Instr(Opcode.shad, r2,r1) },
                { 0x20, Instr(Opcode.shal, r1) },
                { 0x2A, Instr(Opcode.lds, r1,pr) },
                { 0x2C, Instr(Opcode.shad, r2,r1) },
                { 0x2E, Instr(Opcode.ldc, r1,RV) },
                { 0x30, invalid },
                { 0x34, invalid },
                { 0x38, invalid },
                { 0x36, Instr(Opcode.ldc_l, Post1l,sgr) },
                { 0x3C, Instr(Opcode.shad, r2,r1) },
                { 0x40, invalid },
                { 0x41, invalid },
                { 0x44, invalid },
                { 0x48, invalid },
                { 0x4A, Instr(Opcode.ldc, r1,tbr) },
                { 0x4C, Instr(Opcode.shad, r2,r1) },
                { 0x52, invalid },
                { 0x59, invalid },
                { 0x5A, Instr(Opcode.lds, r1,fpul) },
                { 0x5C, Instr(Opcode.shad, r2,r1) },
                { 0x64, invalid },
                { 0x66, Instr(Opcode.lds_l, Post1l,dsr) },
                { 0x68, invalid },
                { 0x6A, Instr(Opcode.lds, r1,dsr) },
                { 0x6C, Instr(Opcode.shad, r2,r1) },
                { 0x70, invalid },
                { 0x74, invalid },
                { 0x7C, Instr(Opcode.shad, r2,r1) },
                { 0x80, Instr(Opcode.mulr, R0,r1) },
                { 0x88, invalid },
                { 0x8C, Instr(Opcode.shad, r2,r1) },
                { 0x90, invalid },
                { 0x94, Instr(Opcode.divs, R0,r1) },
                { 0x98, invalid },
                { 0x9C, Instr(Opcode.shad, r2,r1) },
                { 0xA0, invalid },
                { 0xA4, invalid },
                { 0xA8, invalid },
                { 0xAC, Instr(Opcode.shad, r2,r1) },
                { 0xB4, invalid },
                { 0xB8, invalid },
                { 0xBC, Instr(Opcode.shad, r2,r1) },
                { 0xC4, invalid },
                { 0xC8, invalid },
                { 0xCC, Instr(Opcode.shad, r2,r1) },
                { 0xD0, invalid },
                { 0xD2, invalid },
                { 0xD3, Instr(Opcode.stc_l, RBank2_3bit, r1) },
                { 0xD8, invalid },
                { 0xDC, Instr(Opcode.shad, r2,r1) },
                { 0xE0, invalid },
                { 0xE4, invalid },
                { 0xE8, invalid },
                { 0xEC, Instr(Opcode.shad, r2,r1) },
                { 0xF0, Instr(Opcode.movmu_l, r1,Pre15l)},
                { 0xF4, Instr(Opcode.movmu_l, Post15l,r1)},
                { 0xF8, invalid },
                { 0xFC, Instr(Opcode.shad, r2,r1) },


                { 0x87, Instr(Opcode.ldc_l, Post1l,RBank2_3bit) },
                { 0x97, Instr(Opcode.ldc_l, Post1l,RBank2_3bit) },
                { 0xA7, Instr(Opcode.ldc_l, Post1l,RBank2_3bit) },
                { 0xB7, Instr(Opcode.ldc_l, Post1l,RBank2_3bit) },
                { 0xC7, Instr(Opcode.ldc_l, Post1l,RBank2_3bit) },
                { 0xD7, Instr(Opcode.ldc_l, Post1l,RBank2_3bit) },
                { 0xE7, Instr(Opcode.ldc_l, Post1l,RBank2_3bit) },
                { 0xF7, Instr(Opcode.ldc_l, Post1l,RBank2_3bit) },

                { 0x0D, Instr(Opcode.shld, r2,r1) },
                { 0x1D, Instr(Opcode.shld, r2,r1) },
                { 0x2D, Instr(Opcode.shld, r2,r1) },
                { 0x3D, Instr(Opcode.shld, r2,r1) },
                { 0x4D, Instr(Opcode.shld, r2,r1) },
                { 0x5D, Instr(Opcode.shld, r2,r1) },
                { 0x6D, Instr(Opcode.shld, r2,r1) },
                { 0x7D, Instr(Opcode.shld, r2,r1) },
                { 0x8D, Instr(Opcode.shld, r2,r1) },
                { 0x9D, Instr(Opcode.shld, r2,r1) },
                { 0xAD, Instr(Opcode.shld, r2,r1) },
                { 0xBD, Instr(Opcode.shld, r2,r1) },
                { 0xCD, Instr(Opcode.shld, r2,r1) },
                { 0xDD, Instr(Opcode.shld, r2,r1) },
                { 0xED, Instr(Opcode.shld, r2,r1) },
                { 0xFD, Instr(Opcode.shld, r2,r1) },

                { 0x0F, Instr(Opcode.mac_w, Post2w,Post1w) },
                { 0x1F, Instr(Opcode.mac_w, Post2w,Post1w) },
                { 0x2F, Instr(Opcode.mac_w, Post2w,Post1w) },
                { 0x3F, Instr(Opcode.mac_w, Post2w,Post1w) },
                { 0x4F, Instr(Opcode.mac_w, Post2w,Post1w) },
                { 0x5F, Instr(Opcode.mac_w, Post2w,Post1w) },
                { 0x6F, Instr(Opcode.mac_w, Post2w,Post1w) },
                { 0x7F, Instr(Opcode.mac_w, Post2w,Post1w) },
                { 0x8F, Instr(Opcode.mac_w, Post2w,Post1w) },
                { 0x9F, Instr(Opcode.mac_w, Post2w,Post1w) },
                { 0xAF, Instr(Opcode.mac_w, Post2w,Post1w) },
                { 0xBF, Instr(Opcode.mac_w, Post2w,Post1w) },
                { 0xCF, Instr(Opcode.mac_w, Post2w,Post1w) },
                { 0xDF, Instr(Opcode.mac_w, Post2w,Post1w) },
                { 0xEF, Instr(Opcode.mac_w, Post2w,Post1w) },
                { 0xFF, Instr(Opcode.mac_w, Post2w,Post1w) },


                { 0x10, Instr(Opcode.dt, r1) },
                { 0x11, Instr(Opcode.cmp_pz, r1) },
                { 0x15, Instr(Opcode.cmp_pl, r1) },
                { 0x18, Instr(Opcode.shll8, r1) },
                { 0x19, Instr(Opcode.shlr8, r1) },
                { 0x21, Instr(Opcode.shar, r1) },
                { 0x22, Instr(Opcode.sts_l, pr,Pre1l) },
                { 0x24, Instr(Opcode.rotcl, r1) },
                { 0x25, Instr(Opcode.rotcr, r1) },
                { 0x26, Instr(Opcode.lds_l, Post1l,pr) },
                { 0x28, Instr(Opcode.shll16, r1) },
                { 0x29, Instr(Opcode.shlr16, r1) },
                { 0x2B, Instr(Opcode.jmp, Ind1l) },
                { 0x43, Instr(Opcode.stc_l, spc,r1) }
            }),
            Instr(Opcode.mov_l, D2l,r1),
            // 6...
            new Oprec4Bits(0, new Decoder[]
            {
                Instr(Opcode.mov_b, Ind2b,r1),
                Instr(Opcode.mov_w, Ind2w,r1),
                Instr(Opcode.mov_l, Ind2l,r1),
                Instr(Opcode.mov, r2,r1),

                Instr(Opcode.mov_b, Post2b,r1),
                Instr(Opcode.mov_w, Post2w,r1),
                Instr(Opcode.mov_l, Post2l,r1),
                Instr(Opcode.not, r2,r1),

                invalid,
                Instr(Opcode.swap_w, r2,r1),
                Instr(Opcode.negc, r2,r1),
                Instr(Opcode.neg, r2,r1),

                Instr(Opcode.extu_b, r2,r1),
                Instr(Opcode.extu_w, r2,r1),
                Instr(Opcode.exts_b, r2,r1),
                Instr(Opcode.exts_w, r2,r1),
            }),
            Instr(Opcode.add, I,r1),

            // 8...
            new Oprec4Bits(8, new Decoder[] {
                Instr(Opcode.mov_b, R0,D2b),
                Instr(Opcode.mov_w, R0,D2w),
                invalid,
                invalid,

                Instr(Opcode.mov_b, D2b,R0),
                Instr(Opcode.mov_w, D2w,R0),
                invalid,
                invalid,

                Instr(Opcode.cmp_eq, I,R0),
                Instr(Opcode.bt, j),
                invalid,
                Instr(Opcode.bf, j),

                invalid,
                Instr(Opcode.bt_s, j),
                invalid,
                Instr(Opcode.bf_s, j),
            }),
            Instr(Opcode.mov_w, Pw,r1),
            Instr(Opcode.bra, J),
            Instr(Opcode.bsr, J),

            // C...
            new Oprec4Bits(8, new Decoder[]
            {
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                Instr(Opcode.mova, Pl,R0),

                Instr(Opcode.tst, I,R0),
                Instr(Opcode.and, I,R0),
                Instr(Opcode.xor, I,R0),
                Instr(Opcode.or, I,R0),

                invalid,
                Instr(Opcode.and_b, I,Gb),
                invalid,
                invalid,
            }),
            Instr(Opcode.mov_l, Pl,r1),
            Instr(Opcode.mov, I,r1),
            // F...
            Sparse(0, 5, new Dictionary<int, Decoder>
            {
                { 0x0, Mask(8, 1,
                        Instr(Opcode.fadd, d2,d1),
                        Instr(Opcode.fadd, f2,f1))
                },
                { 0x10, Mask(8, 1,
                        Instr(Opcode.fadd, d2,d1),
                        Instr(Opcode.fadd, f2,f1))
                },
                { 0x01, Mask(8, 1,
                        Instr(Opcode.fsub, d2,d1),
                        Instr(Opcode.fsub, f2,f1))
                },
                { 0x11, Instr(Opcode.fsub, f2,f1) },

                { 0x02, Mask(8, 1,
                        Instr(Opcode.fmul, d2,d1),
                        Instr(Opcode.fmul, f2,f1))
                },
                { 0x12, Instr(Opcode.fmul, f2,f1) },

                { 0x3, Mask(8, 1,
                        Instr(Opcode.fdiv, d2,d1),
                        Instr(Opcode.fdiv, f2,f1))
                },
                { 0x4, Mask(8, 1,
                        Instr(Opcode.fcmp_eq, d2,d1),
                        Instr(Opcode.fcmp_eq, f2,f1))
                },
                { 0x5, Mask(8, 1,
                        Instr(Opcode.fcmp_gt, d2,d1),
                        Instr(Opcode.fcmp_gt, f2,f1))
                },

                { 0x06, Instr(Opcode.fmov_d, X1d,d2) },
                { 0x16, Instr(Opcode.fmov_s, X1l,f2) },

                { 0x08, Instr(Opcode.fmov_d, Ind1d,d2) },
                { 0x18, Instr(Opcode.fmov_s, Ind1l,f2) },

                { 0x9, Mask(8, 1,
                    Instr(Opcode.fcmp_gt, d2,d1),
                    Instr(Opcode.fcmp_gt, f2,f1))
                },

                { 0x0A, Instr(Opcode.fmov_d, Ind1d,d2) },
                { 0x1A, Instr(Opcode.fmov_s, Ind1l,f2) },

                { 0xC, Mask(8, 1,
                    Instr(Opcode.fmov, d2,d1),
                    Instr(Opcode.fmov, f2,f1))
                },
                { 0x1C, Mask(8, 1,
                    Instr(Opcode.fmov, d2,d1),
                    Instr(Opcode.fmov, f2,f1))
                },
                { 0xD, decode_FxxD },
                
                { 0x0E, Instr(Opcode.fmac, F0,f2,f1) },
                { 0x0F, invalid },
                { 0x14, Instr(Opcode.fcmp_eq, f2,f1) },
                { 0x1D, decode_FxxD },
                { 0x1E, Instr(Opcode.fmac, F0,f2,f1) },
                { 0x1F, invalid },
            })
        };
    }
}
