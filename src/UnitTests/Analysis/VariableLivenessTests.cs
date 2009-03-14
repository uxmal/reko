/* 
 * Copyright (C) 1999-2009 John Källén.
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
using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

//$REFACTOR: rename file to IdentifierLivenessTests

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class IdentifierLivenessTests
	{
		private IntelArchitecture arch;
		private Frame frame;
		private IdentifierLiveness vl;

		public IdentifierLivenessTests()
		{
			arch = new IntelArchitecture(ProcessorMode.Real);
		}

		[SetUp]
		public void Setup()
		{
			frame = new Frame(PrimitiveType.Word16);
			vl = new IdentifierLiveness(arch);
			vl.BitSet = arch.CreateRegisterBitset();
		}

		[Test]
		public void BitOffsetAl()
		{
			Identifier al = frame.EnsureRegister(Registers.al);
			Def(al);
			Assert.AreEqual(8, vl.DefBitSize, "al size");
			Assert.AreEqual(0, vl.DefOffset, "al offset");
		}

		[Test]
		public void Subregisters()
		{
			Identifier ax = frame.EnsureRegister(Registers.ax);
			Identifier al = frame.EnsureRegister(Registers.al);
			Identifier ah = frame.EnsureRegister(Registers.ah);
	
			Use(ax, 0, 16);
			Def(ah);
			Assert.AreEqual(" al", Dump());
		}

		[Test]
		public void LocalVar()
		{
			vl.LiveStorages = new Dictionary<Storage,int>();
			Identifier loc = frame.EnsureStackLocal(-8, PrimitiveType.Word32);		// pushed as a word.
			Use(loc, 0, 16); // only read the last 16 bits.
			Assert.AreEqual(1, vl.LiveStorages.Count);
			Assert.AreEqual(16, (int) vl.LiveStorages[frame.FindStackLocal(-8, 4).Storage]);
		}

		[Test]
		public void UseStackArg()
		{
			vl.LiveStorages = new Dictionary<Storage,int>();
			Identifier arg = frame.EnsureStackArgument(4, PrimitiveType.Word32);
			Use(arg, 0, 32);
			Assert.AreEqual(1, vl.LiveStorages.Count);
			Assert.AreEqual(32, (int) vl.LiveStorages[frame.FindStackArgument(4, 4).Storage]);
		}

		[Test]
		public void Sequences()
		{
			Identifier es = frame.EnsureRegister(Registers.es);
			Identifier bx = frame.EnsureRegister(Registers.bx);
			Identifier es_bx = frame.EnsureSequence(es, bx, PrimitiveType.Word32);
			Assert.AreSame(PrimitiveType.Word32, es_bx.DataType);
			vl.Def(es_bx);
			Assert.AreEqual(32, vl.DefBitSize, "es_bx size");
			Assert.AreEqual(0, vl.DefOffset);
		}

		[Test]
		public void StackArgumentDef()
		{
			Identifier arg04 = frame.EnsureStackArgument(4, PrimitiveType.Word32);
			vl.LiveStorages[arg04.Storage] = 16;
			vl.Def(arg04);
			Assert.AreEqual(0, vl.LiveStorages.Count);
			Assert.AreEqual(16, vl.DefBitSize);
		}

		[Test]
		public void UseTemporary()
		{
			vl.LiveStorages = new Dictionary<Storage,int>();
			Identifier tmp = frame.CreateTemporary(PrimitiveType.Word16);
			Use(tmp, 0, tmp.DataType.BitSize);
			Assert.AreEqual(1, vl.LiveStorages.Count);
		}

		private void Def(Identifier id)
		{
			vl.Def(frame.Identifiers[id.Number]);
		}

		private void Use(Identifier id, int bitOffset, int bitSize)
		{
			vl.Use(frame.Identifiers[id.Number], bitOffset, bitSize);
		}

		private string Dump()
		{
			StringWriter w = new StringWriter();
			DataFlow.EmitRegisters(arch, "", 0, vl.BitSet, w);
			return w.ToString();
		}
	}
}
