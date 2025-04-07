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
using Reko.Core.Output;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Reko.Core
{
    /// <summary>
    /// Represents a basic block of <see cref="Statement"/>s.
    /// </summary>
    public class Block : IAddressable
	{
        /// <summary>
        /// Creates an instance of the <see cref="Block"/> class.
        /// </summary>
        /// <param name="proc">The <see cref="Procedure"/> to which this block belongs.</param>
        /// <param name="addr">The address at which the basic block is located.</param>
        /// <param name="id">A reasonably unique identifier.</param>
		public Block(Procedure proc, Address addr, string id)
		{
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Blocks must have a valid id.", nameof(id));
			this.Procedure = proc;
            this.Address = addr;
			this.Id = id;
			this.Statements = new StatementList(this);
            // The great majority of blocks have at most two predecessors / successors.
            this.Pred = new List<Block>(2);
            this.Succ = new List<Block>(2);
		}

        /// <summary>
        /// The starting address of the block. Blocks are _not_ guaranteed 
        /// to have a starting address. 
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// Unique identifier for the block. This identifier should be global across
        /// the whole <see cref="Program" />.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Optional user-supplied label. When rendering blocks, it takes 
        /// precedence over the <see cref="Id"/> property, but should only 
        /// be used for rendering purposes. It must not conflict with any
        /// other UserLabel or Id of the blocks in the <see cref="Procedure" />.
        /// </summary>
        public string? UserLabel { get; set; }

        /// <summary>
        /// This string is used when rendering the block.
        /// </summary>
        public string DisplayName => UserLabel ?? Id;

        /// <summary>
        /// The <see cref="Procedure"/> this block belongs to.
        /// </summary>
        public Procedure Procedure { get; set; }

        /// <summary>
        /// If true, this block is synthesized and not present in the original binary.
        /// </summary>
        public bool IsSynthesized { get; set; }


        /// <summary>
        /// Convenience property to access the first successor of this block.
        /// </summary>
        public Block ElseBlock
        {
            get { return Succ[0]; }
            set { Succ[0] = value; }
        }

        /// <summary>
        /// Convenience property to access the second successor of this block.
        /// </summary>
        public Block ThenBlock
        {
            get { return Succ[1]; }
            set { Succ[1] = value; }
        }

        /// <summary>
        /// The <see cref="Block"/>s that are predecessors of this basic block.
        /// </summary>
        public List<Block> Pred { get; }

        /// <summary>
        /// The <see cref="Block"/>s that are successors of this basic block.
        /// </summary>
        public List<Block> Succ { get; private set; }

        /// <summary>
        /// The <see cref="Statement"/>s contained in this basic block.
        /// </summary>
        public StatementList Statements { get; }

        /// <summary>
        /// Coalesces two basic blocks into one. The statements in the <paramref name="next"/>
        /// basic block are moved into the <paramref name="block"/> basic block, and the edges
        /// between the two blocks are removed.
        /// </summary>
        /// <param name="block">Basic block that will absorb the other basic block's statements.</param>
        /// <param name="next">Basic block that will be absorbed.</param>
		public static void Coalesce(Block block, Block next)
		{
			foreach (Statement stm in next.Statements.ToArray())
			{
				block.Statements.Add(stm);
			}

			block.Succ = new List<Block>(next.Succ);
			ReplaceJumpsFrom(next, block);
			next.Pred.Clear();
			next.Statements.Clear();
			next.Succ.Clear();
		}

        /// <summary>
        /// Replace all jumps to <paramref name="block"/> with jumps to <paramref name="next"/>.
        /// </summary>
        /// <param name="block">The basic block to be replaced.</param>
        /// <param name="next">The basic block to be replaced with.</param>
        /// <returns>True if any replacements were made.</returns>
        public static bool ReplaceJumpsFrom(Block block, Block next)
		{
			bool change = false;
			foreach (Block s in block.Succ)
			{
				for (int i = 0; i < s.Pred.Count; ++i)
				{
					if (s.Pred[i] == block)
					{
						s.Pred[i] = next;
						change = true;
					}
				}
			}
			return change;
		}

		/// <summary>
		/// Replaces all edges incoming to <paramref name="block"/> with edges to <paramref name="next"/>.
		/// </summary>
		/// <param name="block"></param>
		/// <param name="next"></param>
		/// <returns>Whether a replacement was actually made or not.</returns>
		public static bool ReplaceJumpsTo(Block block, Block next)
		{
			bool change = false;
			foreach (Block p in block.Pred.ToArray())
			{
				for (int i = 0; i < p.Succ.Count; ++i)
				{
					if (p.Succ[i] == block)
					{
						p.Succ[i] = next;
						change = true;
					}
				}
                for (int ip = 0; ip < next.Pred.Count; ++ip)
                {
                    if (next.Pred[ip] == block)
                    {
                        next.Pred[ip] = p;
                        change = true;
                        break; // replace only one.
                    }
                }
            }
            block.Pred.Clear();
			return change;
		}

        /// <inheritdoc/>
        public override string ToString()
        {
            return DisplayName;
        }

        /// <summary>
        /// Writes the basic block to the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">Output <see cref="TextWriter"/>.</param>
		public void Write(TextWriter writer)
		{
			writer.WriteLine("{0}:", DisplayName);
			WriteStatements(writer);
		}

        /// <summary>
        /// Writes the statements of this basic block to the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">Output <see cref="TextWriter"/>.</param>
		public void WriteStatements(TextWriter writer)
		{
            var f = new TextFormatter(writer)
            {
                UseTabs = true,
                TabSize = 4
            };
            var cf = new CodeFormatter(f);
			int i = 0;
			foreach (Statement s in Statements)
			{
                f.Begin(s.Address);
				s.Instruction.Accept(cf);
				++i;
			}
		}

        /// <summary>
        /// Debugging helper function that writes the contents of this block to the debugger output.
        /// </summary>
        public void Dump()
        {
            var sb = new StringWriter();
            Write(sb);
            Debug.Print("{0}", sb);
        }
    }
}
