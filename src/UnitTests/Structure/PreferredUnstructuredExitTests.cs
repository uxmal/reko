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

using Decompiler.Core;
using Decompiler.Structure;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Structure
{
	[TestFixture]
	public class PreferredUnstructuredExitTests
	{
		private Procedure proc;
		private Block hdr;
		private Block exit;
		private Block procExit;
		private Linearizer lin;

		[Test]
		public void LinPreferLoopExit()
		{
			Build();
			Block p = lin.PreferredUnstructuredExit(hdr, exit);
			Assert.AreEqual("exit", p.Name);
		}

		[Test]
		public void LinPreferReturn()
		{
			Build();
			Block p = lin.PreferredUnstructuredExit(hdr, procExit);
			Assert.AreEqual("procExit", p.Name);
		}

		private void Build()
		{
			proc = Procedure.Create(null, new Address(0x0100), null);
			hdr = new Block(proc, "hdr");
			proc.RpoBlocks.Add(hdr);
			exit = new Block(proc, "exit");
			proc.RpoBlocks.Add(exit);
			procExit = new Block(proc, "procExit");
			proc.RpoBlocks.Add(procExit);

			lin = new Linearizer(proc, new BlockLinearizer(exit));
			lin.LoopHeader = hdr;
			lin.LoopFollow = exit;
			lin.ProcedureExit = procExit;
		}
	}
}
