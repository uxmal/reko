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
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Environments.SysV;
using System;
using System.Collections.Generic;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class SignatureLibraryTests
	{
		private void EmitSignature(TypeLibrary lib, string fnName, System.IO.TextWriter tw)
		{
			lib.Lookup(fnName).Emit(fnName, FunctionType.EmitFlags.ArgumentKind|FunctionType.EmitFlags.LowLevelInfo, tw);
		}

        [Test]
        public void SlLookupType()
        {
            var slib = new SerializedLibrary
            {
                Types = new SerializedType[]
                {
                    new SerializedTypedef { 
                        Name="int", 
                        DataType=new PrimitiveType_v1 { Domain = Reko.Core.Types.Domain.SignedInt, ByteSize = 4 }
                    }
                }
            };
            var arch = new X86ArchitectureFlat32("x86-protected-32");
            var platform = new SysVPlatform(null, arch);
            var tldser = new TypeLibraryDeserializer(platform, true, new TypeLibrary());
            var lib = tldser.Load(slib);
            Assert.AreEqual(PrimitiveType.Int32, lib.LookupType("int"));
        }
	}
}
