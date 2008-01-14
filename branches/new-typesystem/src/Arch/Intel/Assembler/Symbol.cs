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

using Decompiler.Core;
using Decompiler.Core.Types;
using System;
using System.Collections;
using System.IO;

namespace Decompiler.Arch.Intel.Assembler
{
	public class Symbol
	{
		public string sym;
		public bool fResolved;
		public int offset;
		public ArrayList patches;		
			
		public Symbol(string s)
		{
			sym = s;
			fResolved = false;
			offset = 0;
			patches = new ArrayList();
		}

		public void AddForwardReference(int offset, PrimitiveType width)
		{
			patches.Add(new BackPatch(offset, width));
		}

		public override string ToString()
		{
			return sym;
		}

	}


	public class BackPatch
	{
		public int	offset;
		public PrimitiveType size;

		public BackPatch(int o, PrimitiveType s)
		{
			offset = o; 
			size = s;
		}
	}

	public class SymbolTable 
	{
		private SortedList symbols;
		private Hashtable equates;

		public SymbolTable()
		{
			symbols = new SortedList();
			equates = new Hashtable();
		}

		public Symbol AddDefinition(string s, int off)
		{
			return DefineSymbol(s, off);
		}

		public Symbol CreateSymbol(string s)
		{
			Symbol sym = (Symbol) symbols[s];
			if (sym == null)
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
				sym = (Symbol) symbols[s];
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

		public Hashtable Equates
		{
			get { return equates; }
		}

		public Symbol [] GetUndefinedSymbols()
		{
			ArrayList undef = new ArrayList();
			foreach (Symbol sym in symbols.Values)
			{
				if (!sym.fResolved)
					undef.Add(sym);
			}
			return (Symbol []) undef.ToArray(typeof (Symbol));
		}

		public void Write(TextWriter txt)
		{
			foreach (DictionaryEntry de in symbols)
			{
				string s = (string) de.Key;
				Symbol sym = (Symbol) de.Value;
				txt.WriteLine("{0}: {1} {2:X8} patches: {3} ({4})", 
					(string) de.Key, 
					sym.fResolved ? "resolved" : "unresolved",
					sym.offset, sym.patches.Count,
					(sym.sym != null ? sym.sym : ""));
			}
		}
	}			
}
