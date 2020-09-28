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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class TestConditionTests
    {
        private Identifier id = new Identifier("id", PrimitiveType.Word32, 
            new TemporaryStorage("id", 0, PrimitiveType.Word32));

        private string InvertTest(ConditionCode cc)
        {
            return new TestCondition(cc, id).Invert().ToString();
        }

        [Test]
        public void TC_Invert()
        {
            Assert.AreEqual("Test(ULE,id)", InvertTest(ConditionCode.UGT));
            Assert.AreEqual("Test(UGE,id)", InvertTest(ConditionCode.ULT));	
            Assert.AreEqual("Test(LE,id)", InvertTest(ConditionCode.GT));
            Assert.AreEqual("Test(LT,id)", InvertTest(ConditionCode.GE));	
            Assert.AreEqual("Test(GE,id)", InvertTest(ConditionCode.LT));		
            Assert.AreEqual("Test(GT,id)", InvertTest(ConditionCode.LE));		
            Assert.AreEqual("Test(ULT,id)", InvertTest(ConditionCode.UGE));	
            Assert.AreEqual("Test(OV,id)", InvertTest(ConditionCode.NO));		
            Assert.AreEqual("Test(SG,id)", InvertTest(ConditionCode.NS));		
            Assert.AreEqual("Test(EQ,id)", InvertTest(ConditionCode.NE));		
            Assert.AreEqual("Test(NO,id)", InvertTest(ConditionCode.OV));		
            Assert.AreEqual("Test(NS,id)", InvertTest(ConditionCode.SG));		
            Assert.AreEqual("Test(NE,id)", InvertTest(ConditionCode.EQ));		
            Assert.AreEqual("Test(PO,id)", InvertTest(ConditionCode.PE));    
            Assert.AreEqual("Test(PE,id)", InvertTest(ConditionCode.PO));    
        }
    }      
}          
           
           
           
           
           
           
           
           
           
           
           