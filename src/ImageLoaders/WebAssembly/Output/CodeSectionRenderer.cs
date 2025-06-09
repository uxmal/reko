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
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Output;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.ImageLoaders.WebAssembly.Output
{
    /// <summary>
    /// Renders WASM code sections.
    /// </summary>
    public class CodeSectionRenderer : ImageSegmentRenderer
    {
        private readonly WasmArchitecture arch;
        private readonly CodeSection codeSection;
        private readonly WasmFile file;
        private readonly TypeSection? typeSection;
        private readonly FunctionSection? funcSection;
        private readonly TableSection? tableSection;

        public CodeSectionRenderer(WasmArchitecture arch, CodeSection section, WasmFile file)
        {
            this.arch = arch;
            this.codeSection = section;
            this.file = file;
            this.typeSection = file.Sections.OfType<TypeSection>().FirstOrDefault();
            this.funcSection = file.Sections.OfType<FunctionSection>().FirstOrDefault();
            this.tableSection = file.Sections.OfType<TableSection>().FirstOrDefault();
        }

        public override void Render(ImageSegment segment, Program program, Formatter formatter)
        {
            formatter.WriteComment("WASM functions");
            foreach (var func in codeSection.Functions)
            {
                formatter.WriteLine();
                RenderFunction(func, formatter);
            }
        }

        public void RenderFunction(FunctionDefinition function, Formatter formatter)
        {
            var name = GetFunctionName(function.FunctionIndex);
            formatter.WriteLine("(func {0} (; {1} ;) retval??", name, function.FunctionIndex);
            // Address doesn't matter, we don't display it.
            var mem = new ByteMemoryArea(Address.Ptr32(0x1_0000), codeSection.Bytes);
            var rdr = new LeImageReader(mem, function.Start, function.End);
            var dasm = arch.CreateDisassembler(rdr);
            //$TODO provide real procedures and image symbols.
            var procedures = new Dictionary<Address, Procedure>();
            var symbols = new Dictionary<Address, ImageSymbol>();
            var instrRenderer = new FormatterInstructionWriter(
                formatter,
                procedures,
                symbols,
                false);
            var options = new MachineInstructionRendererOptions();
            var dataStack = new List<DataType>();
            var blockStack = new List<WasmInstruction>();
            foreach (WasmInstruction instr in dasm)
            {
                bool pushBlockStack = false;
                bool popBlockStack = false;
                switch (instr.Mnemonic)
                {
                case Mnemonic.i32_and:
                case Mnemonic.i32_add:
                case Mnemonic.i32_sub:
                case Mnemonic.i32_mul:
                case Mnemonic.i32_rem_s:
                case Mnemonic.i32_rem_u:
                case Mnemonic.i32_shl:
                case Mnemonic.i32_or:
                case Mnemonic.i32_shr_s:
                case Mnemonic.i32_shr_u:
                case Mnemonic.i32_xor:
                    dataStack.RemoveAt(dataStack.Count - 1);
                    dataStack[^1] = PrimitiveType.Word32;
                    break;
                case Mnemonic.i64_and:
                case Mnemonic.i64_add:
                case Mnemonic.i64_sub:
                case Mnemonic.i64_mul:
                case Mnemonic.i64_rem_s:
                case Mnemonic.i64_rem_u:
                case Mnemonic.i64_shl:
                case Mnemonic.i64_or:
                case Mnemonic.i64_shr_s:
                case Mnemonic.i64_shr_u:
                case Mnemonic.i64_xor:
                    dataStack.RemoveAt(dataStack.Count - 1);
                    dataStack[^1] = PrimitiveType.Word32;
                    break;
                case Mnemonic.f32_add:
                case Mnemonic.f32_div:
                case Mnemonic.f32_sub:
                case Mnemonic.f32_max:
                case Mnemonic.f32_min:
                case Mnemonic.f32_mul:
                    PopDataStack(dataStack, 1);
                    dataStack[^1] = PrimitiveType.Real32;
                    break;
                case Mnemonic.f64_add:
                case Mnemonic.f64_div:
                case Mnemonic.f64_sub:
                case Mnemonic.f64_max:
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
                case Mnemonic.br_table:
                    break;
                case Mnemonic.call:
                    if (typeSection is null)
                        throw new BadImageFormatException("Missing type section.");
                    var ifuncCall = ((Constant) instr.Operands[0]).ToInt32();
                    if (ifuncCall >= file.FunctionIndex.Count || this.funcSection is null)
                    {
                        Debug.Print("*** Unknown function {0:X}", ifuncCall);
                        break;
                    }
                    var funcDecl = (int) this.file.FunctionIndex[ifuncCall].TypeIndex;
                    HandleCall(typeSection.Types[funcDecl], dataStack);
                    break;
                case Mnemonic.call_indirect:
                    if (typeSection is null)
                        throw new BadImageFormatException("Missing type section.");
                    var itypeCall = ((Constant)instr.Operands[1]).ToInt32();
                    HandleCall(typeSection.Types[itypeCall], dataStack);
                    break;
                case Mnemonic.i32_const:
                    dataStack.Add(PrimitiveType.Word32);
                    break;
                case Mnemonic.i64_const:
                    dataStack.Add(PrimitiveType.Word64);
                    break;
                case Mnemonic.f32_const:
                    dataStack.Add(PrimitiveType.Real32);
                    break;
                case Mnemonic.f64_const:
                    dataStack.Add(PrimitiveType.Real64);
                    break;
                case Mnemonic.i64_div_s:
                    PopDataStack(dataStack, 1);
                    dataStack[^1] = PrimitiveType.Int64;
                    break;
                case Mnemonic.i64_div_u:
                    PopDataStack(dataStack, 1);
                    dataStack[^1] = PrimitiveType.Int64;
                    break;
                case Mnemonic.drop:
                    PopDataStack(dataStack, 1);
                    break;
                case Mnemonic.@else:
                    if (blockStack.Count < 1 || blockStack[^1].Mnemonic != Mnemonic.@if)
                        throw new BadImageFormatException("Else must follow an 'if'.");
                    blockStack[^1] = instr;
                    break;
                case Mnemonic.end:
                    popBlockStack = true;
                    break;

                case Mnemonic.f32_eq:
                case Mnemonic.f32_ge:
                case Mnemonic.f32_gt:
                case Mnemonic.f32_ne:

                case Mnemonic.f64_eq:
                case Mnemonic.f64_ge:
                case Mnemonic.f64_gt:
                case Mnemonic.f64_ne:

                case Mnemonic.i32_eq:
                case Mnemonic.i32_ge_s:
                case Mnemonic.i32_ge_u:
                case Mnemonic.i32_gt_s:
                case Mnemonic.i32_gt_u:
                case Mnemonic.i32_le_s:
                case Mnemonic.i32_le_u:
                case Mnemonic.i32_lt_s:
                case Mnemonic.i32_lt_u:
                case Mnemonic.i32_ne:

                case Mnemonic.i64_eq:
                case Mnemonic.i64_ge_s:
                case Mnemonic.i64_ge_u:
                case Mnemonic.i64_gt_s:
                case Mnemonic.i64_gt_u:
                case Mnemonic.i64_le_s:
                case Mnemonic.i64_le_u:
                case Mnemonic.i64_lt_s:
                case Mnemonic.i64_lt_u:
                case Mnemonic.i64_ne:
                    PopDataStack(dataStack, 1);
                    dataStack[^1] = PrimitiveType.Bool;
                    break;
                case Mnemonic.i32_eqz:
                case Mnemonic.i64_eqz:
                    dataStack[^1] = PrimitiveType.Bool;
                    break;
                case Mnemonic.i64_extend_s_i32:
                    dataStack[^1] = PrimitiveType.Int64;
                    break;
                case Mnemonic.i64_extend_u_i32:
                    dataStack[^1] = PrimitiveType.UInt64;
                    break;
                case Mnemonic.get_global:
                    dataStack.Add(new UnknownType());
                    break;
                case Mnemonic.get_local:
                    dataStack.Add(new UnknownType());
                    break;
                case Mnemonic.@if:
                    PopDataStack(dataStack, 1);
                    pushBlockStack = true;
                    break;
                case Mnemonic.f32_load:
                    dataStack[^1] = PrimitiveType.Real32;
                    break;
                case Mnemonic.f64_load:
                    dataStack[^1] = PrimitiveType.Real64;
                    break;
                case Mnemonic.i32_load:
                    dataStack[^1] = PrimitiveType.Word32;
                    break;
                case Mnemonic.i64_load:
                    dataStack[^1] = PrimitiveType.Word64;
                    break;
                case Mnemonic.i32_load8_s:
                    dataStack[^1] = PrimitiveType.Int32;
                    break;
                case Mnemonic.i32_load8_u:
                    dataStack[^1] = PrimitiveType.UInt32;
                    break;
                case Mnemonic.i32_load16_s:
                    dataStack[^1] = PrimitiveType.Int32;
                    break;
                case Mnemonic.loop:
                    pushBlockStack = true;
                    break;
                case Mnemonic.f64_abs:
                case Mnemonic.f64_neg:
                case Mnemonic.f64_sqrt:
                    //$TODO: Validate args?
                    dataStack[^1] = PrimitiveType.Real64;
                    break;
                case Mnemonic.nop:
                    break;
                case Mnemonic.f32_convert_s_i32:
                case Mnemonic.f32_demote_f64:
                case Mnemonic.f32_reinterpret_i32:
                    dataStack[^1] = PrimitiveType.Real32;
                    break;
                case Mnemonic.f64_convert_s_i32:
                case Mnemonic.f64_convert_s_i64:
                case Mnemonic.f64_convert_u_i32:
                case Mnemonic.f64_convert_u_i64:
                case Mnemonic.f64_promote_f32:
                case Mnemonic.f64_reinterpret_i64:
                    dataStack[^1] = PrimitiveType.Real64;
                    break;
                case Mnemonic.i32_reinterpret_f32:
                case Mnemonic.i32_wrap_i64:
                    dataStack[^1] = PrimitiveType.Word32;
                    break;
                case Mnemonic.i64_reinterpret_f64:
                    dataStack[^1] = PrimitiveType.Word64;
                    break;
                case Mnemonic.@return:
                    // Leave the structured block we're in
                    break;
                case Mnemonic.select:   // pop 3, but push 1
                    PopDataStack(dataStack, 2);
                    break;
                case Mnemonic.set_global:
                case Mnemonic.set_local:
                    PopDataStack(dataStack, 1);
                    break;
                case Mnemonic.f32_store:
                case Mnemonic.f64_store:
                case Mnemonic.i32_store:
                case Mnemonic.i64_store:
                case Mnemonic.i32_store8:
                case Mnemonic.i32_store16:
                case Mnemonic.i64_store16:
                    PopDataStack(dataStack, 2);
                    break;
                case Mnemonic.tee_local:
                case Mnemonic.unreachable:
                    break;
                default:
                    Debug.WriteLine("$$$$ BLE: " + instr);
                    break;
                    //throw new NotImplementedException(instr.ToString());
                }

                if (popBlockStack)
                {
                    if (blockStack.Count < 1)
                        break; //  throw new BadImageFormatException("Mismatched 'end' instruction.");
                    blockStack.RemoveAt(blockStack.Count - 1);
                    formatter.Indentation -= formatter.TabSize;
                }
                formatter.Indent();
#if VERBOSE
                var debug = $"{instr.Address}:{dataStack.Count,3}{new string(' ', blockStack.Count * 4)} {instr}";
                Debug.WriteLine(debug);
                Console.WriteLine(debug);
#endif
                instr.Render(instrRenderer, options);
                formatter.WriteLine();
                if (pushBlockStack)
                {
                    blockStack.Add(instr);
                    formatter.Indentation += formatter.TabSize;
                }
            }
            formatter.WriteLine(")");
        }

        private void PopDataStack(List<DataType> dataStack, int n)
        {
            if (dataStack.Count < n)
            {
                Debug.WriteLine($"*** stack imbalance, expected {n} stack slots");
                n = dataStack.Count;
            }
            dataStack.RemoveRange(dataStack.Count - n, n);
        }

        private static void HandleCall(FunctionType sig, List<DataType> dataStack)
        {
            var cParams = sig.Parameters!.Length;
            if (cParams > dataStack.Count)
            {
                Debug.WriteLine($"*** stack imbalance, require {cParams} arguments");
                cParams = dataStack.Count;
            }
            dataStack.RemoveRange(dataStack.Count - cParams, cParams);
            if (!sig.HasVoidReturn)
                dataStack.Add(sig.Outputs[0].DataType);
        }

        private string GetFunctionName(int ifunc)
        {
            string? name;
            if (0 <= ifunc && ifunc < file.FunctionIndex.Count)
            {
                name = file.FunctionIndex[ifunc].Name;
                if (name is not null)
                    return name;
            }
            return $"fn{ifunc:000000}";
        }
    }
}