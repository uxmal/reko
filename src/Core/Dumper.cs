#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using Reko.Core.Machine;
using Reko.Core.Output;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Reko.Core
{
	/// <summary>
	/// Dumps low-level information about a binary.
	/// </summary>
	public class Dumper
	{
        private IProcessorArchitecture arch;

		public Dumper(IProcessorArchitecture arch)
		{
            this.arch = arch;
		}

        public bool ShowAddresses { get; set; }
        public bool ShowCodeBytes { get; set; }

        public void Dump(Program program, Formatter formatter)
        {
            var map = program.SegmentMap;
            ImageSegment segment = null;
            foreach (ImageMapItem i in program.ImageMap.Items.Values)
            {
                if (!map.IsValidAddress(i.Address))
                    continue;
                ImageSegment seg;
                if (!map.TryFindSegment(i.Address, out seg))
                    continue;
                if (seg != segment)
                {
                    segment = seg;
                    formatter.WriteLine(";;; Segment {0} ({1})", seg.Name, seg.Address);
                }

                ImageMapBlock block = i as ImageMapBlock;
                if (block != null)
                {
                    formatter.WriteLine();
                    Procedure proc;
                    if (program.Procedures.TryGetValue(block.Address, out proc))
                    {
                        formatter.WriteComment(string.Format(
                            ";; {0}: {1}", proc.Name, block.Address));
                        formatter.WriteLine();

                        formatter.Write(proc.Name);
                        formatter.Write(" ");
                        formatter.Write("proc");
                        formatter.WriteLine();
                    }
                    else
                    {
                        formatter.Write(block.Block.Name);
                        formatter.Write(":");
                        formatter.WriteLine();
                    }
                    DumpAssembler(program.SegmentMap, block.Address, block.Address + block.Size, formatter);
                    continue;
                }

                ImageMapVectorTable table = i as ImageMapVectorTable;
                if (table != null)
                {
                    formatter.WriteLine(";; Code vector at {0} ({1} bytes)",
                        table.Address, table.Size);
                    foreach (Address addr in table.Addresses)
                    {
                        formatter.WriteLine("\t{0}", addr != null ? addr.ToString() : "-- null --");
                    }
                    DumpData(program.SegmentMap, i.Address, i.Size, formatter);
                }
                else
                {
                    var segLast = segment.Address + segment.Size;
                    var size = segLast - i.Address;
                    size = Math.Min(i.Size, size);
                    if (i.DataType == null || i.DataType is UnknownType ||
                        i.DataType is CodeType)
                    {
                        DumpData(program.SegmentMap, i.Address, size, formatter);
                    }
                    else
                    {
                        DumpTypedData(program.SegmentMap, i, formatter);
                    }
                }
            }
        }

        public void DumpData(SegmentMap map, Address address, int cbBytes, Formatter stm)
        {
            if (cbBytes < 0)
                throw new ArgumentException("Must be a nonnegative number.", "cbBytes"); 
            DumpData(map, address, (uint)cbBytes, stm);
        }

        public void DumpData(SegmentMap map, AddressRange range, Formatter stm)
        {
            DumpData(map, range.Begin, (long) (range.End - range.Begin), stm);
        }

		public void DumpData(SegmentMap map, Address address, long cbBytes, Formatter stm)
		{
			ulong cSkip = address.ToLinear() & 0x0F;
            ImageSegment segment;
            if (!map.TryFindSegment(address, out segment) || segment.MemoryArea == null)
                return;
			ImageReader rdr = arch.CreateImageReader(segment.MemoryArea, address);
			while (cbBytes > 0)
			{
				StringBuilder sb = new StringBuilder(0x12);
				try 
				{
					stm.Write("{0} ", rdr.Address);
					for (int i = 0; i < 16; ++i)
					{
						if (cbBytes > 0 && cSkip == 0)
						{
							byte b = rdr.ReadByte();
							stm.Write("{0:X2} ", b);
							sb.Append(0x20 <= b && b < 0x7F
								? (char) b
								: '.');
							--cbBytes;
						}
						else
						{
							stm.Write("   ");
							if (cSkip > 0)
								sb.Append(' ');
							--cSkip;
						}
					}
				} 
				catch
				{
					stm.WriteLine();
					stm.WriteLine(";;; ...end of image");
					return;
				}
				stm.WriteLine(sb.ToString());
			}
		}

        public void DumpAssembler(SegmentMap map, Address addrStart, Address addrLast, Formatter formatter)
        {
            ImageSegment segment;
            if (!map.TryFindSegment(addrStart, out segment))
                return;
            var dasm = arch.CreateDisassembler(arch.CreateImageReader(segment.MemoryArea, addrStart));
            try
            {
                var writer = new InstrWriter(formatter);
                foreach (var instr in dasm)
                {
                    if (instr.Address >= addrLast)
                        break;
                    if (!DumpAssemblerLine(segment.MemoryArea, instr, writer))
                        break;
                }
            }
            catch (Exception ex)
            {
                formatter.WriteLine(ex.Message);
                formatter.WriteLine();
            }
        }

        public bool DumpAssemblerLine(MemoryArea mem, MachineInstruction instr, InstrWriter writer)
        {
            Address addrBegin = instr.Address;
            if (ShowAddresses)
                writer.Write("{0} ", addrBegin);
            if (ShowCodeBytes)
            {
                StringWriter sw = new StringWriter();
                WriteByteRange(mem, instr.Address, instr.Address + instr.Length, writer);
                if (instr.Length * 3 < 16)
                {
                    writer.Write(new string(' ', 16 - (instr.Length * 3)));
                }
            }
            writer.Write("\t");
            instr.Render(writer);
            writer.WriteLine();
            return true;
        }

        private void DumpTypedData(SegmentMap map, ImageMapItem item, Formatter stm)
        {
            ImageSegment segment;
            if (!map.TryFindSegment(item.Address, out segment) || segment.MemoryArea == null)
                return;
            stm.Write(Block.GenerateName(item.Address));
            stm.Write("\t");

            ImageReader rdr = arch.CreateImageReader(segment.MemoryArea, item.Address);
            item.DataType.Accept(new TypedDataDumper(rdr, item.Size, stm));
        }

        public void WriteByteRange(MemoryArea image, Address begin, Address addrEnd, InstrWriter writer)
		{
			ImageReader rdr = arch.CreateImageReader(image, begin);
			while (rdr.Address < addrEnd)
			{
				writer.Write(string.Format("{0:X2} ", rdr.ReadByte()));
			}
		}

        public class InstrWriter : MachineInstructionWriter
        {
            private Formatter formatter;

            public InstrWriter(Formatter formatter)
            {
                this.formatter = formatter;
            }

            public IPlatform Platform { get; private set; }

            public void Tab()
            {
                formatter.Write("\t");
            }

            public void Write(string s)
            {
                formatter.Write(s);
            }

            public void Write(uint n)
            {
                formatter.Write(n.ToString());
            }

            public void Write(char c)
            {
                formatter.Write(c);
            }

            public void Write(string fmt, params object[] parms)
            {
                formatter.Write(fmt, parms);
            }

            public void WriteAddress(string formattedAddress, Address addr)
            {
                formatter.WriteHyperlink(formattedAddress, addr);
            }

            public void WriteOpcode(string opcode)
            {
                formatter.Write(opcode);
            }

            public void WriteLine()
            {
                formatter.WriteLine();
            }
        }
    }
}
