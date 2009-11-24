/* 
 * Copyright (C) 1999-2009 John Källén.
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
using Decompiler.Core;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class DeclarationInserterTests
	{
		private Procedure proc;
		private DominatorGraph doms;
		private SsaIdentifierCollection ssaIds;

		[Test]
		public void DeciWeb()
		{
			Build(new DiamondMock().Procedure);
			DeclarationInserter deci = new DeclarationInserter(ssaIds, doms);
			Web web = new Web();
			SsaIdentifier r_4 = ssaIds[4];
			SsaIdentifier r_5 = ssaIds[5];
			SsaIdentifier r_6 = ssaIds[6];
			web.Add(r_4);
			web.Add(r_5);
			web.Add(r_6);
			deci.InsertDeclaration(web);
			Assert.AreEqual("word32 r_4", proc.RpoBlocks[1].Statements[0].Instruction.ToString());
		}

		private void Build(Procedure proc)
		{
			this.proc = proc;
			this.doms = new DominatorGraph(proc);
			SsaTransform sst = new SsaTransform(proc, doms, false);
			
			this.ssaIds = sst.SsaState.Identifiers;
		}
	}
}
