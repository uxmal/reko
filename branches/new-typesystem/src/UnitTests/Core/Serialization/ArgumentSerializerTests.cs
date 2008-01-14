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

using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Code;
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
		private SignatureSerializer sigser;
		private ArgumentSerializer argser;

		[SetUp]
		public void Setup()
		{
			arch = new IntelArchitecture(ProcessorMode.Real);
			sigser = new SignatureSerializer(arch, "stdapi");
			argser = new ArgumentSerializer(sigser, arch, new Frame(PrimitiveType.Word32));
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
	}
}
