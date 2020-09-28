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
using Reko.Arch.M68k;
using Reko.Environments.MacOS.Classic;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text;

namespace Reko.UnitTests.Environments.MacOS.Classic
{
    [TestFixture]
    public class ResourceForkTests
    {
        [Test]
        public void ReadHeader()
        {
            var arch = new M68kArchitecture("m68k");
            var macOS = new MacOSClassic(new ServiceContainer(), arch);

            MemoryStream stm = new MemoryStream();
            WriteBeUint32(stm, 0x00000010);
            WriteBeUint32(stm, 0x00000020);
            WriteBeUint32(stm, 0x00000010);
            WriteBeUint32(stm, 0x00000010);

            stm.Write(Encoding.ASCII.GetBytes("Hello world!Hello world!"), 0, 0x10); 
            
            stm.Write(new byte[0x10], 0, 0x10);  // copy of header
            WriteBeUint32(stm, 0);               // next resource map.
            WriteBeUint16(stm, 0);
            WriteBeUint16(stm, 0);
            WriteBeUint16(stm, 0x1C);
            WriteBeUint16(stm, 0);
            WriteBeUint16(stm, 0);

            WriteAsciiString(stm, "CODE");      // 001E: type list
            WriteBeUint16(stm, 0);
            WriteBeUint16(stm, 8);

            WriteBeUint16(stm, 0);             // resource ID
            WriteBeUint16(stm, 0x0C);
            stm.WriteByte(0);
            stm.WriteByte(0);
            stm.WriteByte(0);
            stm.WriteByte(0x10);
            WriteBeUint32(stm, 0);

            stm.WriteByte(4);
            WriteAsciiString(stm, "Test");

            ResourceFork rsrc = new ResourceFork(macOS, stm.GetBuffer());
            Assert.AreEqual(1, rsrc.ResourceTypes.Count);
            IEnumerator<ResourceFork.ResourceType> ert = rsrc.ResourceTypes.GetEnumerator();
            Assert.IsTrue(ert.MoveNext());
            ResourceFork.ResourceType rt = ert.Current;
            Assert.AreEqual("CODE", rt.Name);
            Assert.AreEqual(1, rt.References.Count);

        }

        public static void WriteBeUint16(Stream stm, ushort u)
        {
            stm.WriteByte((byte)(u >> 8));
            stm.WriteByte((byte)(u));
        }

        public static void WriteBeUint32(Stream stm, uint u)
        {
            stm.WriteByte((byte)(u >> 24));
            stm.WriteByte((byte)(u >> 16));
            stm.WriteByte((byte)(u >> 8));
            stm.WriteByte((byte)(u));
        }

        public static void WriteAsciiString(Stream stm, string str)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            stm.Write(bytes, 0, bytes.Length);
        }
    }
}
