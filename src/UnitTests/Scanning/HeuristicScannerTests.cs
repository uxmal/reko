#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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
using Decompiler.Arch.Arm;
using Decompiler.Arch.X86;
using Decompiler.Scanning;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Scanning
{
    [TestFixture]
    public class HeuristicScannerTests
    {
        private ProgramImage CreateImage(Address addr, params uint[] opcodes)
        {
            byte[] bytes = new byte[0x20];
            var writer = new LeImageWriter(bytes);
            uint offset = 0;
            for (int i = 0; i < opcodes.Length; ++i, offset += 4)
            {
                writer.WriteLeUInt32(offset, opcodes[i]);
            }
            return new ProgramImage(addr, bytes);
        }

        [Test]
        public void HSC_x86_FindCallOpcode()
        {
            var image = new ProgramImage(new Address(0x001000), new byte[] {
                0xE8, 0x03, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00,
                0xC3
            });
            var prog = new Program
            {
                Image = image,
                Architecture = new IntelArchitecture(ProcessorMode.Protected32),
            };
            var hsc = new HeuristicScanner(prog);

            var addr = hsc.FindCallOpcodes(new Address[] {
                new Address(0x1008)
            }).ToList();

            Assert.AreEqual(1, addr.Count);
            Assert.AreEqual(0x001000, addr[0]);
        }

        [Test]
        public void HSC_x86_FindCallsToProcedure()
        {
            var image = new ProgramImage(new Address(0x001000), new byte[] {
                0xE8, 0x0B, 0x00, 0x00,  0x00, 0xE8, 0x07, 0x00,
                0x00, 0x00, 0xC3, 0x00,  0x00, 0x00, 0x00, 0x00,
                0xC3, 0xC3                                      // 1010, 1011
            });
            var prog = new Program
            {
                Image = image,
                Architecture = new IntelArchitecture(ProcessorMode.Protected32),
            };
            var hsc = new HeuristicScanner(prog);

            var linAddrs = hsc.FindCallOpcodes(new Address[]{
                new Address(0x1010),
                new Address(0x1011)}).ToList();

            Assert.AreEqual(2, linAddrs.Count);
            Assert.IsTrue(linAddrs.Contains(0x1000));
            Assert.IsTrue(linAddrs.Contains(0x1005));
        }

        [Test]
        public void HSC_x86_16bitNearCall()
        {
            var image = new ProgramImage(new Address(0xC00, 0), new byte[] {
                0xC3, 0x90, 0xE8, 0xFB, 0xFF, 0xC3, 
            });
            var prog = new Program
            {
                Image = image,
                Architecture = new IntelArchitecture(ProcessorMode.Real),
            };
            var hsc = new HeuristicScanner(prog);

            var linAddrs = hsc.FindCallOpcodes(new Address[] {
                new Address(0x0C00, 0)}).ToList();

            Assert.AreEqual(1, linAddrs.Count);
            Assert.AreEqual(0xC002, linAddrs[0]);
        }

        [Test]
        public void HSC_x86_16bitFarCall()
        {
            var image = new ProgramImage(new Address(0xC00, 0), new byte[] {
                0xC3, 0x90, 0x9A, 0x00, 0x00, 0x00, 0x0C, 0xC3 
            });
            var prog = new Program
            {
                Image = image,
                Architecture = new IntelArchitecture(ProcessorMode.Real),
            };
            var hsc = new HeuristicScanner(prog);

            var linAddrs = hsc.FindCallOpcodes(new Address[] {
                new Address(0x0C00, 0)}).ToList();

            Assert.AreEqual(1, linAddrs.Count);
            Assert.AreEqual(0xC002, linAddrs[0]);
        }

        [Test]
        public void HSC_ARM32_Calls()
        {
            var image = CreateImage(new Address(0x1000),
                0xE1A0F00E,     // mov r15,r14 (return)
                0xEBFFFFFD,
                0xEBFFFFFC);
            var prog = new Program
            {
                Image = image,
                Architecture = new ArmProcessorArchitecture(),
            };
            var hsc = new HeuristicScanner(prog);
            var linAddrs = hsc.FindCallOpcodes(new Address[] {
                new Address(0x1000),
            }).ToList();

            Assert.AreEqual(2, linAddrs.Count);
            Assert.IsTrue(linAddrs.Contains(0x1004));
            Assert.IsTrue(linAddrs.Contains(0x1008));
        }
    }
}
