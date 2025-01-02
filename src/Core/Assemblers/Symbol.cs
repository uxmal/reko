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

using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Reko.Core.Assemblers
{
	public class Symbol
	{
		public Symbol(string s)
		{
			Name = s;
			IsResolved = false;
			Offset = 0;
			Patches = new List<BackPatch>();
		}

        public bool IsResolved { get; set; }
        public int Offset { get; set; }
        public string Name { get; }
        public List<BackPatch> Patches { get; }


        // Add forward references to the backpatch list.
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

        public void ResolveBe(IEmitter emitter)
        {
            Debug.Assert(IsResolved);
            foreach (BackPatch patch in Patches)
            {
                emitter.PatchBe(patch.offset, Offset, patch.Size);
            }
        }

        public void ResolveLe(IEmitter emitter)
        {
            Debug.Assert(IsResolved);
            foreach (BackPatch patch in Patches)
            {
                emitter.PatchLe(patch.offset, this.Offset / patch.UnitSize, patch.Size);
            }
        }

		public override string ToString()
		{
			return Name;
		}
    }


	public class BackPatch
	{
		public int	offset;
        public int UnitSize;

		public BackPatch(int o, DataType s, int unitSize)
		{
			offset = o; 
			Size = s;
            this.UnitSize = unitSize;
		}

        public DataType Size { get; }
    }

	public class SymbolTable 
	{
        private readonly SortedList<string, Symbol> symbols;

        public SymbolTable()
		{
			symbols = new SortedList<string,Symbol>();
			Equates = new Dictionary<string,int>();
		}

        public Dictionary<string, int> Equates { get; }

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

		public Symbol DefineSymbol(string s, int off)
		{
			Symbol	sym;
			if (symbols.ContainsKey(s))
			{
				sym = symbols[s];
				if (sym.IsResolved || sym.Offset != 0)
					throw new ApplicationException("symbol '" + s + "' redefined");
			}
			else
			{
				sym = new Symbol(s);
				symbols[s] = sym;
			}
			sym.IsResolved = true;
			sym.Offset = off;
			return sym;
		}

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
