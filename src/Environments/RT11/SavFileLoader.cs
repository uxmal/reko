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
using Reko.Core;
using Reko.Arch.Pdp11;
using System.Collections.Generic;
using Reko.Core.Types;

namespace Reko.Environments.RT11
{
    public class SavFileLoader : ImageLoader
    {
        public SavFileLoader(IServiceProvider services, string filename, byte[] imgRaw) : base(services, filename, imgRaw)
        {
            this.PreferredBaseAddress = Address.Ptr16(0);
        }

        public override Address PreferredBaseAddress
        {
            get; set;
        }

        public override Program Load(Address addrLoad)
        {
            var arch = new Pdp11Architecture("pdp11");

            return new Program(
                new SegmentMap(addrLoad,
                new ImageSegment(".text",
                        new MemoryArea(addrLoad, RawImage),
                        AccessMode.ReadWriteExecute)),
                arch,
                new RT11Platform(Services, arch));
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            var header = CreateSavHeader(program.Architecture);
            var uaddrEntry = MemoryArea.ReadLeUInt16(RawImage, 0x20);
            var entry = ImageSymbol.Procedure(program.Architecture, Address.Ptr16(uaddrEntry));
            return new RelocationResults(
                new List<ImageSymbol> { entry },
                new SortedList<Address, ImageSymbol>
                {
                    { header.Address, header }
                });
        }

        // SAV definition from 
        // RT–11 Volume and File Formats Manual
        // Order Number AA–PD6PA–TC
        // August 1991
        // Digital Equipment Corporation
        private ImageSymbol CreateSavHeader(IProcessorArchitecture arch)
        {
            StructureField fld(int offset, DataType dt)
            {
                return new StructureField(offset, dt);
            }

            var w16 = PrimitiveType.Word16;
            var s = new StructureType("sav_header_t", 0x200, true)
            {
                ForceStructure = true,
                Fields =
                {
                    fld(0x00, w16), // VIR in Radix–50 if the Linker / V option was used.
                    fld(0x02, w16), // Virtual high limit if Linker / V option was used.
                    fld(0x04, w16), // Job definition word($JSX) bits.See Table 2–11 for bit definitions.
                    fld(0x06, w16), // Reserved
                    fld(0x08, w16), // Reserved
                    fld(0x0A, w16), // Reserved
                    fld(0x0C, w16), // BPT trap PC(mapped monitors only)
                    fld(0x0E, w16), // BPT trap PSW(mapped monitors only)
                    fld(0x10, w16), // IOT trap PC(mapped monitors only)
                    fld(0x12, w16), // IOT trap PSW(mapped monitors only)
                    fld(0014, w16), // Reserved
                    fld(0x16, w16), // Reserved
                    fld(0x18, w16), // Reserved
                    fld(0x1A, w16), // Overlay definition word(SV.CVH) bits.See tables 2–12 and 2–13 for bit definitions.
                    fld(0x1C, w16), // Trap vector PC(TRAP)
                    fld(0x1E, w16), // Trap vector PSW(TRAP)
                    fld(0x20, w16), // Program’s relative start address
                    fld(0x22, w16), // Initial location of stack pointer(changed by / M option)
                    fld(0x24, w16), // Job Status Word
                    fld(0x26, w16), // USR swap address
                    fld(0x28, w16), // Program’s high limit
                    
                    fld(0x34, w16), // Address of overlay handler table for overlaid files
                    fld(0x36, w16), // Address of start of window definition blocks(if / V used)
                }
            };
            var addr = Address.Ptr16(0);
            var sym = ImageSymbol.DataObject(arch, addr, null, s);
            return sym;
        }
    }
}