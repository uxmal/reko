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

using Reko.Core.Absyn;
using Reko.Core.Code;
using Reko.Core.Lib;
using Reko.Core.Output;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Core
{
	/// <summary>
	/// Represents a procedure that has been decompiled from machine code.
	/// </summary>
	public class Procedure : ProcedureBase
	{
        private List<Block> blocks;

		public Procedure(IProcessorArchitecture arch, string name, Address addrEntry, Frame frame) : base(name)
		{
            this.EntryAddress = addrEntry;
            this.Architecture = arch ?? throw new ArgumentNullException(nameof(arch));
            //$REVIEW consider removing Body completely and use
            // AbsynProcedure instead.
            this.Body = null;
            this.blocks = new List<Block>();
            this.ControlGraph = new BlockGraph(blocks);
			this.Frame = frame;
			this.Signature = new FunctionType();
            this.EntryBlock = AddBlock(addrEntry, Name + "_entry");
            this.ExitBlock = AddBlock(addrEntry, Name + "_exit");
        }

        /// <summary>
        /// The effects of this procedure on registers, stack, and FPU stack.
        /// </summary>
        public override FunctionType Signature { get; set; }

        /// <summary>
        /// True if the user specified this procedure by adding it to the project
        /// file or by marking it in the user interface.
        /// </summary>
        public bool UserSpecified { get; set; }

        public IProcessorArchitecture Architecture { get; }
        public List<AbsynStatement> Body { get; set; }
        public BlockGraph ControlGraph { get; }
        public Block EntryBlock { get; }
        public Block ExitBlock { get; }
        public Frame Frame { get; }
        public Address EntryAddress { get; }

        /// <summary>
        /// Returns all the statements of the procedure, in no particular order.
        /// </summary>
        public IEnumerable<Statement> Statements
        {
            get { return blocks.SelectMany(b => b.Statements); }
        }

		/// <summary>
		/// Creates a procedure with the specified name; if no name is specified (null string)
		/// the address is used instead.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="addr"></param>
		/// <param name="f"></param>
		/// <returns></returns>
		public static Procedure Create(IProcessorArchitecture arch, string name, Address addr, Frame f)
		{
			if (name == null)
			{
				name = NamingPolicy.Instance.ProcedureName(addr);
			}
			return new Procedure(arch, name, addr, f);
		}

		public static Procedure Create(IProcessorArchitecture arch, Address addr, Frame f)
		{
			return new Procedure(arch, NamingPolicy.Instance.ProcedureName(addr), addr, f);
		}

        [Conditional("DEBUG")]
		public void Dump(bool dump)
		{
			if (!dump)
				return;
			
			StringWriter sb = new StringWriter();
			Write(false, sb);
			Debug.WriteLine(sb.ToString());
		}

        public BlockDominatorGraph CreateBlockDominatorGraph()
        {
            return new BlockDominatorGraph(new BlockGraph(blocks), EntryBlock);
        }

        /// <summary>
        /// Used to order blocks within a procedure for display.
        /// </summary>
        public class BlockComparer : IComparer<Block>
        {
            public int Compare(Block x, Block y)
            {
                if (x == y) 
                    return 0;
                // Entry block is always displayed first.
                var eb = x.Procedure.EntryBlock;
                if (x == eb)
                    return -1;
                else if (y == eb) 
                    return 1;

                // Exit block is always displayed last.
                var ex = x.Procedure.ExitBlock;
                if (x == ex)
                    return 1;
                else if (y == ex)
                    return -1;
                    
                return String.Compare(x.Name, y.Name);
            }
        }

        /// <summary>
        /// If the procedure is a member of a class, write the class name first.
        /// </summary>
        /// <returns></returns>
        public string QualifiedName()
        {
            if (EnclosingType == null)
                return Name;
            if (EnclosingType is StructType_v1 str)
                return string.Format("{0}::{1}", str.Name, Name);
            return Name;
        }

        /// <summary>
        /// Writes the blocks sorted by address ascending.
        /// </summary>
        /// <param name="emitFrame"></param>
        /// <param name="writer"></param>
		public void Write(bool emitFrame, TextWriter writer)
        {
            Write(emitFrame, true, writer);
        }

		public void Write(bool emitFrame, bool showEdges, TextWriter writer)
        {
            writer.WriteLine("// {0}", QualifiedName());
            writer.WriteLine("// Return size: {0}", this.Signature.ReturnAddressOnStack);
            if (emitFrame)
                Frame.Write(writer);
            Signature.Emit(QualifiedName(), FunctionType.EmitFlags.None, new TextFormatter(writer));
            writer.WriteLine();
            WriteBody(showEdges, writer);
        }

        public void WriteBody(bool showEdges, TextWriter writer)
        {
            var formatter = new CodeFormatter(new TextFormatter(writer));
            new ProcedureFormatter(this, new BlockDecorator { ShowEdges = showEdges }, formatter).WriteProcedureBlocks();
        }

        public void Write(bool emitFrame, BlockDecorator decorator, TextWriter writer)
        {
            writer.WriteLine("// {0}", Name);
            if (emitFrame)
                Frame.Write(writer);
            var formatter = new TextFormatter(writer);
            Signature.Emit(Name, FunctionType.EmitFlags.None, new TextFormatter(writer));
            writer.WriteLine();
            var codeFormatter = new CodeFormatter(formatter);
            new ProcedureFormatter(this, decorator, codeFormatter).WriteProcedureBlocks();
        }

        public void WriteGraph(TextWriter writer)
        {
            foreach (var b in SortBlocksByName())
            {
                writer.WriteLine(b.Name);
                writer.Write("    Pred:");
                foreach (var p in b.Pred)
                    writer.Write(" {0}", p.Name);
                writer.WriteLine();
                writer.Write("    Succ:");
                foreach (var s in b.Succ)
                    writer.Write(" {0}", s.Name);
                writer.WriteLine();
            }
        }

        public IOrderedEnumerable<Block> SortBlocksByName()
        {
            return blocks.OrderBy((x => x), new BlockComparer());
        }

        public Block AddBlock(Address addr, string name)
        {
            Block block = new Block(this, name) { Address = addr };
            blocks.Add(block);
            return block;
        }

        public Block AddSyntheticBlock(Address addr, string name)
        {
            var block = AddBlock(addr, name);
            block.IsSynthesized = true;
            return block;
        }

        public Block AddBlock(string name)
        {
            Block block = new Block(this, name);
            blocks.Add(block);
            return block;
        }

        public void AddBlock(Block block)
        {
            blocks.Add(block);
        }

        public void RemoveBlock(Block block)
        {
            blocks.Remove(block);
        }
    }
}
