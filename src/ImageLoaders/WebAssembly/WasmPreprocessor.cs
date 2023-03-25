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
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.ImageLoaders.WebAssembly
{
    public class WasmPreprocessor
    {
        private const int WasmPageSize = 0x1_0000;

        private readonly WasmArchitecture arch;
        private readonly WasmPlatform platform;
        private readonly WasmFile wasmFile;
        private ImageSegment? codeSegment;

        public WasmPreprocessor(WasmArchitecture arch, WasmPlatform platform, WasmFile wasmFile)
        {
            this.arch = arch;
            this.platform = platform;
            this.wasmFile = wasmFile;
        }

        public Program Preprocess()
        {
            var segmentMap = BuildSegmentMap(wasmFile.Sections);
            var program = new Program(segmentMap, arch, platform);
            GenerateProcedures(program);
            return program;
        }

        private void GenerateProcedures(Program program)
        {
            if (wasmFile.CodeSection is null)
                return;
            var funidxToProc = GenerateFuncIdxToProcedureMap();
            foreach (var func in wasmFile.CodeSection.Functions)
            {
                var procBuilder = new WasmProcedureBuilder(func, arch, wasmFile, funidxToProc);
                var proc = procBuilder.GenerateProcedure();
                program.Procedures.Add(proc.EntryAddress, proc);
            }
        }

        private Dictionary<int, ProcedureBase> GenerateFuncIdxToProcedureMap()
        {
            Debug.Assert(wasmFile.CodeSection is not null, "If there are functions there must be a code segment.");
            Debug.Assert(codeSegment is not null, "If there are functions there must be a code segment.");
            var result = new Dictionary<int, ProcedureBase>();
            if (wasmFile.ImportSection != null)
            {
                int iProc = 0;
                foreach (var func in wasmFile.ImportSection.Imports)
                {
                    if (func.Type == SymbolType.ExternalProcedure)
                    {
                        var exp = new ExternalProcedure(func.Name ?? "???", new Core.Types.FunctionType());
                        result.Add(iProc, exp);
                        ++iProc;
                    }
                }   
            }
            Address addrProc = codeSegment.Address;
            foreach (var func in wasmFile.CodeSection.Functions)
            {
                result.Add(func.FunctionIndex, Procedure.Create(arch, func.Name, addrProc, arch.CreateFrame()));
                addrProc += (func.End - func.Start);
            }
            return result;
        }

        private SegmentMap BuildSegmentMap(List<Section> sections)
        {
            var segments = new List<ImageSegment>();
            var segmentNames = new HashSet<string>();
            var addr = Address.Ptr32(0x1_0000); // Wasm pages are 64k
            foreach (var section in sections)
            {
                var designer = section.CreateDesigner(arch, wasmFile);
                var segment = new ImageSegment(
                    section.Name,
                    new ByteMemoryArea(addr, section.Bytes),
                    AccessMode.ReadExecute);
                segment.Designer = designer;
                segments.Add(segment);
                if (section is CodeSection)
                    this.codeSegment = segment;
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