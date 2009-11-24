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
using Decompiler.Core.Machine;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Core.Serialization
{
	[TestFixture]
	public class ArgumentSerializerTests
	{
		private IntelArchitecture arch;
		private ProcedureSerializer sigser;
		private ArgumentSerializer argser;

		[SetUp]
		public void Setup()
		{
			arch = new IntelArchitecture(ProcessorMode.Real);
			sigser = new ProcedureSerializer(arch, "stdapi");
            argser = new ArgumentSerializer(sigser, arch, arch.CreateFrame());
		}

        [Test]
        public void SerializeNullArgument()
        {
            Assert.IsNull(argser.Serialize(null));
        }

        [Test]
        public void SerializeRegister()
        {
            Identifier arg = new Identifier(Registers.ax.Name, 0, Registers.ax.DataType, new RegisterStorage(Registers.ax));
            SerializedArgument sarg = argser.Serialize(arg);
            Assert.AreEqual("ax", sarg.Name);
            SerializedRegister sreg = (SerializedRegister) sarg.Kind;
            Assert.IsNotNull(sreg);
            Assert.AreEqual("ax", sreg.Name);
        }

        [Test]
        public void SargSerializeFlag()
        {
            Identifier arg = new Identifier("SZ", 0, PrimitiveType.Byte, new FlagGroupStorage(3, "SZ"));
            SerializedArgument sarg = argser.Serialize(arg);
            Assert.AreEqual("SZ", sarg.Name);
            SerializedFlag sflag = (SerializedFlag) sarg.Kind;
            Assert.AreEqual("SZ", sflag.Name);
        }

		[Test]
		public void DeserializeRegister()
		{
			SerializedRegister reg = new SerializedRegister("eax");
			SerializedArgument arg = new SerializedArgument();
			arg.Name = "eax"; 
			arg.Kind = reg;
			Identifier id = argser.Deserialize(arg);
			Assert.AreEqual("eax", id.Name);
			Assert.AreEqual(32, id.DataType.BitSize);
		}

        [Test]
        public void SerializeOutArgument()
        {
            Identifier id = new Identifier("qOut", 42, PrimitiveType.Word32,
                new OutArgumentStorage(new Identifier("q", 33, PrimitiveType.Word32, new RegisterStorage(new MachineRegister("q", 4, PrimitiveType.Word32)))));
            SerializedArgument arg = argser.Serialize(id);
            Assert.AreEqual("qOut", arg.Name);
            Assert.IsTrue(arg.OutParameter);
            SerializedRegister sr = (SerializedRegister) arg.Kind;
            Assert.AreEqual("q", sr.Name);
        }
	}
}
