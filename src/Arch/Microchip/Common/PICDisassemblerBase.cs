#region License
/* 
 * Copyright (C) 2017-2025 Christian Hostelet.
 * inspired by work from:
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
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Libraries.Microchip;
using System;

namespace Reko.Arch.MicrochipPIC.Common
{
    /// <summary>
    /// A Microchip 8-bit MCU disassembler frame. Must be inherited.
    /// Valid for most of program memory regions (code, eeprom, config, ...).
    /// </summary>
    public abstract class PICDisassemblerBase : DisassemblerBase<PICInstruction, Mnemonic>
    {
        public readonly PICArchitecture arch;
        public readonly EndianImageReader rdr;

        protected PICInstruction instrCur;
        public Address addrCur;

        protected static IMemoryRegion? lastusedregion = null;

        /// <summary>
        /// Instantiates a base PIC disassembler.
        /// </summary>
        /// <param name="arch">The PIC architecture.</param>
        /// <param name="rdr">The memory reader.</param, Mnemonic>
        protected PICDisassemblerBase(PICArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.instrCur = null!;
            this.addrCur = default;
        }

        /// <summary>
        /// Gets the PIC instruction-set identifier. Must be overriden.
        /// </summary>
        public virtual InstructionSetID InstructionSetID => InstructionSetID.UNDEFINED;

        /// <summary>
        /// Disassemble a single instruction. Return null if the end of the reader has been reached.
        /// </summary>
        /// <returns>
        /// A <seealso cref="PICInstruction"/> instance.
        /// </returns>
        /// <exception cref="AddressCorrelatedException">Thrown when the Address Correlated error
        ///                                              condition occurs.</exception>
        public override PICInstruction? DisassembleInstruction()
        {
            IMemoryRegion? GetProgRegion()
            {
                if (lastusedregion is not null && lastusedregion.Contains(addrCur))
                    return lastusedregion;
                return lastusedregion = PICMemoryDescriptor.GetProgramRegion(addrCur);
            }

            if (!rdr.IsValid)
                return null;
            addrCur = rdr.Address;
            var regn = GetProgRegion();
            if (regn is null)
                throw new InvalidOperationException($"Unable to retrieve program memory region for address {addrCur.ToString()}.");
            if ((addrCur.Offset % (regn.Trait?.LocSize ?? 1)) != 0)
            {
                instrCur = new PICInstructionNoOpnd(Mnemonic.unaligned)
                {
                    Address = addrCur,
                    Length = 1
                };
                rdr.Offset += 1; // Consume only the first byte of the binary instruction.
                return instrCur;
            }

            switch (regn.SubtypeOfMemory)
            {
                case PICMemorySubDomain.Code:
                case PICMemorySubDomain.ExtCode:
                case PICMemorySubDomain.Debugger:
                    if (!rdr.TryReadUInt16(out ushort uInstr))
                        return null;
                    return DecodePICInstruction(uInstr, rdr.Address);

                case PICMemorySubDomain.EEData:
                    return DecodeEEPROMInstruction();

                case PICMemorySubDomain.UserID:
                    return DecodeUserIDInstruction();

                case PICMemorySubDomain.DeviceConfig:
                    return DecodeConfigInstruction();

                case PICMemorySubDomain.DeviceID:
                    return DecodeDWInstruction();

                case PICMemorySubDomain.DeviceConfigInfo:  //TODO: Decode DCI
                    return DecodeDCIInstruction();

                case PICMemorySubDomain.DeviceInfoAry:     //TODO: Decode DIA 
                    return DecodeDIAInstruction();

                case PICMemorySubDomain.RevisionID:        //TODO: Decode Revision ID
                    return DecodeRevisionIDInstruction();

                case PICMemorySubDomain.Test:
                case PICMemorySubDomain.Other:
                default:
                    throw new NotImplementedException($"Disassembly of '{regn.SubtypeOfMemory}' memory region is not yet implemented.");
            }
        }

        public override PICInstruction CreateInvalidInstruction()
        {
            return new PICInstructionNoOpnd(Mnemonic.invalid)
            {
                InstructionClass = InstrClass.Invalid
            };
        }

        #region Classes/Methods defined for derived classes

        protected abstract class Decoder
        {
            public abstract PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm);
        }

        protected class SubDecoder : Decoder
        {
            private int bitpos;
            private int width;
            private Decoder[] decoders;

            public SubDecoder(int bitpos, int width, Decoder[] decoders)
            {
                this.decoders = decoders ?? throw new ArgumentNullException(nameof(decoders));
                this.bitpos = (bitpos < 0 ? 0 : bitpos);
                this.width = (width <= 0 ? 1 : width);
                if (decoders.Length != (1 << width))
                    throw new ArgumentOutOfRangeException(nameof(width), "Wrong decoder table size.");
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var bits = uInstr.Extract(bitpos, width);
                return decoders[bits].Decode(uInstr, dasm);
            }
        }

        /// <summary>
        /// Forgot to define a valid entry in the decoder table. Should not occur...
        /// </summary>
        protected class WrongDecoder : Decoder
        {
            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                throw new InvalidOperationException($"BUG! Missing decoder entry for PIC instruction 0x{uInstr:X4}.");
            }
        }

        /// <summary>
        /// Invalid instruction. Return <code>null</code> to indicate an invalid instruction.
        /// </summary>
        protected class InvalidDecoder : Decoder
        {
            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                return new PICInstructionNoOpnd(Mnemonic.invalid);
            }
        }

        protected abstract PICInstruction DecodePICInstruction(ushort uInstr, Address addr);

        protected abstract PICInstruction DecodeEEPROMInstruction();

        protected abstract PICInstruction? DecodeDAInstruction();

        protected abstract PICInstruction DecodeDBInstruction();

        protected abstract PICInstruction DecodeDTInstruction();

        protected abstract PICInstruction DecodeDTMInstruction();

        protected abstract PICInstruction DecodeDWInstruction();

        protected abstract PICInstruction DecodeUserIDInstruction();

        protected abstract PICInstruction? DecodeConfigInstruction();

        protected abstract PICInstruction DecodeDCIInstruction();

        protected abstract PICInstruction DecodeDIAInstruction();

        protected abstract PICInstruction DecodeRevisionIDInstruction();

        #endregion

    }

}
