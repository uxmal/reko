/* 
 * Copyright (C) 1999-2007 John Källén.
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
using Decompiler.Core.Serialization;
using Decompiler.Scanning;
using NUnit.Framework;
using System;
using System.Collections;


namespace Decompiler.UnitTests.Scanning
{
	[TestFixture]
	public class RewriterHostTests
	{
		[Test]
		public void LoadCallSignatures()
		{
			IntelArchitecture arch = new IntelArchitecture(ProcessorMode.Real);
			Program prog = new Program();
			prog.Architecture = arch;
			RewriterHost host = new RewriterHost(prog, null, null, null, null);
			ArrayList al = new ArrayList();
			SerializedSignature sig = new SerializedSignature();
			sig.Arguments = new SerializedArgument[2];
			sig.Arguments[0] = new SerializedArgument();
			sig.Arguments[0].Kind = new SerializedRegister("ds");
			sig.Arguments[1] = new SerializedArgument();
			sig.Arguments[1].Kind = new SerializedRegister("bx");
			al.Add(new SerializedCall(new Address(0x0C32, 0x3200), sig));
			host.LoadCallSignatures(al);

			ProcedureSignature ps = host.GetCallSignatureAtAddress(new Address(0x0C32, 0x3200));
			Assert.IsNotNull(ps, "Expected a call signature for address");
		}
	}
}
