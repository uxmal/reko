#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Expressions;
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
			sigser = new X86ProcedureSerializer(arch, new TypeLibraryLoader(arch, true), "stdapi");
            argser = new ArgumentSerializer(sigser, arch, arch.CreateFrame(), null);
		}

        [Test]
        public void SerializeNullArgument()
        {
            Assert.IsNull(argser.Serialize(null));
        }

        [Test]
        public void SerializeRegister()
        {
            var arg = new Identifier(Registers.ax.Name, Registers.ax.DataType, Registers.ax);
            Argument_v1 sarg = argser.Serialize(arg);
            Assert.AreEqual("ax", sarg.Name);
            Register_v1 sreg = (Register_v1) sarg.Kind;
            Assert.IsNotNull(sreg);
            Assert.AreEqual("ax", sreg.Name);
        }

        [Test]
        public void SargSerializeFlag()
        {
            var arg = new Identifier("SZ", PrimitiveType.Byte, new FlagGroupStorage(3, "SZ", PrimitiveType.Byte));
            Argument_v1 sarg = argser.Serialize(arg);
            Assert.AreEqual("SZ", sarg.Name);
            FlagGroup_v1 sflag = (FlagGroup_v1) sarg.Kind;
            Assert.AreEqual("SZ", sflag.Name);
        }

		[Test]
		public void ArgSer_DeserializeRegister()
		{
			Register_v1 reg = new Register_v1("eax");
            Argument_v1 arg = new Argument_v1
            {
                Name = "eax",
                Kind = reg,
            };
            Identifier id = argser.Deserialize(arg);
			Assert.AreEqual("eax", id.Name);
			Assert.AreEqual(32, id.DataType.BitSize);
		}

        [Test]
        public void ArgSer_SerializeOutArgument()
        {
            Identifier id = new Identifier("qOut",  PrimitiveType.Word32,
                new OutArgumentStorage(new Identifier("q", PrimitiveType.Word32, new RegisterStorage("q", 4, PrimitiveType.Word32))));
            Argument_v1 arg = argser.Serialize(id);
            Assert.AreEqual("qOut", arg.Name);
            Assert.IsTrue(arg.OutParameter);
            Register_v1 sr = (Register_v1) arg.Kind;
            Assert.AreEqual("q", sr.Name);
        }

        [Test]
        public void ArgSet_DerserializeReturnRegisterWithType()
        {
            var arg = new Argument_v1
            {
                Kind = new Register_v1("eax"),
                Type = new PointerType_v1 { DataType = new PrimitiveType_v1 { ByteSize = 1, Domain = Domain.Character } }
            };
            var id = argser.DeserializeReturnValue(arg);
            Assert.AreEqual("(ptr char)", id.DataType.ToString());
        }
	}
}
