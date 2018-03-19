#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Libraries.Microchip;
using System;

namespace Reko.Arch.Microchip.Common
{
    /// <summary>
    /// A Microchip 8-bit MCU disassembler frame. Must be inherited.
    /// Valid for most of program memory regions (code, eeprom, config, ...).
    /// </summary>
    public abstract class PICDisassemblerBase : DisassemblerBase<PICInstruction>
    {
        public readonly PICArchitecture arch;
        public readonly EndianImageReader rdr;
        public readonly IMemoryMap memorymap;

        protected PICInstruction instrCur;
        public Address addrCur;

        protected static IMemoryRegion lastusedregion = null;

        /// <summary>
        /// Instantiates a base PIC disassembler.
        /// </summary>
        /// <param name="arch">The PIC architecture.</param>
        /// <param name="rdr">The memory reader.</param>
        protected PICDisassemblerBase(PICArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            memorymap = arch.MemoryDescriptor.MemoryMap;
        }

        /// <summary>
        /// Gets the PIC instruction-set identifier. Must be overriden.
        /// </summary>
        public virtual InstructionSetID InstructionSetID => InstructionSetID.UNDEFINED;

        /// <summary>
        /// Gets the PIC execution mode this disassembler is configured to.
        /// </summary>
        public abstract PICExecMode ExecMode { get; }

        /// <summary>
        /// Disassemble a single instruction. Return null if the end of the reader has been reached.
        /// </summary>
        /// <returns>
        /// A <seealso cref="PICInstruction"/> instance.
        /// </returns>
        /// <exception cref="AddressCorrelatedException">Thrown when the Address Correlated error
        ///                                              condition occurs.</exception>
        public override PICInstruction DisassembleInstruction()
        {
            IMemoryRegion GetProgRegion()
            {
                if (lastusedregion != null && lastusedregion.Contains(addrCur))
                    return lastusedregion;
                return lastusedregion = memorymap.GetProgramRegion(addrCur);
            }

            if (!rdr.IsValid)
                return null;
            addrCur = rdr.Address;
            var regn = GetProgRegion();
            if (regn is null)
                throw new InvalidOperationException($"Unable to retrieve program memory region for address {addrCur.ToString()}.");
            if ((addrCur.Offset % (regn.Trait?.LocSize ?? 1)) != 0)
            {
                instrCur = new PICInstruction(Opcode.unaligned)
                {
                    Address = addrCur,
                    Length = 1,
                    ExecMode = ExecMode
                };
                rdr.Offset += 1; // Consume only the first byte of the binary instruction.
                return instrCur;
            }

            switch (regn.SubtypeOfMemory)
            {
                case MemorySubDomain.Code:
                case MemorySubDomain.ExtCode:
                case MemorySubDomain.Debugger:
                    if (!rdr.TryReadUInt16(out ushort uInstr))
                        return null;
                    return DecodePICInstruction(uInstr, PICProgAddress.Ptr(rdr.Address));

                case MemorySubDomain.EEData:
                    return DecodeEEPROMInstruction();

                case MemorySubDomain.UserID:
                    return DecodeUserIDInstruction();

                case MemorySubDomain.DeviceConfig:
                    return DecodeConfigInstruction();

                case MemorySubDomain.DeviceID:
                    return DecodeDWInstruction();

                case MemorySubDomain.DeviceConfigInfo:  //TODO: Decode DCI
                    return DecodeDCIInstruction();

                case MemorySubDomain.DeviceInfoAry:     //TODO: Decode DIA 
                    return DecodeDIAInstruction();

                case MemorySubDomain.RevisionID:        //TODO: Decode Revision ID
                    return DecodeRevisionIDInstruction();

                case MemorySubDomain.Test:
                case MemorySubDomain.Other:
                default:
                    throw new NotImplementedException($"Disassembly of '{regn.SubtypeOfMemory}' memory region is not yet implemented.");
            }

        }

        #region Methods defined by derived classes

        protected abstract PICInstruction DecodePICInstruction(ushort uInstr, PICProgAddress addr);

        protected abstract PICInstruction DecodeEEPROMInstruction();

        protected abstract PICInstruction DecodeDAInstruction();

        protected abstract PICInstruction DecodeDBInstruction();

        protected abstract PICInstruction DecodeDTInstruction();

        protected abstract PICInstruction DecodeDTMInstruction();

        protected abstract PICInstruction DecodeDWInstruction();

        protected abstract PICInstruction DecodeUserIDInstruction();

        protected abstract PICInstruction DecodeConfigInstruction();

        protected abstract PICInstruction DecodeDCIInstruction();

        protected abstract PICInstruction DecodeDIAInstruction();

        protected abstract PICInstruction DecodeRevisionIDInstruction();

        #endregion

    }

}
