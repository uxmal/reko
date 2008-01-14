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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.Typing;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Typing
{
	[TestFixture]
	public class AddressTraitCollectorTests
	{
		private TypeFactory factory;
		private TypeStore store;
		private ProcedureMock m;
		private AddressTraitCollector atrco;
		private MockTraitHandler handler;
		private EquivalenceClassBuilder eqb;
		private InductionVariableCollection ivs;
		private Identifier globals;

		[Test]
		public void AtrcoTestIdPlusConst()
		{
			Identifier r = m.Local32("r");
			MemoryAccess mem = m.Load(PrimitiveType.Word32, m.Add(r, 4));
			mem.Accept(eqb);
			atrco.Collect(null, mem.TypeVariable, mem.EffectiveAddress);
			Verify(null, "Typing/AtrcoTestIdPlusConst.txt");
		}

		[Test]
		public void AtrcoTestId()
		{
			Identifier r = m.Local32("r");
			MemoryAccess mem = m.Load(PrimitiveType.Byte, r);
			mem.Accept(eqb);
			atrco.Collect(null, mem.TypeVariable, mem.EffectiveAddress);
			Verify(null, "Typing/AtrcoTestId.txt");
		}

		[Test]
		public void AtrcoTestidPlusId()
		{
			Identifier r = m.Local32("r");
			Identifier s = m.Local32("s");
			MemoryAccess mem = m.Load(PrimitiveType.Byte, m.Add(r, s));
			mem.Accept(eqb);
			atrco.Collect(null, mem.TypeVariable, mem.EffectiveAddress);
		}

		[Test]
		public void AtrcoIdMinusConst()
		{
			Identifier r = m.Local32("r");
			MemoryAccess mem = m.Load(PrimitiveType.Word32, m.Sub(r, 4));
			mem.Accept(eqb);
			atrco.Collect(null, mem.TypeVariable, mem.EffectiveAddress);
			Verify(null, "Typing/AtrcoIdMinusConst.txt");

		}

		[Test]
		public void AtrcoMem()
		{
			Identifier pp = m.Local32("pp");
			MemoryAccess mem = m.Load(PrimitiveType.Byte, m.Load(PrimitiveType.Word32, pp));
			mem.Accept(eqb);
			atrco.Collect(null, mem.TypeVariable, mem.EffectiveAddress);
			Verify(null, "Typing/AtrcoMem.txt");
		}

		[Test]
		public void AtrcoSegPtr()
		{
			Identifier ds = m.Local16("ds");
			Identifier bx = m.Local16("bx");
			MemoryAccess mem = m.SegMem(PrimitiveType.Word16, ds, bx);
			mem.Accept(eqb);
			atrco.Collect(globals.TypeVariable, mem.TypeVariable, mem.EffectiveAddress);
			Verify(null, "Typing/AtrcoSegPtr.txt");
		}

		/// <summary>
		/// Tests the case when we have an induction variable where we only know that
		/// the increment.
		/// </summary>
		[Test]
		public void AtrcoInductionVariablIncr()
		{
			Identifier id = m.Local16("si");
			LinearInductionVariable iv = new LinearInductionVariable(null, new Constant(PrimitiveType.Int16, 1), null);
			MemoryAccess mem = m.Load(PrimitiveType.Byte, id);
			mem.Accept(eqb);
			atrco.VisitInductionVariable(id, iv);
			Verify(null, "Typing/AtrcoInductionVariableIncr.txt");
		}

		[SetUp]
		public void Setup()
		{
			factory = new TypeFactory();
			store = new TypeStore();
			handler = new MockTraitHandler();
			eqb = new EquivalenceClassBuilder(factory, store);
			globals = new Identifier("globals", 0, PrimitiveType.Pointer, null);
			store.EnsureTypeVariable(factory, globals);
			
			ivs = new InductionVariableCollection();
			atrco = new AddressTraitCollector(factory, store, handler, globals, ivs);
			m = new ProcedureMock();
		}

		private void Verify(Program prog, string outputFilename)
		{
			using (FileUnitTester fut = new FileUnitTester(outputFilename))
			{
				if (prog != null)
				{
					foreach (Procedure proc in prog.Procedures.Values)
					{
						proc.Write(false, fut.TextWriter);
						fut.TextWriter.WriteLine();
					}
				}
				handler.Traits.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}
	}
}
