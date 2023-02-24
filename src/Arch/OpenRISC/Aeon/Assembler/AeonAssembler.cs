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
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Arch.OpenRISC.Aeon.Assembler
{
    internal class AeonAssembler
    {
        private AeonArchitecture arch;
        private List<ImageSymbol> entryPoints;


        public AeonAssembler(
            AeonArchitecture arch,
            Address addrBase,
            List<ImageSymbol> entryPoints)
        {
            this.arch = arch;
            this.BaseAddress = addrBase;
            this.entryPoints = entryPoints;
            this.Emitter = new Emitter();
        }

        public Address BaseAddress { get; }
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

        public void bg_sb(MemoryOperand mop, RegisterStorage rs)
        {
            var opcode = 0b111110u << 26;
            opcode |= R(rs, 21);
            opcode |= R(mop.Base, 16);
            opcode |= S16(mop.Offset, 0);
            Emitter.EmitBeUInt32(opcode);
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
            return new Program(
                new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment("code", mem, AccessMode.ReadWriteExecute)),
                arch,
                new DefaultPlatform(arch.Services, arch));
        }

        internal void ReportUnresolvedSymbols()
        {
            //$TODO
        }

    }
}