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

using Decompiler.Arch.PowerPC;
using Decompiler.Core;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Arch.PowerPC
{
    [TestFixture]  
    public class PowerPcArchitectureTests
    {
        [Test]
        public void Create()
        {
            IProcessorArchitecture arch = new PowerPcArchitecture(PrimitiveType.Word32);
        }

        [Test]
        public void InvalidWordSize()
        {
            try
            {
                new PowerPcArchitecture(PrimitiveType.Word16);
                Assert.Fail("There is no 16-bit powerPC architecture.");
            }
            catch (ArgumentException)
            {
            }
        }
    }
}
