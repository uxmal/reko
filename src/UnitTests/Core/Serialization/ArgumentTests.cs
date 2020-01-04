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
using Reko.Core.Code;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Arch.X86;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Core.Serialization
{
	[TestFixture]
	public class ArgumentTests
	{
		private IProcessorArchitecture arch;

		public ArgumentTests()
		{
			arch = new X86ArchitectureReal("x86-real-16");
		}

		[Test]
		public void SargWriteRegisterArgument()
		{
			Argument_v1 sarg = new Argument_v1 {
			    Name = "foo",
			    Type = new TypeReference_v1("int"),
			    Kind = new Register_v1("eax"),
            };
			Verify(sarg, "Core/SargWriteRegisterArgument.txt");
		}

		[Test]
		public void SargWriteNamelessRegisterArgument()
		{
			Argument_v1 sarg = new Argument_v1
            {
			    Type = new TypeReference_v1("int"),
			    Kind = new Register_v1("eax"),
            };
			Verify(sarg, "Core/SargWriteNamelessRegisterArgument.txt");
		}

		[Test]
		public void SargWriteStackArgument()
		{
			Argument_v1 sarg = new Argument_v1
            {
			    Name = "bar",
			    Type = new TypeReference_v1("int"),
			    Kind = new StackVariable_v1()
            };
			Verify(sarg, "Core/SargWriteStackArgument.txt");
		}
	
		[Test]
		public void SargWriteOutArgument()
		{
			Argument_v1 sarg = new Argument_v1
            {
			    Name = "bxOut",
			    Type = new TypeReference_v1("int"),
			    OutParameter = true,
			    Kind = new Register_v1("bx")
            };
			Verify(sarg, "Core/SargWriteOutArgument.txt");
		}

		private void Verify(Argument_v1 sarg, string outputFilename)
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
            if (!ignore)
            {
                base.WriteEndAttribute();
            }
            ignore = false;
		}
	}
}
