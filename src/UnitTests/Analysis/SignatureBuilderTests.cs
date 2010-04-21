/* 
 * Copyright (C) 1999-2010 John Källén.
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
using Decompiler.Core.Types;
using Decompiler.Analysis;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class SignatureBuilderTests
	{
		[Test]
		public void TestGrfWithOneBit()
		{
			IProcessorArchitecture arch = new IntelArchitecture(ProcessorMode.Real);
			SignatureBuilder sb = new SignatureBuilder(null, arch);
            sb.AddFlagGroupReturnValue((uint) FlagM.CF, arch.CreateFrame());
			ProcedureSignature sig = sb.BuildSignature();
			Assert.AreEqual("bool", sig.ReturnValue.DataType.ToString());
		}
	}
}
