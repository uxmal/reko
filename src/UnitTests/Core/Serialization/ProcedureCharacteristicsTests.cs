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

using Reko.Core;
using Reko.Core.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Reko.UnitTests.Core.Serialization
{
    [TestFixture]
    public class ProcedureCharacteristicsTests
    {
        [Test]
        public void ProcChar_DeserializeIsMalloc()
        {
            string x = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<ProcedureCharacteristics>" +
                "   <allocator>1</allocator>" +
                "</ProcedureCharacteristics>";
            ProcedureCharacteristics pc = DeserializeXml(x);
            Assert.IsTrue(pc.Allocator);
        }

        private static ProcedureCharacteristics DeserializeXml(string x)
        {
            XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(typeof(ProcedureCharacteristics));
            ProcedureCharacteristics pc = (ProcedureCharacteristics) ser.Deserialize(new StringReader(x));
            return pc;
        }

        [Test]
        public void ProcChar_DeserializeArraySize()
        {
            string x = "<?xml version=\"1.0\" encoding=\"utf-16\"?>"+ 
                "<ProcedureCharacteristics>"+ 
                "  <array-size argument=\"0\">" +
                "    <factor argument=\"1\"/>" +
                "    <factor constant=\"30\"/>" +
                "  </array-size>" +
                "</ProcedureCharacteristics>";
            XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(typeof(ProcedureCharacteristics));
            ProcedureCharacteristics pc = (ProcedureCharacteristics) ser.Deserialize(new StringReader(x));
            ArraySizeCharacteristic arrs = pc.ArraySize;
            Assert.IsNotNull(arrs, "Should have deserialized an array size.");
            Assert.IsNotNull(arrs.Argument, "Array size is useless without an argument to refer to.");
            Assert.AreEqual(2, arrs.Factors.Length);
            Assert.AreEqual("1", arrs.Factors[0].Argument);
            Assert.AreEqual("30", arrs.Factors[1].Constant);
        }
    }
}
