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
using Reko.Core;
using Reko.Core.Types;
using Reko.Scanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    public class StorageBinderTests
    {
        [Test]
        public void Stgb_FlagReg()
        {
            var stgb = new StorageBinder();
            var flagReg1 = new RegisterStorage("flagreg1", 41, 0, PrimitiveType.Word32);
            var flagReg2 = new RegisterStorage("flagreg2", 42, 0, PrimitiveType.Word32);
            var grf = stgb.EnsureFlagGroup(flagReg1, 0x05, "CZ", PrimitiveType.Byte);
            var grf2 = stgb.EnsureFlagGroup(flagReg1, 0x05, "CZ", PrimitiveType.Byte);
            Assert.AreSame(grf, grf2);
            var grf3 = stgb.EnsureFlagGroup(flagReg2, 0x05, "cz", PrimitiveType.Byte);
            Assert.AreNotSame(grf, grf3);
        }

        [Test]
        public void Stgb_Sequence()
        {
            var stgb = new StorageBinder();
            var r1 = RegisterStorage.Reg32("r1", 1);
            var r2 = RegisterStorage.Reg32("r2", 2);
            var r3 = RegisterStorage.Reg32("r3", 3);
            var seqA = stgb.EnsureSequence(PrimitiveType.CreateWord(96), r1, r2, r3);
            var seqB = stgb.EnsureSequence(PrimitiveType.CreateWord(96), r1, r2, r3);
            Assert.AreSame(seqA, seqB);
        }
    }
}
