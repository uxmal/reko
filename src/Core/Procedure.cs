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

using Reko.Core.Absyn;
using Reko.Core.Graphs;
using Reko.Core.Output;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.Core
{
    /// <summary>
    /// Represents a procedure that has been decompiled from machine code.
    /// </summary>
    public class Procedure : ProcedureBase, IAddressable
	{
        private readonly List<Block> blocks;

        /// <summary>
        /// Constructs an instance of the <inheritdoc cref="Procedure"/> class.
        /// </summary>
        /// <param name="arch"><see cref="IProcessorArchitecture"/> for the procedure.</param>
        /// <param name="name">Name of the procedure.</param>
        /// <param name="addrEntry">Address of the entry point of the procedure.</param>
        /// <param name="frame">The <see cref="Frame"/> of the procedure, containing
        /// all registers, flags etc used by it.
        /// </param>
		public Procedure(
            IProcessorArchitecture arch, 
            string name, 
            Address addrEntry, 
            Frame frame) : base(name, true)
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

        /// <summary>
        /// The <see cref="IProcessorArchitecture"/> used to decompile this procedure.
        /// </summary>
        public IProcessorArchitecture Architecture { get; }

        public List<AbsynStatement>? Body { get; set; }

        /// <summary>
        /// The control flow graph (CFG) of this procedure.
        /// </summary>
        public BlockGraph ControlGraph { get; }

        /// <summary>
        /// A synthetic <see cref="Block"/> modelling entry into a procedure.
        /// Architecture and platform invariants can be implemented by injecting
        /// <see cref="Reko.Core.Code.DefInstruction"/>s or <see cref="Code.Assignment"/>s
        /// into this block.
        /// </summary>
        public Block EntryBlock { get; }

        /// <summary>
        /// A synthetic <see cref="Block"/> modelling all exits from the procedure in a 
        /// single block. During analysis it is used for <see cref="Code.UseInstruction"/>s
        /// that keep presumed live-out registers alive during dead code elimination.
        /// </summary>
        public Block ExitBlock { get; }

        /// <summary>
        /// <see cref="Reko.Core.Frame"/> associated with this procedure.
        /// </summary>
        public Frame Frame { get; }

        /// <summary>
        /// The machine address where the machine code for the procdure entry point is 
        /// located.
        /// </summary>
        public Address EntryAddress { get; }

        Address IAddressable.Address => EntryAddress;

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
        /// <param name="arch"><see cref="IProcessorArchitecture"/> used by this procedure.</param>
        /// <param name="name">Optional name of the procedure; if null a name is automatically
        /// generated.</param>
        /// <param name="addr">Entry point address of the procedure.</param>
        /// <param name="f"><see cref="Frame"/> of the procedure.</param>
        /// <returns>A new instance of a <see cref="Procedure"/>.</returns>
		public static Procedure Create(IProcessorArchitecture arch, string? name, Address addr, Frame f)
		{
			name ??= NamingPolicy.Instance.ProcedureName(addr);
			return new Procedure(arch, name, addr, f);
		}

        /// <summary>
        /// Creates a procedure. The address of the procedure is used to generate
        /// a name.
        /// </summary>
        /// <param name="arch">The architecture of the procedure.</param>
        /// <param name="addr">Entry point address of the procedure.</param>
        /// <param name="f"><see cref="Frame"/> of the procedure.</param>
        /// <returns>A new instance of a <see cref="Procedure"/>.</returns>
		public static Procedure Create(IProcessorArchitecture arch, Address addr, Frame f)
		{
			return new Procedure(arch, NamingPolicy.Instance.ProcedureName(addr), addr, f);
		}

        /// <summary>
        /// Writes the procedure to the debug output window.
        /// </summary>
        /// <param name="dump">If true performs the dump; otherwise no dump is made.</param>
        [Conditional("DEBUG")]
		public void Dump(bool dump)
		{
			if (!dump)
				return;
			
			StringWriter sb = new StringWriter();
			Write(false, sb);
			Debug.WriteLine(sb.ToString());
		}

        /// <summary>
        /// Creates a dominator graph for the procedure.
        /// </summary>
        public BlockDominatorGraph CreateBlockDominatorGraph()
        {
            return new BlockDominatorGraph(new BlockGraph(blocks), EntryBlock);
        }

        /// <summary>
        /// Used to order blocks within a procedure for display.
        /// </summary>
        public class BlockComparer : IComparer<Block>
        {
            /// <summary>
            /// Compares two blocks for display purposes.
            /// </summary>
            /// <param name="x">First block.</param>
            /// <param name="y">Second block.</param>
            /// <returns>A number indicating the expected ordering 
            /// of the two blocks.
            /// </returns>
            public int Compare(Block? x, Block? y)
            {
                if (x == y) 
                    return 0;
                if (x is null)
                    return y is null ? 0 : -1;
                if (y is null)
                    return 1;

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
                    
                return String.Compare(x.Id, y.Id);
            }
        }

        /// <summary>
        /// Writes the blocks sorted by address ascending.
        /// </summary>
        /// <param name="emitFrame">If true, write the contents of the procedure's <see cref="Frame"/> also.</param>
        /// <param name="writer">Output sink.</param>
		public void Write(bool emitFrame, TextWriter writer)
        {
            Write(emitFrame, true, false, writer);
        }

        /// <summary>
        /// Writes the procedure to a text writer.
        /// </summary>
        /// <param name="emitFrame">If true, write the contents of the procedure's <see cref="Frame"/> also.</param>
        /// <param name="showEdges">If true, write the successor edges as well.</param>
        /// <param name="lowLevelInfo">If true, display low level information.</param>
        /// <param name="writer">Output sink.</param>
		public void Write(
            bool emitFrame, bool showEdges, bool lowLevelInfo, TextWriter writer)
        {
            writer.WriteLine("// {0}", QualifiedName());
            writer.WriteLine("// Return size: {0}", this.Signature.ReturnAddressOnStack);
            if (emitFrame)
                Frame.Write(writer);
            FunctionType.EmitFlags flags;
            if (lowLevelInfo)
            {
                flags = FunctionType.EmitFlags.LowLevelInfo;
            }
            else
            {
                flags = FunctionType.EmitFlags.None;
            }
            Signature.Emit(QualifiedName(), flags, new TextFormatter(writer));
            writer.WriteLine();
            WriteBody(showEdges, writer);
        }

        /// <summary>
        /// Writes the body of the procedure to a text writer.
        /// </summary>
        /// <param name="showEdges">If true, write the successor edges as well.</param>
        /// <param name="writer">Output sink.</param>
        public void WriteBody(bool showEdges, TextWriter writer)
        {
            var formatter = CreateCodeFormatter(new TextFormatter(writer));
            new ProcedureFormatter(this, new BlockDecorator { ShowEdges = showEdges }, formatter).WriteProcedureBlocks();
        }

        /// <summary>
        /// Writes the procedure to a text writer.
        /// </summary>
        /// <param name="emitFrame">If true, writes the contents of the procedure's 
        /// <see cref="Frame"/> also.</param>
        /// <param name="decorator">Auxiliary class that generates extra output
        /// for each block.</param>
        /// <param name="writer">Output sink.</param>
        public void Write(bool emitFrame, BlockDecorator decorator, TextWriter writer)
        {
            writer.WriteLine("// {0}", Name);
            if (emitFrame)
                Frame.Write(writer);
            var formatter = new TextFormatter(writer);
            Signature.Emit(Name, FunctionType.EmitFlags.None, new TextFormatter(writer));
            writer.WriteLine();
            var codeFormatter = CreateCodeFormatter(formatter);
            new ProcedureFormatter(this, decorator, codeFormatter).WriteProcedureBlocks();
        }

        /// <summary>
        /// Creates a code formatter, suitable for this procedure.
        /// </summary>
        /// <param name="formatter">Output sink.</param>
        public CodeFormatter CreateCodeFormatter(Formatter formatter)
        {
            return new CodeFormatter(formatter);
        }

        /// <summary>
        /// Returns the blocks of this procedure, sorted by their name.
        /// </summary>
        public IOrderedEnumerable<Block> SortBlocksByName()
        {
            return blocks.OrderBy((x => x), new BlockComparer());
        }

        /// <summary>
        /// Adds a block to the procedure.
        /// </summary>
        /// <param name="addr">Address of the block.</param>
        /// <param name="name">Identifier for the block.</param>
        /// <returns>An empty <see cref="Block"/> instance.</returns>
        public Block AddBlock(Address addr, string name)
        {
            var block = new Block(this, addr, name);
            blocks.Add(block);
            return block;
        }

        /// <summary>
        /// Adds a synthetic block to the procedure. A synthetic block is one that
        /// doesn't correspond to code in the original binary, but is created by the decompiler.
        /// </summary>
        /// <param name="addr">Address of the block.</param>
        /// <param name="name">Identifier for the block.</param>
        /// <returns>An empty <see cref="Block"/> instance.</returns>
        public Block AddSyntheticBlock(Address addr, string name)
        {
            var block = AddBlock(addr, name);
            block.IsSynthesized = true;
            return block;
        }

        /// <summary>
        /// Adds a block to the procedure.
        /// </summary>
        /// <param name="block">Block to add.</param>
        public void AddBlock(Block block)
        {
            blocks.Add(block);
        }

        /// <summary>
        /// Removes a block from the procedure.
        /// </summary>
        /// <remarks>
        /// Note that any graph edges
        /// are not removed, so calling this method may leave the procedure's 
        /// block graph in an inconsistent state. Consider using <see cref="ControlGraph"/>
        /// methods instead.
        /// </remarks>
        /// <param name="block">Block to remove.</param>
        public void RemoveBlock(Block block)
        {
            blocks.Remove(block);
        }
    }
}
