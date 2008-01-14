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

using Decompiler.Arch.Intel.Assembler;
using NUnit.Framework;
using System;
using System.IO;

namespace Decompiler.UnitTests.Intel
{
	[TestFixture]
	public class SymbolTableTests
	{
		[Test]
		public void SymCreateSymbol()
		{
			SymbolTable symtab = new SymbolTable();
			Symbol sym = symtab.CreateSymbol("foo");
			StringWriter writer = new StringWriter();
			symtab.Write(writer);
			Assert.AreEqual(
@"foo: unresolved 00000000 patches: 0 (foo)
", writer.ToString());
		}

		[Test]
		public void SymResolveReference()
		{
			SymbolTable symtab = new SymbolTable();
			Symbol sym = symtab.CreateSymbol("foo");
			Symbol sym2 = symtab.DefineSymbol("foo", 3);
			StringWriter writer = new StringWriter();
			symtab.Write(writer);
			Assert.AreEqual(
@"foo: resolved 00000003 patches: 0 (foo)
", 
				writer.ToString());
		}

		[Test]
		public void SymUndefinedSymbols()
		{
			SymbolTable symtab = new SymbolTable();
			Symbol foo = symtab.CreateSymbol("foo");
			Symbol bar = symtab.CreateSymbol("bar");
			Symbol fred = symtab.DefineSymbol("fred", 0x10);
			Symbol [] undef = symtab.GetUndefinedSymbols();
			Assert.AreEqual(2, undef.Length);
			Assert.AreSame(bar, undef[0]);
			Assert.AreSame(foo, undef[1]);
		}
	}
}
