#region License
/* 
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
using Reko.Core.Code;
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Loading;
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
        private ImageSegment? dataSegment;

        public WasmPreprocessor(WasmArchitecture arch, WasmPlatform platform, WasmFile wasmFile)
        {
            this.arch = arch;
            this.platform = platform;
            this.wasmFile = wasmFile;
        }

        public Program Preprocess()
        {
            var segmentMap = BuildSegmentMap(wasmFile.Sections);
            var program = new Program(new ByteProgramMemory(segmentMap), arch, platform);
            program.NeedsScanning = false;
            GenerateProcedures(program);
            return program;
        }

        private void GenerateProcedures(Program program)
        {
            if (wasmFile.CodeSection is null)
                return;
            var funidxToProc = GenerateFuncIdxToProcedureMap(program);
            var globidxToAddr = GenerateGlobalIdxToAddressMap();
            foreach (var func in wasmFile.CodeSection.Functions)
            {
                try
                {
                    var procBuilder = new WasmProcedureBuilder(func, arch, wasmFile, funidxToProc, globidxToAddr);
                    var proc = procBuilder.GenerateProcedure();
                    program.Procedures.Add(proc.EntryAddress, proc);
                    program.CallGraph.AddProcedure(proc);
                    PopulateImageMap(proc, program.ImageMap);
                } 
                catch
                {

                }

            }
            CreateCallGraph(program);
        }

        private void PopulateImageMap(Procedure proc, ImageMap imageMap)
        {
            foreach (var block in proc.ControlGraph.Blocks)
            {
                if (block == proc.EntryBlock || block == proc.ExitBlock)
                    continue;
                var blockItem = new ImageMapBlock(block.Address)
                {
                    Block = block,
                };
                imageMap.AddItem(block.Address, blockItem);
            }
        }

        private void CreateCallGraph(Program program)
        {
            static Expression? CallTargetOf(Statement stm)
            {
                switch (stm.Instruction)
                {
                case CallInstruction call:
                    return call.Callee;
                case Assignment ass:
                    if (ass.Src is Application appl)
                        return appl.Procedure;
                    break;
                case SideEffect side:
                    if (side.Expression is Application appl2)
                        return appl2.Procedure;
                    break;
                }
                return null;
            }

                
            foreach (var proc in program.Procedures.Values)
            {
                foreach (var stm in proc.Statements)
                {
                    var callTarget = CallTargetOf(stm);
                    if (callTarget is ProcedureConstant pc &&
                    pc.Procedure is Procedure procCallee)
                    { 
                        program.CallGraph.AddEdge(stm, procCallee);
                    }
                }
            }
        }

        private Dictionary<int, ProcedureBase> GenerateFuncIdxToProcedureMap(Program program)
        {
            Debug.Assert(wasmFile.CodeSection is not null, "If there are functions there must be a code segment.");
            Debug.Assert(codeSegment is not null, "If there are functions there must be a code segment.");
            var result = new Dictionary<int, ProcedureBase>();
            if (wasmFile.ImportSection is not null)
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

        private Dictionary<int, Address> GenerateGlobalIdxToAddressMap()
        {
            var result = new Dictionary<int, Address>();
            if (dataSegment is null)
                return result;
            //$BUG: this assertion is firing in the Debug version of the regression tests.
            // Investigate and fix.
            //Debug.Assert(wasmFile.GlobalSection is not null, "If there are globals there must be a data segment.");
            //Debug.Assert(dataSegment is not null, "If there are globals there must be a datasegment.");
            var addr = dataSegment.Address;
            //$BUG: until semantics are clear we allocate slots for each imported
            // global even if that's strictly incorrect.
            foreach (var entry in wasmFile.GlobalIndex)
            {
                var dt = entry.Type.Item1;
                result.Add(entry.GlobalIndex, addr);
                //$BUG: we don't take alignment into account here.
                addr += dt.Size;
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
                else if (section is DataSection)
                    this.dataSegment = segment;
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