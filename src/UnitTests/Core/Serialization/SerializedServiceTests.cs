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

using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Environments.Msdos;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Reko.UnitTests.Core.Serialization
{
	[TestFixture]
	public class SerializedServiceTests
	{
		private IProcessorArchitecture arch;
		private SerializedService svc;
        private MsdosPlatform platform;
        private ServiceContainer sc;

        [SetUp]
		public void Setup()
		{
            this.sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
            this.arch = new X86ArchitectureReal("x86-real-16");
            this.platform = new MsdosPlatform(sc, arch);

            ArgumentSerializer argSer = new ArgumentSerializer(arch);

            svc = new SerializedService
            {
                Name = "msdos_ioctl_get_device_info",
                SyscallInfo = new SyscallInfo_v1
                {
                    Vector = "21",
                    RegisterValues = new[] {
                        new SerializedRegValue("ah", "44"),
                        new SerializedRegValue("al", "00"),
                    }
                },
                Signature = new SerializedSignature
                {
                    ReturnValue = argSer.Serialize(
                        new Identifier("C", PrimitiveType.Bool,
                        new FlagGroupStorage(Registers.eflags, (uint)FlagM.CF, "C", PrimitiveType.Byte)))
                }
            };
		}

		[Test]
		public void SserSerialize()
		{
			StringWriter sw = new StringWriter();
			XmlTextWriter xml = new XmlTextWriter(sw);
			XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(svc.GetType());
			ser.Serialize(xml, svc);
			string s = sw.ToString();
			int i = s.IndexOf("<syscallin");
			s = s.Substring(i, s.IndexOf("</syscalli") - i);
			Assert.AreEqual("<syscallinfo><vector>21</vector><regvalue reg=\"ah\">44</regvalue><regvalue reg=\"al\">00</regvalue>", s);
		}

		[Test]
		public void SserBuild()
		{
			SystemService ssvc = svc.Build(platform, new TypeLibrary());
			
			Assert.AreEqual(svc.Name, ssvc.Name);
			Assert.AreEqual(0x21, ssvc.SyscallInfo.Vector);
			Assert.AreSame(Registers.ah, ssvc.SyscallInfo.RegisterValues[0].Register);
			Assert.AreEqual(0x44, ssvc.SyscallInfo.RegisterValues[0].Value);
			Assert.AreSame(Registers.al, ssvc.SyscallInfo.RegisterValues[1].Register);
			Assert.AreEqual(0, ssvc.SyscallInfo.RegisterValues[1].Value);
		}

        [Test]
        public void SserStackValues()
        {
            var xml = new MemoryStream(
                Encoding.Unicode.GetBytes(
                "<?xml version=\"1.0\"?>\r\n" +
                "<library xmlns=\"http://schemata.jklnet.org/Decompiler\">" +
                "  <service name=\"test\">" +
                "    <syscallinfo>" +
                "      <vector>10</vector>" +
                "      <stackvalue offset=\"0c\">2a</stackvalue>" +
                "    </syscallinfo>" +
                "  </service>" +
                "</library>"
                ));
            var slib = SerializedLibrary.LoadFromStream(xml);
            Assert.AreEqual(1, slib.Procedures.Count);
            var ssvc = (SerializedService)slib.Procedures[0];
            Assert.AreEqual(1, ssvc.SyscallInfo.StackValues.Length);

            var svc = ssvc.Build(platform, new TypeLibrary());
            Assert.AreEqual(0, svc.SyscallInfo.RegisterValues.Length);
            Assert.AreEqual(1, svc.SyscallInfo.StackValues.Length);
            Assert.AreEqual(0x0C, svc.SyscallInfo.StackValues[0].Offset);
            Assert.AreEqual(0x2A, svc.SyscallInfo.StackValues[0].Value);
        }

        [Test]
        public void SserEmptyRegs()
        {
            var ssvc = new SerializedService
            {
                Ordinal = 3,
            };

            var svc = ssvc.Build(platform, new TypeLibrary());

            Assert.AreEqual(3, svc.SyscallInfo.Vector);
            Assert.AreEqual(0, svc.SyscallInfo.RegisterValues.Length);
        }
	}
}
