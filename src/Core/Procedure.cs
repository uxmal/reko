/* 
 * Copyright (C) 1999-2008 John Källén.
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

 using Decompiler.Core.Absyn;
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using Decompiler.Core.Serialization;
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Decompiler.Core
{
	/// <summary>
	/// Represents a procedure that has been decompiled from code.
	/// </summary>
	public class Procedure : ProcedureBase
	{
		private AbsynStatementList body = new AbsynStatementList();
		private BlockList rpoBlocks;
		private Block blockEntry;
		private Block blockExit;
		private Frame frame;
		private ProcedureSignature signature;

		public Procedure(string name, Frame frame) : base(name)
		{
			Init(frame);
		}
		
		private void Init(Frame frame)
		{
			this.rpoBlocks = new BlockList();
			this.frame = frame;
			this.signature = new ProcedureSignature();
			this.blockEntry = new Block(this, Name + "_entry");		// Entry block
			this.blockExit = new Block(this, Name + "_exit");		// Exit block.
		}

		public AbsynStatementList Body
		{
			get { return body; }
		}

		/// <summary>
		/// Creates a procedure with the specified name; if no name is specified (null string)
		/// the address is used instead.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="addr"></param>
		/// <param name="f"></param>
		/// <returns></returns>
		public static Procedure Create(string name, Address addr, Frame f)
		{
			if (name == null)
			{
				name = addr.GenerateName("fn", "");
			}
			return new Procedure(name, f);
		}

		public static Procedure Create(Address addr, Frame f)
		{
			return new Procedure(addr.GenerateName("fn", ""), f);
		}

		public BitSet CreateBlocksBitset()
		{
			return new BitSet(RpoBlocks.Count);
		}


#if DEBUG
		public void Dump(bool dump, bool emitFrame)
		{
			if (!dump)
				return;
			
			StringWriter sb = new StringWriter();
			Write(emitFrame, sb);
			System.Diagnostics.Debug.WriteLine(sb.ToString());
		}
#else
		public void Dump(bool dump, bool emitFrame)
		{
		}
#endif

		public Block EntryBlock
		{
			get { return blockEntry; }
			set { blockEntry = value; }
		}

		public Block ExitBlock
		{
			get { return blockExit; }
		}

		public void Write(bool emitFrame, TextWriter tw)
		{
			tw.WriteLine("// {0}", Name);
			if (emitFrame)
				frame.Write(tw);
			Signature.Emit(Name, ProcedureSignature.EmitFlags.None, tw);
			tw.WriteLine();
			foreach (Block block in RpoBlocks)
			{
				if (block != null) block.Write(tw);
			}
		}

		public Frame Frame
		{
			get { return frame; }
		}
		
		public void RenumberBlocks()
		{
			BlockRenumbering br = new BlockRenumbering();
			rpoBlocks = br.Renumber(blockEntry);
		}

		public BlockList RpoBlocks
		{
			get { return rpoBlocks; }
		}

		/// <summary>
		/// The effects of this procedure on registers, stack, and FPU stack.
		/// </summary>
		public override ProcedureSignature Signature
		{
			get { return signature; }
			set { signature = value; }
		}

		public override string ToString()
		{
			return Name;
		}


		private class BlockRenumbering
		{
			private BlockList rpoBlocks;
			private int rpo;

			private void DFS(Block block, Hashtable visited)
			{
				visited[block] = block;
				foreach (Block s in block.Succ)
				{
					if (!visited.Contains(s) && !IsEmpty(s))
					{
						DFS(s, visited);
					}
				}
			}

			public bool IsEmpty(Block b)
			{
				if (b.Statements.Count > 0)
					return false;
				if (b.Pred.Count > 0)
					return false;
				return b.Succ.Count == 0;
			}

			public BlockList Renumber(Block block)
			{
				Hashtable visited = new Hashtable();
				DFS(block, visited);
				rpo = visited.Count;

				rpoBlocks = new BlockList(rpo);
				RenumberBlock(block, new Hashtable());

				return rpoBlocks;
			}

			private void RenumberBlock(Block block, Hashtable visited)
			{
				visited[block] = block;
				for (int i = 0; i < block.Succ.Count; ++i)
				{
					Block s = block.Succ[i];
					if (!visited.ContainsKey(s))
					{
						RenumberBlock(s, visited);
					}
				}
				block.RpoNumber = --rpo;
				rpoBlocks[rpo] = block;
			}
		}
	}
}
