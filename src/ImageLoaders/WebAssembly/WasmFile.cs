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

using Reko.Core.Loading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.ImageLoaders.WebAssembly
{
    public class WasmFile
    {
        public WasmFile(List<Section> sections)
        {
            this.Sections = sections;
            this.TypeSection = sections.OfType<TypeSection>().FirstOrDefault();
            this.ImportSection = sections.OfType<ImportSection>().FirstOrDefault();
            this.ExportSection = sections.OfType<ExportSection>().FirstOrDefault();
            this.GlobalSection = sections.OfType<GlobalSection>().FirstOrDefault();
            this.FunctionSection = sections.OfType<FunctionSection>().FirstOrDefault();
            this.CodeSection = sections.OfType<CodeSection>().FirstOrDefault();
            this.FunctionIndex = BuildFunctionsIndex();
            this.GlobalIndex = BuildGlobalsIndex();
        }

        public List<Section> Sections { get; }
        public TypeSection? TypeSection { get; }
        public ImportSection? ImportSection { get; }
        public ExportSection? ExportSection { get; }
        public GlobalSection? GlobalSection { get; }
        public FunctionSection? FunctionSection { get; }
        public CodeSection? CodeSection { get; }
        public List<FunctionDefinition> FunctionIndex { get; }
        public List<GlobalEntry> GlobalIndex { get; }

        private List<FunctionDefinition> BuildFunctionsIndex()
        {
            var result = new List<FunctionDefinition>();

            // First the imported functions...
            if (ImportSection is not null)
            {
                foreach (var entry in ImportSection.Imports)
                {
                    if (entry.Type == SymbolType.ExternalProcedure)
                    {
                        result.Add(new FunctionDefinition(
                            -1, -1,
                            Array.Empty<LocalVariable>(),
                            Array.Empty<byte>())
                        {
                            Name = entry.Name,
                            FunctionIndex = result.Count,
                            TypeIndex = entry.Index,
                        });
                    }
                }
            }
            // ...then the local functions.

            if (CodeSection is null || FunctionSection is null)
                return result;
            Debug.Assert(CodeSection.Functions.Count == FunctionSection!.Declarations.Count);
            int i = 0;
            foreach (var func in CodeSection.Functions)
            {
                func.FunctionIndex = result.Count;
                func.TypeIndex = FunctionSection.Declarations[i++];
                result.Add(func);
            }

            // Give exported functions names.
            if (ExportSection is null)
                return result;
            foreach (var export in ExportSection.ExportEntries)
            {
                if (export.Type == SymbolType.Procedure)
                {
                    result[(int)export.Index].Name = export.Name;
                }
            }
            return result;
        }

        private List<GlobalEntry> BuildGlobalsIndex()
        {
            var result = new List<GlobalEntry>();

            // First the imported globals...
            if (ImportSection is not null)
            {
                foreach (var entry in ImportSection.Imports)
                {
                    if (entry.Type == SymbolType.Data)
                    {
                        result.Add(new GlobalEntry
                        {
                            Type = entry.GlobalType,
                            GlobalIndex = result.Count,
                            Name = entry.Name,
                        });
                    }
                }
            }
            // ...then the local globals.
            if (this.GlobalSection is not null)
            {
                foreach (var entry in GlobalSection.Globals)
                {
                    entry.GlobalIndex = result.Count;
                    result.Add(entry);
                }
            }
            return result;
        }
    }
}
