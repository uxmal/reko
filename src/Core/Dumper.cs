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

        public void Dump(Program program, TextWriter stm)
        {
            var map = program.ImageMap;
            ImageSegment segment = null;
            foreach (ImageMapItem i in map.Items.Values)
            {
                if (!program.ImageMap.IsValidAddress(i.Address))
                    continue;
                ImageSegment seg;
                if (!map.TryFindSegment(i.Address, out seg))
                    continue;
                if (seg != segment)
                {
                    segment = seg;
                    stm.WriteLine(";;; Segment {0} ({1})", seg.Name, seg.Address);
                }

                // Address addrLast = i.Address + i.Size;
                ImageMapBlock block = i as ImageMapBlock;
                if (block != null)
                {
                    stm.WriteLine();
                    if (program.Procedures.ContainsKey(block.Address))
                    {
                        stm.WriteLine(block.Address.GenerateName("fn", "()"));
                    }
                    else
                    {
                        stm.WriteLine(block.Address.GenerateName("l", ":"));
                    }
                    DumpAssembler(program.ImageMap, block.Address, block.Address + block.Size, stm);
                    continue;
                }

                ImageMapVectorTable table = i as ImageMapVectorTable;
                if (table != null)
                {
                    stm.WriteLine("Code vector at {0} ({1} bytes)",
                        table.Address, table.Size);
                    foreach (Address addr in table.Addresses)
                    {
                        stm.WriteLine("\t{0}", addr != null ? addr.ToString() : "-- null --");
                    }
                    DumpData(program.ImageMap, i.Address, i.Size, stm);
                }
                else
                {
                    var segLast = segment.Address + segment.ContentSize;
                    var size = segLast - i.Address;
                    size = Math.Min(i.Size, size);
                    DumpData(program.ImageMap, i.Address, size, stm);
                }
            }
        }

        public void DumpData(ImageMap map, Address address, int cbBytes, TextWriter stm)
        {
            if (cbBytes < 0)
                throw new ArgumentException("Must be a nonnegative number.", "cbBytes"); 
            DumpData(map, address, (uint)cbBytes, stm);
        }

        public void DumpData(ImageMap map, AddressRange range, TextWriter stm)
        {
            DumpData(map, range.Begin, (long) (range.End - range.Begin), stm);
        }

		public void DumpData(ImageMap map, Address address, long cbBytes, TextWriter stm)
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
					stm.WriteLine("...end of image");
					return;
				}
				stm.WriteLine(sb.ToString());
			}
		}

        public void DumpAssembler(ImageMap map, Address addrStart, Address addrLast, TextWriter writer)
        {
            ImageSegment segment;
            if (!map.TryFindSegment(addrStart, out segment))
                return;
            var dasm = arch.CreateDisassembler(arch.CreateImageReader(segment.MemoryArea, addrStart));
            try
            {
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
                writer.WriteLine(ex.Message);
                writer.WriteLine();
            }
        }

        public bool DumpAssemblerLine(MemoryArea mem, MachineInstruction instr, TextWriter writer)
        {
            Address addrBegin = instr.Address;
            if (ShowAddresses)
                writer.Write("{0} ", addrBegin);
            if (ShowCodeBytes)
            {
                StringWriter sw = new StringWriter();
                WriteByteRange(mem, instr.Address, instr.Address + instr.Length, sw);
                writer.WriteLine("{0,-16}\t{1}", sw.ToString(), instr);
            }
            else
            {
                writer.WriteLine("\t{0}", instr.ToString());
            }
            return true;
        }

		public void WriteByteRange(MemoryArea image, Address begin, Address addrEnd, TextWriter writer)
		{
			ImageReader rdr = arch.CreateImageReader(image, begin);
			while (rdr.Address < addrEnd)
			{
				writer.Write("{0:X2} ", rdr.ReadByte());
			}
		}
	}
}
