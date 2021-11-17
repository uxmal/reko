#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

using Reko.Arch.Mos6502;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Environments.C64
{
    /// <summary>
    /// Given a PRG image of a C64 basic program, loads it into a Reko <see cref="Program" />
    /// </summary>
    public class PrgLoader : ProgramImageLoader
    {
        public PrgLoader(IServiceProvider services, string filename, byte[] rawImage): base(services, filename, rawImage)
        {
            PreferredBaseAddress = Address.Ptr16(0x0801);
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program LoadProgram(Address? addrLoad)
        {
            var cfgSvc = Services.RequireService<IConfigurationService>();
            return base.LoadProgram(addrLoad!, null!, null!);
        }

        public override Program LoadProgram(Address addrLoad, IProcessorArchitecture arch, IPlatform platform)
        {
            var stm = new MemoryStream();
            ushort preferredAddress = ByteMemoryArea.ReadLeUInt16(RawImage, 0);
            ushort alignedAddress = (ushort) (preferredAddress & ~0xF);
            int pad = preferredAddress - alignedAddress;
            while (pad-- > 0)
                stm.WriteByte(0);
            stm.Write(RawImage, 2, RawImage.Length - 2);
            var loadedBytes = stm.ToArray();
            var image = new ByteMemoryArea(
                Address.Ptr16(alignedAddress),
                loadedBytes);
            var rdr = new C64BasicReader(image, 0x0801);
            var lines = rdr.ToSortedList(line => line.LineNumber, line => line);
            var cfgSvc = Services.RequireService<IConfigurationService>();
            arch = new C64Basic(Services, lines);
            platform = cfgSvc.GetEnvironment("c64").Load(Services, arch);
            var arch6502 = cfgSvc.GetArchitecture("m6502")!;
            SegmentMap segMap = CreateSegmentMap(platform, image, lines);
            var program = new Program(segMap, arch, platform);
            program.Architectures.Add(arch6502.Name, arch6502);
            var addrBasic = lines.Values[0].Address;
            var sym = ImageSymbol.Procedure(arch, addrBasic, state: arch.CreateProcessorState());
            program.EntryPoints.Add(sym.Address, sym);
            AddLineNumberSymbols(lines, program);
            return program;
        }

        private void AddLineNumberSymbols(SortedList<ushort, C64BasicInstruction> lines, Program program)
        {
            foreach (var line in lines.Values)
            {
                var sym = ImageSymbol.Location(program.Architecture, line.Address);
                sym.Name = $"L{line.LineNumber}";
                program.ImageSymbols.Add(line.Address, sym);
            }
        }

        private SegmentMap CreateSegmentMap(IPlatform platform, ByteMemoryArea bmem, IDictionary<ushort, C64BasicInstruction> lines)
        {
            var segMap = platform.CreateAbsoluteMemoryMap()!;
            Address addrStart = bmem.BaseAddress;
            if (lines.Count > 0)
            {
                segMap.AddSegment(new ImageSegment("basic", bmem.BaseAddress, bmem, AccessMode.ReadExecute));
                var lastLine = lines.Values.OrderByDescending(l => l.Address).First();
                addrStart = lastLine.Address + lastLine.Line.Length;
            }
            segMap.AddSegment(new ImageSegment("code", addrStart, bmem, AccessMode.ReadWriteExecute));
            return segMap;
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            return new RelocationResults(
                new List<ImageSymbol>(),
                new SortedList<Address, ImageSymbol>());
        }
    }
}
