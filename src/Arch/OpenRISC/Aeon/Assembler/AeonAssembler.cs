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
using Reko.Core.Assemblers;
using Reko.Core.Lib;
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

#pragma warning disable IDE1006

namespace Reko.Arch.OpenRISC.Aeon.Assembler
{
    public class AeonAssembler
    {
        public const int RT_AEON_BG_DISP13_3 = 1;
        public const int RT_AEON_BN_DISP18 = 2;
        public const int RT_AEON_BN_DISP8_2 = 3;
        public const int RT_AEON_BG_HI16_5 = 4;
        public const int RT_AEON_BG_LO16_0 = 5;
        public const int RT_AEON_BG_LO14_2 = 6;

        private AeonArchitecture arch;
        private List<ImageSymbol> entryPoints;
        private Dictionary<string, ImageSymbol> symbolsByName;
        private List<Relocation> relocations;
        private Relocation? hiReloc;

        public AeonAssembler(
            AeonArchitecture arch,
            Address addrBase,
            List<ImageSymbol> entryPoints)
        {
            this.arch = arch;
            this.BaseAddress = addrBase;
            this.entryPoints = entryPoints;
            this.symbolsByName = new Dictionary<string, ImageSymbol>();
            this.Emitter = new Emitter();
            this.relocations = new();
        }

        public Address BaseAddress { get; }

        public Address CurrentAddress => this.BaseAddress + this.Emitter.Size;

        public Emitter Emitter { get; }

        public void bg_andi(RegisterStorage rdst, RegisterStorage rsrc1, ImmediateOperand immop)
        {
            uint opcode = 0b110001u << 26;
            opcode |= R(rdst, 21);
            opcode |= R(rsrc1, 16);
            opcode |= U16(immop, 0);
            Emitter.EmitBeUInt32(opcode);
        }

        public void bg_beqi(RegisterStorage rsrc, ImmediateOperand imm, ImmediateOperand displacement)
        {
            uint opcode = (0b110100u << 26) | 0b010u;
            opcode |= R(rsrc, 21);
            opcode |= U(imm, 16, 5);
            opcode |= S(displacement, 3, 13);
            Emitter.EmitBeUInt32(opcode);
        }

        public void bg_movhi(RegisterStorage rd, ImmediateOperand immop)
        {
            uint opcode = (0b110000u << 26) | 0b0001u;
            opcode |= R(rd, 21);
            opcode |= U16(immop, 5);
            Emitter.EmitBeUInt32(opcode);
        }

        public void bg_ori(RegisterStorage rdst, RegisterStorage rsrc1, ImmediateOperand immop)
        {
            uint opcode = 0b110010u << 26;
            opcode |= R(rdst, 21);
            opcode |= R(rsrc1, 16);
            opcode |= U16(immop, 0);
            Emitter.EmitBeUInt32(opcode);
        }

        public void bg_lbz(RegisterStorage rdst, MemoryOperand mop)
        {
            var opcode = 0b111100u << 26;
            opcode |= R(rdst, 21);
            opcode |= R(mop.Base, 16);
            opcode |= S16(mop.Offset, 0);
            Emitter.EmitBeUInt32(opcode);
        }

        public void bg_lwz(RegisterStorage rdst, MemoryOperand mop)
        {
            var opcode = (0b111011u << 26) | 0b10u;
            opcode |= R(rdst, 21);
            opcode |= R(mop.Base, 16);
            opcode |= S16(mop.Offset, 0);
            Emitter.EmitBeUInt32(opcode);
        }

        public void bg_sb(MemoryOperand mop, RegisterStorage rs)
        {
            var opcode = 0b111110u << 26;
            opcode |= R(rs, 21);
            opcode |= R(mop.Base, 16);
            opcode |= S16(mop.Offset, 0);
            Emitter.EmitBeUInt32(opcode);
        }

        public void bn_bnei(RegisterStorage reg, ImmediateOperand imm, ImmediateOperand displacement)
        {
            var opcode = (0b001000u << 18) | 0b10;
            opcode |= R(reg, 13);
            opcode |= U(imm, 10, 3);
            opcode |= S(displacement, 2, 8);
            EmitUInt24(opcode);
        }

        public void bn_j(ImmediateOperand displacement)
        {
            var opcode = 0b001011u << 18;
            opcode |= S(displacement, 0, 18);
            EmitUInt24(opcode);
        }

        public void bn_xor(RegisterStorage rdst, RegisterStorage rsrc1, RegisterStorage rsrc2)
        {
            var opcode = (0b010001u << 18) | 0b110;
            opcode |= R(rdst, 13);
            opcode |= R(rsrc1, 8);
            opcode |= R(rsrc2, 3);
            EmitUInt24(opcode);
        }

        public void bt_addi(RegisterStorage reg, ImmediateOperand imm)
        {
            var opcode = 0b100111u << 10;
            opcode |= R(reg, 5);
            opcode |= S(imm, 0, 5);
            Emitter.EmitBeUInt16(opcode);
        }

        public void bt_jr(RegisterStorage reg)
        {
            var opcode = (0b100001u << 10) | 0b01001u;
            opcode |= R(reg, 5);
            Emitter.EmitBeUInt16(opcode);
        }

        public void AddRelocation(in Relocation reloc)
        {
            this.relocations.Add(reloc);
        }

        private void EmitUInt24(uint u)
        {
            Emitter.EmitByte((byte)(u >> 16));
            Emitter.EmitByte((byte)(u >> 8));
            Emitter.EmitByte((byte)u);
        }

        private uint R(RegisterStorage? r, int bitPos)
        {
            if (r is null)
                return 0;
            return (uint)r.Number << bitPos;
        }

        private uint S(ImmediateOperand imm, int bitPos, int bitlength)
        {
            var mask = Bits.Mask(bitPos, bitlength);
            return (uint) (((uint)imm.Value.ToInt32() << bitPos) & mask);
        }

        private uint S16(int value, int bitPos)
        {
            return ((uint)value & 0xFFFF) << bitPos;
        }

        private uint U(ImmediateOperand imm, int bitPos, int bitlength)
        {
            var mask = Bits.Mask(bitPos, bitlength);
            return (uint) ((imm.Value.ToUInt32() << bitPos) & mask);
        }

        private uint U16(ImmediateOperand imm, int bitPos)
        {
            return (imm.Value.ToUInt32() & 0xFFFFu) << bitPos;
        }

        public Program GetImage(Address addrBase)
        {
            var mem = new ByteMemoryArea(addrBase, Emitter.GetBytes());
            var program = new Program(
                new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment("code", mem, AccessMode.ReadWriteExecute)),
                arch,
                new DefaultPlatform(arch.Services, arch));
            foreach (var sym in symbolsByName.Values)
            {
                program.ImageSymbols.Add(sym.Address, sym);
            }
            return program;
        }

        public void PerformRelocations()
        {
            var unresolved = new List<string>();
            foreach (var reloc in this.relocations)
            {
                if (!this.symbolsByName.TryGetValue(reloc.SymbolName, out var symbol))
                {
                    unresolved.Add(reloc.SymbolName);
                    continue;
                }
                Relocate(reloc, symbol!);
            }
        }

        public void EmitWord16(uint halfword)
        {
            Emitter.EmitBeUInt16((ushort)halfword);
        }

        public void EmitWord32(uint word)
        {
            Emitter.EmitBeUInt32(word);
        }

        private uint ReadBeUInt24(Address address)
        {
            int offset = (int) (address - this.BaseAddress);
            uint u0 = Emitter.ReadByte(offset);
            uint u1 = Emitter.ReadByte(offset + 1);
            uint u2 = Emitter.ReadByte(offset + 2);
            return (u0 << 16) | (u1 << 8) | u2;
        }

        private uint ReadBeUInt32(Address address)
        {
            int offset = (int) (address - this.BaseAddress);
            return Emitter.ReadBeUInt32(offset);
        }

        private void WriteBeUInt24(Address address, uint value)
        {
            int offset = (int) (address - this.BaseAddress);
            Emitter.WriteByte(offset, (byte) (value >> 16));
            Emitter.WriteByte(offset+1, (byte) (value >> 8));
            Emitter.WriteByte(offset+2, (byte) value);
        }

        private void WriteBeUInt32(Address address, uint value)
        {
            int offset = (int) (address - this.BaseAddress);
            Emitter.WriteBeUInt32(offset, value);
        }

        public void Relocate(in Relocation reloc, ImageSymbol symbol)
        {
            long displacement = symbol.Address - reloc.Address;
            switch (reloc.RelocationType)
            {
            case RT_AEON_BG_DISP13_3:
                Relocate32(reloc, displacement, 3, 13);
                break;
            case RT_AEON_BN_DISP18:
                Relocate24(reloc, displacement, 0, 18);
                break;
            case RT_AEON_BN_DISP8_2:
                Relocate24(reloc, displacement, 2, 8);
                break;
            case RT_AEON_BG_HI16_5:
                this.hiReloc = reloc;
                return;
            case RT_AEON_BG_LO16_0:
                if (!hiReloc.HasValue)
                    throw new ApplicationException($"Expected a high relocation before this one {reloc}.");
                RelocateHiLo(hiReloc.Value, reloc, symbol, 0xFFFFu);
                break;
            case RT_AEON_BG_LO14_2:
                if (!hiReloc.HasValue)
                    throw new ApplicationException($"Expected a high relocation before this one {reloc}.");
                RelocateHiLo(hiReloc.Value, reloc, symbol, 0xFFFCu);
                break;
            default:
                throw new NotImplementedException($"Relocation type {reloc.RelocationType}");
            }
            hiReloc = null;
        }

        private void RelocateHiLo(
            in Relocation hiReloc, 
            in Relocation loReloc,
            ImageSymbol symbol,
            uint maskLo)
        {
            uint instrHi = ReadBeUInt32(hiReloc.Address);
            uint instrLo = ReadBeUInt32(loReloc.Address);
            var maskHi = (uint) Bits.Mask(5, 16);
            uint hi = (instrHi & maskHi) >> 5;
            uint lo = (instrLo & maskLo);
            uint relocatedValue = ((hi << 16) | lo) + symbol.Address.ToUInt32();
            hi = relocatedValue >> 16;
            lo = relocatedValue & 0xFFFFu;
            instrHi = (instrHi & ~maskHi) | (hi << 5);
            instrLo = (instrLo & ~maskLo) | lo;
            WriteBeUInt32(hiReloc.Address, instrHi);
            WriteBeUInt32(loReloc.Address, instrLo);
        }

        private void Relocate24(in Relocation reloc, long displacement, int bitPos, int bitLength)
        {
            var mask = (uint) Bits.Mask(bitPos, bitLength);
            uint instr = ReadBeUInt24(reloc.Address);
            instr &= ~mask;
            instr |= (((uint) displacement) << bitPos) & mask;
            WriteBeUInt24(reloc.Address, instr);
        }

        private void Relocate32(in Relocation reloc, long displacement, int bitPos, int bitLength)
        {
            var mask = (uint) Bits.Mask(bitPos, bitLength);
            uint instr = ReadBeUInt32(reloc.Address);
            instr &= ~mask;
            instr |= (((uint) displacement) << bitPos) & mask;
            WriteBeUInt32(reloc.Address, instr);
        }

        public void RegisterSymbol(string name)
        {
            var addr = this.BaseAddress + this.Emitter.Size;
            var sym = ImageSymbol.Create(SymbolType.Unknown, arch, addr, name);
            entryPoints.Add(sym);
            symbolsByName.Add(name, sym);
        }
    }
}