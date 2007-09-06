/* 
 * Copyright (C) 1999-2007 John Källén.
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

using Decompiler.Core;
using System;
using System.Text;
using System.IO;

namespace Decompiler.Arch.Intel
{
	public class IntelDumper : Dumper
	{
		private IntelArchitecture arch;

		public IntelDumper(IntelArchitecture arch) 
		{
			this.arch = arch;
		}

		private string ByteString(ProgramImage image, Address begin, Address addrEnd)
		{
			ImageReader rdr = image.CreateReader(begin);
			StringBuilder sb = new StringBuilder();
			while (rdr.Address < addrEnd)
			{
				sb.AppendFormat("{0:X2} ", rdr.ReadByte());
			}
			return sb.ToString();
		}

		public override void DumpAssembler(ProgramImage image, Address addrStart, Address addrLast, TextWriter writer)
		{
			IntelDisassembler dasm = new IntelDisassembler(image.CreateReader(addrStart), arch.WordWidth);
			while (dasm.Address < addrLast)
			{
				Address addrBegin = dasm.Address;
				if (ShowAddresses)
					writer.Write("{0} ", addrBegin);
				IntelInstruction instr = dasm.Disassemble();
				if (ShowCodeBytes)
				{
					writer.WriteLine("{0,-16}\t{1}", ByteString(image, addrBegin, dasm.Address), instr);
				}
				else
				{
					writer.WriteLine("\t{0}", instr.ToString());
				}
			}
		}
	}
}
