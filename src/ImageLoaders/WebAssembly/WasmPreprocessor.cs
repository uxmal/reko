#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.ImageLoaders.WebAssembly
{
    public class WasmPreprocessor
    {
        private const int WasmPageSize = 0x1_0000;

        private readonly List<Section> sections;
        private readonly WasmArchitecture arch;
        private readonly DefaultPlatform platform;

        public WasmPreprocessor(List<Section> sections, WasmArchitecture arch, DefaultPlatform platform)
        {
            this.sections = sections;
            this.arch = arch;
            this.platform = platform;
        }

        public Program Preprocess()
        {
            var segmentMap = BuildSegmentMap(sections);
            var program = new Program(segmentMap, arch, platform);
            GenerateProcedures(program);
            return program;
        }

        private void GenerateProcedures(Program program)
        {
        }

        private SegmentMap BuildSegmentMap(List<Section> sections)
        {
            var segments = new List<ImageSegment>();
            var segmentNames = new HashSet<string>();
            var addr = Address.Ptr32(0x1_0000); // Wasm pages are 64k
            foreach (var section in sections)
            {
                var designer = section.CreateDesigner(arch, sections);
                var segment = new ImageSegment(
                    section.Name,
                    new ByteMemoryArea(addr, section.Bytes),
                    AccessMode.ReadExecute);
                segment.Designer = designer;
                segments.Add(segment);
                addr = (addr + section.Bytes.Length + WasmPageSize - 1).Align(WasmPageSize);
            }

            var dataSection = sections.OfType<DataSection>().SingleOrDefault();
            if (dataSection is not null)
            {
                segments.AddRange(dataSection.Segments.Select(s => new ImageSegment(
                    $".data{s.MemoryIndex}",
                    new ByteMemoryArea(Address.Ptr32(s.Offset), s.Bytes),
                    AccessMode.ReadWrite)));
            }

            if (segments.Count == 0)
            {
                return new SegmentMap(Address.Ptr32(0));
            }
            return new SegmentMap(segments.ToArray());
        }
    }
}