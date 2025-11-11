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
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Reko.Arch.Kalimba;

internal class KalimbaDisassembler : DisassemblerBase<KalimbaInstruction, Mnemonic>
{
    private static readonly Decoder<KalimbaDisassembler , Mnemonic, KalimbaInstruction> rootDecoder;
    private static readonly Bitfield bf0_2 = new Bitfield(0, 2);
    private static readonly Bitfield bf0_4 = new Bitfield(0, 4);
    private static readonly Bitfield bf0_16 = new Bitfield(0, 16);
    private static readonly Bitfield bf2_2 = new Bitfield(2, 2);
    private static readonly Bitfield bf4_3 = new Bitfield(4, 3);
    private static readonly Bitfield bf4_4 = new Bitfield(4, 4);
    private static readonly Bitfield bf8_2 = new Bitfield(10, 2);
    private static readonly Bitfield bf10_2 = new Bitfield(10, 2);
    private static readonly Bitfield bf12_3 = new Bitfield(12, 3);
    private static readonly Bitfield bf16_4 = new Bitfield(16, 4);
    private static readonly Bitfield bf20_4 = new Bitfield(20, 4);
    private static readonly Bitfield bf26_2 = new Bitfield(26, 2);

    private static readonly Constant[] modifyConstants = [
        Constant.Create(KalimbaArchitecture.Int24, -1),
        Constant.Create(KalimbaArchitecture.Int24, 0),
        Constant.Create(KalimbaArchitecture.Int24, 1),
        Constant.Create(KalimbaArchitecture.Int24, 2),
        ];

    private readonly KalimbaArchitecture arch;
    private readonly EndianImageReader rdr;
    private readonly List<MachineOperand> ops;
    private Address addr;
    private uint? signSelect;
    private CCode ccode;

    public KalimbaDisassembler(KalimbaArchitecture kalimbaArchitecture, EndianImageReader imageReader)
    {
        this.arch = kalimbaArchitecture;
        this.rdr = imageReader;
        this.ops = [];
    }

    public override KalimbaInstruction? DisassembleInstruction()
    {
        this.addr = rdr.Address;
        if (!rdr.TryReadUInt32(out var uInstr))
            return null;
        this.ccode = CCode.Always;
        var instr = rootDecoder.Decode(uInstr, this);
        if (uInstr == 0)
            instr.InstructionClass |= InstrClass.Zero;
        instr.Length = (int) (rdr.Address - this.addr);
        ops.Clear();
        signSelect = null;
        return instr;
    }

    public override KalimbaInstruction CreateInvalidInstruction()
    {
        return new KalimbaInstruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.Invalid,
            Condition = CCode.Always,
            SignSelect = this.signSelect,
        };
    }

    public override KalimbaInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
    {
        return new KalimbaInstruction
        {
            Mnemonic = mnemonic,
            InstructionClass = iclass,
            SignSelect = this.signSelect,
            Condition = this.ccode,
            Operands = ops.ToArray(),
            Address = this.addr,
        };
    }

    public override KalimbaInstruction NotYetImplemented(string message)
    {
        var svc = arch.Services.GetService<ITestGenerationService>();
        svc?.ReportMissingDecoder("KalimbaDis", this.addr, this.rdr, message);
        return CreateInvalidInstruction();
    }

    private static bool RegA(uint uInstr, KalimbaDisassembler dasm)
    {
        var r = bf16_4.Read(uInstr);
        var reg = Registers.ByDomain[(StorageDomain) r];
        dasm.ops.Add(reg);
        return true;
    }

    private static bool RegA1(uint uInstr, KalimbaDisassembler dasm)
    {
        var r = bf16_4.Read(uInstr);
        var reg = Registers.ByDomain[(StorageDomain) r];
        dasm.ops.Add(reg);
        return true;
    }

    private static bool RegA2(uint uInstr, KalimbaDisassembler dasm)
    {
        var r = bf16_4.Read(uInstr);
        var reg = Registers.ByDomain[(StorageDomain) r + 16];
        dasm.ops.Add(reg);
        return true;
    }

    private static bool RegB(uint uInstr, KalimbaDisassembler dasm)
    {
        var r = bf4_4.Read(uInstr);
        var reg = Registers.ByDomain[(StorageDomain) r];
        dasm.ops.Add(reg);
        return true;
    }

    private static bool RegB1(uint uInstr, KalimbaDisassembler dasm)
    {
        var r = bf4_4.Read(uInstr);
        var reg = Registers.ByDomain[(StorageDomain) r];
        dasm.ops.Add(reg);
        return true;
    }
    private static bool RegB2(uint uInstr, KalimbaDisassembler dasm)
    {
        var r = bf4_4.Read(uInstr);
        var reg = Registers.ByDomain[(StorageDomain) r + 16];
        dasm.ops.Add(reg);
        return true;
    }

    private static bool RegC(uint uInstr, KalimbaDisassembler dasm)
    {
        var r = bf20_4.Read(uInstr);
        var reg = Registers.ByDomain[(StorageDomain) r];
        dasm.ops.Add(reg);
        return true;
    }

    private static bool RegC1(uint uInstr, KalimbaDisassembler dasm)
    {
        var r = bf20_4.Read(uInstr);
        var reg = Registers.ByDomain[(StorageDomain) r];
        dasm.ops.Add(reg);
        return true;
    }
    private static bool RegC2(uint uInstr, KalimbaDisassembler dasm)
    {
        var r = bf20_4.Read(uInstr);
        var reg = Registers.ByDomain[(StorageDomain) r + 16];
        dasm.ops.Add(reg);
        return true;
    }

    private static bool RMac(uint uInstr, KalimbaDisassembler dasm)
    {
        dasm.ops.Add(Registers.rMAC);
        return true;
    }

    private static bool Carry(uint uInstr, KalimbaDisassembler dasm)
    {
        dasm.ops.Add(Registers.Carry);
        return true;
    }

    private static bool SS(uint uInstr, KalimbaDisassembler dasm)
    {
        dasm.signSelect = bf26_2.Read(uInstr);
        return true;
    }

    private static bool Cond(uint uInstr, KalimbaDisassembler dasm)
    {
        dasm.ccode = (CCode) bf0_4.Read(uInstr);
        return true;
    }

    private static bool K16(uint uInstr, KalimbaDisassembler dasm)
    {
        var imm = bf0_16.Read(uInstr);
        dasm.ops.Add(Constant.Word16((ushort) imm));
        return true;
    }

    private static bool D16(uint uInstr, KalimbaDisassembler dasm)
    {
        var imm = bf0_16.ReadSigned(uInstr);
        dasm.ops.Add(dasm.addr + imm);
        return true;
    }

    private static bool MRegA(uint uInstr, KalimbaDisassembler dasm)
    {
        var r = bf16_4.Read(uInstr);
        var reg = Registers.ByDomain[(StorageDomain) r];
        var mem = new MemoryOperand(KalimbaArchitecture.Word24, reg);
        dasm.ops.Add(mem);
        return true;
    }

    private static bool MRegB(uint uInstr, KalimbaDisassembler dasm)
    {
        var r = bf4_4.Read(uInstr);
        var reg = Registers.ByDomain[(StorageDomain) r];
        var mem = new MemoryOperand(KalimbaArchitecture.Word24, reg);
        dasm.ops.Add(mem);
        return true;
    }

    private static bool MRegC(uint uInstr, KalimbaDisassembler dasm)
    {
        var r = bf20_4.Read(uInstr);
        var reg = Registers.ByDomain[(StorageDomain) r];
        var mem = new MemoryOperand(KalimbaArchitecture.Word24, reg);
        dasm.ops.Add(mem);
        return true;
    }

    private static bool MK16(uint uInstr, KalimbaDisassembler dasm)
    {
        var imm = bf0_16.Read(uInstr);
        var mem = new MemoryOperand(KalimbaArchitecture.Word24, Constant.Word16((ushort) imm));
        dasm.ops.Add(mem);
        return true;
    }

    private static bool MRegARegB(uint uInstr, KalimbaDisassembler dasm)
    {
        var ra = bf16_4.Read(uInstr);
        var rb = bf4_4.Read(uInstr);
        var regA = Registers.ByDomain[(StorageDomain) ra];
        var regB = Registers.ByDomain[(StorageDomain) rb];
        var mem = new MemoryOperand(KalimbaArchitecture.Word24, regA, regB);
        dasm.ops.Add(mem);
        return true;
    }

    private static bool MRegAK16(uint uInstr, KalimbaDisassembler dasm)
    {
        var ra = bf16_4.Read(uInstr);
        var imm = bf0_16.Read(uInstr);
        var regA = Registers.ByDomain[(StorageDomain) ra];
        var mem = new MemoryOperand(KalimbaArchitecture.Word24, regA, Constant.Int64((short) imm));
        dasm.ops.Add(mem);
        return true;
    }

    private static bool MRegCRegA(uint uInstr, KalimbaDisassembler dasm)
    {
        var ra = bf16_4.Read(uInstr);
        var rc = bf20_4.Read(uInstr);
        var regA = Registers.ByDomain[(StorageDomain) ra];
        var regC = Registers.ByDomain[(StorageDomain) rc];
        var mem = new MemoryOperand(KalimbaArchitecture.Word24, regC, regA);
        dasm.ops.Add(mem);
        return true;
    }


    private static bool Div(uint uInstr, KalimbaDisassembler dasm)
    {
        dasm.ops.Add(new LiteralOperand("div"));
        return true;
    }

    private static bool DivResult(uint uInstr, KalimbaDisassembler dasm)
    {
        dasm.ops.Add(new LiteralOperand("divResult"));
        return true;
    }

    private static bool DivRemainder(uint uInstr, KalimbaDisassembler dasm)
    {
        dasm.ops.Add(new LiteralOperand("divRemainder"));
        return true;
    }

    private static Decoder<KalimbaDisassembler, Mnemonic, KalimbaInstruction> Instr(
        Mnemonic mnemonic,
        params Mutator<KalimbaDisassembler>[] mutators)
    {
        return new InstrDecoder<KalimbaDisassembler, Mnemonic, KalimbaInstruction>(
            InstrClass.Linear,
            mnemonic,
            mutators);
    }

    private static Decoder<KalimbaDisassembler, Mnemonic, KalimbaInstruction> InstrA(
         Mnemonic mnemonic,
         params Mutator<KalimbaDisassembler>[] mutators)
    {
        return new AInstrDecoder(
            mnemonic,
            mutators);
    }

    private static Decoder<KalimbaDisassembler, Mnemonic, KalimbaInstruction> InstrB(
        Mnemonic mnemonic,
        params Mutator<KalimbaDisassembler>[] mutators)
    {
        return new BInstrDecoder(
            mnemonic,
            mutators);
    }

    private static Decoder<KalimbaDisassembler, Mnemonic, KalimbaInstruction> InstrC1(
        Mnemonic mnemonic,
        params Mutator<KalimbaDisassembler>[] mutators)
    {
        return new C1InstrDecoder(
            mnemonic,
            mutators);
    }

    private static Decoder<KalimbaDisassembler, Mnemonic, KalimbaInstruction> InstrC2(
         Mnemonic mnemonic,
         params Mutator<KalimbaDisassembler>[] mutators)
    {
        return new C2InstrDecoder(
            mnemonic,
            mutators);
    }

    private static Decoder<KalimbaDisassembler, Mnemonic, KalimbaInstruction> Instr(
        Mnemonic mnemonic,
        InstrClass iclass,
        params Mutator<KalimbaDisassembler>[] mutators)
    {
        return new InstrDecoder<KalimbaDisassembler, Mnemonic, KalimbaInstruction>(
            iclass,
            mnemonic,
            mutators);
    }

    private class AInstrDecoder : InstrDecoder<KalimbaDisassembler, Mnemonic, KalimbaInstruction>
    {
        public AInstrDecoder(
            Mnemonic mnemonic,
            params Mutator<KalimbaDisassembler>[] mutators)
            : base(InstrClass.Linear, mnemonic, mutators)
        {
        }

        public override KalimbaInstruction Decode(uint uInstr, KalimbaDisassembler dasm)
        {
            var instr = base.Decode(uInstr, dasm);
            var reg = Registers.GpRegisters[bf12_3.Read(uInstr)];
            var idx = Registers.IndexRegisters[bf10_2.Read(uInstr)];
            var mod = Registers.ModifyRegisters[bf8_2.Read(uInstr)];
            instr.Condition = (CCode) bf0_4.Read(uInstr);
            instr.MemAccess1 = MakeMemaccess(uInstr, 15, reg, idx, mod);
            return instr;
        }
    }

    private class BInstrDecoder : InstrDecoder<KalimbaDisassembler, Mnemonic, KalimbaInstruction>
    {
        public BInstrDecoder(
            Mnemonic mnemonic,
            params Mutator<KalimbaDisassembler>[] mutators)
            : base(InstrClass.Linear, mnemonic, mutators)
        {
        }

        public override KalimbaInstruction Decode(uint uInstr, KalimbaDisassembler dasm)
        {
            var instr = base.Decode(uInstr, dasm);
            instr.Condition = CCode.Always;
            return instr;
        }
    }

    private class C1InstrDecoder : InstrDecoder<KalimbaDisassembler, Mnemonic, KalimbaInstruction>
    {
        public C1InstrDecoder(
            Mnemonic mnemonic,
            params Mutator<KalimbaDisassembler>[] mutators)
            : base(InstrClass.Linear, mnemonic, mutators)
        {
        }

        public override KalimbaInstruction Decode(uint uInstr, KalimbaDisassembler dasm)
        {
            var instr = base.Decode(uInstr, dasm);
            var reg1 = Registers.GpRegisters[bf12_3.Read(uInstr)];
            var idx1 = Registers.IndexRegisters[bf10_2.Read(uInstr)];
            var mod1 = Registers.ModifyRegisters[bf8_2.Read(uInstr)];
            instr.Condition = CCode.Always;
            instr.MemAccess1 = MakeMemaccess(uInstr, 15, reg1, idx1, mod1);
            var reg2 = Registers.GpRegisters[bf4_3.Read(uInstr)];
            var idx2 = Registers.IndexRegisters[bf2_2.Read(uInstr)];
            var mod2 = Registers.ModifyRegisters[bf0_2.Read(uInstr)];
            instr.MemAccess2 = MakeMemaccess(uInstr, 7, reg2, idx2, mod2);
            return instr;
        }
    }

    private class C2InstrDecoder : InstrDecoder<KalimbaDisassembler, Mnemonic, KalimbaInstruction>
    {
        public C2InstrDecoder(
            Mnemonic mnemonic,
            params Mutator<KalimbaDisassembler>[] mutators)
            : base(InstrClass.Linear, mnemonic, mutators)
        {
        }

        public override KalimbaInstruction Decode(uint uInstr, KalimbaDisassembler dasm)
        {
            var instr = base.Decode(uInstr, dasm);
            var reg1 = Registers.GpRegisters[bf12_3.Read(uInstr)];
            var idx1 = Registers.IndexRegisters[bf10_2.Read(uInstr)];
            var mod1 = modifyConstants[bf8_2.Read(uInstr)];
            instr.Condition = CCode.Always;
            instr.MemAccess1 = MakeMemaccess(uInstr, 15, reg1, idx1, mod1);
            var reg2 = Registers.GpRegisters[bf4_3.Read(uInstr)];
            var idx2 = Registers.IndexRegisters[bf2_2.Read(uInstr)];
            var mod2 = modifyConstants[bf0_2.Read(uInstr)];
            instr.MemAccess2 = MakeMemaccess(uInstr, 7, reg2, idx2, mod2);
            return instr;
        }
    }

    private static KalimbaInstruction? MakeMemaccess(uint uInstr, int bitWrite, RegisterStorage reg, RegisterStorage idx, MachineOperand mod)
    {
        if (reg.Number == 0)
            return null;
        KalimbaInstruction memAccess;
        if (Bits.IsBitSet(uInstr, bitWrite))
        {
            memAccess = new KalimbaInstruction
            {
                InstructionClass = InstrClass.Linear,
                Mnemonic = Mnemonic.Store,
                Condition = CCode.Always,
                Operands = [
                    new MemoryOperand(KalimbaArchitecture.Word24, idx, mod),
                    reg
                ]
            };
        }
        else
        {
            memAccess = new KalimbaInstruction
            {
                InstructionClass = InstrClass.Linear,
                Mnemonic = Mnemonic.Load,
                Condition = CCode.Always,
                Operands = [
                    reg,
                    new MemoryOperand(KalimbaArchitecture.Word24, idx, mod),
                ]
            };
        }
        return memAccess;
    }


    static KalimbaDisassembler()
    {
        var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

        var add00 = Mask(24, 2, "  add00",
            InstrA(Mnemonic.add, RegC, RegA, RegB),
            InstrB(Mnemonic.add, RegC, RegA, K16),
            InstrC1(Mnemonic.add, RegC, RegC, RegA),
            InstrC2(Mnemonic.add, RegC, RegC, RegA));
        var addc00 = Mask(24, 2, "  add00",
            InstrA(Mnemonic.add, RegC, RegA, RegB, Carry),
            InstrB(Mnemonic.add, RegC, RegA, K16, Carry),
            InstrC1(Mnemonic.add, RegC, RegC, K16, Carry),
            InstrC2(Mnemonic.add, RegC, RegC, RegA, Carry));

        var add01 = Mask(24, 2, "  add01",
            InstrA(Mnemonic.add, RegC, RegA, MRegB),
            InstrB(Mnemonic.add, RegC, RegA, MK16),
            InstrC1(Mnemonic.add, RegC, RegC, MRegA),
            InstrC2(Mnemonic.add, RegC, RegC, MRegA));
        var addc01 = Mask(24, 2, "  add01",
            InstrA(Mnemonic.add, RegC, RegA, MRegB, Carry),
            InstrB(Mnemonic.add, RegC, RegA, MK16, Carry),
            InstrC1(Mnemonic.add, RegC, RegC, MRegA, Carry),
            InstrC2(Mnemonic.add, RegC, RegC, MRegA, Carry));

        var add02 = Mask(24, 2, "  add02",
            InstrA(Mnemonic.add, RegC, MRegA, RegB),
            InstrB(Mnemonic.add, RegC, MRegA, K16),
            invalid,
            invalid);
        var addc02 = Mask(24, 2, "  add02",
            InstrA(Mnemonic.add, RegC, MRegA, RegB, Carry),
            InstrB(Mnemonic.add, RegC, MRegA, K16, Carry),
            invalid,
            invalid);

        var add03 = Mask(24, 2, "  add03",
            InstrA(Mnemonic.add, MRegC, RegA, RegB),
            InstrB(Mnemonic.add, MK16, RegC, RegA),
            invalid,
            invalid);
        var addc03 = Mask(24, 2, "  add03",
            InstrA(Mnemonic.add, MRegC, RegA, RegB, Carry),
            InstrA(Mnemonic.add, MK16, RegC, RegA, Carry),
            invalid,
            invalid);

        var sub00 = Mask(24, 2, "  sub00",
            InstrA(Mnemonic.sub, RegC, RegA, RegB),
            InstrB(Mnemonic.sub, RegC, RegA, K16),
            InstrC1(Mnemonic.sub, RegC, RegC, RegA),
            InstrC2(Mnemonic.sub, RegC, RegC, RegA));
        var subc00 = Mask(24, 2, "  sub00",
            InstrA(Mnemonic.sub, RegC, RegA, RegB, Carry),
            InstrB(Mnemonic.sub, RegC, RegA, K16, Carry),
            InstrC1(Mnemonic.sub, RegC, RegC, K16, Carry),
            InstrC2(Mnemonic.sub, RegC, RegC, RegA, Carry));

        var sub01 = Mask(24, 2, "  sub01",
            InstrA(Mnemonic.sub, RegC, RegA, MRegB),
            InstrB(Mnemonic.sub, RegC, RegA, MK16),
            InstrC1(Mnemonic.sub, RegC, RegC, MRegA),
            InstrC2(Mnemonic.sub, RegC, RegC, MRegA));
        var subc01 = Mask(24, 2, "  sub01",
            InstrA(Mnemonic.sub, RegC, RegA, MRegB, Carry),
            InstrB(Mnemonic.sub, RegC, RegA, MK16, Carry),
            InstrC1(Mnemonic.sub, RegC, RegC, MRegA, Carry),
            InstrC2(Mnemonic.sub, RegC, RegC, MRegA, Carry));

        var sub02 = Mask(24, 2, "  sub02",
            Instr(Mnemonic.sub, RegC, MRegA, RegB),
            Instr(Mnemonic.sub, RegC, MRegA, K16),
            invalid,
            invalid);
        var subc02 = Mask(24, 2, "  sub02",
            Instr(Mnemonic.sub, RegC, MRegA, RegB, Carry),
            Instr(Mnemonic.sub, RegC, MRegA, K16, Carry),
            invalid,
            invalid);

        var sub03 = Mask(24, 2, "  sub03",
            Instr(Mnemonic.sub, MRegC, RegA, RegB),
            Instr(Mnemonic.sub, MK16, RegC, RegA),
            invalid,
            invalid);
        var subc03 = Mask(24, 2, "  sub03",
            Instr(Mnemonic.sub, MRegC, RegA, RegB, Carry),
            Instr(Mnemonic.sub, MK16, RegC, RegA, Carry),
            invalid,
            invalid);

        var bankAdd = Mask(24, 5, "  bankAdd",
            InstrA(Mnemonic.add, RegC1, RegA1, RegB1),
            InstrB(Mnemonic.add, RegC1, RegA1, K16),
            InstrC1(Mnemonic.add, RegC1, RegC1, RegA1),
            InstrC2(Mnemonic.add, RegC1, RegC1, RegA1),

            InstrA(Mnemonic.add, RegC1, RegA1, RegB2),
            invalid,
            InstrC1(Mnemonic.add, RegC1, RegC1, RegA2),
            InstrC2(Mnemonic.add, RegC1, RegC1, RegA2),

            InstrA(Mnemonic.add, RegC1, RegA2, RegB1),
            InstrB(Mnemonic.add, RegC1, RegA2, K16),
            invalid,
            invalid,

            InstrA(Mnemonic.add, RegC1, RegA2, RegB2),
            invalid,
            invalid,
            invalid,

            InstrA(Mnemonic.add, RegC2, RegA1, RegB1),
            InstrB(Mnemonic.add, RegC2, RegA1, K16),
            invalid,
            invalid,

            InstrA(Mnemonic.add, RegC2, RegA1, RegB2),
            invalid,
            invalid,
            invalid,

            InstrA(Mnemonic.add, RegC2, RegA2, RegB1),
            InstrB(Mnemonic.add, RegC2, RegA2, K16),
            InstrC1(Mnemonic.add, RegC2, RegC2, RegA1),
            InstrC2(Mnemonic.add, RegC2, RegC2, RegA1),

            InstrA(Mnemonic.add, RegC2, RegA2, RegB2),
            invalid,
            InstrC1(Mnemonic.add, RegC2, RegC2, RegA2),
            InstrC2(Mnemonic.add, RegC2, RegC2, RegA2));

        var bankSub = Mask(24, 5, "  bankSub",
            InstrA(Mnemonic.sub, RegC1, RegA1, RegB1),
            InstrB(Mnemonic.sub, RegC1, RegA1, K16),
            InstrC1(Mnemonic.sub, RegC1, RegC1, RegA1),
            InstrC2(Mnemonic.sub, RegC1, RegC1, RegA1),

            InstrA(Mnemonic.sub, RegC1, RegA1, RegB2),
            InstrB(Mnemonic.sub, RegC1, K16, RegA1),
            InstrC1(Mnemonic.sub, RegC1, RegC1, RegA2),
            InstrC2(Mnemonic.sub, RegC1, RegC1, RegA2),

            InstrA(Mnemonic.sub, RegC1, RegA2, RegB1),
            InstrB(Mnemonic.sub, RegC1, RegA2, K16),
            invalid,
            invalid,

            InstrA(Mnemonic.sub, RegC1, RegA2, RegB2),
            InstrB(Mnemonic.sub, RegC1, K16, RegA2),
            invalid,
            invalid,

            InstrA(Mnemonic.sub, RegC2, RegA1, RegB1),
            InstrB(Mnemonic.sub, RegC2, RegA1, K16),
            invalid,
            invalid,

            InstrA(Mnemonic.sub, RegC2, RegA1, RegB2),
            InstrB(Mnemonic.sub, RegC2, K16, RegA1),
            invalid,
            invalid,

            InstrA(Mnemonic.sub, RegC2, RegA2, RegB1),
            InstrB(Mnemonic.sub, RegC2, RegA2, K16),
            InstrC1(Mnemonic.sub, RegC2, RegC2, RegA1),
            InstrC2(Mnemonic.sub, RegC2, RegC2, RegA1),

            InstrA(Mnemonic.sub, RegC2, RegA2, RegB2),
            InstrB(Mnemonic.sub, RegC2, K16, RegA2),
            InstrC1(Mnemonic.sub, RegC2, RegC2, RegA2),
            InstrC2(Mnemonic.sub, RegC2, RegC2, RegA2));

        var and = Mask(24, 2, "  and",
            InstrA(Mnemonic.and, RegC, RegA, RegB),
            InstrB(Mnemonic.and, RegC, RegA, K16),
            InstrC1(Mnemonic.and, RegC, RegC, RegA),
            InstrC2(Mnemonic.and, RegC, RegC, RegA));

        var or = Mask(24, 2, "  or",
            InstrA(Mnemonic.or, RegC, RegA, RegB),
            InstrB(Mnemonic.or, RegC, RegA, K16),
            InstrC1(Mnemonic.or, RegC, RegC, RegA),
            InstrC2(Mnemonic.or, RegC, RegC, RegA));

        var xor = Mask(24, 2, "  xor",
            InstrA(Mnemonic.xor, RegC, RegA, RegB),
            InstrB(Mnemonic.xor, RegC, RegA, K16),
            InstrC1(Mnemonic.xor, RegC, RegC, RegA),
            InstrC2(Mnemonic.xor, RegC, RegC, RegA));

        var lsh = Mask(24, 2, "  lsh",
            InstrA(Mnemonic.lsh, RegC, RegA, RegB),
            InstrB(Mnemonic.lsh, RegC, RegA, K16),  //$TODO: rMac
            InstrC1(Mnemonic.lsh, RegC, RegC, RegA),
            InstrC2(Mnemonic.lsh, RegC, RegC, RegA));

        var ash = Mask(24, 2, "  ash",
            InstrA(Mnemonic.ash, RegC, RegA, RegB),
            InstrB(Mnemonic.ash, RegC, RegA, K16),  //$TODO: rMac
            InstrC1(Mnemonic.ash, RegC, RegC, RegA),
            InstrC2(Mnemonic.ash, RegC, RegC, RegA));

        var fracmul = Mask(24, 2, "  fracmul",
            InstrA(Mnemonic.fracmul, RegC, RegA, RegB),
            InstrB(Mnemonic.fracmul, RegC, RegA, K16),
            InstrC1(Mnemonic.fracmul, RegC, RegC, RegA),
            InstrC2(Mnemonic.fracmul, RegC, RegC, RegA));
        var smul = Mask(24, 2, "  smul",
            InstrA(Mnemonic.smul, RegC, RegA, RegB),
            InstrB(Mnemonic.smul, RegC, RegA, K16),
            InstrC1(Mnemonic.smul, RegC, RegC, RegA),
            InstrC2(Mnemonic.smul, RegC, RegC, RegA));
        var smulv = Mask(24, 2, "  smulv",
            InstrA(Mnemonic.smulv, RegC, RegA, RegB),
            InstrB(Mnemonic.smulv, RegC, RegA, K16),
            InstrC1(Mnemonic.smulv, RegC, RegC, RegA),
            InstrC2(Mnemonic.smulv, RegC, RegC, RegA));

        var maca = Mask(24, 2, "  maca",
            InstrA(Mnemonic.maca, SS, RMac, RegA, RegB),
            InstrB(Mnemonic.maca, SS, RMac, RegA, K16),
            InstrC1(Mnemonic.maca, SS, RMac, RegC, RegA),
            InstrC2(Mnemonic.maca, SS, RMac, RegC, RegA));
        var macs = Mask(24, 2, "  macs",
            InstrA(Mnemonic.macs, SS, RMac, RegA, RegB),
            InstrB(Mnemonic.macs, SS, RMac, RegA, K16),
            InstrC1(Mnemonic.macs, SS, RMac, RegC, RegA),
            InstrC2(Mnemonic.macs, SS, RMac, RegC, RegA));
        var mul48 = Mask(24, 2, "  mul48",
            InstrA(Mnemonic.macs, SS, RMac, RegA, RegB),
            InstrB(Mnemonic.macs, SS, RMac, RegA, K16),
            InstrC1(Mnemonic.macs, SS, RMac, RegC, RegA),
            InstrC2(Mnemonic.macs, SS, RMac, RegC, RegA));

        var load = Mask(24, 2, "  load",
            InstrA(Mnemonic.Load, RegC, MRegARegB),
            InstrB(Mnemonic.Load, RegC, MRegAK16),
            InstrC1(Mnemonic.Load, RegC, MRegCRegA),
            InstrC2(Mnemonic.Load, RMac, RegC, RegA));
        var store = Mask(24, 2, "  store",
            InstrA(Mnemonic.Store, MRegARegB, RegC),
            InstrB(Mnemonic.Store, MRegAK16, RegC),
            invalid,
            invalid);
        var signDet_div = Mask(24, 2, "  signdet / div",
            InstrA(Mnemonic.signDet, RegC, RegA),
            Mask(0, 2, "  div",
                Instr(Mnemonic.div, Div, RMac, RegA),
                Instr(Mnemonic.Load, RegC, DivResult),
                Instr(Mnemonic.Load, RegC, DivRemainder),
                invalid),
            InstrC1(Mnemonic.blkSignDet, RegC, RegA),
            InstrC2(Mnemonic.blkSignDet, RegC, RegA));
        var jumpRet = Mask(24, 2, "  jumpRet",
            Instr(Mnemonic.jump, RegA),
            Instr(Mnemonic.jump, Cond, D16),
            Instr(Mnemonic.rts, Cond),
            Instr(Mnemonic.rts, Cond));
        var callReti = Mask(24, 2, "  callReti",
            Instr(Mnemonic.call, RegA),
            Instr(Mnemonic.call, Cond, D16),
            Instr(Mnemonic.rti, Cond),
            Instr(Mnemonic.rti, Cond));
        var sleepLoopBreak = Mask(24, 2, "  sleepLoopBreak",
            Instr(Mnemonic.sleep),
            Instr(Mnemonic.loop),
            Instr(Mnemonic.@break),
            Instr(Mnemonic.@break));

        var pushPop = invalid;
        var prefix = invalid;


        rootDecoder = Mask(26, 6, "Kalimba",
            add00,
            addc00,
            add01,
            addc01,
            add02,
            addc02,
            add03,
            addc03,

            sub00,
            subc00,
            sub01,
            subc01,
            sub02,
            subc02,
            sub03,
            subc03,

            bankAdd,
            bankAdd,
            bankAdd,
            bankAdd,
            bankAdd,
            bankAdd,
            bankAdd,
            bankAdd,

            bankSub,
            bankSub,
            bankSub,
            bankSub,
            bankSub,
            bankSub,
            bankSub,
            bankSub,

            and,
            or,
            xor,
            lsh,
            ash,
            fracmul,
            smul,
            smulv,

            maca,
            maca,
            maca,
            maca,
            macs,
            macs,
            macs,
            macs,

            mul48,
            mul48,
            mul48,
            mul48,
            load,
            store,
            signDet_div,
            jumpRet,

            callReti,
            sleepLoopBreak,
            invalid,
            invalid,

            pushPop,
            invalid,
            invalid,
            prefix);
    }
}