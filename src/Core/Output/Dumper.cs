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

using Reko.Core.Collections;
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Core.Output
{
    /// <summary>
    /// Dumps low-level information about a binary.
    /// </summary>
	public class Dumper
    {
        private readonly Program program;

        /// <summary>
        /// Creates a <see cref="Dumper"/> for the given <see cref="Program"/>.
        /// </summary>
        /// <param name="program">Program to dump.</param>
        public Dumper(Program program)
		{
            this.program = program;
		}

        /// <summary>
        /// If true, the disassembler will render instructions in a canonical form.
        /// If false, pseudoinstructions will be rendered where possible.
        /// </summary>
        public bool RenderInstructionsCanonically { get; set; }

        /// <summary>
        /// If true, the disassembler will show the address of each instruction.
        /// </summary>
        public bool ShowAddresses { get; set; }

        /// <summary>
        /// If true, the disassembler will show the raw bytes of each instruction.
        /// </summary>
        public bool ShowCodeBytes { get; set; }

        /// <summary>
        /// Dumps the program to the given formatter.
        /// </summary>
        /// <param name="formatter">Output sink.</param>
        public void Dump(Formatter formatter)
        {
            var mappedItems = program.GetItemsBySegment();
            Dump(mappedItems, formatter);
        }

        /// <summary>
        /// Dumps the segment items to the given formatter.
        /// </summary>
        /// <param name="segmentItems">Segment items to dump.</param>
        /// <param name="formatter">Output sink.</param>
        public void Dump(Dictionary<ImageSegment, List<ImageMapItem>> segmentItems, Formatter formatter)
        {
            foreach (var segmentEntry in segmentItems)
            {
                var seg = segmentEntry.Key;
                formatter.WriteLine(";;; Segment {0} ({1})", seg.Name, seg.Address);
                if (seg.Designer is not null)
                {
                    seg.Designer.Render(seg, program, new AsmCommentFormatter("; ", formatter));
                }
                else
                {
                    foreach (var item in segmentEntry.Value)
                    {
                        DumpItem(seg, item, formatter);
                    }
                }
            }
        }

        private void DumpItem(ImageSegment segment, ImageMapItem i, Formatter formatter)
        {
            if (i is ImageMapBlock block && block.Block is not null)
            {
                formatter.WriteLine();
                if (program.Procedures.TryGetValue(block.Address, out var proc))
                {
                    DumpProcedureHeaderComment(proc, formatter);

                    formatter.Write(proc.Name);
                    formatter.Write(" ");
                    formatter.Write("proc");
                    formatter.WriteLine();
                }
                else
                {
                    formatter.Write(block.Block.DisplayName);
                    formatter.Write(":");
                    formatter.WriteLine();
                }
                var arch = block.Block.Procedure.Architecture;
                if (program.SegmentMap.TryFindSegment(block.Address, out var seg))
                {
                    DumpAssembler(arch, seg.MemoryArea, block.Address, block.Size, formatter);
                }
                return;
            }

            if (i is ImageMapVectorTable table)
            {
                formatter.WriteLine(";; Code vector at {0} ({1} bytes)",
                    table.Address, table.Size);
                foreach (Address addr in table.Addresses)
                {
                    formatter.WriteLine("\t{0}", addr.ToString());
                }
                DumpData(program.SegmentMap, program.Architecture, i.Address, i.Size, formatter);
            }
            else
            {
                var segLast = segment.Address + segment.Size;
                var size = segLast - i.Address;
                size = Math.Min(i.Size, size);

                if (i.DataType is null || (i.DataType is UnknownType && i.DataType.Size == 0) ||
                    i.DataType is CodeType)
                {
                    if (segment.IsBss)
                        DumpBssData(program.Architecture, i.Address, size, formatter);
                    else
                        DumpData(program.SegmentMap, program.Architecture, i.Address, size, formatter);
                }
                else
                {
                    DumpTypedData(program.SegmentMap, program.Architecture, i, formatter);
                }
            }
        }

        private void DumpBssData(IProcessorArchitecture architecture, Address address, long size, Formatter formatter)
        {
            WriteLabel(address, formatter);
            formatter.WriteKeyword("db");
            formatter.Write("\t");
            formatter.Write("0 dup {0}", size);
            formatter.WriteLine();
        }

        private void DumpProcedureHeaderComment(Procedure proc, Formatter formatter)
        {
            formatter.WriteComment($";; {proc.Name}: {proc.EntryAddress}");
            formatter.WriteLine();
            var callerStms = program.CallGraph.FindCallerStatements(proc)
                .OrderBy(c => c.Address)
                .ToArray();
            if (callerStms.Length > 0)
            {
                formatter.WriteComment(";;   Called from:");
                formatter.WriteLine();
                foreach (var stm in callerStms)
                {
                    var addr = stm.Address;
                    formatter.WriteComment(";;     ");
                    formatter.WriteHyperlink(addr.ToString(), addr);
                    formatter.Write(" (in ");
                    formatter.WriteHyperlink(stm.Block.Procedure.Name, stm.Block.Procedure);
                    formatter.Write(")");
                    formatter.WriteLine();
                }
            }
        }

        /// <summary>
        /// Dumps the data at the given address in the given segment.
        /// </summary>
        /// <param name="map">Segment map of the program.</param>
        /// <param name="arch">Processor architecture to use.</param>
        /// <param name="address">Starting address.</param>
        /// <param name="cUnits">Number of storage units to dump.</param>
        /// <param name="formatter">Output sink.</param>
        public void DumpData(SegmentMap map, IProcessorArchitecture arch, Address address, int cUnits, Formatter formatter)
        {
            if (cUnits < 0)
                throw new ArgumentException("Must be a nonnegative number.", "cbBytes"); 
            DumpData(map, arch, address, (uint)cUnits, formatter);
        }

        /// <summary>
        /// Dumps the data at the given address in the given segment.
        /// </summary>
        /// <param name="map">Segment map of the program.</param>
        /// <param name="arch">Processor architecture to use.</param>
        /// <param name="range">A range of addresses to render.</param>
        /// <param name="formatter">Output sink.</param>
        public void DumpData(SegmentMap map, IProcessorArchitecture arch, AddressRange range, Formatter formatter)
        {
            DumpData(map, arch, range.Begin, range.End - range.Begin, formatter);
        }

        /// <summary>
        /// Dumps the data at the given address in the given segment.
        /// </summary>
        /// <param name="map">Segment map of the program.</param>
        /// <param name="arch">Processor architecture to use.</param>
        /// <param name="address">Starting address.</param>
        /// <param name="cUnits">Number of storage units to dump.</param>
        /// <param name="formatter">Output sink.</param>
        public void DumpData(SegmentMap map, IProcessorArchitecture arch, Address address, long cUnits, Formatter formatter)
        {
            if (!map.TryFindSegment(address, out var segment) || segment.MemoryArea is null)
                return;
            DumpData(arch, segment.MemoryArea, address, cUnits, formatter);
        }

        /// <summary>
        /// Dumps the data at the given address in the given segment.
        /// </summary>
        /// <param name="arch">Processor architecture to use.</param>
        /// <param name="mem">Memory area to render.</param>
        /// <param name="address">Starting address.</param>
        /// <param name="cUnits">Number of storage units to dump.</param>
        /// <param name="formatter">Output sink.</param>
        public void DumpData(
            IProcessorArchitecture arch,
            MemoryArea mem,
            Address address,
            long cUnits,
            Formatter formatter)
        {
            long offset = address - mem.BaseAddress;
            if (offset < 0 || cUnits <= 0)
                return;
			var rdr = arch.CreateImageReader(mem, address, cUnits);
            var memfmt = mem.Formatter;
            var output = new TextMemoryFormatterOutput(formatter);

            //try
            {
                memfmt.RenderMemory(rdr, program.TextEncoding, output);
            }
            //catch
            //{
            //    stm.WriteLine();
            //    stm.WriteLine(";;; ...end of image");
            //}
        }


        /// <summary>
        /// Dumps the assembler code at the given address in the given segment to 
        /// the formatter.
        /// </summary>
        /// <param name="arch">Processor architecture to use.</param>
        /// <param name="mem">Memory area containing the address.</param>
        /// <param name="addrStart">Address at which to start.</param>
        /// <param name="cUnits">Number of storage units to write.</param>
        /// <param name="formatter">Output sink.</param>
        public void DumpAssembler(
            IProcessorArchitecture arch,
            MemoryArea mem,
            Address addrStart,
            long cUnits,
            Formatter formatter)
        {
            var dasm = arch.CreateDisassembler(arch.CreateImageReader(mem, addrStart));
            try
            {
                var flags = MachineInstructionRendererFlags.ResolvePcRelativeAddress;
                if (this.RenderInstructionsCanonically)
                    flags |= MachineInstructionRendererFlags.RenderInstructionsCanonically;
                var writer = new FormatterInstructionWriter(formatter, program.Procedures, program.ImageSymbols, true);
                var options = new MachineInstructionRendererOptions(
                    flags: flags,
                    syntax: "");
                foreach (var instr in dasm)
                {
                    if (cUnits <= 0)
                        break;
                    if (!DumpAssemblerLine(mem, arch, instr, writer, options))
                        break;

                    cUnits -= instr.Length;
                }
            }
            catch (Exception ex)
            {
                formatter.WriteLine(ex.Message);
                formatter.WriteLine();
            }
        }

        /// <summary>
        /// Writes a single assembler line to the formatter.
        /// </summary>
        /// <param name="mem">Memory area containing the instruction.</param>
        /// <param name="arch">Processor architecture to use.</param>
        /// <param name="instr">Disassembled instruction to write.</param>
        /// <param name="writer">Output sink.</param>
        /// <param name="options">Rendering options.</param>
        /// <returns></returns>
        public bool DumpAssemblerLine(
            MemoryArea mem, 
            IProcessorArchitecture arch, 
            MachineInstruction instr, 
            FormatterInstructionWriter writer,
            MachineInstructionRendererOptions options)
        {
            var instrAddress = instr.Address;
            Address addrBegin = instrAddress;
            if (ShowAddresses)
                writer.WriteFormat("{0} ", addrBegin);
            if (ShowCodeBytes)
            {
                WriteOpcodes(mem, arch, instrAddress, instrAddress + instr.Length, writer);
                if (instr.Length * 3 < 16)
                {
                    writer.WriteString(new string(' ', 16 - (instr.Length * 3)));
                }
            }
            writer.WriteString("\t");
            instr.Render(writer, options);
            writer.WriteLine();
            return true;
        }

        private void DumpTypedData(SegmentMap map, IProcessorArchitecture arch, ImageMapItem item, Formatter w)
        {
            if (!map.TryFindSegment(item.Address, out var segment) || segment.MemoryArea is null)
                return;
            WriteLabel(item.Address, w);

            var rdr = arch.CreateImageReader(segment.MemoryArea, item.Address);
            item.DataType.Accept(new TypedDataDumper(rdr, item.Size, w));
        }

        private void WriteLabel(Address addr, Formatter w)
        {
            if (program.ImageSymbols.TryGetValue(addr, out var sym) &&
                !string.IsNullOrEmpty(sym.Name))
            {
                w.Write(sym.Name!);
                w.Write("\t\t; {0}",addr);

                w.WriteLine();
            }
            else
            {
                var label = program.NamingPolicy.BlockName(addr);
                w.Write(label);
            }
            w.Write("\t");
        }

        private void WriteOpcodes(MemoryArea image, IProcessorArchitecture arch, Address begin, Address addrEnd, FormatterInstructionWriter writer)
		{
			EndianImageReader rdr = arch.CreateImageReader(image, begin);
            var byteSize = (7 + arch.InstructionBitSize) / 8;
            string instrByteFormat = $"{{0:X{byteSize * 2}}} "; // each byte is two nybbles.
            var instrByteSize = PrimitiveType.CreateWord(arch.InstructionBitSize);

            while (rdr.Address < addrEnd && rdr.TryRead(instrByteSize, out var v))
            {
                writer.WriteFormat(instrByteFormat, v.ToUInt64());
			}
		}

        /// <summary>
        /// This <see cref="Formatter"/> renders comments appropriately for assembly-language output
        /// </summary>
        private class AsmCommentFormatter : Formatter
        {
            private readonly string asmCommentPrefix;
            private readonly Formatter w;
            private bool needPrefix;

            public AsmCommentFormatter(string asmCommentPrefix, Formatter w)
            {
                this.asmCommentPrefix = asmCommentPrefix;
                this.w = w;
                this.needPrefix = true;
            }

            public override void Begin(object? tag)
            {
            }

            public override void Terminate()
            {
                WritePrefix();
                w.Terminate();
            }

            public override Formatter Write(char ch)
            {
                WritePrefix();
                return w.Write(ch);
            }

            public override void Write(string s)
            {
                WritePrefix();
                w.Write(s);
            }

            public override void Write(string format, params object[] arguments)
            {
                WritePrefix();
                w.Write(format, arguments);
            }

            public override void WriteComment(string comment)
            {
                WritePrefix();
                w.WriteComment(comment);
            }

            public override void WriteHyperlink(string text, object href)
            {
                WritePrefix();
                w.WriteHyperlink(text, href);
            }

            public override void WriteLabel(string label, object block)
            {
                WritePrefix();
                w.Write(label);
            }

            public override void WriteKeyword(string keyword)
            {
                WritePrefix();
                w.WriteKeyword(keyword);
            }

            public override void WriteLine()
            {
                WritePrefix();
                w.WriteLine();
                needPrefix = true;
            }

            public override void WriteLine(string s)
            {
                WritePrefix();
                w.WriteLine(s);
                needPrefix = true;
            }

            public override void WriteLine(string format, params object[] arguments)
            {
                WritePrefix();
                w.WriteLine(format, arguments);
                needPrefix = true;
            }

            public override void WriteType(string typeName, DataType dt)
            {
                WritePrefix();
                w.WriteType(typeName, dt);
            }

            private void WritePrefix()
            {
                if (needPrefix)
                {
                    w.Write(asmCommentPrefix);
                    needPrefix = false;
                }
            }
        }
    }
}
