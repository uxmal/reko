#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Serialization;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Decompiler.UnitTests.Core.Serialization
{
	[TestFixture]
	public class SerializedLibraryTests
	{
		[Test]
		public void SlibWriteOneProcedure()
		{
			SerializedLibrary slib = new SerializedLibrary();

			slib.Procedures.Add(MkMalloc());

			Verify(slib, "Core/SlibWriteOneProcedure.txt");
		}

		[Test]
		public void SlibReadOneProcedure()
		{
			XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(typeof (SerializedLibrary));
			SerializedLibrary lib;
			using (FileStream stm = new FileStream(FileUnitTester.MapTestPath("Core/SlibOneProcedure.xml"), FileMode.Open))
			{
				lib = (SerializedLibrary) ser.Deserialize(stm);
			}
			Assert.AreEqual(1, lib.Procedures.Count);
			Procedure_v1 proc = (Procedure_v1) lib.Procedures[0];
			Assert.AreEqual("malloc", proc.Name);
			Assert.AreEqual(1, proc.Signature.Arguments.Length);
			Assert.AreEqual("int", proc.Signature.Arguments[0].Type.ToString());
		}

		[Test]
		public void SlibReadMsvcrtXml()
		{
            XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(typeof(SerializedLibrary));
			SerializedLibrary lib;
			using (FileStream stm = new FileStream(FileUnitTester.MapTestPath("../Environments/Win32/msvcrt.xml"), FileMode.Open))
			{
				lib = (SerializedLibrary) ser.Deserialize(stm);
			}
			Assert.AreEqual(18, lib.Procedures.Count);
		}

		[Test]
		public void SlibReadRealModeIntServices()
		{
            XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(typeof(SerializedLibrary));
			SerializedLibrary lib;
			using (FileStream stm = new FileStream(FileUnitTester.MapTestPath("../Environments/Msdos/realmodeintservices.xml"), FileMode.Open))
			{
				lib = (SerializedLibrary) ser.Deserialize(stm);
			}
			Assert.AreEqual(88, lib.Procedures.Count);
		}

        [Test]
        public void SlibLoadVoidFn()
        {

        }
		private Procedure_v1 MkMalloc()
		{
			Procedure_v1 proc = new Procedure_v1();
			proc.Name = "malloc";
			proc.Signature = new SerializedSignature();
			proc.Signature.Convention = "cdecl";
			proc.Signature.ReturnValue = new Argument_v1();
			proc.Signature.ReturnValue.Kind = new Register_v1("eax");

			proc.Signature.Arguments = new Argument_v1[1];
			proc.Signature.Arguments[0] = new Argument_v1();
			proc.Signature.Arguments[0].Name = "cb";
			proc.Signature.Arguments[0].Kind = new StackVariable_v1();

			return proc;
		}

		private void Verify(SerializedLibrary slib, string outputFile)
		{
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				XmlTextWriter writer = new FilteringXmlWriter(fut.TextWriter);
				writer.Formatting = Formatting.Indented;
                XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(slib.GetType());
				ser.Serialize(writer, slib);

				fut.AssertFilesEqual();
			}
		}
	}
}
