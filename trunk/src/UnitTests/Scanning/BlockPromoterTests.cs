#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using Decompiler.Scanning;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.UnitTests.Scanning
{
    [TestFixture]   
    public class BlockPromoterTests
    {
        [Test]
        public void PromoteSimpleBlock()
        {
            ProcedureBuilder m;
            Block blockToPromote;
            BuildSimpleProc(out m, out blockToPromote);

            Procedure promotedProcedure = Procedure.Create(new Address(0x123450), new Frame(PrimitiveType.Word32));
            var blockPromoter = new BlockPromoter(blockToPromote, promotedProcedure, new ArchitectureMock());
            blockPromoter.Promote();

            var sw = new StringWriter();
            promotedProcedure.WriteGraph(sw);
            sw.WriteLine();
            m.Procedure.WriteGraph(sw);
            Console.WriteLine(sw);

            var sExp = @"fn00123450_entry
    Pred:
    Succ: blockToPromote
blockToPromote
    Pred: fn00123450_entry
    Succ: fn00123450_exit
fn00123450_exit
    Pred: blockToPromote
    Succ:

fn00123440_entry
    Pred:
    Succ: l1
blockToPromote_tmp
    Pred: l1
    Succ: fn00123440_exit
l1
    Pred: fn00123440_entry
    Succ: blockToPromote_tmp
fn00123440_exit
    Pred: blockToPromote_tmp
    Succ:
";
            Assert.AreEqual(sExp, sw.ToString());

        }

        private static void BuildSimpleProc(out ProcedureBuilder m, out Block blockToPromote)
        {
            m = new ProcedureBuilder("fn00123440");
            var r0 = m.Frame.EnsureRegister(new MachineRegister("r0", 0, PrimitiveType.Word32));
            m.Assign(r0, 1);
            blockToPromote = m.Label("blockToPromote");
            m.Store(m.Word32(0x123456), r0);
            m.Return();
        }
    }
}
