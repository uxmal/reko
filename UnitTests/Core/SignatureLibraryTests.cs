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
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class SignatureLibraryTests
	{
		private void EmitSignature(TypeLibrary lib, string fnName, System.IO.TextWriter tw)
		{
			lib.Lookup(fnName).Emit(fnName, ProcedureSignature.EmitFlags.ArgumentKind|ProcedureSignature.EmitFlags.LowLevelInfo, tw);
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
                        DataType=new PrimitiveType_v1 { Domain = Decompiler.Core.Types.Domain.SignedInt, ByteSize = 4 }
                    }
                }
            };
            var lib = TypeLibrary.Load(new IntelArchitecture(ProcessorMode.Protected32), slib);
            Assert.AreEqual(PrimitiveType.Int32, lib.LookupType("int"));
        }
	}
}
