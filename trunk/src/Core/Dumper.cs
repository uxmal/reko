/* 
 * Copyright (C) 1999-2009 John Källén.
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

using System;
using System.IO;
using System.Text;

namespace Decompiler.Core
{
	/// <summary>
	/// Dumps low-level information about a binary.
	/// </summary>
	public class Dumper
	{
		private bool showAddress;
		private bool showBytes;

		public Dumper()
		{
		}

		public void Dump(Program program, ImageMap map, TextWriter stm)
		{
			if (map == null)
			{
				DumpAssembler(program.Image, program.Image.BaseAddress, program.Image.BaseAddress + program.Image.Bytes.Length, stm);
			}
			else
			{
				foreach (ImageMapItem i in map.Items.Values)
				{
					//				Address addrLast = i.Address + i.Size;
					ImageMapBlock block = i as ImageMapBlock;
					if (block != null)
					{
                        stm.WriteLine();
                        if (program.Procedures.ContainsKey(block.Address))
						{
							stm.WriteLine(block.Address.GenerateName("fn","()"));
						}
						else																			 
						{
							stm.WriteLine(block.Address.GenerateName("l",":"));
						}
						DumpAssembler(program.Image, block.Address, block.Address + block.Size, stm);
						continue;
					}

					ImageMapVectorTable table = i as ImageMapVectorTable;
					if (table != null)
					{
						stm.WriteLine("{0} table at {1} ({2} bytes)",
							table.IsCallTable?"Call":"Jump",
							table.Address, table.Size);
						foreach (Address addr in table.Addresses)
						{
							stm.WriteLine("\t{0}", addr != null ? addr.ToString() : "-- null --");
						}
						DumpData(program.Image, i.Address, i.Size, stm);
					}
					else
					{
						DumpData(program.Image, i.Address, i.Size, stm);
					}							   
				}
			}
		}

		public void DumpData(ProgramImage image, Address address, int cbBytes, TextWriter stm)
		{
			int cSkip = address.Linear & 0x0F;
			ImageReader rdr = image.CreateReader(address);
			while (cbBytes > 0)
			{
				StringBuilder sb = new StringBuilder(0x12);
				try 
				{
					stm.Write("{0} ", rdr.Address);
					for (int i = 0; i < 16; ++i)
					{
						if (cbBytes > 0 && cSkip <= 0)
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

		public virtual void DumpAssembler(ProgramImage image, Address addrStart, Address addrEnd, TextWriter writer)
		{
		}

		public virtual void DumpAssemblerLine(ProgramImage image, Disassembler dis, bool showAddress, bool showCodeBytes, TextWriter writer)
		{
		}


		public bool ShowAddresses
		{
			get { return showAddress; }
			set { showAddress = value; }
		}

		public bool ShowCodeBytes
		{
			get { return showBytes; }
			set { showBytes = value; }
		}

		public void WriteByteRange(ProgramImage image, Address begin, Address addrEnd, TextWriter writer)
		{
			ImageReader rdr = image.CreateReader(begin);
			while (rdr.Address < addrEnd)
			{
				writer.Write("{0:X2} ", rdr.ReadByte());
			}
		}
	}
}
