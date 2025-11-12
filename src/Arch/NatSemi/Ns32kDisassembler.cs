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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;

namespace Reko.Arch.NatSemi;

public class Ns32kDisassembler : DisassemblerBase<Ns32kInstruction, Mnemonic>
{
    private static readonly Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> rootDecoder;

    private static readonly Bitfield bf7l4 = new(7, 4);
    private static readonly Bitfield bf11l3 = new(11, 3);
    private static readonly Bitfield bf15l4 = new(15, 4);
    private static readonly Bitfield bf17l2 = new(17, 2);

    private static readonly RegisterStorage?[] procRegs = [
        Registers.UPSR,
        Registers.DCR,
        Registers.BPC,
        Registers.DSR,

        Registers.CAR,
        null,
        null,
        null,

        Registers.FP,
        Registers.SP,
        Registers.SB,
        Registers.USP,

        Registers.CFG,
        Registers.PSR,
        Registers.INTBASE,
        Registers.MOD
    ];

    private static readonly RegisterStorage?[] mmuRegs = [
        null,
        null,
        null,
        null,

        null,
        null,
        null,
        null,

        Registers.MCR,
        null,
        Registers.MSR,
        Registers.TEAR,

        Registers.PTB0,
        Registers.PTB1,
        Registers.IVAR0,
        Registers.IVAR1
    ];

    private readonly Ns32kArchitecture arch;
    private readonly EndianImageReader rdr;
    private readonly List<MachineOperand> ops;
    private Address addr;
    private PrimitiveType dataSize;
    private PrimitiveType signedDataSize;

    public Ns32kDisassembler(Ns32kArchitecture arch, EndianImageReader rdr)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.ops = [];
        this.dataSize = arch.WordWidth;
        this.signedDataSize = PrimitiveType.Int32;
    }

    public override Ns32kInstruction? DisassembleInstruction()
    {
        this.dataSize = null!;
        this.signedDataSize = null!;
        this.addr = this.rdr.Address;
        if (!rdr.TryReadByte(out byte opcode))
            return null;
        var instr = rootDecoder.Decode(opcode, this);
        instr.Address = this.addr;
        instr.Length = (int) (rdr.Address - addr);
        ops.Clear();
        return instr;
    }

    public override Ns32kInstruction CreateInvalidInstruction()
    {
        return new Ns32kInstruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.Invalid,
        };
    }

    public override Ns32kInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
    {
        return new Ns32kInstruction
        {
            InstructionClass = iclass,
            Mnemonic = mnemonic,
            Operands = ops.ToArray()
        };
    }

    public override Ns32kInstruction NotYetImplemented(string message)
    {
        var testSvc = arch.Services.GetService<ITestGenerationService>();
        testSvc?.ReportMissingDecoder("Ns32kDis", this.addr, this.rdr, message);
        return MakeInstruction(InstrClass.Invalid, Mnemonic.Nyi);
    }

    private static bool reg11(uint uInstr, Ns32kDisassembler dasm)
    {
        uint ireg = bf11l3.Read(uInstr);
        dasm.ops.Add(Registers.GpRegisters[ireg]);
        return true;
    }

    private static bool imm11_3(uint uInstr, Ns32kDisassembler dasm)
    {
        uint ireg = bf11l3.Read(uInstr);
        var imm = Constant.Byte((byte)ireg);
        dasm.ops.Add(imm);
        return true;
    }

    private static Mutator<Ns32kDisassembler> GenericOperand(int bitpos, int bitlength, RegisterStorage[] registers)
    {   
        var field = new Bitfield(bitpos, bitlength);
        return (u, d) =>
        {
            Decoder.DumpMaskedInstruction(32, u, field.Mask << field.Position, "  generic");
            var op = field.Read(u);

            var operand = DecodeGenericOperand(registers, d, op);
            if (operand is null)
                return false;
            d.ops.Add(operand);
            return true;
        };
    }

    private static MachineOperand? DecodeGenericOperand(RegisterStorage[] registers, Ns32kDisassembler d, uint op)
    {
        switch (op)
        {
        // Register modes
        case 0:
        case 1:
        case 2:
        case 3:
        case 4:
        case 5:
        case 6:
        case 7:
            return registers[op & 0x7];
        // Register relative modes.
        case 0b01000:
        case 0b01001:
        case 0b01010:
        case 0b01011:
        case 0b01100:
        case 0b01101:
        case 0b01110:
        case 0b01111:
            var reg = Registers.GpRegisters[op & 0x7];
            return RegisterRelative(d, reg);
        // Memory relative modes.
        case 0b10000: return MemoryRelative(d, Registers.FP);
        case 0b10001: return MemoryRelative(d, Registers.SP);
        case 0b10010: return MemoryRelative(d, Registers.SB);
        case 0b10100: // Immediate
            // The processor manual states that the immediate value is
            // stored MSB.
            if (!d.rdr.TryReadBe(d.dataSize, out var imm))
                return null;
            return imm;
        case 0b10101: // Absolute
            var uAddr = ReadSignedDisplacement(d.rdr);
            if (uAddr is null)
                return null;
            return MemoryOperand.Absolute(d.dataSize, Address.Ptr32((uint) uAddr.Value));
        case 0b10110: // External
            return External(d);
        case 0b10111: // Top of stack.
            return Registers.TOS;
        // Memory space
        case 0b11000: return RegisterRelative(d, Registers.FP);
        case 0b11001: return RegisterRelative(d, Registers.SP);
        case 0b11010: return RegisterRelative(d, Registers.SB);
        case 0b11011: return Ns32kDisassembler.ProgramMemory(d);
        case 0b11100: return Indexed(d, 1);
        case 0b11101: return Indexed(d, 2);
        case 0b11110: return Indexed(d, 4);
        case 0b11111: return Indexed(d, 8);
        default: return null;
        }
    }

    private static MachineOperand? Indexed(Ns32kDisassembler dasm, int scale)
    {
        if (!dasm.rdr.TryReadByte(out byte indexByte))
            return null;
        var indexReg = Registers.GpRegisters[indexByte & 7];
        var generic = (uint)(indexByte >> 3) & 0b11111;
        var baseOperand = DecodeGenericOperand(Registers.GpRegisters, dasm, generic);
        if (baseOperand is null)
            return null;
        return MemoryOperand.Indexed(
            dasm.dataSize,
            baseOperand,
            indexReg,
            scale);
    }

    private static MachineOperand? ProgramMemory(Ns32kDisassembler d)
    {
        int? displacement = ReadSignedDisplacement(d.rdr);
        if (displacement is null)
            return null;
        return d.addr + displacement.Value;
    }

    private static MachineOperand? External(Ns32kDisassembler d)
    {
        d.NotYetImplemented("External addressing mode");
        return null;
    }

    private static MachineOperand? MemoryRelative(Ns32kDisassembler d, RegisterStorage reg)
    {
        int? disp1 = ReadSignedDisplacement(d.rdr);
        if (disp1 is null)
        {
            return null;
        }
        int? disp2 = ReadSignedDisplacement(d.rdr);
        if (disp2 is null)
        {
            return null;
        }
        var mem = MemoryOperand.MemoryRelative(
            d.dataSize,
            reg,
            disp1.Value,
            disp2.Value);
        return mem;
    }

    private static MachineOperand? RegisterRelative(Ns32kDisassembler dasm, RegisterStorage reg)
    {
        int? disp = ReadSignedDisplacement(dasm.rdr);
        if (disp is null)
        {
            return null;
        }
        var mem = MemoryOperand.RegisterRelative(
            dasm.dataSize,
            reg,
            disp.Value);
        return mem;

    }

    /// <summary>
    /// Reads a signed displacement from the reader. It is one of the following
    /// big-endian formats:
    /// <list type="bullet">
    /// <item>0...... - 7-bit signed displacement</item>
    /// <item>10..... - 14-bit signed displacement</item>
    /// <item>11..... - 30-bit signed displacement</item>
    /// </list>
    /// </summary>
    private static int? ReadSignedDisplacement(EndianImageReader rdr)
    {
        if (!rdr.TryReadByte(out byte b))
            return null;
        switch (b >> 6)
        {
        default:
            return (int) Bits.SignExtend(b, 7);
        case 0b10:
            if (!rdr.TryReadByte(out byte b2))
                return null;
            return (int) Bits.SignExtend((uint) (b << 8) | b2, 14);
        case 0b11:
            if (!rdr.TryReadByte(out b2))
                return null;
            if (!rdr.TryReadBeUInt16(out ushort w))
                return null;
            return (int) Bits.SignExtend((uint) (b << 24) | (uint) (b2 << 16) | w, 30);
        }
    }

    private static readonly Mutator<Ns32kDisassembler> gen6_5 = GenericOperand(6, 5, Registers.GpRegisters);
    private static readonly Mutator<Ns32kDisassembler> gen11_5 = GenericOperand(11, 5, Registers.GpRegisters);
    private static readonly Mutator<Ns32kDisassembler> gen14_5 = GenericOperand(14, 5, Registers.GpRegisters);
    private static readonly Mutator<Ns32kDisassembler> gen19_5 = GenericOperand(19, 5, Registers.GpRegisters);
    private static readonly Mutator<Ns32kDisassembler> fgen14_5 = GenericOperand(14, 5, Registers.FpRegisters);
    private static readonly Mutator<Ns32kDisassembler> fgen19_5 = GenericOperand(19, 5, Registers.FpRegisters);

    /// <summary>
    /// Processor register.
    /// </summary>
    private static bool procreg(uint uInstr, Ns32kDisassembler dasm)
    {
        var regEncoding = bf7l4.Read(uInstr);
        var reg = procRegs[regEncoding];
        if (reg is null)
            return false;
        dasm.ops.Add(reg);
        return true;
    }
    /// <summary>
    /// MMU register.
    /// </summary>
    private static bool mmureg(uint uInstr, Ns32kDisassembler dasm)
    {
        var mmuEncoding = bf15l4.Read(uInstr);
        var mmuReg = mmuRegs[mmuEncoding];
        if (mmuReg is null)
            return false;
        dasm.ops.Add(mmuReg);
        return true;
    }

    /// <summary>
    /// A 4-bit signed quick value.
    /// </summary>
    private static Mutator<Ns32kDisassembler> Quick(int bitpos, int bitlength)
    {
        var field = new Bitfield(bitpos, bitlength);
        return (u, d) =>
        {
            var q = field.ReadSigned(u);
            var imm = Constant.Create(d.signedDataSize, q);
            d.ops.Add(imm);
            return true;
        };
    }
    private static readonly Mutator<Ns32kDisassembler> quick7_4 = Quick(7, 4);

    /// <summary>
    /// Relative jump displacement.
    /// </summary>
    private static bool jdisp(uint uInstr, Ns32kDisassembler dasm)
    {
        int? disp = ReadSignedDisplacement(dasm.rdr);
        if (disp is null)
            return false;
        dasm.ops.Add(dasm.addr + disp.Value);
        return true;
    }

    /// <summary>
    /// Displacement.
    /// </summary>
    private static bool disp(uint uInstr, Ns32kDisassembler dasm)
    {
        int? disp = ReadSignedDisplacement(dasm.rdr);
        if (disp is null)
            return false;
        dasm.ops.Add(Constant.Create(PrimitiveType.Word32, disp.Value));
        return true;
    }

    private static Mutator<Ns32kDisassembler> DatatypeSetter(PrimitiveType dtWord, PrimitiveType dtSigned)
    {
        return (u, d) => 
        {
            d.dataSize = dtWord;
            d.signedDataSize = dtSigned;
            return true;
        };
    }
    private static readonly Mutator<Ns32kDisassembler> dtByte = DatatypeSetter(PrimitiveType.Byte, PrimitiveType.Int8);
    private static readonly Mutator<Ns32kDisassembler> dtWord = DatatypeSetter(PrimitiveType.Word16, PrimitiveType.Int16);
    private static readonly Mutator<Ns32kDisassembler> dtDword = DatatypeSetter(PrimitiveType.Word32, PrimitiveType.Int32);

    private static bool cinvOptions(uint uInstr, Ns32kDisassembler dasm)
    {
        List<char> opts = [];
        if (Bits.IsBitSet(uInstr, 15))
            opts.Add('d');
        if (Bits.IsBitSet(uInstr, 16))
            opts.Add('i');
        if (Bits.IsBitSet(uInstr, 17))
            opts.Add('a');
        if (opts.Count == 0)
            return false;
        dasm.ops.Add(new LiteralOperand(string.Join(',', opts)));
        return true;
    }

    private static bool cmpsOptions(uint uInstr, Ns32kDisassembler dasm)
    {
        List<char> opts = [];
        if (Bits.IsBitSet(uInstr, 16))
            opts.Add('b');
        switch (bf17l2.Read(uInstr))
        {
        case 0b01:
            opts.Add('w');
            break;
        case 0b11:
            opts.Add('u');
            break;
        }
        if (opts.Count == 0)
            return true;
        dasm.ops.Add(new LiteralOperand(string.Join(',', opts)));
        return true;
    }

    private static bool cfglist(uint uInstr, Ns32kDisassembler dasm)
    {
        List<char> opts = [];
        if (Bits.IsBitSet(uInstr, 15))
            opts.Add('i');
        if (Bits.IsBitSet(uInstr, 16))
            opts.Add('f');
        if (Bits.IsBitSet(uInstr, 17))
            opts.Add('m');
        if (Bits.IsBitSet(uInstr, 18))
            opts.Add('c');
        dasm.ops.Add(new LiteralOperand($"[{string.Join(',', opts)}]"));
        return true;
    }

    private static bool mregs(uint uInstr, Ns32kDisassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte mregByte))
            return false;
        dasm.ops.Add(new RegisterSetOperand(mregByte));
        return true;
    }

    private static bool mregsinv(uint uInstr, Ns32kDisassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte mregByte))
            return false;
        mregByte = Bits.ReverseBits(mregByte);
        dasm.ops.Add(new RegisterSetOperand(mregByte));
        return true;
    }

    /// <summary>
    /// Unpacks a byte immediate value into field values for the 
    /// <c>extsI</c> instruction.
    /// </summary>
    /// <returns></returns>
    private static bool shortField(uint uInstr, Ns32kDisassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte fieldEncoding))
            return false;
        var offset = fieldEncoding >> 5;
        var length = (fieldEncoding & 0b11111) + 1;
        dasm.ops.Add(Constant.Byte((byte) offset));
        dasm.ops.Add(Constant.Byte((byte) length));
        return true;
    }

    private class Instr16Decoder : InstrDecoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction>
    {
        public Instr16Decoder(InstrClass instrClass, Mnemonic mnemonic, Mutator<Ns32kDisassembler>[] mutators)
            : base(instrClass, mnemonic, mutators)
        {
        }

        public override Ns32kInstruction Decode(uint uInstr, Ns32kDisassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte secondByte))
                return dasm.CreateInvalidInstruction();
            uInstr = uInstr | ((uint)secondByte << 8);
            return base.Decode(uInstr, dasm);
        }
    }

    private class Next8Decoder : Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction>
    {
        private readonly Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> decoder;

        public Next8Decoder(Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> decoder)
        {
            this.decoder = decoder;
        }

        public override Ns32kInstruction Decode(uint wInstr, Ns32kDisassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte top8))
                return dasm.CreateInvalidInstruction();
            wInstr = wInstr | ((uint) top8 << 8);
            return decoder.Decode(wInstr, dasm);
        }
    }

    private class Next16Decoder : Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction>
    {
        private readonly Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> decoder;

        public Next16Decoder(Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> decoder)
        {
            this.decoder = decoder;
        }

        public override Ns32kInstruction Decode(uint wInstr, Ns32kDisassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt16(out ushort top16))
                return dasm.CreateInvalidInstruction();
            wInstr = wInstr | ((uint)top16 << 8);
            return decoder.Decode(wInstr, dasm);
        }
    }

    /// <summary>
    /// Creates a decoder that reads an additional 8 bits after the first 8 bits of an instruction,
    /// resulting in a 16bit instruction.
    /// </summary>
    private static Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> Read16(
        Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> decoder)
    {
        return new Next8Decoder(decoder);
    }

    /// <summary>
    /// Creates a decoder that reads an additional 16 bits after the first 8 bits of an instruction,
    /// resulting in a 24-bit instruction.
    /// </summary>
    private static Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> Read24(
        Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> decoder)
    {
        return new Next16Decoder(decoder);
    }

    /// <summary>
    /// Decodes an instruction.
    /// </summary>
    private static Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> Instr(
        Mnemonic mnemonic,
        params Mutator<Ns32kDisassembler>[] mutators)
    {
        return new InstrDecoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> (InstrClass.Linear, mnemonic, mutators);
    }


    /// <summary>
    /// Decodes an instruction.
    /// </summary>
    private static Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> Instr(
        Mnemonic mnemonic,
        InstrClass iclass,
        params Mutator<Ns32kDisassembler>[] mutators)
    {
        return new InstrDecoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction>(InstrClass.Linear, mnemonic, mutators);
    }
    /// <summary>
    /// Decodes a 16-bit instruction.
    /// </summary>
    private static Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> Instr16(
        Mnemonic mnemonic,
        params Mutator<Ns32kDisassembler>[] mutators)
    {
        return new Instr16Decoder(InstrClass.Linear, mnemonic, mutators);
    }

    /// <summary>
    /// Decodes a 16-bit instruction.
    /// </summary>
    private static Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> Instr16(
        Mnemonic mnemonic,
        InstrClass instrClass,
        params Mutator<Ns32kDisassembler>[] mutators)
    {
        return new Instr16Decoder(instrClass, mnemonic, mutators);
    }

    /// <summary>
    /// Decodes an instruction with byte, word, and dword variants.
    /// </summary>
    private static Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> Instr_I(
        Mnemonic mnb,
        Mnemonic mnw,
        Mnemonic mnd,
        params Mutator<Ns32kDisassembler>[] m)
    {
        return Mask(0, 2, mnb.ToString(),
            Instr(mnb, AddDatatypeMutator(m, PrimitiveType.Byte, PrimitiveType.Int8)),
            Instr(mnw, AddDatatypeMutator(m, PrimitiveType.Word16, PrimitiveType.Int16)),
            Instr(Mnemonic.Invalid, InstrClass.Invalid, Array.Empty<Mutator<Ns32kDisassembler>>()),
            Instr(mnd, AddDatatypeMutator(m, PrimitiveType.Word32, PrimitiveType.Int32)));
    }

    /// <summary>
    /// Decodes a 16-bit instruction integer instruction with byte, word, and dword variants.
    /// </summary>
    private static Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> Instr16_I(
        Mnemonic mnb,
        Mnemonic mnw,
        Mnemonic mnd,
        params Mutator<Ns32kDisassembler>[] m)
    {
        return Mask(0, 2, mnb.ToString(),
            Instr16(mnb, AddDatatypeMutator(m, PrimitiveType.Byte, PrimitiveType.Int8)),
            Instr16(mnw, AddDatatypeMutator(m, PrimitiveType.Word16, PrimitiveType.Int16)),
            Instr16(Mnemonic.Invalid, InstrClass.Invalid, Array.Empty<Mutator<Ns32kDisassembler>>()),
            Instr16(mnd, AddDatatypeMutator(m, PrimitiveType.Word32, PrimitiveType.Int32)));
    }

    /// <summary>
    /// Decodes a 16-bit instruction integer instruction with byte, word, and dword variants.
    /// </summary>
    private static Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> Instr16_I(
        InstrClass iclass,
        Mnemonic mnb,
        Mnemonic mnw,
        Mnemonic mnd,
        params Mutator<Ns32kDisassembler>[] m)
    {
        return Mask(0, 2, mnb.ToString(),
            Instr16(mnb, iclass, AddDatatypeMutator(m, PrimitiveType.Byte, PrimitiveType.Int8)),
            Instr16(mnw, iclass, AddDatatypeMutator(m, PrimitiveType.Word16, PrimitiveType.Int16)),
            Instr16(Mnemonic.Invalid, InstrClass.Invalid, Array.Empty<Mutator<Ns32kDisassembler>>()),
            Instr16(mnd, iclass, AddDatatypeMutator(m, PrimitiveType.Word32, PrimitiveType.Int32)));
    }

    /// <summary>
    /// Decodes a 24-bit instruction integer instruction with byte, word, and dword variants.
    /// </summary>
    private static Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> Instr24_I(
        Mnemonic mnb,
        Mnemonic mnw,
        Mnemonic mnd,
        params Mutator<Ns32kDisassembler>[] m)
    {
        return Mask(8, 2, mnb.ToString(),
            Instr(mnb, AddDatatypeMutator(m, PrimitiveType.Byte, PrimitiveType.Int8)),
            Instr(mnw, AddDatatypeMutator(m, PrimitiveType.Word16, PrimitiveType.Int16)),
            Instr(Mnemonic.Invalid, InstrClass.Invalid),
            Instr(mnd, AddDatatypeMutator(m, PrimitiveType.Word32, PrimitiveType.Int32)));
    }


    /// <summary>
    /// Decodes a 24-bit instruction shift instruction with byte, word, and dword variants.
    /// </summary>
    private static Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> Shift24(
        Mnemonic mnb,
        Mnemonic mnw,
        Mnemonic mnd,
        params Mutator<Ns32kDisassembler>[] m)
    {
        return Mask(8, 2, mnb.ToString(),
            Instr(mnb, dtByte, gen19_5, dtByte, gen14_5),
            Instr(mnw, dtByte, gen19_5, dtWord, gen14_5),
            Instr(Mnemonic.Invalid, InstrClass.Invalid),
            Instr(mnd, dtByte, gen19_5, dtDword, gen14_5));
    }

    /// <summary>
    /// Decodes a 24-bit instruction floating point instruction with 32- and 64-bit variants.
    /// </summary>
    private static Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> Instr_F(
        Mnemonic mnf,
        Mnemonic mnl,
        params Mutator<Ns32kDisassembler>[] m)
    {
        return Mask(8, 1, mnf.ToString(),
            Instr(mnl, AddDatatypeMutator(m, PrimitiveType.Real64, PrimitiveType.Real64)),
            Instr(mnf, AddDatatypeMutator(m, PrimitiveType.Real32, PrimitiveType.Real32)));
    }

    private static Mutator<Ns32kDisassembler>[] AddDatatypeMutator(
        Mutator<Ns32kDisassembler>[] m,
        PrimitiveType dtWord,
        PrimitiveType dtSigned)
    {
        var result = new Mutator<Ns32kDisassembler>[m.Length + 1];
        result[0] = DatatypeSetter(dtWord, dtSigned); 
        Array.Copy(m, 0, result, 1, m.Length);
        return result;
    }

    private static Decoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction> Nyi(string message)
    {
        return new NyiDecoder<Ns32kDisassembler, Mnemonic, Ns32kInstruction>(message);
    }

    static Ns32kDisassembler()
    {
        var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid, Array.Empty<Mutator<Ns32kDisassembler>>());

        var acbi =Instr16_I(Mnemonic.acbb, Mnemonic.acbw, Mnemonic.acbd, quick7_4, gen11_5, jdisp);
        var addi = Instr16_I(Mnemonic.addb, Mnemonic.addw, Mnemonic.addd, gen11_5, gen6_5);
        var addci = Instr16_I(Mnemonic.addcb, Mnemonic.addcw, Mnemonic.addcd, gen11_5, gen6_5);
        var addqi = Instr16_I(Mnemonic.addqb, Mnemonic.addqw, Mnemonic.addqd, quick7_4, gen11_5);
        var adjspi = Instr_I(Mnemonic.adjspb, Mnemonic.adjspw, Mnemonic.adjspd, gen11_5);
        var andi = Instr16_I(Mnemonic.andb, Mnemonic.andw, Mnemonic.andd, gen11_5, gen6_5);
        var addr = Read16(Instr(Mnemonic.addr, gen11_5, gen6_5));
        var bici = Instr16_I(Mnemonic.bicb, Mnemonic.bicw, Mnemonic.bicd, gen11_5, gen6_5);
        var casei = Instr_I(Mnemonic.caseb, Mnemonic.casew, Mnemonic.cased, gen11_5);
        var cmpi = Instr16_I(Mnemonic.cmpb, Mnemonic.cmpw, Mnemonic.cmpd, gen11_5, gen6_5);
        var cmpqi = Instr16_I(Mnemonic.cmpqb, Mnemonic.cmpqw, Mnemonic.cmpqd, quick7_4, gen11_5);
        var lpri = Instr16_I(Mnemonic.lprb, Mnemonic.lprw, Mnemonic.lprd, procreg, gen11_5);
        var movi = Instr16_I(Mnemonic.movb, Mnemonic.movw, Mnemonic.movd, gen11_5, gen6_5);
        var movqi = Instr16_I(Mnemonic.movqb, Mnemonic.movqw, Mnemonic.movqd, quick7_4, gen11_5);
        var ori = Instr16_I(Mnemonic.orb, Mnemonic.orw, Mnemonic.ord, gen11_5, gen6_5);
        var spri = Instr16_I(InstrClass.Linear|InstrClass.Privileged, Mnemonic.sprb, Mnemonic.sprw, Mnemonic.sprd, procreg, gen11_5);
        var subci = Instr16_I(Mnemonic.subcb, Mnemonic.subcw, Mnemonic.subcd, gen11_5, gen6_5);
        var subi = Instr16_I(Mnemonic.subb, Mnemonic.subw, Mnemonic.subd, gen11_5, gen6_5);
        var tbiti = Instr16_I(Mnemonic.tbitb, Mnemonic.tbitw, Mnemonic.tbitd, gen11_5, gen6_5);
        var xori = Instr16_I(Mnemonic.xorb, Mnemonic.xorw, Mnemonic.xord, gen11_5, gen6_5);

        var decode0011 = Sparse(0, 8, "  0011", Nyi("0011"),
            (0b00001100, Instr16_I(Mnemonic.addqb, Mnemonic.addqw, Mnemonic.addqd, quick7_4, gen11_5)),
            (0b10001100, Instr16_I(Mnemonic.addqb, Mnemonic.addqw, Mnemonic.addqd, quick7_4, gen11_5)),
            (0b11001100, Instr16_I(Mnemonic.acbb, Mnemonic.acbw, Mnemonic.acbd, quick7_4, gen11_5, jdisp)));

        var decode0E = Read24(Sparse(10, 5, "  00001110", Nyi("00001110"),
            (0b00000, Mask(15, 1, "  00001",
                Instr24_I(Mnemonic.movsb, Mnemonic.movsw, Mnemonic.movsd, cmpsOptions),
                Instr(Mnemonic.movst, cmpsOptions))),
            (0b00001, Mask(15, 1, "  00001",
                Instr24_I(Mnemonic.cmpsb, Mnemonic.cmpsw, Mnemonic.cmpsd, cmpsOptions),
                Instr(Mnemonic.cmpst, cmpsOptions))),
            (0b00010, Mask(8, 2, "  00010",
                Nyi("00"),
                Nyi("01"),
                Nyi("10"),
                Instr(Mnemonic.setcfg, cfglist))),
            (0b00011, Mask(15, 1, "  00001",
                Instr24_I(Mnemonic.skpsb, Mnemonic.skpsw, Mnemonic.skpsd, cmpsOptions),
                Instr(Mnemonic.skpst, cmpsOptions)))));

        var decode1E = Read24(Sparse(8, 7, "  00011110", Nyi("00011110"),
            (0b0000011, Sparse(15, 4, "  0000011", Nyi("0000011"),
                (0b0000, Instr(Mnemonic.rdval, InstrClass.Linear|InstrClass.Privileged, gen19_5)))),
            (0b0000111, Sparse(15, 4, "  0000111", Nyi("0000111"),
                (0b0000, Instr(Mnemonic.wrval, InstrClass.Linear | InstrClass.Privileged, gen19_5)))),
            (0b0001011, Instr(Mnemonic.lmr, InstrClass.Linear | InstrClass.Privileged, mmureg, gen19_5)),
            (0b0001111, Instr(Mnemonic.smr, InstrClass.Linear | InstrClass.Privileged, mmureg, gen19_5)),
            (0b0100111, Instr(Mnemonic.cinv, InstrClass.Linear | InstrClass.Privileged, cinvOptions, gen19_5))));

        var decode2E = Read24(Mask(10, 1, "  00101110",
            Instr24_I(Mnemonic.extb, Mnemonic.extw, Mnemonic.extd, imm11_3, gen19_5, gen14_5, disp),
            Instr24_I(Mnemonic.indexb, Mnemonic.indexw, Mnemonic.indexd, reg11, gen19_5, gen14_5)));

        var decode3E = Read24(Sparse(10, 4, "  00111110", Nyi("00111110"),
            (0b0000, Instr24_I(Mnemonic.movbl, Mnemonic.movwl, Mnemonic.movdl, fgen19_5, fgen14_5)),
            (0b0001, Instr24_I(Mnemonic.movbf, Mnemonic.movwf, Mnemonic.movdf, fgen19_5, fgen14_5)),
            (0b0011, Sparse(8, 11, "  0011", Nyi("0011"),
                (0b00000001111, Instr(Mnemonic.lfsr, gen19_5)))),
            (0b0101, Sparse(8, 6, "  0101", Nyi("0101"),
                (0b010110, Instr(Mnemonic.movlf, fgen19_5, fgen14_5)))),
            (0b0110, Sparse(8, 6, "  0110", Nyi("0110"),
                (0b011011, Instr(Mnemonic.movfl, fgen19_5, fgen14_5)))),
            (0b1000, Instr24_I(Mnemonic.roundlb, Mnemonic.roundlw, Mnemonic.roundld, fgen19_5, gen14_5)),
            (0b1001, Instr24_I(Mnemonic.roundfb, Mnemonic.roundfw, Mnemonic.roundfd, fgen19_5, gen14_5)),
            (0b1010, Instr24_I(Mnemonic.trunclb, Mnemonic.trunclw, Mnemonic.truncld, fgen19_5, gen14_5)),
            (0b1011, Instr24_I(Mnemonic.truncfb, Mnemonic.truncfw, Mnemonic.truncfd, fgen19_5, gen14_5)),
            (0b1101, Instr(Mnemonic.sfsr, dtDword, gen14_5)),
            (0b1110, Instr24_I(Mnemonic.floorlb, Mnemonic.floorlw, Mnemonic.floorld, fgen19_5, gen14_5)),
            (0b1111, Instr24_I(Mnemonic.floorfb, Mnemonic.floorfw, Mnemonic.floorfd, fgen19_5, gen14_5))));

        var decode4E = Read24(Sparse(10, 4, "  01001110", Nyi("01001110"),
            (0b0000, Shift24(Mnemonic.rotb, Mnemonic.rotw, Mnemonic.rotd, gen19_5, gen14_5)),
            (0b0001, Shift24(Mnemonic.ashb, Mnemonic.ashw, Mnemonic.ashd, gen19_5, gen14_5)),
            (0b0010, Instr24_I(Mnemonic.cbitb, Mnemonic.cbitw, Mnemonic.cbitd, gen19_5, gen14_5)),
            (0b0011, Instr24_I(Mnemonic.cbitib, Mnemonic.cbitiw, Mnemonic.cbitid, gen19_5, gen14_5)),
            (0b0101, Shift24(Mnemonic.lshb, Mnemonic.lshw, Mnemonic.lshd, gen19_5, gen14_5)),
            (0b0110, Instr24_I(Mnemonic.sbitb, Mnemonic.sbitw, Mnemonic.sbitd, gen19_5, gen14_5)),
            (0b0111, Instr24_I(Mnemonic.sbitib, Mnemonic.sbitiw, Mnemonic.sbitid, gen19_5, gen14_5)),
            (0b1000, Instr24_I(Mnemonic.negb, Mnemonic.negw, Mnemonic.negd, gen19_5, gen14_5)),
            (0b1001, Instr24_I(Mnemonic.notb, Mnemonic.notw, Mnemonic.notd, gen19_5, gen14_5)),
            (0b1011, Instr24_I(Mnemonic.subpb, Mnemonic.subpw, Mnemonic.subpd, gen19_5, gen14_5)),
            (0b1100, Instr24_I(Mnemonic.absb, Mnemonic.absw, Mnemonic.absd, gen19_5, gen14_5)),
            (0b1101, Instr24_I(Mnemonic.comb, Mnemonic.comw, Mnemonic.comd, gen19_5, gen14_5)),
            (0b1110, Instr24_I(Mnemonic.ibitb, Mnemonic.ibitw, Mnemonic.ibitd, gen19_5, gen14_5)),
            (0b1111, Instr24_I(Mnemonic.addpb, Mnemonic.addpw, Mnemonic.addpd, gen19_5, gen14_5))));

        var decode6E = Read24(Mask(8, 3, "  01101110",
            Nyi("000"),
            Nyi("001"),
            Nyi("010"),
            Instr(Mnemonic.cvtp, reg11, gen19_5, gen14_5),
            Instr24_I(Mnemonic.ffsb, Mnemonic.ffsw, Mnemonic.ffsd, gen19_5, dtByte, gen14_5),
            Nyi("101"),
            Nyi("110"),
            Nyi("111")));

        var decode7C = Read16(Sparse(8, 3, "  01111100", Nyi("01111100"),
            (0b001, Instr(Mnemonic.bicpsrb, dtByte,  gen11_5)),
            (0b101, adjspi),
            (0b111, casei)));

        var decode7D = Read16(Sparse(8, 3, "  01111101", Nyi("01111101"),
            (0b001, Instr(Mnemonic.bicpsrw, dtWord, gen11_5)),
            (0b101, adjspi),
            (0b111, casei)));

        var decode7E = Read16(Sparse(8, 3, "  01111110", Nyi("01111110"),
            (0b111, casei)));

        var decode7F = Read16(Sparse(8, 3, "  01111111", Nyi("01111111"),
            (0b000, Instr(Mnemonic.cxpd, gen11_5)),
            (0b010, Instr(Mnemonic.jump, InstrClass.Transfer, gen11_5)),
            (0b101, adjspi),
            (0b110, Instr(Mnemonic.jsr, InstrClass.Transfer | InstrClass.Call, gen11_5)),
            (0b111, casei)));

        var decodeAE = Read24(Mask(10, 1, "  10101110",
            Instr24_I(Mnemonic.insb, Mnemonic.insw, Mnemonic.insd, reg11, gen19_5, gen14_5, disp),
            Mask(11, 3, "  1",
                Nyi("000"),
                Instr24_I(Mnemonic.movsub, Mnemonic.movsuw, Mnemonic.movsud, gen19_5, gen14_5),
                Nyi("010"),
                Instr24_I(Mnemonic.movusb, Mnemonic.movusw, Mnemonic.movusd, gen19_5, gen14_5),
                Nyi("100"),
                Nyi("101"),
                Nyi("110"),
                Nyi("111"))));

        var decodeBE = Read24(Sparse(9, 5, "  10111110", Nyi("10111110"),
            (0b00000, Instr_F(Mnemonic.addf, Mnemonic.addl, fgen19_5, fgen14_5)),
            (0b00010, Instr_F(Mnemonic.movf, Mnemonic.movl, fgen19_5, fgen14_5)),
            (0b00100, Instr_F(Mnemonic.cmpf, Mnemonic.cmpl, fgen19_5, fgen14_5)),
            (0b01000, Instr_F(Mnemonic.subf, Mnemonic.subl, fgen19_5, fgen14_5)),
            (0b01010, Instr_F(Mnemonic.negf, Mnemonic.negl, fgen19_5, fgen14_5)),
            (0b10000, Instr_F(Mnemonic.divf, Mnemonic.divl, fgen19_5, fgen14_5)),
            (0b11000, Instr_F(Mnemonic.mulf, Mnemonic.mull, fgen19_5, fgen14_5)),
            (0b11010, Instr_F(Mnemonic.absf, Mnemonic.absl, fgen19_5, fgen14_5))));

        var decodeCE = Read24(Sparse(10, 4, "  11011110", Nyi("11011110"),
            (0b0000, Instr24_I(Mnemonic.movmb, Mnemonic.movmw, Mnemonic.movmd, gen19_5, gen14_5, disp)),
            (0b0001, Instr24_I(Mnemonic.cmpmb, Mnemonic.cmpmw, Mnemonic.cmpmd, gen19_5, gen14_5, disp)),
            (0b0010, Instr24_I(Mnemonic.inssb, Mnemonic.inssw, Mnemonic.inssd, gen19_5, gen14_5, shortField)),
            (0b0011, Instr24_I(Mnemonic.extsb, Mnemonic.extsw, Mnemonic.extsd, gen19_5, gen14_5, shortField)),
            (0b0100, Mask(8, 2, "  0100",
                Instr(Mnemonic.movxbw, dtByte, gen19_5, dtWord, gen14_5),
                Nyi("01"),
                Nyi("10"),
                Nyi("11"))),
            (0b0101, Mask(8, 2, "  0101",
                Instr(Mnemonic.movzbw, dtByte, gen19_5, dtWord, gen14_5),
                Nyi("01"),
                Nyi("10"),
                Nyi("11"))),
            (0b0110, Mask(8, 2, "  0110",
                Instr(Mnemonic.movzbd, dtByte, gen19_5, dtDword, gen14_5),
                Instr(Mnemonic.movzwd, dtByte, gen19_5, dtDword, gen14_5),
                Nyi("10"),
                Nyi("11"))),
            (0b0111, Mask(8, 2, "  0111",
                Instr(Mnemonic.movxbd, dtByte, gen19_5, dtDword, gen14_5),
                Instr(Mnemonic.movxwd, dtWord, gen19_5, dtDword, gen14_5),
                Nyi("10"),
                Nyi("11"))),
            (0b1000, Instr24_I(Mnemonic.mulb, Mnemonic.mulw, Mnemonic.muld, gen19_5, gen14_5)),
            (0b1001, Instr24_I(Mnemonic.meib, Mnemonic.meiw, Mnemonic.meid, gen19_5, gen14_5)),
            (0b1011, Instr24_I(Mnemonic.deib, Mnemonic.deiw, Mnemonic.deid, gen19_5, gen14_5)),
            (0b1100, Instr24_I(Mnemonic.quob, Mnemonic.quow, Mnemonic.quod, gen19_5, gen14_5)),
            (0b1101, Instr24_I(Mnemonic.remb, Mnemonic.remw, Mnemonic.remd, gen19_5, gen14_5)),
            (0b1110, Instr24_I(Mnemonic.modb, Mnemonic.modw, Mnemonic.modd, gen19_5, gen14_5)),
            (0b1111, Instr24_I(Mnemonic.divb, Mnemonic.divw, Mnemonic.divd, gen19_5, gen14_5))));

        var decodeEE = Read24(Mask(10, 1, "  11101110",
            Instr24_I(Mnemonic.checkb, Mnemonic.checkw, Mnemonic.checkd, reg11, gen19_5, gen14_5),
            Nyi("1")));

        var decodeFE = Read24(Sparse(9, 5, "  11111110", Nyi("11111110"),
            (0b00100, Instr_F(Mnemonic.polyf, Mnemonic.polyl, fgen19_5, fgen14_5)),
            (0b00110, Instr_F(Mnemonic.dotf, Mnemonic.dotl, fgen19_5, fgen14_5)),
            (0b01000, Instr_F(Mnemonic.scalbf, Mnemonic.scalbl, fgen19_5, fgen14_5)),
            (0b01010, Instr_F(Mnemonic.logbf, Mnemonic.logbl, fgen19_5, fgen14_5))));

        var bcond = Mask(4, 4, "  bcond",
            Instr(Mnemonic.beq, InstrClass.ConditionalTransfer, jdisp),
            Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, jdisp),
            Instr(Mnemonic.bcs, InstrClass.ConditionalTransfer, jdisp),
            Instr(Mnemonic.bcc, InstrClass.ConditionalTransfer, jdisp),

            Instr(Mnemonic.bhi, InstrClass.ConditionalTransfer, jdisp),
            Instr(Mnemonic.bls, InstrClass.ConditionalTransfer, jdisp),
            Instr(Mnemonic.bgt, InstrClass.ConditionalTransfer, jdisp),
            Instr(Mnemonic.ble, InstrClass.ConditionalTransfer, jdisp),

            Instr(Mnemonic.bfs, InstrClass.ConditionalTransfer, jdisp),
            Instr(Mnemonic.bfc, InstrClass.ConditionalTransfer, jdisp),
            Instr(Mnemonic.blo, InstrClass.ConditionalTransfer, jdisp),
            Instr(Mnemonic.bhs, InstrClass.ConditionalTransfer, jdisp),

            Instr(Mnemonic.blt, InstrClass.ConditionalTransfer, jdisp),
            Instr(Mnemonic.bge, InstrClass.ConditionalTransfer, jdisp),
            invalid,
            invalid);

        var scond = Read16(Mask(7, 4, "  scond",
            Instr_I(Mnemonic.seqb, Mnemonic.seqw, Mnemonic.seqd, gen11_5),
            Instr_I(Mnemonic.sneb, Mnemonic.snew, Mnemonic.sned, gen11_5),
            Instr_I(Mnemonic.scsb, Mnemonic.scsw, Mnemonic.scsd, gen11_5),
            Instr_I(Mnemonic.sccb, Mnemonic.sccw, Mnemonic.sccd, gen11_5),

            Instr_I(Mnemonic.shib, Mnemonic.shiw, Mnemonic.shid, gen11_5),
            Instr_I(Mnemonic.slsb, Mnemonic.slsw, Mnemonic.slsd, gen11_5),
            Instr_I(Mnemonic.sgtb, Mnemonic.sgtw, Mnemonic.sgtd, gen11_5),
            Instr_I(Mnemonic.sleb, Mnemonic.slew, Mnemonic.sled, gen11_5),

            Instr_I(Mnemonic.sfsb, Mnemonic.sfsw, Mnemonic.sfsd, gen11_5),
            Instr_I(Mnemonic.sfcb, Mnemonic.sfcw, Mnemonic.sfcd, gen11_5),
            Instr_I(Mnemonic.slob, Mnemonic.slow, Mnemonic.slod, gen11_5),
            Instr_I(Mnemonic.shsb, Mnemonic.shsw, Mnemonic.shsd, gen11_5),

            Instr_I(Mnemonic.sltb, Mnemonic.sltw, Mnemonic.sltd, gen11_5),
            Instr_I(Mnemonic.sgeb, Mnemonic.sgew, Mnemonic.sged, gen11_5),
            invalid,
            invalid));

        rootDecoder = Mask(0, 8, "NS32k",
            // 00
            addi,
            addi,
            Instr(Mnemonic.bsr, InstrClass.Transfer|InstrClass.Call, jdisp),
            addi,

            cmpi,
            Nyi("00000101"),
            cmpi,
            cmpi,

            bici,
            bici,
            bcond,
            bici,

            addqi,
            addqi,
            decode0E,
            addqi,

            // 10
            addci,
            addci,
            Instr(Mnemonic.ret, InstrClass.Transfer|InstrClass.Return, disp),
            addci,

            movi,
            movi,
            Nyi("00010110"),
            movi,

            ori,
            ori,
            bcond,
            ori,

            cmpqi,
            cmpqi,
            decode1E,
            cmpqi,

            // 20
            subi,
            subi,
            Instr(Mnemonic.cxp, disp),
            subi,

            Nyi("00100100"),
            Nyi("00100101"),
            Nyi("00100110"),
            addr,

            andi,
            andi,
            bcond,
            andi,

            spri,
            spri,
            decode2E,
            spri,

            // 30
            subci,
            subci,
            Instr(Mnemonic.rxp, InstrClass.Transfer | InstrClass.Return, disp),
            subci,

            tbiti,
            tbiti,
            Nyi("00110110"),
            tbiti,

            xori,
            xori,
            bcond,
            xori,

            scond,
            scond,
            decode3E,
            scond,

            // 40
            addi,
            addi,
            Instr(Mnemonic.rett, InstrClass.Transfer | InstrClass.Return | InstrClass.Privileged, disp),
            addi,

            cmpi,
            cmpi,
            Nyi("01000101"),
            cmpi,

            bici,
            bici,
            bcond,
            bici,

            acbi,
            acbi,
            decode4E,
            acbi,

            // 50
            addci,
            addci,
            Instr(Mnemonic.reti, InstrClass.Transfer | InstrClass.Return | InstrClass.Privileged),
            addci,

            movi,
            movi,
            Nyi("01010110"),
            movi,

            ori,
            ori,
            bcond,
            ori,

            movqi,
            movqi,
            Nyi("00011110"),
            movqi,

            // 60
            subi,
            subi,
            Instr(Mnemonic.save, mregs),
            subi,

            Nyi("01100100"),
            Nyi("01100101"),
            Nyi("01100110"),
            addr,

            andi,
            andi,
            bcond,
            andi,

            lpri,
            lpri,
            decode6E,
            lpri,

            // 70
            subci,
            subci,
            Instr(Mnemonic.restore, mregsinv),
            subci,

            tbiti,
            tbiti,
            Nyi("01110110"),
            tbiti,

            xori,
            xori,
            bcond,
            xori,

            decode7C,
            decode7D,
            decode7E,
            decode7F,

            // 80
            addi,
            addi,
            Instr(Mnemonic.enter, mregs, disp),
            addi,

            cmpi,
            Nyi("10000101"),
            cmpi,
            cmpi,

            bici,
            bici,
            bcond,
            bici,

            addqi,
            addqi,
            Nyi("10001110"),
            addqi,

            // 90
            addci,
            addci,
            Instr(Mnemonic.exit, mregsinv),
            addci,

            movi,
            movi,
            Nyi("10010110"),
            movi,

            ori,
            ori,
            bcond,
            ori,

            cmpqi,
            cmpqi,
            Nyi("10011110"),
            cmpqi,

            // A0
            subi,
            subi,
            Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding),
            subi,

            Nyi("10100100"),
            Nyi("10100101"),
            Nyi("10100110"),
            addr,

            andi,
            andi,
            bcond,
            andi,

            spri,
            spri,
            decodeAE,
            spri,

            // B0
            subci,
            subci,
            Instr(Mnemonic.wait),
            subci,

            tbiti,
            tbiti,
            Nyi("10110110"),
            tbiti,

            xori,
            xori,
            bcond,
            xori,

            scond,
            scond,
            decodeBE,
            scond,

            // C0
            addi,
            addi,
            Instr(Mnemonic.dia),
            addi,

            cmpi,
            cmpi,
            Nyi("11000110"),
            cmpi,

            bici,
            bici,
            bcond,
            bici,

            acbi,
            acbi,
            decodeCE,
            acbi,

            // D0
            addci,
            Instr(Mnemonic.flag),
            addci,
            addci,

            movi,
            movi,
            Nyi("11010110"),
            movi,

            ori,
            ori,
            bcond,
            ori,

            movqi,
            movqi,
            Nyi("11011110"),
            movqi,

            // E0
            subi,
            subi,
            Instr(Mnemonic.svc),
            subi,

            Nyi("11100100"),
            Nyi("11100101"),
            Nyi("11100110"),
            addr,

            andi,
            andi,
            Instr(Mnemonic.br, InstrClass.Transfer, jdisp),
            andi,

            lpri,
            lpri,
            decodeEE,
            lpri,

            // F0
            subci,
            subci,
            Instr(Mnemonic.bpt),
            subci,

            tbiti,
            tbiti,
            Nyi("11110110"),
            tbiti,

            xori,
            xori,
            Nyi("11111010"),
            xori,

            Nyi("11111100"),
            Nyi("11111101"),
            decodeFE,
            Nyi("11111111"));
    }
}
