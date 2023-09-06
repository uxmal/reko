#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Arch.M68k.Assembler;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Output;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Arch.M68k.Assembler
{
    [TestFixture]
    public class M68kTextAssemblerTests
    {
        private M68kTextAssembler asm;

        [SetUp]
        public void Setup()
        {
            asm = new M68kTextAssembler();
        }

        protected void RunFragment(string fragment, string outputFile, Address addrBase)
        {
            var program = RunTestFromFragment(fragment, addrBase);
            RenderResult(program, outputFile);
        }

        protected void RunTest(string sourceFile, string outputFile, Address addrBase)
        {
            var program = RunTestFromFile(sourceFile, addrBase);

            RenderResult(program, outputFile);
        }

        private void RenderResult(Program program, string outputFile)
        {
            foreach (var item in asm.ImportReferences)
            {
                program.ImportReferences.Add(item.Key, item.Value);
            }

            using (FileUnitTester fut = new FileUnitTester(outputFile))
            {
                Dumper dumper = new Dumper(program);
                dumper.ShowAddresses = true;
                dumper.ShowCodeBytes = true;
                var mem = (ByteMemoryArea) program.SegmentMap.Segments.Values.First().MemoryArea;
                var formatter = new TextFormatter(fut.TextWriter);
                dumper.DumpData(program.SegmentMap, program.Architecture, mem.BaseAddress, mem.Bytes.Length, formatter);
                fut.TextWriter.WriteLine();
                dumper.DumpAssembler(program.Architecture, mem, mem.BaseAddress, mem.Length, formatter);
                if (program.ImportReferences.Count > 0)
                {
                    var list = new SortedList<Address, ImportReference>(program.ImportReferences);
                    foreach (var de in list)
                    {
                        fut.TextWriter.WriteLine("{0}: {1}", de, de.Value);
                    }
                }
                fut.AssertFilesEqual();
            }
        }

        private Program RunTestFromFile(string sourceFile, Address addrBase)
        {
            using (var rdr = new StreamReader(FileUnitTester.MapTestPath(sourceFile)))
            {
                var program = asm.Assemble(addrBase, sourceFile, rdr);
                program.Platform = new DefaultPlatform(null, program.Architecture);
                return program;
            }
        }

        private Program RunTestFromFragment(string fragment, Address addrBase)
        {
            var program = asm.AssembleFragment(addrBase, fragment);
            program.Platform = new DefaultPlatform(null, program.Architecture);
            return program;
        }

        [Test]
        [Ignore("Branches proving a little hairy here.")]
        public void M68kta_Max()
        {
            RunTest("Fragments/m68k/MAX.asm", "Arch/M68k/M68kta_Max.txt", Address.Ptr32(0x04000000));
        }

        [Test]
        public void M68kta_BackJump()
        {
            string txt = @"
    clr.l   d0
lupe
    cmp.l   (a3)+, d0
    bne     lupe
    rts
";
            RunFragment(txt, "Arch/M68k/M68kta_BackJump.txt", Address.Ptr32(0x00010000));
        }

        [Test]
        public void M68kta_FwdJump()
        {
            string txt = @"
    clr.l   d0
    cmp.b   (a3),d1
    bne     return
    addq.l  #1,d0
return
    rts
";
            RunFragment(txt, "Arch/M68k/M68kta_FwdJump.txt", Address.Ptr32(0x00010000));
        }

        [Test]
        public void M68kta_Jsr()
        {
            string txt = @"
    move.l  -$10(a7),d0
    move.l  d0,-(a7)
    jsr     mutate
    addq.l  #4,a7
    rts
mutate
    move.l  4(a7),d0
    addq.l  #2,d0
    rts
";
            RunFragment(txt, "Arch/M68k/M68kta_Jsr.txt", Address.Ptr32(0x00010000));
        }
    }
}
