/* 
 * Copyright (C) 1999-2010 John Källén.
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
using BitSet = Decompiler.Core.Lib.BitSet;
using Block = Decompiler.Core.Block;
using Identifier = Decompiler.Core.Code.Identifier;
using IProcessorArchitecture = Decompiler.Core.IProcessorArchitecture;
using Storage = Decompiler.Core.Storage;
using TextWriter = System.IO.TextWriter;
using StringWriter = System.IO.StringWriter;
using System.Collections.Generic;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Dataflow summary information about the dataflow of a basic block.
	/// </summary>
	public class BlockFlow : DataFlow
	{
		public Block Block;
		public int iStm;
		public BitSet DataOut;						    // each bit corresponds to a register that is live at the end of the
		public uint grfOut;							    // each bit corresponds to a condition code register that is live at the end of the block
		public Dictionary<Storage,int> StackVarsOut;    // stack-based storages that are live at the end of the block.
		public uint grfTrashedIn;					    // each bit corresnpots to a condition code register that is trashed on entrance.
		public Dictionary<Storage, Storage> TrashedIn;	// maps which identifiers are trashed on entrance to the block.

		public BlockFlow(Block block, BitSet dataOut)
		{
			this.Block = block;
			this.DataOut = dataOut;
			this.StackVarsOut = new Dictionary<Storage,int>();
			this.TrashedIn = new Dictionary<Storage, Storage>();
		}

		public override void Emit(IProcessorArchitecture arch, TextWriter writer)
		{
			EmitRegisters(arch, "// DataOut:", DataOut, writer);
			EmitFlagGroup(arch, "// DataOut (flags):", grfOut, writer);
			if (StackVarsOut.Count > 0)
			{
				writer.WriteLine();
				writer.Write("// LocalsOut:");
				SortedList<string,string> list = new SortedList<string,string>();
				foreach (KeyValuePair<Storage,int> de in StackVarsOut)
				{
					Storage id = (Storage) de.Key;
					StringWriter sb = new StringWriter();
					id.Write(sb);
					string sName = sb.ToString();

					list[sName] = string.Format("{0}({1})", sName, de.Value);
				}
				foreach (string s in list.Values)
				{
					writer.Write(" ");
					writer.Write(s);
				}
				writer.WriteLine();
			}
		}
	}
}
