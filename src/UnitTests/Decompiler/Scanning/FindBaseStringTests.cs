#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Output;
using Reko.Scanning;
using System;
using System.Text;
using System.Threading;

namespace Reko.UnitTests.Decompiler.Scanning
{
    [TestFixture]
    internal class FindBaseStringTests : AbstractBaseFinderTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
        }


        // Note this test is very slow
        [Test]
        public void Fbs_FindStrings_32()
        {
            var str = "This is longer than 10 bytes";
            
            Given_String(0x100, str);
            Given_String(0x125, str);
            Given_String(0x15A, str);
            Given_String(0x17A, str);

            Given_Pointer(0x300, 0xFF0100);
            Given_Pointer(0x308, 0xFF0125);
            Given_Pointer(0x312, 0xFF015A);
            Given_Pointer(0x240, 0xFF017A);

            var fbs = new FindBaseString(arch.Object, mem, NullProgressIndicator.Instance );
            fbs.MinAddress = 0xF0_0000;
            var results = fbs.Run(new CancellationToken());
            Assert.AreEqual(0xFF0000, results[0].Address);
        }
    }
}
