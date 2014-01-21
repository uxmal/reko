#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using Decompiler.Arch.X86;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Core.Serialization
{
	[TestFixture]
	public class SerializedArgumentTests
	{
		private IProcessorArchitecture arch;

		public SerializedArgumentTests()
		{
			arch = new IntelArchitecture(ProcessorMode.Real);
		}

		[Test]
		public void SargWriteRegisterArgument()
		{
			SerializedArgument sarg = new SerializedArgument {
			    Name = "foo",
			    Type = new SerializedTypeReference("int"),
			    Kind = new SerializedRegister("eax"),
            };
			Verify(sarg, "Core/SargWriteRegisterArgument.txt");
		}

		[Test]
		public void SargWriteNamelessRegisterArgument()
		{
			SerializedArgument sarg = new SerializedArgument
            {
			    Type = new SerializedTypeReference("int"),
			    Kind = new SerializedRegister("eax"),
            };
			Verify(sarg, "Core/SargWriteNamelessRegisterArgument.txt");
		}

		[Test]
		public void SargWriteStackArgument()
		{
			SerializedArgument sarg = new SerializedArgument
            {
			    Name = "bar",
			    Type = new SerializedTypeReference("int"),
			    Kind = new SerializedStackVariable()
            };
			Verify(sarg, "Core/SargWriteStackArgument.txt");
		}
	
		[Test]
		public void SargWriteOutArgument()
		{
			SerializedArgument sarg = new SerializedArgument
            {
			    Name = "bxOut",
			    Type = new SerializedTypeReference("int"),
			    OutParameter = true,
			    Kind = new SerializedRegister("bx")
            };
			Verify(sarg, "Core/SargWriteOutArgument.txt");
		}

		private void Verify(SerializedArgument sarg, string outputFilename)
		{
			using (FileUnitTester fut = new FileUnitTester(outputFilename))
			{
				XmlTextWriter x = new FilteringXmlWriter(fut.TextWriter);
				x.Formatting = Formatting.Indented;
				XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(sarg.GetType());
				ser.Serialize(x, sarg);

				fut.AssertFilesEqual();
			}
		}
	}

	public class FilteringXmlWriter : XmlTextWriter
	{
		private bool ignore;

		public FilteringXmlWriter(TextWriter writer) : base(writer)
		{
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			if (prefix == "xmlns")
				ignore = true;
			else
				base.WriteStartAttribute(prefix, localName, ns);
		}

		public override void WriteString(string text)
		{
			if (!ignore)
				base.WriteString(text);
		}

		public override void WriteEndAttribute()
		{
			ignore = false;
		}
	}
}
