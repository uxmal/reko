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
using System.Diagnostics;
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
            var blockStack = new List<WasmInstruction>();
            foreach (WasmInstruction instr in dasm)
            {
                bool pushBlockStack = false;
                bool popBlockStack = false;
                switch (instr.Mnemonic)
                {
                case Mnemonic.i32_add:
                case Mnemonic.i32_sub:
                case Mnemonic.i32_mul:
                    dataStack.RemoveAt(dataStack.Count - 1);
                    dataStack[^1] = PrimitiveType.Word32;
                    break;
                case Mnemonic.f64_add:
                case Mnemonic.f64_div:
                case Mnemonic.f64_sub:
                case Mnemonic.f64_min:
                case Mnemonic.f64_mul:
                    dataStack.RemoveAt(dataStack.Count - 1);
                    dataStack[^1] = PrimitiveType.Real64;
                    break;

                case Mnemonic.block:
                    pushBlockStack = true;
                    break;
                case Mnemonic.br:
                    break;
                case Mnemonic.br_if:
                    dataStack.RemoveAt(dataStack.Count - 1);
                    break;
                case Mnemonic.call: //$TODO: stack effects.
                    break;
                case Mnemonic.i32_const:
                    dataStack.Add(PrimitiveType.Word32);
                    break;
                case Mnemonic.f64_const:
                    dataStack.Add(PrimitiveType.Real64);
                    break;
                case Mnemonic.i64_div_s:
                    dataStack.RemoveAt(dataStack.Count - 1);
                    dataStack[^1] = PrimitiveType.Int64;
                    break;
                case Mnemonic.@else:
                    if (blockStack.Count < 1 || blockStack[^1].Mnemonic != Mnemonic.@if)
                        throw new BadImageFormatException("Else must follow an 'if'.");
                    blockStack[^1] = instr;
                    break;
                case Mnemonic.end:
                    popBlockStack = true;
                    break;
                case Mnemonic.i32_eq:
                case Mnemonic.i32_ge_s:
                case Mnemonic.i32_gt_s:
                case Mnemonic.i32_le_s:
                case Mnemonic.i32_lt_s:
                case Mnemonic.i32_ne:
                    dataStack.RemoveAt(dataStack.Count - 1);
                    dataStack[^1] = PrimitiveType.Bool;
                    break;
                case Mnemonic.get_global:
                    dataStack.Add(new UnknownType());
                    break;
                case Mnemonic.get_local:
                    dataStack.Add(new UnknownType());
                    break;
                case Mnemonic.@if:
                    pushBlockStack = true;
                    break;
                case Mnemonic.f64_load:
                    dataStack[^1] = PrimitiveType.Real64;
                    break;
                case Mnemonic.loop:
                    blockStack.Add(instr);
                    break;
                case Mnemonic.f64_neg:
                case Mnemonic.f64_sqrt:
                    //$TODO: Validate args?
                    dataStack[^1] = PrimitiveType.Real64;
                    break;
                case Mnemonic.nop:
                    break;
                case Mnemonic.@return:
                    // Leave the structured block we're in
                    break;
                case Mnemonic.set_global:
                case Mnemonic.set_local:
                    dataStack.RemoveAt(dataStack.Count - 1);
                    break;
                case Mnemonic.f64_store:
                    dataStack.RemoveRange(dataStack.Count - 2, 2);
                    break;
                case Mnemonic.tee_local:
                case Mnemonic.unreachable:
                    break;
                default:
                    throw new NotImplementedException(instr.ToString());
                }

                if (popBlockStack)
                {
                    if (blockStack.Count < 1)
                        break; //  throw new BadImageFormatException("Mismatched 'end' instruction.");
                    blockStack.RemoveAt(blockStack.Count - 1);
                    formatter.Indentation -= formatter.TabSize;
                }
                formatter.Indent();
                instr.Render(instrRenderer, options);
                formatter.WriteLine();
                if (pushBlockStack)
                {
                    blockStack.Add(instr);
                    formatter.Indentation += formatter.TabSize;
                }
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