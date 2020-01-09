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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Expressions;
using System;
using System.Text;

namespace Reko.ImageLoaders.OdbgScript
{
    [TestFixture]
    public class OllyLangTests
    {
        private Mock<IHost> host;
        private Mock<IProcessorEmulator> emu;
        private Mock<IProcessorArchitecture> arch;
        private OllyLang engine;
        private MemoryArea mem;
        private SegmentMap imageMap;

        [SetUp]
        public void Setup()
        {
            this.emu = new Mock<IProcessorEmulator>();
            this.arch = new Mock<IProcessorArchitecture>();
        }

        [Test]
        public void Ose_var()
        {
            Given_Engine();
            Given_Script("var foo\r\n");

            engine.Run();

            Assert.IsTrue(engine.variables.ContainsKey("foo"));
        }

        private void Given_Script(string script)
        {
            engine.script.Clear();
            engine.script.LoadScriptFromString(script, ".");
        }

        private void Given_Engine()
        {
            this.host = new Mock<IHost>();
            engine = new OllyLang(null, arch.Object);
            engine.Host = host.Object;
            engine.Debugger = new Debugger(emu.Object);
        }

        private void Given_Image(uint addr, params byte[] bytes)
        {
            mem = new MemoryArea(Address.Ptr32(addr), bytes);
            imageMap = new SegmentMap(
                mem.BaseAddress,
                new ImageSegment(".text", mem, AccessMode.ReadExecute));
            host.Setup(h => h.SegmentMap).Returns(imageMap);
            var meminfo = new MEMORY_BASIC_INFORMATION
            {
                BaseAddress = mem.BaseAddress.ToLinear(),
                RegionSize = (uint) mem.Length,
                AllocationBase = mem.BaseAddress.ToLinear()
            };
            host.Setup(h => h.TE_GetMemoryInfo(
                It.IsAny<ulong>(),
                out meminfo)).Returns(true);
        }

        [Test]
        public void Ose_LineArgs()
        {
            var line = new OllyScript.Line();
            OllyScript.ParseArgumentsIntoLine( " hello,world", line);
            Assert.AreEqual(2, line.args.Length);
        }

        [Test]
        public void Ose_LineArgString()
        {
            var line = new OllyScript.Line();
            OllyScript.ParseArgumentsIntoLine(" \"hello,world\"", line);
            Assert.AreEqual(1, line.args.Length);
        }

        [Test]
        public void Ose_Find()
        {
            Given_Engine();
            Given_Image(0x001000, new byte[] { 0, 0, 0, 0, 0, 1, 2, 3 });
            Given_Script("find 001000, #01#\r\n");

            engine.Run();

            Assert.AreEqual(0x1000 + 5, engine.variables["$RESULT"].ToUInt64());
        }

        [Test]
        public void Ose_FindReplace()
        {
            Given_Engine();
            Given_Image(0x001000, new byte[] { 0x2a, 0x2a, 0x2a, 0x21, 0x23, 0x21, 0x23 });
            host.Setup(h => h.WriteMemory(
                It.IsAny<ulong>(),
                3,
                It.IsNotNull<byte[]>()))
                .Returns((ulong a, int l, byte[] b) =>
            {
                MemoryArea.WriteBytes(b, (long) a - (long) mem.BaseAddress.ToLinear(), l, mem.Bytes);
                return true;
            });

            Given_Script(
                "find 001000, #21??21#\r\n" +
                "test $RESULT,$RESULT\r\n" +
                "jz done\r\n"+
                "fill $RESULT,3,2d\r\n"+
                "done:\r\n"
                );

            engine.Run();

            Assert.AreEqual("***---#", Encoding.ASCII.GetString(mem.Bytes));
        }
        
        [Test]
        public void Ose_FormatWords()
        {
            Given_Engine();
            //Assert.AreEqual("foo", engine.FormatAsmDwords("foo "));
            //Assert.AreEqual("foo 0xa", engine.FormatAsmDwords("foo a"));
            //Assert.AreEqual("foo [0xa]", engine.FormatAsmDwords("foo [a]"));
            Assert.AreEqual("foo [0xa],0xb", engine.FormatAsmDwords("foo [a],b"));
        }

        [Test]
        public void Ose_Breakpoint()
        {
            emu.Setup(e => e.SetBreakpoint(
                0x00123400u,
                It.IsNotNull<Action>()))
                .Verifiable();
            arch.Setup(a => a.MakeAddressFromConstant(
                It.IsNotNull<Constant>(),
                false))
                .Returns(Address.Ptr32(0x00123400));

            Given_Engine();
            Given_Script("bp 00123400");
            engine.Run();

            emu.Verify();
        }

        [Test]
        public void Ose_Breakpoint_SegmentedAddress()
        {
            emu.Setup(e => e.SetBreakpoint(
                0x00008010,
                It.IsNotNull<Action>()))
                .Verifiable();
            var addr = Address.SegPtr(0x800, 0x10);
            arch.Setup(a => a.MakeSegmentedAddress(
                It.IsNotNull<Constant>(),
                It.IsNotNull<Constant>()))
                .Returns(addr);

            Given_Engine();
            Given_Script("bp 0800:0010");
            engine.Run();

            emu.Verify();
        }
    }

    [TestFixture]
    public class VarTests
    {
        [Test]
        public void Reverse_Pattern()
        {
            var v = Var.Create("#1234#");
            v = v.reverse();
            Assert.AreEqual("3412", v.to_bytes());
            v = Var.Create("#123456#");
            v = v.reverse();
            Assert.AreEqual("563412", v.to_bytes());
        }

        [Test]
        public void Resize_Pattern()
        {
            var v = Var.Create("#1234#");
            v.resize(1);
            Assert.AreEqual("12", v.to_bytes());
        }
    }
}

