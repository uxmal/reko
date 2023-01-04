#region License
/* 
 * Copyright (C) 2018-2023 Stefano Moioli <smxdev4@gmail.com>.
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
using Reko.ImageLoaders.Pef;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.ImageLoaders.Pef
{
    [TestFixture]
    public class PefOpcodeTests
    {
        private MemoryStream output;

        [SetUp]
        public void Setup()
        {
            output = new MemoryStream();
        }

        byte[] RunProgram(string hexData)
        {
            var data = Reko.Core.BytePattern.FromHexBytes(hexData);
            var interp = new PefOpcodeInterpreter(data, output);
            interp.RunProgram();
            return output.ToArray();
        }

        byte[] RunProgram(byte[] data)
        {
            var interp = new PefOpcodeInterpreter(data, output);
            interp.RunProgram();
            return output.ToArray();
        }


        [Test]
        public void ZeroOpcode()
        {
            string bytecode = "008771";
            var output = RunProgram(bytecode);
            Assert.AreEqual(1009, output.Length);
            Assert.IsTrue(output.All(b => b == 0));
        }

        [Test]
        public void BlockCopy()
        {
            string bytecode = "23044D04";
            var output = RunProgram(bytecode);
            Assert.AreEqual(3, output.Length);
            Assert.AreEqual(new byte[] { 0x04, 0x4D, 0x04 }, output);
        }

        [Test]
        public void BlockCopyExtended()
        {
            var bytecode = "2023DEADBEEFDEADBEEFDEADBEEFDEADBEEFDEADBEEFDEADBEEFDEADBEEFDEADBEEFDEADBE";
            var output = RunProgram(bytecode);
            Assert.AreEqual(0x23, output.Length);
            Assert.AreEqual(new byte[]
            {
                0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD,
                0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF,
                0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE
            }, output);
        }

        [Test]
        public void RepeatedBlock()
        {
            // copy "DEADBEEF" (4 bytes) 4 times
            var bytecode = "4403DEADBEEF";
            var output = RunProgram(bytecode);
            Assert.AreEqual(16, output.Length);
            Assert.AreEqual(new byte[]
            {
                0xDE, 0xAD, 0xBE, 0xEF,
                0xDE, 0xAD, 0xBE, 0xEF,
                0xDE, 0xAD, 0xBE, 0xEF,
                0xDE, 0xAD, 0xBE, 0xEF,
            }, output);
        }

        [Test]
        public void InterleavedBlockCopy()
        {
            // commonSize -> 3 (number of data bytes)
            // customSize -> 1 (number of data bytes)
            // repeatCount -> 6 (number of pairs)
            // 000012 -> commonData
            // C8D8D0E0E8F0 -> customData (6 * 1 byte)

            var bytecode = "630106000012C8D8D0E0E8F0";
            var output = RunProgram(bytecode);
            Assert.AreEqual(27, output.Length);
            Assert.AreEqual(new byte[]
            {
                0x00, 0x00, 0x12, 0xC8,
                0x00, 0x00, 0x12, 0xD8,
                0x00, 0x00, 0x12, 0xD0,
                0x00, 0x00, 0x12, 0xE0,
                0x00, 0x00, 0x12, 0xE8,
                0x00, 0x00, 0x12, 0xF0,
                0x00, 0x00, 0x12
            }, output);
        }

        [Test]
        public void InterleavedZero()
        {
            // commonSize -> 2 (number of zeros)
            // customSize -> 2 (number of data bytes)
            // repeatCount -> 4 (number of pairs)
            var bytecode = "820204";
            // custom data
            bytecode += "1CC817B018801DB8";

            var output = RunProgram(bytecode);
            Assert.AreEqual(18, output.Length);
            Assert.AreEqual(new byte[]
            {
                0x00, 0x00, 0x1C, 0xC8,
                0x00, 0x00, 0x17, 0xB0,
                0x00, 0x00, 0x18, 0x80,
                0x00, 0x00, 0x1D, 0xB8,
                0x00, 0x00
            }, output);
        }
    }
}
