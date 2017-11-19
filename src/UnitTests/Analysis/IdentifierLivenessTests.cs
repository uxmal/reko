#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Analysis;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.UnitTests.Analysis
{
	[TestFixture]
	public class IdentifierLivenessTests
	{
		private IntelArchitecture arch;
		private Frame frame;
		private IdentifierLiveness vl;

		public IdentifierLivenessTests()
		{
			arch = new X86ArchitectureReal();
		}

		[SetUp]
		public void Setup()
		{
			frame = new Frame(PrimitiveType.Word16);
			vl = new IdentifierLiveness(arch);
            vl.Identifiers = new HashSet<RegisterStorage>();
		}

		[Test]
		public void Idlv_BitOffsetAl()
		{
			Identifier al = frame.EnsureRegister(Registers.al);
			Def(al);
			Assert.AreEqual(8, vl.DefBitSize, "al size");
			Assert.AreEqual(0, vl.DefOffset, "al offset");
		}

		[Test]
		public void Idlv_Subregisters()
		{
			Identifier ax = frame.EnsureRegister(Registers.ax);
			frame.EnsureRegister(Registers.al);
			Identifier ah = frame.EnsureRegister(Registers.ah);
	
			Use(ax, 0, 16);
			Def(ah);
			Assert.AreEqual(" al", Dump());
		}

		[Test]
		public void Idlv_LocalVar()
		{
			vl.LiveStorages = new Dictionary<Storage,int>();
			Identifier loc = frame.EnsureStackLocal(-8, PrimitiveType.Word32);		// pushed as a word.
			Use(loc, 0, 16); // only read the last 16 bits.
			Assert.AreEqual(1, vl.LiveStorages.Count);
			Assert.AreEqual(16, (int) vl.LiveStorages[frame.FindStackLocal(-8, 4).Storage]);
		}

		[Test]
		public void Idlv_UseStackArg()
		{
			vl.LiveStorages = new Dictionary<Storage,int>();
			Identifier arg = frame.EnsureStackArgument(4, PrimitiveType.Word32);
			Use(arg, 0, 32);
			Assert.AreEqual(1, vl.LiveStorages.Count);
			Assert.AreEqual(32, (int) vl.LiveStorages[frame.FindStackArgument(4, 4).Storage]);
		}

		[Test]
		public void Idlv_Sequences()
		{
			Identifier es = frame.EnsureRegister(Registers.es);
			Identifier bx = frame.EnsureRegister(Registers.bx);
			Identifier es_bx = frame.EnsureSequence(es.Storage, bx.Storage, PrimitiveType.Word32);
			Assert.AreSame(PrimitiveType.Word32, es_bx.DataType);
			vl.Def(es_bx);
			Assert.AreEqual(32, vl.DefBitSize, "es_bx size");
			Assert.AreEqual(0, vl.DefOffset);
		}

		[Test]
		public void Idlv_StackArgumentDef()
		{
			Identifier arg04 = frame.EnsureStackArgument(4, PrimitiveType.Word32);
			vl.LiveStorages[arg04.Storage] = 16;
			vl.Def(arg04);
			Assert.AreEqual(0, vl.LiveStorages.Count);
			Assert.AreEqual(16, vl.DefBitSize);
		}

		[Test]
		public void Idlv_UseTemporary()
		{
			vl.LiveStorages = new Dictionary<Storage,int>();
			Identifier tmp = frame.CreateTemporary(PrimitiveType.Word16);
			Use(tmp, 0, tmp.DataType.BitSize);
			Assert.AreEqual(1, vl.LiveStorages.Count);
		}

		private void Def(Identifier id)
		{
			vl.Def(id);
		}

		private void Use(Identifier id, int bitOffset, int bitSize)
		{
			vl.Use(id, bitOffset, bitSize);
		}

		private string Dump()
		{
			StringWriter w = new StringWriter();
			DataFlow.EmitRegisters(arch, "", 0, vl.Identifiers, w);
			return w.ToString();
		}
	}
}
