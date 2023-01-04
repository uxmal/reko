#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Arch.MilStd1750;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.MilStd1750
{
    [TestFixture]
    public class MilStd1750ArchitectureTests
    {
        [Test]
        public void MilStd1750Arch_Real32()
        {
            Assert.AreEqual(1.7014116317805963E+38, MilStd1750Architecture.Real32ToIEEE(0x7FFFFF_7F));
            Assert.AreEqual(8.5070591730234616E+37, MilStd1750Architecture.Real32ToIEEE(0x400000_7F));
            Assert.AreEqual(10.0,                   MilStd1750Architecture.Real32ToIEEE(0x500000_04));
            Assert.AreEqual(1.0,                    MilStd1750Architecture.Real32ToIEEE(0x400000_01));
            Assert.AreEqual(0.5,                    MilStd1750Architecture.Real32ToIEEE(0x400000_00));
            Assert.AreEqual(0.25,                   MilStd1750Architecture.Real32ToIEEE(0x400000_FF));
            Assert.AreEqual(1.4693679385278594E-39, MilStd1750Architecture.Real32ToIEEE(0x400000_80));
            Assert.AreEqual(0.0,                    MilStd1750Architecture.Real32ToIEEE(0x000000_00));
            Assert.AreEqual(-1.0,                   MilStd1750Architecture.Real32ToIEEE(0x800000_00));
            Assert.AreEqual(-1.4693682888524755E-39, MilStd1750Architecture.Real32ToIEEE(0xBFFFFF_80));
            Assert.AreEqual(-12.000001907348633,    MilStd1750Architecture.Real32ToIEEE(0x9FFFFF_04));
        }

        [Test]
        public void MilStd1750Arch_Real48()
        {
            Assert.AreEqual(8.5070591730234616E+37, MilStd1750Architecture.Real48ToIEEE(0x400000_7F_0000));
            Assert.AreEqual(1.0,                    MilStd1750Architecture.Real48ToIEEE(0x400000_01_0000)); //0.5 x 2^1      
            Assert.AreEqual(0.5,                    MilStd1750Architecture.Real48ToIEEE(0x400000_00_0000)); //0.5 x 2^0        
            Assert.AreEqual(0.25,                   MilStd1750Architecture.Real48ToIEEE(0x400000_FF_0000)); //0.5 x 2^-1       
            Assert.AreEqual(1.4693679385278594E-39, MilStd1750Architecture.Real48ToIEEE(0x400000_80_0000)); //0.5 x 2^-128     
            Assert.AreEqual(-1.7014118346046923E+38, MilStd1750Architecture.Real48ToIEEE(0x800000_7F_0000)); //- 1.0 x 2^127    
            Assert.AreEqual(-1.0,                   MilStd1750Architecture.Real48ToIEEE(0x800000_00_0000)); //- 1.0 x 2^0      
            Assert.AreEqual(-0.5,                   MilStd1750Architecture.Real48ToIEEE(0x800000_FF_0000)); //- 1.0 x 2^-1     
            Assert.AreEqual(-2.9387358770557188E-39, MilStd1750Architecture.Real48ToIEEE(0x800000_80_0000)); //- 1.0 x 2^-128   
            Assert.AreEqual(0.0,                    MilStd1750Architecture.Real48ToIEEE(0x000000_00_0000)); //0.0 x 2^0        
            Assert.AreEqual(-0.375,                 MilStd1750Architecture.Real48ToIEEE(0xA00000_FF_0000)); //- 0.75 x 2^-1    
        }
    }
}
