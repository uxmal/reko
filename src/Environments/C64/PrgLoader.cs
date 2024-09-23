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
using Reko.Core.Collections;
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.Environments.C64
{
    /// <summary>
    /// Given a PRG image of a C64 basic program, loads it into a Reko <see cref="Program" />
    /// </summary>
    public class PrgLoader : ProgramImageLoader
    {
        public PrgLoader(IServiceProvider services, ImageLocation imageUri, byte[] rawImage) : base(services, imageUri, rawImage)
        {
            PreferredBaseAddress = Address.Ptr16(0x0801);
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program LoadProgram(Address? addrLoad)
        {
            var stm = new MemoryStream();
            ushort preferredAddress = ByteMemoryArea.ReadLeUInt16(RawImage, 0);
            ushort alignedAddress = (ushort) (preferredAddress & ~0xF);
            int pad = preferredAddress - alignedAddress;
            var c64Ram = new ByteMemoryArea(
                Address.Ptr16(0),
                new byte[0x10000]);
            Array.Copy(RawImage, 2, c64Ram.Bytes, preferredAddress, RawImage.Length - 2);
            var rdr = new C64BasicReader(c64Ram, 0x0801);
            var lines = rdr.ToSortedList(line => line.LineNumber, line => line);
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = new C64Basic(Services, lines);
            var platform = cfgSvc.GetEnvironment("c64").Load(Services, arch);
            var arch6502 = cfgSvc.GetArchitecture("m6502")!;
            SegmentMap segMap = CreateSegmentMap(platform, c64Ram, Address.Ptr16(alignedAddress), lines);
            var program = new Program(new ByteProgramMemory(segMap), arch, platform);
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

        private SegmentMap CreateSegmentMap(
            IPlatform platform, 
            ByteMemoryArea c64Ram, 
            Address addrStart,
            IDictionary<ushort, C64BasicInstruction> lines)
        {
            var segMap = platform.CreateAbsoluteMemoryMap()!;
            if (lines.Count > 0)
            {
                segMap.AddSegment(new ImageSegment("basic", addrStart, c64Ram, AccessMode.ReadExecute));
                var lastLine = lines.Values.OrderByDescending(l => l.Address).First();
                addrStart = lastLine.Address + lastLine.Line.Length;
            }
            segMap.AddSegment(new ImageSegment("code", addrStart, c64Ram, AccessMode.ReadWriteExecute));
            return segMap;
        }
    }
}
