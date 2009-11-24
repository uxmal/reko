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

using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Decompiler.UnitTests.Core.Serialization
{
	[TestFixture]
	public class SerializedSequenceTests
	{
		private IntelArchitecture arch = new IntelArchitecture(ProcessorMode.Real);

		[Test]
		public void SseqCreate()
		{
			Identifier head = new Identifier(Registers.dx.Name, 0, Registers.dx.DataType, new RegisterStorage(Registers.dx));
			Identifier tail = new Identifier(Registers.ax.Name, 1, Registers.ax.DataType, new RegisterStorage(Registers.ax));
			Identifier seq = new Identifier("dx_ax", 2, PrimitiveType.Word32, new SequenceStorage(head, tail));
			SerializedSequence sq = new SerializedSequence((SequenceStorage) seq.Storage);
			Assert.AreEqual("dx", sq.Registers[0].Name);
			Assert.AreEqual("ax", sq.Registers[1].Name);

			XmlSerializer ser = new XmlSerializer(sq.GetType());
			StringWriter sw = new StringWriter();
			XmlTextWriter xml = new XmlTextWriter(sw);
			xml.Formatting = Formatting.None;
			ser.Serialize(xml, sq);
			string s = sw.ToString();
			int i = s.IndexOf("<reg");
			Assert.AreEqual("<reg>dx</reg><reg>ax</reg>", s.Substring(i, s.IndexOf("</Se") - i));
		}

		[Test]
		public void SseqBuildSequence()
		{
            Frame f = arch.CreateFrame();
			Identifier head = f.EnsureRegister(Registers.dx);
			Identifier tail = f.EnsureRegister(Registers.ax);
			Identifier seq = f.EnsureSequence(head, tail, PrimitiveType.Word32);
			SerializedSequence sq = new SerializedSequence((SequenceStorage) seq.Storage);
			SerializedArgument sa = new SerializedArgument();
			sa.Kind = sq;
			SerializedSignature ssig = new SerializedSignature();
			ssig.Arguments = new SerializedArgument[] { sa };

			ProcedureSerializer ser = new ProcedureSerializer(arch, "stdapi");
			ProcedureSignature ps = ser.Deserialize(ssig, f);
			Assert.AreEqual("void foo(Sequence word32 dx_ax)", ps.ToString("foo"));
		}

		[Test]
		public void VoidFunctionSignature()
		{
			SerializedSignature sig = new SerializedSignature();
			ProcedureSerializer ser = new ProcedureSerializer(arch, "stdapi");
            ProcedureSignature ps = ser.Deserialize(sig, arch.CreateFrame());
			Assert.AreEqual("void foo()", ps.ToString("foo"));
			Assert.IsTrue(ps.ArgumentsValid);
		}
	}
}
