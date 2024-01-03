#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Arch.SuperH;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.SuperH
{
    [TestFixture]
    public class SuperHArchitectureTests
    {
        private SuperHArchitecture arch = new (new ServiceContainer(), "superH", new ()
        {
            { ProcessorOption.Endianness, "be" }
        });
        
        [Test]
        public void ShArch_GetRegister_Real32()
        {
            var f1 = arch.GetRegister("fr1");
            var f2 = arch.GetRegister(f1.Domain, f1.GetBitRange());
            Assert.AreEqual(f1, f2);
        }

        [Test]
        public void ShArch_GetRegister_Real364()
        {
            var d1 = arch.GetRegister("dr2");
            var d2 = arch.GetRegister(d1.Domain, d1.GetBitRange());
            Assert.AreEqual(d1, d2);
        }
    }

}
