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
using Decompiler.Core.Code;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using Decompiler.Arch.Intel;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Decompiler.UnitTests.Core.Serialization
{
	[TestFixture]
	public class SerializedSignatureTests
	{
		private IProcessorArchitecture arch;

		public SerializedSignatureTests()
		{
			this.arch = new IntelArchitecture(ProcessorMode.Real);
		}


		[Test]
		public void SsigCreate()
		{
			SerializedSignature ssig = BuildSsigAxBxCl();
			Verify(ssig, "Core/SsigCreate.txt");
		}

		[Test]
		public void SsigReadAxBxCl()
		{
			SerializedSignature ssig;
			using (FileStream stm = new FileStream(FileUnitTester.MapTestPath("Core/AxBxCl.xml"), FileMode.Open))
			{
				XmlTextReader rdr = new XmlTextReader(stm);
				XmlSerializer ser = new XmlSerializer(typeof (SerializedSignature));
				ssig = (SerializedSignature) ser.Deserialize(rdr);
			}
			SignatureSerializer sser = new SignatureSerializer(this.arch, "stdapi");
			Assert.AreEqual("Register word16 AxBxCl(Register word16 bx, Register byte cl)", sser.Deserialize(ssig, new Frame(null)).ToString("AxBxCl"));
		}

		[Test]
		public void SsigBuildSig()
		{
			SerializedSignature ssig = BuildSsigAxBxCl();
			SignatureSerializer sser = new SignatureSerializer(arch, "stdapi");
			ProcedureSignature sig = sser.Deserialize(ssig, new Frame(null));
			Assert.AreEqual("Register word16 AxBxCl(Register word16 bx, Register byte cl)", sig.ToString("AxBxCl"));
			Assert.AreEqual(PrimitiveType.Word16, sig.ReturnValue.DataType);
		}

		[Test]
		public void SsigWriteStdapi()
		{
			SerializedSignature ssig = BuildSsigStack();
			ssig.Convention = "stdapi";
			Verify(ssig, "Core/SsigWriteStdapi.txt");
		}

		[Test]
		public void SsigSerializeAxBxCl()
		{
			SerializedSignature ssig = new SerializedSignature(MkSigAxBxCl());
			Verify(ssig, "Core/SsigSerializeAxBxCl.txt");
		}

		[Test]
		public void SsigSerializeSequence()
		{
			Identifier seq = new Identifier("es_bx", 0, PrimitiveType.Word32, new SequenceStorage(
				new Identifier(Registers.es.Name, 0, Registers.es.DataType, new RegisterStorage(Registers.es)), 
				new Identifier(Registers.bx.Name, 1, Registers.bx.DataType, new RegisterStorage(Registers.bx)))); 
			SerializedSignature ssig = new SerializedSignature(
				new ProcedureSignature(seq, new Identifier[0]));
			Verify(ssig, "Core/SsigSerializeSequence.txt");

		}

		[Test]
		public void DeserializeOutArgument()
		{
			SerializedArgument arg = new SerializedArgument();
			arg.Kind = new SerializedRegister("bp");
			arg.OutParameter = true;
			SerializedSignature sig = new SerializedSignature();
			sig.Arguments = new SerializedArgument[] { arg };
			SignatureSerializer ser = new SignatureSerializer(arch, "stdapi");
			ProcedureSignature ps =  ser.Deserialize(sig, new Frame(null));
			Assert.AreEqual("void foo(Register out ptr0 bpOut)", ps.ToString("foo"));
		}

		private SerializedSignature BuildSsigAxBxCl()
		{
			SerializedSignature ssig = new SerializedSignature();
			
			SerializedArgument sarg = new SerializedArgument();
			sarg.Type = "int";
			sarg.Kind = new SerializedRegister("ax");
			ssig.ReturnValue = sarg;

			ssig.Arguments = new SerializedArgument[2];
			ssig.Arguments[0] = new SerializedArgument();
			ssig.Arguments[0].Kind = new SerializedRegister("bx");

			ssig.Arguments[1] = new SerializedArgument();
			ssig.Arguments[1].Kind = new SerializedRegister("cl");
			return ssig;
		}

		private ProcedureSignature MkSigAxBxCl()
		{
			Identifier ret = new Identifier(Registers.ax.Name, 0, Registers.ax.DataType, new RegisterStorage(Registers.ax));
			Identifier [] args = new Identifier[2];
			args[0] = new Identifier(Registers.bx.Name, 1, Registers.bx.DataType, new RegisterStorage(Registers.bx));
			args[1] = new Identifier(Registers.cl.Name, 2, Registers.cl.DataType, new RegisterStorage(Registers.cl));
			return new ProcedureSignature(ret, args);
		}


		private SerializedSignature BuildSsigStack()
		{
			SerializedSignature ssig = new SerializedSignature();
			
			SerializedArgument sarg = new SerializedArgument();
			sarg.Type = "int";
			sarg.Kind = new SerializedRegister("ax");
			ssig.ReturnValue = sarg;

			ssig.Arguments = new SerializedArgument[2];
			ssig.Arguments[0] = new SerializedArgument();
			ssig.Arguments[0].Name = "lParam";
			ssig.Arguments[0].Kind = new SerializedStackVariable(4);

			ssig.Arguments[1] = new SerializedArgument();
			ssig.Arguments[1].Kind = new SerializedStackVariable(2);
			return ssig;
		}

		private void Verify(SerializedSignature ssig, string outputFilename)
		{
			using (FileUnitTester fut = new FileUnitTester(outputFilename))
			{
				XmlTextWriter x = new FilteringXmlWriter(fut.TextWriter);
				x.Formatting = Formatting.Indented;
				XmlSerializer ser = new XmlSerializer(ssig.GetType());
				ser.Serialize(x, ssig);

				fut.AssertFilesEqual();
			}
		}
	}
}
