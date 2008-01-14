/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Decompiler.UnitTests.Core.Serialization
{
	[TestFixture]
	public class SerializedTypeTests
	{
		[Test]
		public void StCreate()
		{
			SerializedType st = new SerializedPrimitiveType(Domain.SignedInt, 4);
			Assert.AreEqual("prim(SignedInt,4)", st.ToString());
			st = new SerializedPointerType(new SerializedPrimitiveType(Domain.UnsignedInt, 4));
			Assert.AreEqual("ptr(prim(UnsignedInt,4))", st.ToString());
		}

		[Test]
		public void StWriteStruct()
		{
			SerializedStructType str = new SerializedStructType();
			str.Fields.Add(new SerializedStructField(0, null, new SerializedPrimitiveType(Domain.UnsignedInt, 4)));
			str.Fields.Add(new SerializedStructField(4, null, new SerializedPrimitiveType(Domain.Real, 8)));
			Assert.AreEqual("struct(0, (0, ?, prim(UnsignedInt,4)), (4, ?, prim(Real,8)))", str.ToString());
		}

		[Test]
		public void StReadStruct()
		{
			SerializedStructType str = new SerializedStructType();
			str.ByteSize = 32;
			str.Fields.Add(new SerializedStructField(0, null, new SerializedPrimitiveType(Domain.SignedInt, 4)));
			str.Fields.Add(new SerializedStructField(4, null, new SerializedPrimitiveType(Domain.Real, 8)));
			StringWriter writer = new StringWriter();
			XmlTextWriter x = new XmlTextWriter(writer);
			x.Formatting = Formatting.None;
			XmlSerializer ser = new XmlSerializer(str.GetType());
			ser.Serialize(x, str);

			string s = writer.ToString();
			Console.WriteLine(s);
			int b = s.IndexOf("<field");
			int e = s.IndexOf("</Ser");
			Assert.AreEqual("<field offset=\"0\"><prim domain=\"SignedInt\" size=\"4\" /></field><field offset=\"4\"><prim domain=\"Real\" size=\"8\" /></field>", s.Substring(b, e-b));
			StringReader rdr = new StringReader(s);
			XmlSerializer deser = new XmlSerializer(typeof (SerializedStructType));
			SerializedStructType destr = (SerializedStructType) deser.Deserialize(rdr);
		}

		[Test]
		public void StWritePrimitive()
		{
			SerializedType st = new SerializedPointerType(new SerializedPrimitiveType(Domain.SignedInt, 4));
			StringWriter writer = new StringWriter();
			XmlSerializer ser = new XmlSerializer(st.GetType());
			ser.Serialize(writer, st);
			string s = writer.ToString();
			int b = s.IndexOf("<prim");
			int e = s.IndexOf("/>");
			Assert.AreEqual("<prim domain=\"SignedInt\" size=\"4\" />", s.Substring(b, 36));
		}
	}
}
