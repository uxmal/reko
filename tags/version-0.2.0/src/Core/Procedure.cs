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

using Decompiler.Core.Absyn;
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using Decompiler.Core.Output;
using Decompiler.Core.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Core
{
	/// <summary>
	/// Represents a procedure that has been decompiled from code.
	/// </summary>
	public class Procedure : ProcedureBase
	{
		private List<AbsynStatement> body;
		private List<Block> rpoBlocks;
        private DirectedGraphImpl<Block> controlGraph;
		private Block blockEntry;
		private Block blockExit;
		private Frame frame;
		private ProcedureSignature signature;
        private bool userSpecified;

		public Procedure(string name, Frame frame) : base(name)
		{
            this.body = new List<AbsynStatement>();
			this.rpoBlocks = new List<Block>();
            this.controlGraph = new DirectedGraphImpl<Block>();
			this.frame = frame;
			this.signature = new ProcedureSignature();
			this.blockEntry = AddBlock(Name + "_entry");	
			this.blockExit = AddBlock(Name + "_exit");
		}

		public List<AbsynStatement> Body
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

        public BlockDominatorGraph CreateBlockDominatorGraph()
        {
            return new BlockDominatorGraph(new BlockGraph(RpoBlocks), EntryBlock);
        }

		public Block EntryBlock
		{
			get { return blockEntry; }
		}

		public Block ExitBlock
		{
			get { return blockExit; }
		}

		public void Write(bool emitFrame, TextWriter writer)
		{
			writer.WriteLine("// {0}", Name);
			if (emitFrame)
				frame.Write(writer);
            Signature.Emit(Name, ProcedureSignature.EmitFlags.None, new Formatter(writer));
			writer.WriteLine();
			foreach (Block block in RpoBlocks)
			{
				if (block != null) block.Write(writer);
			}
		}

		public Frame Frame
		{
			get { return frame; }
		}
		
        //$REVIEW: this causes a lot of bugs. It's needed to generate dominator trees, but not much used elsewhere.
        // Consider moving numbering into dominator graph generation.
		public void RenumberBlocks()
		{
			BlockRenumbering br = new BlockRenumbering();
			rpoBlocks = br.Renumber(blockEntry);
		}

        /// <summary>
        /// Returns a list of the blocks of this procedure in reverse post order.
        /// </summary>
		public List<Block> RpoBlocks
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

        /// <summary>
        /// True if the user specified this procedure by adding it to the project
        /// file or by marking it in the user interface.
        /// </summary>
        public bool UserSpecified
        {
            get { return userSpecified; }
            set { userSpecified = value; }
        }

        /// <summary>
        /// Auxiliary class used to renumber the blocks in RPO (reverse post order).
        /// </summary>
		private class BlockRenumbering
		{
			private List<Block> rpoBlocks;
			private int rpo;

			private void DFS(Block block, Dictionary<Block,Block> visited)
			{
				visited[block] = block;
				foreach (Block s in block.Succ)
				{
					if (!visited.ContainsKey(s)) // && !IsEmpty(s))
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

			public List<Block> Renumber(Block block)
			{
                Dictionary<Block, Block> visited = new Dictionary<Block, Block>();
				DFS(block, visited);
				rpo = visited.Count;

				rpoBlocks = new List<Block>(rpo);
                for (int i = 0; i < rpo; ++i)
                {
                    rpoBlocks.Add(null);
                }
                RenumberBlock(block, new Dictionary<Block, Block>());

				return rpoBlocks;
			}

            private void RenumberBlock(Block block, Dictionary<Block, Block> visited)
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

        public Block AddBlock(string name)
        {
            Block block = new Block(this, name);
            controlGraph.AddNode(block);
            return block;
        }

        [Obsolete("Change references to ControlGraph")]
        public void AddEdge(Block block, Block blockTo)
        {
            block.Succ.Add(blockTo);
            blockTo.Pred.Add(block);
            controlGraph.AddEdge(block, blockTo);
        }

        [Obsolete("Change references to ControlGraph")]
        public void RemoveEdge(Block from, Block to)
        {
            if (from.Succ.Contains(to) && to.Pred.Contains(from))
            {
                from.Succ.Remove(to);
                to.Pred.Remove(from);
            }
            controlGraph.RemoveEdge(from, to);
        }
    }
}
