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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Types;
using Reko.Analysis;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Reko.UnitTests.Analysis
{
	[TestFixture]
	public class SignatureBuilderTests
	{
		[Test]
		public void TestGrfWithOneBit()
		{
			IProcessorArchitecture arch = new X86ArchitectureReal("x86-real-16");
			SignatureBuilder sb = new SignatureBuilder(null, arch);
            sb.AddFlagGroupReturnValue(
                new KeyValuePair<RegisterStorage, uint>(Registers.eflags, (uint) FlagM.CF),
                arch.CreateFrame());
			FunctionType sig = sb.BuildSignature();
			Assert.AreEqual("bool", sig.ReturnValue.DataType.ToString());
		}
	}
}
