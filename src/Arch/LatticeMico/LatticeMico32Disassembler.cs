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

using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.LatticeMico
{
    using Decoder = Decoder<LatticeMico32Disassembler, Mnemonic, LatticeMico32Instruction>;

    public class LatticeMico32Disassembler : DisassemblerBase<LatticeMico32Instruction, Mnemonic>
    {
        private const InstrClass CT = InstrClass.ConditionalTransfer;

        private static readonly Decoder rootDecoder;

        private readonly LatticeMico32Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public LatticeMico32Disassembler(LatticeMico32Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override LatticeMico32Instruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadBeUInt32(out uint uInstr))
                return null;
            ops.Clear();
            var instr = rootDecoder.Decode(uInstr, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override LatticeMico32Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new LatticeMico32Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
            return instr;
        }

        public override LatticeMico32Instruction CreateInvalidInstruction()
        {
            return new LatticeMico32Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
                Operands = MachineInstruction.NoOperands
            };
        }

        public override LatticeMico32Instruction NotYetImplemented(uint wInstr, string message)
        {
            var hexBytes = wInstr.ToString("X8");
            EmitUnitTest("LatticeMico32", hexBytes, message, "Lm32Dis", Address.Ptr32(0x00100000), w =>
            {
                w.WriteLine("Assert_HexBytes(\"@@@\", \"{0}\");", hexBytes);
            });
            return base.NotYetImplemented(wInstr, message);
        }

        #region Mutators

        private static Mutator<LatticeMico32Disassembler> Reg(int bitpos, int bitsize)
        {
            var field = new Bitfield(bitpos, bitsize);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                var reg = Registers.GpRegs[iReg];
                d.ops.Add(new RegisterOperand(reg));
                return true; 
            };
        }
        private static readonly Mutator<LatticeMico32Disassembler> r11 = Reg(11, 5);
        private static readonly Mutator<LatticeMico32Disassembler> r16 = Reg(16, 5);
        private static readonly Mutator<LatticeMico32Disassembler> r21 = Reg(21, 5);

        private static Mutator<LatticeMico32Disassembler> ImmS(int bitpos, int bitsize)
        {
            var field = new Bitfield(bitpos, bitsize);
            return (u, d) =>
            {
                var imm = field.ReadSigned(u);
                d.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };
        }

        private static readonly Mutator<LatticeMico32Disassembler> Is5 = ImmS(0, 5);
        private static readonly Mutator<LatticeMico32Disassembler> Is16 = ImmS(0, 16);

        private static Mutator<LatticeMico32Disassembler> PcRel(int bitpos, int bitsize)
        {
            var field = new Bitfield(bitpos, bitsize);
            return (u, d) =>
            {
                var offset = field.ReadSigned(u) << 2;
                d.ops.Add(AddressOperand.Create(d.addr + offset));
                return true;
            };
        }

        private static readonly Mutator<LatticeMico32Disassembler> PcRel16 = PcRel(0, 16);
        private static readonly Mutator<LatticeMico32Disassembler> PcRel26 = PcRel(0, 26);

        private static Mutator<LatticeMico32Disassembler> nyi(string msg)
        {
            return (u, d) =>
            {
                d.NotYetImplemented(u, msg);
                return false;
            };
        }

        private static Mutator<LatticeMico32Disassembler> M(PrimitiveType dt)
        {
            var offField = new Bitfield(0, 16);
            var baseField = new Bitfield(21, 5);
            return (u, d) =>
            {
                var offset = offField.ReadSigned(u);
                var iReg = baseField.Read(u);
                var reg = Registers.GpRegs[iReg];
                d.ops.Add(new MemoryOperand(dt, reg, offset));
                return true;
            };
        }

        private static Mutator<LatticeMico32Disassembler> IsZero(int bitpos, int bitlength)
        {
            var field = new Bitfield(bitpos, bitlength);
            return (u, d) =>
            {
                var n = field.Read(u);
                return n == 0;
            };
        }

        private static readonly Mutator<LatticeMico32Disassembler> z11 = IsZero(0, 11);
        private static readonly Mutator<LatticeMico32Disassembler> z21 = IsZero(0, 21);

        #endregion 

        #region Decoders

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<LatticeMico32Disassembler> [] mutators)
        {
            return new InstrDecoder<LatticeMico32Disassembler, Mnemonic, LatticeMico32Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<LatticeMico32Disassembler>[] mutators)
        {
            return new InstrDecoder<LatticeMico32Disassembler, Mnemonic, LatticeMico32Instruction>(iclass, mnemonic, mutators);
        }

        private static Decoder Nyi(string msg)
        {
            return new NyiDecoder<LatticeMico32Disassembler, Mnemonic, LatticeMico32Instruction>(msg);
        }

        #endregion

        /*


        */
        static LatticeMico32Disassembler()
        {
            rootDecoder = Mask(26, 6, "LatticeMico32",
                Instr(Mnemonic.srui, r16, r21, Is5),
                Instr(Mnemonic.nori, r16, r21, Is16),
                Instr(Mnemonic.muli, r16, r21, Is16),
                Instr(Mnemonic.sh, M(PrimitiveType.Word16), r16),

                Instr(Mnemonic.lb, r16, M(PrimitiveType.SByte)),
                Instr(Mnemonic.sri, r16, r21, Is5),
                Instr(Mnemonic.xori, r16, r21, Is16),
                Instr(Mnemonic.lh, r16, M(PrimitiveType.Int16)),

                Instr(Mnemonic.andi, r16, r21, Is16),
                Instr(Mnemonic.xnori, r16, r21, Is16),
                Instr(Mnemonic.lw, r16, M(PrimitiveType.Word32)),
                Instr(Mnemonic.lhu, r16, M(PrimitiveType.Word16)),

                Instr(Mnemonic.sb, M(PrimitiveType.Byte), r16),
                Instr(Mnemonic.addi, r16, r21, Is16),
                Instr(Mnemonic.ori, r16, r21, Is16),
                Instr(Mnemonic.sli, r16, r21, Is5),

                // 10
                Instr(Mnemonic.lbu, r16, M(PrimitiveType.Byte)),
                Instr(Mnemonic.be,  CT, r16, r21, PcRel16),
                Instr(Mnemonic.bg,  CT, r16, r21, PcRel16),
                Instr(Mnemonic.bge, CT, r16, r21, PcRel16),

                Instr(Mnemonic.bgeu, CT, r16, r21, PcRel16),
                Instr(Mnemonic.bgu,  CT, r16, r21, PcRel16),
                Instr(Mnemonic.sw, M(PrimitiveType.Word32), r16),
                Instr(Mnemonic.bne, CT, r16, r21, PcRel16),

                Instr(Mnemonic.andhi, r16, r21, Is16),
                Instr(Mnemonic.cmpei, r16, r21, Is16),
                Instr(Mnemonic.cmpgi, r16, r21, Is16),
                Instr(Mnemonic.cmpgei, r16, r21, Is16),

                Instr(Mnemonic.cmpgeui, r16, r21, Is16),
                Instr(Mnemonic.cmpgui, r16, r21, Is16),
                Instr(Mnemonic.orhi, r16, r21, Is16),
                Instr(Mnemonic.cmpnei, r16, r21, Is16),

                // 20
                Instr(Mnemonic.sru, z11, r11, r21, r16),
                Instr(Mnemonic.nor, z11, r11, r21, r16),
                Instr(Mnemonic.mul, z11, r11, r21, r16),
                Instr(Mnemonic.divu, z11, r11, r21, r16),

                Instr(Mnemonic.rcsr, nyi("rcsr")),
                Instr(Mnemonic.sr, z11, r11, r21, r16),
                Instr(Mnemonic.xor, z11, r11, r21, r16),
                Instr(Mnemonic.div, z11, r11, r21, r16),

                Instr(Mnemonic.and, z11, r11, r21, r16),
                Instr(Mnemonic.xnor, z11, r11, r21, r16),
                Instr(Mnemonic.reserved, InstrClass.Invalid),
                Instr(Mnemonic.raise, nyi("raise")),

                Instr(Mnemonic.sextb, z11, r11,r21),
                Instr(Mnemonic.add, z11, r11, r21, r16),
                Instr(Mnemonic.or, z11, r11, r21, r16),
                Instr(Mnemonic.sl, z11, r11, r21, r16),

                // 30
                Instr(Mnemonic.b, InstrClass.Transfer, z21, r21),
                Instr(Mnemonic.modu, z11, r11, r21, r16),
                Instr(Mnemonic.sub, z11, r11, r21, r16),
                Instr(Mnemonic.reserved, InstrClass.Invalid),

                Instr(Mnemonic.wcsr, nyi("wcsr")),
                Instr(Mnemonic.mod, z11, r11, r21, r16),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, z21, r21),
                Instr(Mnemonic.sexth, z11, r11, r21),

                Instr(Mnemonic.bi, InstrClass.Transfer, PcRel26),
                Instr(Mnemonic.cmpe, z11, r11, r21, r16),
                Instr(Mnemonic.cmpg, z11, r11, r21, r16),
                Instr(Mnemonic.cmpge, z11, r11, r21, r16),

                Instr(Mnemonic.cmpgeu, z11, r11, r21, r16),
                Instr(Mnemonic.cmpgu, z11, r11, r21, r16),
                Instr(Mnemonic.calli, InstrClass.Transfer|InstrClass.Call, PcRel26),
                Instr(Mnemonic.cmpne, z11, r11, r21, r16));
        }
    }
}
