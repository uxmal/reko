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

using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Reko.Core.Assemblers
{
    /// <summary>
    /// Represents a symbol encountered during assembly.
    /// </summary>
	public class Symbol
	{
        /// <summary>
        /// Creates a symbol named <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name of the symbol.</param>
		public Symbol(string name)
		{
			Name = name;
			IsResolved = false;
			Offset = 0;
            Patches = [];
		}

        /// <summary>
        /// True if the symbol's offset is resolved; otherwise false.
        /// </summary>
        public bool IsResolved { get; set; }

        /// <summary>
        /// The offset of the symbol in the binary
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// The name of the symbol.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The list of backpatches that need to be resolved.
        /// </summary>
        public List<BackPatch> Patches { get; }

        /// <summary>
        /// Add a forward reference to the backpatch list.
        /// </summary>
        /// <param name="offset">Position at which the reference is made.</param>
        /// <param name="width">Size of the reference.</param>
        /// <param name="unitSize">Size of the storage units used in 
        /// the backing memory (usually 8 bits).</param>
        public void AddForwardReference(int offset, DataType width, int unitSize)
		{
			Patches.Add(new BackPatch(offset, width, unitSize));
		}

        /// <summary>
        /// Refer to this symbol, and if the symbol's address is 
        /// resolved, patch the target big-endian.
        /// </summary>
        public void ReferToBe(int off, DataType width, IEmitter emitter)
        {
            if (IsResolved)
            {
                emitter.PatchBe(off, Offset, width);
            }
            else
            {
                // Add forward references to the backpatch list.
                AddForwardReference(off, width, 1);
            }
        }

        /// <summary>
        /// Refer to this symbol, and if the symbol's address is 
        /// resolved, patch the target little-endian.
        /// </summary>
        public void ReferToLe(int off, DataType width, IEmitter emitter)
        {
            if (IsResolved)
            {
                emitter.PatchLe(off, Offset, width);
            }
            else
            {
                // Add forward references to the backpatch list.
                AddForwardReference(off, width, 1);
            }
        }

        /// <inheritdoc/>
        public void ReferToLeWordCount(int off, DataType width, IEmitter emitter)
        {
            if (IsResolved)
            {
                emitter.PatchLe(off, this.Offset / 2, width);
            }
            else
            {
                // Add forward references to the backpatch list.
                AddForwardReference(off, width, 2);
            }
        }

        /// <summary>
        /// Resolve all backpatches to this symbol in big-endian order.
        /// </summary>
        /// <param name="emitter">Emitter to write to.
        /// </param>
        public void ResolveBe(IEmitter emitter)
        {
            Debug.Assert(IsResolved);
            foreach (BackPatch patch in Patches)
            {
                emitter.PatchBe(patch.Offset, Offset, patch.Size);
            }
        }

        /// <summary>
        /// Resolve all backpatches to this symbol in little-endian order.
        /// </summary>
        /// <param name="emitter">Emitter to write to.
        /// </param>
        public void ResolveLe(IEmitter emitter)
        {
            Debug.Assert(IsResolved);
            foreach (BackPatch patch in Patches)
            {
                emitter.PatchLe(patch.Offset, this.Offset / patch.UnitSize, patch.Size);
            }
        }

        /// <summary>
        /// Returns the name of the symbol.
        /// </summary>
		public override string ToString()
		{
			return Name;
		}
    }

    /// <summary>
    /// A relocation, or "backpatch" of a forward reference.
    /// </summary>
	public class BackPatch
	{
        /// <summary>
        /// Offset in the output where the patch is to be made.
        /// </summary>
		public int Offset { get; set; }

        /// <summary>
        /// Size of the patch in storage units.
        /// </summary>
        public int UnitSize { get; set; }

        /// <summary>
        /// Constructs a backpatch.
        /// </summary>
        /// <param name="offset">Offset for the patch.</param>
        /// <param name="dt">Data type of the patch.</param>
        /// <param name="unitSize">Size of the type in storage units.
        /// </param>
		public BackPatch(int offset, DataType dt, int unitSize)
		{
			Offset = offset; 
			Size = dt;
            this.UnitSize = unitSize;
		}

        /// <summary>
        /// Data type of the patch.
        /// </summary>

        public DataType Size { get; set; }
    }

    /// <summary>
    /// Symbol table to keep track of the symbols found during assembly.
    /// </summary>
	public class SymbolTable 
	{
        private readonly SortedList<string, Symbol> symbols;

        /// <summary>
        /// Constructs a symbol table.
        /// </summary>
        public SymbolTable()
		{
			symbols = new SortedList<string,Symbol>();
			Equates = new Dictionary<string,int>();
		}

        /// <summary>
        /// A dictionary of equates that have been encountered.
        /// </summary>
        public Dictionary<string, int> Equates { get; }

        /// <summary>
        /// Creates a symbol. If the symbol already exists, it is returned.
        /// If it doesn't exist already, a forward reference is generated.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Symbol CreateSymbol(string s)
		{
            if (!symbols.TryGetValue(s, out Symbol? sym))
            {
                // Forward reference to a symbol. 
                sym = new Symbol(s);
                symbols.Add(s, sym);
            }
            return sym;
		}

        /// <summary>
        /// Defines a symbol. If the symbol already exists, an error 
        /// is raised.
        /// </summary>
        /// <param name="symbolName">Symbol name.</param>
        /// <param name="offset">Offset for the symbol.</param>
        /// <returns>The defined <see cref="Symbol"/>.</returns>
		public Symbol DefineSymbol(string symbolName, int offset)
		{
			Symbol	sym;
			if (symbols.ContainsKey(symbolName))
			{
				sym = symbols[symbolName];
				if (sym.IsResolved || sym.Offset != 0)
					throw new ApplicationException("symbol '" + symbolName + "' redefined");
			}
			else
			{
				sym = new Symbol(symbolName);
				symbols[symbolName] = sym;
			}
			sym.IsResolved = true;
			sym.Offset = offset;
			return sym;
		}

        /// <summary>
        /// Get an array of all symbols that haven't been resolved yet.
        /// </summary>
        public Symbol [] GetUndefinedSymbols()
		{
            var undef = new List<Symbol>();
			foreach (Symbol sym in symbols.Values)
			{
				if (!sym.IsResolved)
					undef.Add(sym);
			}
            return undef.ToArray();
		}

        /// <summary>
        /// Writes the symbol table to a text writer.
        /// </summary>
        /// <param name="txt">Output sink.</param>
		public void Write(TextWriter txt)
		{
			foreach (KeyValuePair<string,Symbol> de in symbols)
			{
				txt.WriteLine("{0}: {1} {2:X8} patches: {3} ({4})", 
					de.Key, 
					de.Value.IsResolved ? "resolved" : "unresolved",
					de.Value.Offset, 
                    de.Value.Patches.Count,
					de.Value.Name ?? "");
			}
		}
	}
}
