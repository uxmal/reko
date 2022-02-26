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
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Output;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.ImageLoaders.WebAssembly
{
    public class CodeSectionRenderer : ImageSegmentRenderer
    {
        private readonly WasmArchitecture arch;
        private readonly CodeSection codeSection;
        private readonly List<Section> sections;

        public CodeSectionRenderer(WasmArchitecture arch, CodeSection section, List<Section> sections)
        {
            this.arch = arch;
            this.codeSection = section;
            this.sections = sections;
        }

        public override void Render(ImageSegment segment, Program program, Formatter formatter)
        {
            formatter.WriteComment("WASM functions");
            int iFunc = -1;
            foreach (var func in codeSection.Functions)
            {
                ++iFunc;
                formatter.WriteLine();
                RenderFunction(iFunc, func, formatter);
            }
        }

        public void RenderFunction(int ifunc, FunctionDefinition function, Formatter formatter)
        {
            var name = GetFunctionName(ifunc);
            formatter.WriteLine("(func {0} (; {1} ;) retval??", name, ifunc);
            // Address doesn't matter, we don't display it.
            var mem = new ByteMemoryArea(Address.Ptr32(0x1_0000), codeSection.Bytes);
            var rdr = new LeImageReader(mem, function.Start, function.End);
            var dasm = arch.CreateDisassembler(rdr);
            var instrRenderer = new FormatterInstructionWriter(formatter, false);
            var options = new MachineInstructionRendererOptions();
            var dataStack = new List<DataType>();
            foreach (WasmInstruction instr in dasm)
            {
                formatter.Indent();
                switch (instr.Mnemonic)
                {
                case Mnemonic.i32_const:
                    dataStack.Add(PrimitiveType.Word32);
                    break;
                case Mnemonic.@return:
                    formatter.Indentation -= formatter.TabSize; // Leave the structured block we're in
                    break;
                default:
                    throw new NotImplementedException(instr.ToString());
                }
                instr.Render(instrRenderer, options);
                formatter.WriteLine();
            }
            formatter.Indentation -= formatter.TabSize;
            formatter.WriteLine(")");
        }

        private string GetFunctionName(int ifunc)
        {
            var exports = sections.OfType<ExportSection>().FirstOrDefault();
            if (exports is not null)
            {
                var funcExport = exports.ExportEntries.Where(e => e.Kind == ExportEntry.Func && e.Index == ifunc).FirstOrDefault();
                if (funcExport != null && !string.IsNullOrEmpty(funcExport.Field))
                    return funcExport.Field;
            }
            return $"fn{ifunc:6}";
        }
    }
}