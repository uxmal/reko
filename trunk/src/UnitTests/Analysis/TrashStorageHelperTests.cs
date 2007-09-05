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

using Decompiler.Analysis;
using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class TrashStorageHelperTests
	{
		private Frame frame;
		private TrashStorageHelper tsh;

		[SetUp]
		public void Setup()
		{
			frame = new Frame(PrimitiveType.Word32);
			tsh = new TrashStorageHelper();
		}

		[Test]
		public void TrashIdentifier()
		{
			Identifier eax = frame.EnsureRegister(Registers.eax);
			tsh.Trash(eax, "TRASH");
			Assert.AreEqual(1, tsh.TrashedRegisters.Count);
			Assert.AreEqual("TRASH", tsh.TrashedRegisters[eax.Storage]);
		}
		
		[Test]
		public void CopyIdentifier()
		{
			Identifier eax = frame.EnsureRegister(Registers.eax);
			Identifier ebx = frame.EnsureRegister(Registers.ebx);
			tsh.Copy(eax, ebx);
			Assert.AreEqual(1, tsh.TrashedRegisters.Count);
			Assert.AreEqual("ebx", ((RegisterStorage) tsh.TrashedRegisters[eax.Storage]).Register.Name);
		}
	}
}
