/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Code;
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
	public class SerializedServiceTests
	{
		private IProcessorArchitecture arch = new IntelArchitecture(ProcessorMode.Real);
		private SerializedService svc;

		[SetUp]
		public void Setup()
		{
			svc = new SerializedService();
			svc.Name = "msdos_ioctl_get_device_info";
			svc.SyscallInfo = new SerializedSyscallInfo();
			svc.SyscallInfo.Vector = "21";
			svc.SyscallInfo.RegisterValues = new SerializedRegValue[2];
			svc.SyscallInfo.RegisterValues[0] = new SerializedRegValue("ah", "44");
			svc.SyscallInfo.RegisterValues[1] = new SerializedRegValue("al", "00");
			svc.Signature = new SerializedSignature();
            ArgumentSerializer argSer = new ArgumentSerializer(null, arch, null);
            svc.Signature.ReturnValue = argSer.Serialize(new Identifier("C", 0, PrimitiveType.Bool, new FlagGroupStorage((uint) FlagM.CF, "C")));
		}

		[Test]
		public void SserSerialize()
		{
			StringWriter sw = new StringWriter();
			XmlTextWriter xml = new XmlTextWriter(sw);
			XmlSerializer ser = new XmlSerializer(svc.GetType());
			ser.Serialize(xml, svc);
			string s = sw.ToString();
			int i = s.IndexOf("<syscallin");
			s = s.Substring(i, s.IndexOf("</syscalli") - i);
			Assert.AreEqual("<syscallinfo><vector>21</vector><regvalue reg=\"ah\">44</regvalue><regvalue reg=\"al\">00</regvalue>", s);
		}

		[Test]
		public void SserBuild()
		{
			SystemService ssvc = svc.Build(arch);
			
			Assert.AreEqual(svc.Name, ssvc.Name);
			Assert.AreEqual(0x21, ssvc.SyscallInfo.Vector);
			Assert.AreSame(Registers.ah, ssvc.SyscallInfo.RegisterValues[0].Register);
			Assert.AreEqual(0x44, ssvc.SyscallInfo.RegisterValues[0].Value);
			Assert.AreSame(Registers.al, ssvc.SyscallInfo.RegisterValues[1].Register);
			Assert.AreEqual(0, ssvc.SyscallInfo.RegisterValues[1].Value);
		}
	}
}
