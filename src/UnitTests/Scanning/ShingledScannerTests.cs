#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Arch.Mips;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Scanning;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    public class ShingledScannerTests
    {
        private MockRepository mr;
        private Program program;
        private ShingledScanner sh;
        private RelocationDictionary rd;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            rd = null;
        }

        private void Given_Mips_Image(params uint[] words)
        {
            var image = new MemoryArea(
                Address.Ptr32(0x10000),
                words.Select(w => new[]
                {
                    (byte) w,
                    (byte) (w >> 8),
                    (byte) (w >> 16),
                    (byte) (w >> 24)
                })
                .SelectMany(w => w)
                .ToArray());
            var arch = new MipsLe32Architecture();
            CreateProgram(image, arch);
        }

        private void Given_x86_Image(params byte[] bytes)
        {
            var image = new MemoryArea(
                Address.Ptr32(0x10000),
                bytes);
            this.rd = image.Relocations;
            var arch = new X86ArchitectureFlat32();
            CreateProgram(image, arch);
        }

        private void Given_x86_64_Image(params byte[] bytes)
        {
            var image = new MemoryArea(
                Address.Ptr64(0x0100000000000000),
                bytes);
            var arch = new X86ArchitectureFlat64();
            CreateProgram(image, arch);
        }

        private void CreateProgram(MemoryArea mem, IProcessorArchitecture arch)
        {
            var segmentMap = new SegmentMap(mem.BaseAddress);
            var seg = segmentMap.AddSegment(new ImageSegment(
                ".text",
                mem,
                AccessMode.ReadExecute)
            {
                Size = (uint)mem.Bytes.Length
            });
            seg.Access = AccessMode.ReadExecute;
            var platform = new DefaultPlatform(null, arch);
            program = new Program(
                segmentMap,
                arch,
                platform);
        }

        /// <summary>
        /// Take only each 'n'th item
        /// </summary>
        private T[] TakeEach<T>(IEnumerable<T> items, int n)
        {
            return items.Where((item, i) => (i % n) == 0).ToArray();
        }

        private void Given_Scanner()
        {
            var host = mr.Stub<IRewriterHost>();
            var dev = mr.Stub<DecompilerEventListener>();
            host.Stub(h => h.EnsurePseudoProcedure(null, null, 0))
                .IgnoreArguments()
                .Return(new PseudoProcedure("<>", PrimitiveType.Word32, 2));
            host.Replay();
            dev.Replay();
            this.sh = new ShingledScanner(program, host, dev);
        }

        [Test]
        public void Shsc_Invalid()
        {
            Given_Mips_Image(0x00001403);
            Given_Scanner();
            var seg = program.SegmentMap.Segments.Values.First();
            var by = sh.ScanSegment(seg, 0);
            Assert.AreEqual(new byte[] { 0 }, TakeEach(by, 4));
        }

        [Test]
        public void Shsc_Return()
        {
            Given_Mips_Image(0x03E00008, 0);
            Given_Scanner();
            var by = sh.ScanSegment(program.SegmentMap.Segments.Values.First(), 0);
            Assert.AreEqual(new byte[] { 1, 1 }, TakeEach(by, 4));
        }

        [Test]
        public void Shsc_CondJump()
        {
            Given_Mips_Image(0x1C60FFFF, 0, 0x03e00008, 0);
            Given_Scanner();
            var by = sh.ScanSegment(program.SegmentMap.Segments.Values.First(), 0);
            Assert.AreEqual(new byte[] { 1, 1, 1, 1, }, TakeEach(by, 4));
        }

        [Test]
        public void Shsc_Overlapping()
        {
            Given_x86_Image(0x33, 0xC0, 0xC0, 0x90, 0xc3);
            Given_Scanner();
            var by = sh.ScanSegment(program.SegmentMap.Segments.Values.First(), 0);
            Assert.AreEqual(new byte[] { 0, 1, 0, 1, 1 }, by);
        }

        [Test]
        public void Shsc_FindPossiblePointersToCode()
        {
            Given_x86_Image(
                0x90, 0x90, 0x90, 0x90,      // Padding
                0x55, 0x5B, 0xC3, 0x00,     // Actual code
                0x04, 0x00, 0x01, 0x00);     // Pointer to code
            Given_Scanner();
            var pointedTo = sh.GetPossiblePointerTargets();
            Assert.AreEqual(new byte[] {
                    0, 0, 0, 0,
                    1, 0, 0, 0,
                    0, 0, 0, 0,
                },
                pointedTo.Values.First());
        }

        [Test]
        public void Shsc_FindPossibleMultiplePointersToCode()
        {
            Given_x86_Image(
                0x90, 0x90, 0x90, 0x90,     // Padding
                0x55, 0x5B, 0xC3, 0x00,     // Actual code
                0x04, 0x00, 0x01, 0x00,     // Pointer to code
                0x04, 0x00, 0x01, 0x00);    // Another pointer to code
            Given_Scanner();
            var by = sh.ScanExecutableSegments();
            var pointedTo = sh.GetPossiblePointerTargets();
            Assert.AreEqual(new byte[] {
                    0, 0, 0, 0,
                    2, 0, 0, 0,
                    0, 0, 0, 0,
                    0, 0, 0, 0,
                },
                pointedTo.Values.First());
        }

        [Test]
        public void Shsc_FindPossiblePointersToEndOfSegment()
        {
            Given_x86_Image(
                0x0F, 0x00, 0x01, 0x00,     // Pointer to last byte in segment
                0x0F, 0x00, 0x01, 0x00,     // Pointer to last byte in segment
                0x0F, 0x00, 0x01, 0x00,     // Pointer to last byte in segment
                0xCC, 0xCC, 0xCC, 0xC3);    // Another pointer to code
            Given_Scanner();
            var by = sh.ScanExecutableSegments();
            var pointedTo = sh.GetPossiblePointerTargets();
            Assert.AreEqual(new byte[] {
                    0, 0, 0, 0,
                    0, 0, 0, 0,
                    0, 0, 0, 0,
                    0, 0, 0, 3,
                },
                pointedTo.Values.First());
        }

        [Test]
        public void Shsc_x86_64_FindPossiblePointersToEndOfSegment()
        {
            Given_x86_64_Image(
                0x1F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,     // Pointer to last byte in segment
                0x1F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,    // Pointer to last byte in segment
                0x1F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,     // Pointer to last byte in segment
                0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xC3);    // Return at the end
            Given_Scanner();
            var by = sh.ScanExecutableSegments();
            var pointedTo = sh.GetPossiblePointerTargets();
            Assert.AreEqual(new byte[] {
                    0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 3,
                },
                pointedTo.Values.First());
        }

        [Test(Description ="Instructions whose extent overlaps a relocation are not valid.")]
        public void Shsc_Relocation_CrossesInstruction()
        {
            Given_x86_Image(
                0x01, 0x02, 0xC3, 0x04, 0x4);
            rd.AddPointerReference(0x10001, 0x11000000);
            Given_Scanner();

            var seg = program.SegmentMap.Segments.Values.First();
            var by = this.sh.ScanSegment(seg, 0);
            Assert.AreEqual(new byte[]
                {
                    0, 0, 0, 0, 0
                },
                by);
        }

        [Test(Description ="Calls to functions that turn out to be bad should also be bad")]
        public void Shsc_BadCall()
        {
            // "It was a bad call, Ripley; it was a bad call."
            // "'Bad call'?! Those people are *dead*, Burke!"
            Given_x86_Image(
                0x90, 0xC3,                                     // nop ret, should be OK
                0x55, 0xE8, 0x01, 0x00, 0x00, 0x00, 0xC3,       // call to beyond ret
                0x50, 0x00);                                    // push eax and then bad instruction
            Given_Scanner();

            var seg = program.SegmentMap.Segments.Values.First();
            var by = this.sh.ScanSegment(seg, 0);
            Assert.AreEqual(new byte[]
                {
                    1, 1,
                    0, 0, 1, 0, 1, 0, 1,
                    0, 0
                },
                by);
        }
    }
}
