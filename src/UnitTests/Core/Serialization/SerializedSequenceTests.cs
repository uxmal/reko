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

using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Environments.Msdos;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Reko.UnitTests.Core.Serialization
{
	[TestFixture]
	public class SerializedSequenceTests
	{
        private IPlatform platform;
        private ProcedureSerializer ser;

        [SetUp]
        public void Setup()
        {
            var sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
            var arch = new X86ArchitectureReal("x86-real-16");
            this.platform = new MsdosPlatform(sc, arch);
        }

        private void Given_X86ProcedureSerializer()
        {
            this.ser = new ProcedureSerializer(
                platform,
                new TypeLibraryDeserializer(platform, true, new TypeLibrary()),
                "stdapi");
        }

		[Test]
		public void SseqCreate()
		{
			Identifier head = new Identifier(Registers.dx.Name, Registers.dx.DataType, Registers.dx);
			Identifier tail = new Identifier(Registers.ax.Name, Registers.ax.DataType, Registers.ax);
			Identifier seq = new Identifier("dx_ax", PrimitiveType.Word32, new SequenceStorage(PrimitiveType.Word32, head.Storage, tail.Storage));
			SerializedSequence sq = new SerializedSequence((SequenceStorage) seq.Storage);
			Assert.AreEqual("dx", sq.Registers[0].Name);
			Assert.AreEqual("ax", sq.Registers[1].Name);

            XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(sq.GetType());
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
            Frame f = platform.Architecture.CreateFrame();
			Identifier head = f.EnsureRegister(Registers.dx);
			Identifier tail = f.EnsureRegister(Registers.ax);
			Identifier seq = f.EnsureSequence(PrimitiveType.Word32, head.Storage, tail.Storage);
			SerializedSequence sq = new SerializedSequence((SequenceStorage) seq.Storage);
			Argument_v1 sa = new Argument_v1();
			sa.Kind = sq;
			SerializedSignature ssig = new SerializedSignature();
			ssig.Arguments = new Argument_v1[] { sa };

            Given_X86ProcedureSerializer();
			FunctionType ps = ser.Deserialize(ssig, f);
			Assert.AreEqual("void foo(Sequence word32 dx_ax)", ps.ToString("foo"));
		}

		[Test]
		public void VoidFunctionSignature()
		{
			var sig = new SerializedSignature();
            Given_X86ProcedureSerializer();
            FunctionType ps = ser.Deserialize(sig, platform.Architecture.CreateFrame());
			Assert.AreEqual("void foo()", ps.ToString("foo"));
			Assert.IsTrue(ps.ParametersValid);
		}
	}
}
