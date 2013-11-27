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

using Decompiler.Assemblers.M68k;
using Decompiler.Core;
using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Assemblers.M68k
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
            var prog = RunTestFromFragment(fragment, addrBase);
            RenderResult(prog, outputFile); 
        }

        protected void RunTest(string sourceFile, string outputFile, Address addrBase)
        {
            var prog = RunTestFromFile(sourceFile, addrBase);

            RenderResult(prog, outputFile);
        }

        private void RenderResult(Program prog, string outputFile)
        {
            foreach (KeyValuePair<uint, PseudoProcedure> item in asm.ImportThunks)
            {
                prog.ImportThunks.Add(item.Key, item.Value);
            }

            using (FileUnitTester fut = new FileUnitTester(outputFile))
            {
                Dumper dumper = new Dumper(asm.Architecture);
                dumper.ShowAddresses = true;
                dumper.ShowCodeBytes = true;
                dumper.DumpData(prog.Image, prog.Image.BaseAddress, prog.Image.Bytes.Length, fut.TextWriter);
                fut.TextWriter.WriteLine();
                dumper.DumpAssembler(prog.Image, prog.Image.BaseAddress, prog.Image.BaseAddress + prog.Image.Bytes.Length, fut.TextWriter);
                if (prog.ImportThunks.Count > 0)
                {
                    SortedList<uint, PseudoProcedure> list = new SortedList<uint, PseudoProcedure>(prog.ImportThunks);
                    foreach (KeyValuePair<uint, PseudoProcedure> de in list)
                    {
                        fut.TextWriter.WriteLine("{0:X8}: {1}", de.Key, de.Value);
                    }
                }
                fut.AssertFilesEqual();
            }
        }

        private Program RunTestFromFile(string sourceFile, Address addrBase)
        {
            Program prog = new Program();
            using (var rdr = new StreamReader(FileUnitTester.MapTestPath(sourceFile)))
            {
                var lr = asm.Assemble(addrBase, rdr);
                prog.Image = lr.Image;
                prog.ImageMap = lr.ImageMap;
            }
            return prog;
        }

        private Program RunTestFromFragment(string fragment, Address addrBase)
        {
            Program prog = new Program();
            var lr = asm.AssembleFragment(addrBase, fragment);
            prog.Image = lr.Image;
            prog.ImageMap = lr.ImageMap;
            return prog;
        }

        [Test]
        public void M68kta_Max()
        {
            RunTest("Fragments/m68k/MAX.asm", "M68k/M68kta_Max.txt", new Address(0x04000000));
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
            RunFragment(txt, "M68k/M68kta_BackJump.txt", new Address(0x00010000));
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
            RunFragment(txt, "M68k/M68kta_FwdJump.txt", new Address(0x00010000));
        }
    }
}
