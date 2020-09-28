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
		public string sym;
		public bool fResolved;
		public int offset;
        public List<BackPatch> Patches { get; private set; }
			
		public Symbol(string s)
		{
			sym = s;
			fResolved = false;
			offset = 0;
			Patches = new List<BackPatch>();
		}

        // Add forward references to the backpatch list.
        public void AddForwardReference(int offset, DataType width, int unitSize)
		{
			Patches.Add(new BackPatch(offset, width, unitSize));
		}

        public void ReferToBe(int off, DataType width, IEmitter emitter)
        {
            if (fResolved)
            {
                emitter.PatchBe(off, offset, width);
            }
            else
            {
                // Add forward references to the backpatch list.
                AddForwardReference(off, width, 1);
            }
        }

        public void ReferToLe(int off, DataType width, IEmitter emitter)
        {
            if (fResolved)
            {
                emitter.PatchLe(off, offset, width);
            }
            else
            {
                // Add forward references to the backpatch list.
                AddForwardReference(off, width, 1);
            }
        }

        public void ReferToLeWordCount(int off, DataType width, IEmitter emitter)
        {
            if (fResolved)
            {
                emitter.PatchLe(off, this.offset / 2, width);
            }
            else
            {
                // Add forward references to the backpatch list.
                AddForwardReference(off, width, 2);
            }
        }

        public void ResolveBe(IEmitter emitter)
        {
            Debug.Assert(fResolved);
            foreach (BackPatch patch in Patches)
            {
                emitter.PatchBe(patch.offset, offset, patch.Size);
            }
        }

        public void ResolveLe(IEmitter emitter)
        {
            Debug.Assert(fResolved);
            foreach (BackPatch patch in Patches)
            {
                emitter.PatchLe(patch.offset, this.offset / patch.UnitSize, patch.Size);
            }
        }

		public override string ToString()
		{
			return sym;
		}
    }


	public class BackPatch
	{
		public int	offset;
		private DataType size;
        public int UnitSize;

		public BackPatch(int o, DataType s, int unitSize)
		{
			offset = o; 
			size = s;
            this.UnitSize = unitSize;
		}

		public DataType Size { get { return size; } }
	}

	public class SymbolTable 
	{
		private SortedList<string,Symbol> symbols;
		private Dictionary<string,int> equates;

		public SymbolTable()
		{
			symbols = new SortedList<string,Symbol>();
			equates = new Dictionary<string,int>();
		}

		public Symbol CreateSymbol(string s)
		{
			Symbol sym;
            if (!symbols.TryGetValue(s, out sym))
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
				if (sym.fResolved || sym.offset != 0)
					throw new ApplicationException("symbol '" + s + "' redefined");
			}
			else
			{
				sym = new Symbol(s);
				symbols[s] = sym;
			}
			sym.fResolved = true;
			sym.offset = off;
			return sym;
		}

		public Dictionary<string,int> Equates
		{
			get { return equates; }
		}

		public Symbol [] GetUndefinedSymbols()
		{
            List<Symbol> undef = new List<Symbol>();
			foreach (Symbol sym in symbols.Values)
			{
				if (!sym.fResolved)
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
					de.Value.fResolved ? "resolved" : "unresolved",
					de.Value.offset, 
                    de.Value.Patches.Count,
					(de.Value.sym != null ? de.Value.sym : ""));
			}
		}
	}			
}
