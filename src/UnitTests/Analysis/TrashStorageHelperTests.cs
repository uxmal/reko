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

using Decompiler.Analysis;
using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections;
using System.Text;

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

		[Test]
		public void TrashSequence()
		{
			Identifier es = frame.EnsureRegister(Registers.es);
			Identifier bx = frame.EnsureRegister(Registers.bx);
			Identifier es_bx = frame.EnsureSequence(es, bx, PrimitiveType.Pointer32);
			tsh.Trash(es_bx, "TRASH");
			Assert.AreEqual("(Register bx:TRASH) (Register es:TRASH) (Sequence es:bx:TRASH) ", Dump(tsh.TrashedRegisters));
		}

		private string Dump(Hashtable trash)
		{
			StringBuilder sb = new StringBuilder();
			SortedList sl = new SortedList();
			foreach (DictionaryEntry de in trash)
			{
				sl[de.Key.ToString()] = de.Value.ToString();
			}
			foreach (DictionaryEntry de in sl)
			{
				sb.AppendFormat("({0}:{1}) ", de.Key, de.Value);
			}
			return sb.ToString();
		}
	}
}
