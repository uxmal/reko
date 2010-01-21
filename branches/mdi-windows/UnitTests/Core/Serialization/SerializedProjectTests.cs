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

using Decompiler.Core.Serialization;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Decompiler.UnitTests.Core.Serialization
{
	[TestFixture]
	public class SerializedProjectTests
	{
		[Test]
		public void SudWrite()
		{
			DecompilerProject ud = new DecompilerProject();
			ud.Input.Address = "0x1000:0x0";
			ud.Output.DisassemblyFilename = "foo.asm";
			ud.Output.IntermediateFilename = "foo.cod";
			SerializedProcedure proc = new SerializedProcedure();
			proc.Name = "foo";
			proc.Signature = new SerializedSignature();
			proc.Signature.ReturnValue = new SerializedArgument();
			proc.Signature.ReturnValue.Kind = new SerializedRegister("eax");
			proc.Signature.Arguments = new SerializedArgument[2];
			proc.Signature.Arguments[0] = new SerializedArgument();
			proc.Signature.Arguments[0].Kind = new SerializedStackVariable(4);
			proc.Signature.Arguments[1] = new SerializedArgument();
			proc.Signature.Arguments[1].Kind = new SerializedStackVariable(4);
			ud.UserProcedures.Add(proc);

			using (FileUnitTester fut = new FileUnitTester("Core/SudWrite.txt"))
			{
				FilteringXmlWriter writer = new FilteringXmlWriter(fut.TextWriter);
				writer.Formatting = System.Xml.Formatting.Indented;
				XmlSerializer ser = new XmlSerializer(typeof (DecompilerProject));
				ser.Serialize(writer, ud);
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void SudRead()
		{
			DecompilerProject proj = null;
			using (FileStream stm = new FileStream(FileUnitTester.MapTestPath("Core/SudRead.xml"), FileMode.Open))
			{
				XmlSerializer ser = new XmlSerializer(typeof (DecompilerProject));
				proj = (DecompilerProject) ser.Deserialize(stm);
			}
			Assert.AreEqual("10003330", proj.Input.Address);
			Assert.AreEqual(2, proj.UserProcedures.Count);
			SerializedProcedure proc = (SerializedProcedure) proj.UserProcedures[0];
			Assert.IsNull(proc.Signature.ReturnValue);
		}

		[Test]
		public void SudReadAlloca()
		{
			DecompilerProject proj = DecompilerProject.Load(FileUnitTester.MapTestPath("Fragments/multiple/alloca.xml"));
			SerializedProcedure proc = (SerializedProcedure) proj.UserProcedures[0];
			Assert.AreEqual("alloca", proc.Name);
			Assert.IsTrue(proc.Characteristics.IsAlloca);
		}


	}
}
