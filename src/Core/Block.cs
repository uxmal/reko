#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core.Output;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Reko.Core
{
	/// <summary>
	/// Represents a basic block of statements.
	/// </summary>
	public class Block
	{
		public Block(Procedure proc, string name)
		{
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Blocks must have a valid name.", nameof(name));
			this.Procedure = proc;
			this.Name = name;
			this.Statements = new StatementList(this);
            // The great majority of blocks have at most two predecessors / successors.
            this.Pred = new List<Block>(2);
            this.Succ = new List<Block>(2);
		}

        /// <summary>
        /// The starting address of the Block. Blocks are _not_ guaranteed 
        /// to have a starting address. 
        /// </summary>
        public Address Address { get; set; }
        public string Name { get; }
        public Procedure Procedure { get; set; }

        /// <summary>
        /// If true, this block is synthesized and not present in the original binary.
        /// </summary>
        public bool IsSynthesized { get; set; }
       
        public Block ElseBlock
        {
            get { return Succ[0]; }
            set { Succ[0] = value; }
        }

        public Block ThenBlock
        {
            get { return Succ[1]; }
            set { Succ[1] = value; }
        }

        public List<Block> Pred { get; private set; }
        public List<Block> Succ { get; private set; }
        public StatementList Statements { get; private set; }
        
		public static void Coalesce(Block block, Block next)
		{
			foreach (Statement stm in next.Statements)
			{
				block.Statements.Add(stm);
			}

			block.Succ = new List<Block>(next.Succ);
			ReplaceJumpsFrom(next, block);
			next.Pred.Clear();
			next.Statements.Clear();
			next.Succ.Clear();
		}


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
			foreach (Block p in block.Pred)
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

        public override string ToString()
        {
            return Name;
        }

		public void Write(TextWriter sb)
		{
			sb.WriteLine("{0}:", Name);
			WriteStatements(sb);
		}

		public void WriteStatements(TextWriter writer)
		{
            var f = new TextFormatter(writer)
            {
                UseTabs = true,
                TabSize = 4
            };
            CodeFormatter cf = new CodeFormatter(f);
			int i = 0;
			foreach (Statement s in Statements)
			{
				s.Instruction.Accept(cf);
				++i;
			}
		}

        public void Dump()
        {
            var sb = new StringWriter();
            Write(sb);
            Debug.Print("{0}", sb);
        }
    }
}
