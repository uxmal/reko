#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Cray
{
    public class UnicosLoader : ProgramImageLoader
    {
        private ulong a_entry;

        public UnicosLoader(IServiceProvider services, ImageLocation imageLocation, byte[] rawImage)
            : base(services, imageLocation, rawImage)
        {

        }

        public override Address PreferredBaseAddress
        {
            get { return Address.Ptr32(0); }
            set { throw new NotSupportedException(); } 
        }

#if NOT

struct exec {
    union {
        long omagic; /* old magic number */
        struct {
            unsigned : 32; /* new, reserved - must be zero */
            unsigned st : 1; /* new, shared text indicator */
            unsigned : 7; /* new, reserved - must be zero */
            unsigned pmt : 8; /* new, primary machine type */
            unsigned id : 16; /* magic identifier */
        }
        nmagic;
    } u_mag;
    long a_text; /* size of text area in words */
    long a_data; /* size of data area in words */
    long a_bss; /* size of bss area in words */
    long a_syms; /* size of symbol table in words */
    long a_entry; /* entry point (parcel address) */
    long a_origin; /* old base address (usually zero) */
    union
    {
        long ofill1; /* flag, 1 = relocation info stripped */
        struct {
            unsigned ptr : 32; /* new, byte offset of _infoblk */
            unsigned : 31; /* new, reserved - must be zero */
            unsigned str : 1; /* new, stripped bit */
        } info;
    } u_fill1; */
}
}
#endif
        public enum MachineType
        {
            A_PMT_UNDF = 0,         // undefined machine type =>old hdr 
            A_PMT_INC = 1,          // incremental load code fragment 
            A_PMT_CRAY1 = 2,        // CRAY-1S 
            A_PMT_XMP_NOEMA = 3,    // CRAY-X/MP, 22-bit mode 
            A_PMT_XMP_ANY = 4,      // CRAY-X/MP, mode indifferent 
            A_PMT_XMP_EMA = 5,      // CRAY-X/MP, 24-bit mode 
            A_PMT_CRAY2 = 6,        // CRAY-2 
            A_PMT_YMP = 7,          // CRAY-Y/MP 
            A_PMT_C90 = 8,          // CRAY C90 
        }

        public override Program LoadProgram(Address? addrLoad)
        {
            var rdr = new BeImageReader(RawImage);
            if (!rdr.TryReadBeUInt64(out ulong magic))
                throw new BadImageFormatException();
            if (!rdr.TryReadBeUInt64(out ulong a_text))
                throw new BadImageFormatException();
            if (!rdr.TryReadBeUInt64(out ulong a_data))
                throw new NotImplementedException();
            if (!rdr.TryReadBeUInt64(out ulong a_bss))
                throw new NotImplementedException();

            if (!rdr.TryReadBeUInt64(out ulong a_yms))
                throw new NotImplementedException();
            if (!rdr.TryReadBeUInt64(out this.a_entry))
                throw new NotImplementedException();
            if (!rdr.TryReadBeUInt64(out ulong a_origin))
                throw new NotImplementedException();
            if (!rdr.TryReadBeUInt64(out ulong u_fill))
                throw new NotImplementedException();

            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = CreateArchitecture(cfgSvc, (MachineType)((magic >> 16) & 0xFF));
            var text = new Word16MemoryArea(Address.Ptr32((uint) a_origin * 4), ReadWords16(rdr, a_text * 4));
            var data = new Word64MemoryArea(Address.Ptr32((uint) (a_origin + a_text)), ReadWords64(rdr, a_data));
            var bss = new Word64MemoryArea(Address.Ptr32((uint) (a_origin + a_text + a_bss)), new ulong[a_bss]);
            var segs = new SegmentMap(
                text.BaseAddress,
                new ImageSegment("text", text, AccessMode.ReadExecute),
                new ImageSegment("data", data, AccessMode.ReadWrite),
                new ImageSegment("bss", bss, AccessMode.ReadWrite));
            var platform = cfgSvc.GetEnvironment("unicos").Load(Services, arch);
            var program = new Program(new ByteProgramMemory(segs), arch, platform);
            var entry = ImageSymbol.Procedure(program.Architecture, Address.Ptr32((uint) a_entry), "_start");
            program.EntryPoints[entry.Address] = entry;
            return program;
        }

        private IProcessorArchitecture CreateArchitecture(IConfigurationService cfgSvc, MachineType machineType)
        {
            string sArch;
            switch (machineType)
            {
            case MachineType.A_PMT_YMP:
                sArch = "crayYmp";
                break;
            default:
                throw new NotImplementedException($"Architecture {machineType} not implemented.");
            }
            //$REFACTOR: don't need the dict.
            return cfgSvc.GetArchitecture(sArch, new Dictionary<string,object>())!;
        }

        private ushort[] ReadWords16(EndianImageReader rdr, ulong count)
        {
            var words = new ushort[count];
            for (int i = 0; i < words.Length; ++i)
            {
                if (!rdr.TryReadUInt16(out words[i]))
                    throw new BadImageFormatException();
            }
            return words;
        }

        private ulong[] ReadWords64(EndianImageReader rdr, ulong count)
        {
            var words = new ulong[count];
            for (int i = 0; i < words.Length; ++i)
            {
                if (!rdr.TryReadUInt64(out words[i]))
                    throw new BadImageFormatException();
            }
            return words;
        }
    }
}