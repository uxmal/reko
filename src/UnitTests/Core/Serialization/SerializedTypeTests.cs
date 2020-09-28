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
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Reko.UnitTests.Core.Serialization
{
	[TestFixture]
	public class SerializedTypeTests
	{
		[Test]
		public void StCreate()
		{
			SerializedType st = new PrimitiveType_v1(Domain.SignedInt, 4);
			Assert.AreEqual("prim(SignedInt,4)", st.ToString());
			st = new PointerType_v1(new PrimitiveType_v1(Domain.UnsignedInt, 4));
			Assert.AreEqual("ptr(prim(UnsignedInt,4))", st.ToString());
		}

		[Test]
		public void StWriteStruct()
		{
            StructType_v1 str = new StructType_v1
            {
                Fields = new StructField_v1[] {
                    new StructField_v1(0, null, new PrimitiveType_v1(Domain.UnsignedInt, 4)),
			        new StructField_v1(4, null, new PrimitiveType_v1(Domain.Real, 8)),
                }
            };
			Assert.AreEqual("struct((0, ?, prim(UnsignedInt,4))(4, ?, prim(Real,8)))", str.ToString());
		}

		[Test]
		public void StReadStruct()
		{
			StructType_v1 str = new StructType_v1
            {
			    ByteSize = 32,
			    Fields = new StructField_v1 []
                {
                    new StructField_v1(0, null, new PrimitiveType_v1(Domain.SignedInt, 4)),
			        new StructField_v1(4, null, new PrimitiveType_v1(Domain.Real, 8)),
                }
            };
			StringWriter writer = new StringWriter();
			XmlTextWriter x = new XmlTextWriter(writer);
			x.Formatting = Formatting.None;
            XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(str.GetType());
			ser.Serialize(x, str);

			string s = writer.ToString();
			int b = s.IndexOf("<field");
			int e = s.IndexOf("</Struc");
			Assert.AreEqual(
                "<field offset=\"0\"><prim domain=\"SignedInt\" size=\"4\" xmlns=\"http://schemata.jklnet.org/Decompiler\" /></field>" +
                "<field offset=\"4\"><prim domain=\"Real\" size=\"8\" xmlns=\"http://schemata.jklnet.org/Decompiler\" /></field>",
                s.Substring(b, e-b));
			StringReader rdr = new StringReader(s);
            XmlSerializer deser = SerializedLibrary.CreateSerializer_v1(typeof(StructType_v1));
			StructType_v1 destr = (StructType_v1) deser.Deserialize(rdr);
		}

		[Test]
		public void StWritePrimitive()
		{
			SerializedType st = new PointerType_v1(new PrimitiveType_v1(Domain.SignedInt, 4));
			StringWriter writer = new StringWriter();
			XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(st.GetType());
			ser.Serialize(writer, st);
			string s = writer.ToString();
			int b = s.IndexOf("<prim");
			int e = s.IndexOf("/>");
            Assert.AreEqual("<prim domain=\"SignedInt\" size=\"4\" xmlns=\"http://schemata.jklnet.org/Decompiler\" ", s.Substring(b, e-b));
		}
	}
}
