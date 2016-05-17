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

using Reko.Arch.Arm;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Scanning;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    public class HeuristicScannerTests : HeuristicTestBase
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
        }

        [Test]
        public void HSC_x86_FindCallOpcode()
        {
            Given_Image32(
                0x001000, 
                "E8 03 00 00  00 00 00 00 " +
                "C3");
            Given_x86_32();
            Given_RewriterHost();
            mr.ReplayAll();

            var hsc = new HeuristicScanner(program, host, eventListener);
            var addr = hsc.FindCallOpcodes(segment.MemoryArea, new Address[] {
                Address.Ptr32(0x1008)
            }).ToList();

            Assert.AreEqual(1, addr.Count);
            Assert.AreEqual(0x001000, (uint)addr[0].ToLinear());
        }


        [Test]
        public void HSC_x86_FindCallsToProcedure()
        {
#if OLD
            var image = new LoadedImage(Address.Ptr32(0x001000), new byte[] {
                0xE8, 0x0B, 0x00, 0x00,  0x00, 0xE8, 0x07, 0x00,
                0x00, 0x00, 0xC3, 0x00,  0x00, 0x00, 0x00, 0x00,
                0xC3, 0xC3                                      // 1010, 1011
            });
            prog = new Program
            {
                Image = image,
                ImageMap = image.CreateImageMap(),
                Architecture = new X86ArchitectureFlat32(),
            };
#else
            Given_Image32(0x001000, 
                "E8 0B 00 00 00 E8 07 00 " +
                "00 00 C3 00 00 00 00 00 " +
                "C3 C3 ");                                     // 1010, 1011
            Given_x86_32();
#endif
            Given_RewriterHost();
            mr.ReplayAll();

            Assert.AreEqual(18, segment.MemoryArea.Length);

            var hsc = new HeuristicScanner(program, host, eventListener);
            var linAddrs = hsc.FindCallOpcodes(segment.MemoryArea, new Address[]{
                Address.Ptr32(0x1010),
                Address.Ptr32(0x1011)}).ToList();

            Assert.AreEqual(2, linAddrs.Count);
            Assert.IsTrue(linAddrs.Contains(Address.Ptr32(0x1000)));
            Assert.IsTrue(linAddrs.Contains(Address.Ptr32(0x1005)));
        }


        [Test]
        public void HSC_x86_16bitNearCall()
        {
            base.Given_ImageSeg(0xC00, 0,
                "C3 90 E8 FB FF C3");
            base.Given_x86_16();
            Given_RewriterHost();
            mr.ReplayAll();

            var hsc = new HeuristicScanner(program, host, eventListener);
            var linAddrs = hsc.FindCallOpcodes(segment.MemoryArea, new Address[] {
                Address.SegPtr(0x0C00, 0)}).ToList();

            Assert.AreEqual(1, linAddrs.Count);
            Assert.AreEqual("0C00:0002", linAddrs[0].ToString());
        }

        [Test]
        public void HSC_x86_16bitFarCall()
        {
            Given_ImageSeg(0xC00, 0, 
               "C3 90 9A 00 00 00 0C C3 ");
            Given_x86_16();
            Given_RewriterHost();
            mr.ReplayAll();

            var hsc = new HeuristicScanner(program, host, eventListener);

            var linAddrs = hsc.FindCallOpcodes(segment.MemoryArea, new Address[] {
                Address.SegPtr(0x0C00, 0)}).ToList();

            Assert.AreEqual(1, linAddrs.Count);
            Assert.AreEqual("0C00:0002", linAddrs[0].ToString());
        }

        [Test]
        public void HSC_ARM32_Calls()
        {
            var mem = CreateMemoryArea(Address.Ptr32(0x1000),
                0xE1A0F00E,     // mov r15,r14 (return)
                0xEBFFFFFD,
                0xEBFFFFFC);
            this.segment = new ImageSegment(".text", mem, AccessMode.ReadExecute);
            var imageMap = new SegmentMap(
                mem.BaseAddress,
                segment);
            program = new Program
            {
                SegmentMap = imageMap,
                Architecture = new Arm32ProcessorArchitecture(),
            };
            var host = mr.Stub<IRewriterHost>();
            mr.ReplayAll();

            var hsc = new HeuristicScanner(program, host, eventListener);
            var linAddrs = hsc.FindCallOpcodes(segment.MemoryArea, new Address[] {
                Address.Ptr32(0x1000),
            }).ToList();

            Assert.AreEqual(2, linAddrs.Count);
            Assert.IsTrue(linAddrs.Contains(Address.Ptr32(0x1004)));
            Assert.IsTrue(linAddrs.Contains(Address.Ptr32(0x1008)));
        }

        [Test]
        public void HSC_FindPossible_x86_ProcedureEntries() // "Starts"
        {
            Given_Image32(0x10000, "CC CC CC 55 8B EC C3 00   00 00 55 8B EC");
            Given_x86_32();
            var host = mr.Stub<IRewriterHost>();
            mr.ReplayAll();

            var hsc = new HeuristicScanner(program, host, eventListener);
            var r = hsc.FindUnscannedRanges();
            var ranges = hsc.FindPossibleFunctions(r).ToArray();
            Assert.AreEqual(0x10003, ranges[0].Item1.ToLinear());
            Assert.AreEqual(0x1000A, ranges[0].Item2.ToLinear());
            Assert.AreEqual(0x1000A, ranges[1].Item1.ToLinear());
            Assert.AreEqual(0x1000D, ranges[1].Item2.ToLinear());
        }

        [Test]
        public void HSC_HeuristicDisassembleProc()
        {
            Given_Image32(
                0x10000,
                TrickyProc);
            Given_x86_32();
            Given_RewriterHost();
            host.Stub(h => h.GetImportedProcedure(null, null))
                .IgnoreArguments()
                .Return(null);
            mr.ReplayAll();

            var segment = program.SegmentMap.Segments.Values.First();
            var hsc = new HeuristicScanner(program, host, eventListener);
            var proc = hsc.DisassembleProcedure(
                segment.MemoryArea.BaseAddress,
                segment.MemoryArea.BaseAddress + segment.ContentSize);
            var sExp =
                #region Expected
 @"l00010000:  // pred:
    push ebp
l00010001:  // pred: l00010000
    mov ebp,esp
l00010002:  // pred:
    in eax,E8
l00010003:  // pred: l00010001
    call 11750008
l00010004:  // pred: l00010002
    add [eax],al
l00010005:  // pred:
    add [ecx+edx+0A],dh
l00010006:  // pred: l00010004
    jz 00010019
l00010007:  // pred:
    adc [edx],ecx
l00010008:  // pred: l00010006
    or al,[0675003C]
l00010009:  // pred: l00010005 l00010007
    add eax,0675003C
l0001000A:  // pred:
    cmp al,00
l0001000B:  // pred:
    add [ebp+06],dh
l0001000C:  // pred: l0001000A
    jnz 00010014
l0001000D:  // pred:
    push es
l0001000E:  // pred: l00010008 l00010009 l0001000B l0001000C l0001000D
    mov al,00
l0001000F:  // pred:
    add bl,ch
l00010010:  // pred: l0001000E
    jmp 00010019
l00010011:  // pred: l0001000F
    pop es
l00010012:  // pred: l00010011
    or al,[740000A1]
l00010013:  // pred:
    add eax,740000A1
l00010014:  // pred: l0001000C
    mov eax,[01740000]
l00010015:  // pred:
    add [eax],al
l00010016:  // pred:
    add [ecx+eax-77],dh
l00010017:  // pred: l00010015
    jz 0001001A
l00010018:  // pred: l00010012 l00010013
    add [ecx+90C35DEC],ecx
l00010019:  // pred: l00010006 l00010010 l00010014 l00010017
    mov esp,ebp
l0001001A:  // pred: l00010016 l00010017
    in al,dx
l0001001B:  // pred: l00010019 l0001001A
    pop ebp
l0001001C:  // pred: l0001001B
    ret 
l0001001D:  // pred:
    nop 
";
            #endregion
            AssertBlocks(sExp, proc.Cfg);
        }
    }
}
