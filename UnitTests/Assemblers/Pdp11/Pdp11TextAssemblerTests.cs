#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Decompiler.Core.Assemblers;
using Decompiler.Assemblers.Pdp11;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Assemblers.Pdp11
{
    [TestFixture]
    public class Pdp11TextAssemblerTests
    {
        private MockRepository mr;
        private Pdp11TextAssembler asm;
        private IEmitter emitter;
        private Program ldr;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.emitter = new Emitter();
            this.asm = new Pdp11TextAssembler(emitter);
        }

        [Test]
        public void Pdp11Tasm_Comment()
        {
            var ldr = asm.AssembleFragment(Address.Ptr16(0x0100), "; nothing");
        }

        [Test]
        public void Pdp11Tasm_Equate()
        {
            var ldr = asm.AssembleFragment(Address.Ptr16(0x0100), " a = r");
            Assert.AreEqual("r", asm.Assembler.Equates["a"]);
        }

        [Test]
        public void Pdp11Tasm_Decimal()
        {
            var ldr = asm.AssembleFragment(Address.Ptr16(0x0100), " a = 42.");
            Assert.AreEqual(42, asm.Assembler.Equates["a"]);
        }

        [Test]
        public void Pdp11Tasm_Sum()
        {
            var ldr = asm.AssembleFragment(Address.Ptr16(0x0100), @"
a = 42.; decimal
b = 10 ; octal
c = a + b");
            Assert.AreEqual(50, asm.Assembler.Equates["c"]);
        }

        [Test]
        public void Pdp11Tasm_PageDirective()
        {
            var ldr = asm.AssembleFragment(Address.Ptr16(0x0100), ".page");
        }

        [Test]
        public void Pdp11Tasm_LabelledWord()
        {
            var ldr = asm.AssembleFragment(Address.Ptr16(0x0100), "label1: .word 14");
            Assert.AreEqual(2, ldr.Image.Bytes.Length);
            Assert.AreEqual(12, ldr.Image.ReadLeInt16(0));
        }

        [Test]
        public void Pdp11Tasm_Dot()
        {
            var ldr = asm.AssembleFragment(Address.Ptr16(0x100), "label1: .word .");
            Assert.AreEqual(0x100, ldr.Image.ReadLeUInt16(0));
        }

        [Test]
        public void Pdp11Tasm_ReserveIdiom()
        {
            var ldr = asm.AssembleFragment(Address.Ptr16(0x100), ".=.+10");
            Assert.AreEqual(8, ldr.Image.Bytes.Length);
        }

        [Test]
        public void Pdp11Tasm_Reset()
        {
            var ldr = asm.AssembleFragment(Address.Ptr16(0x100), " rEsEt");
            Assert.AreEqual(0x0005, ldr.Image.ReadLeUInt16(0));
        }

        private void AssertWords(byte [] actual, params ushort[] expected)
        {
            var actualWords = actual.AsWords().ToArray();
            Assert.AreEqual(expected.Length, actualWords.Length);
            for (int i = 0; i < expected.Length; ++i)
            {
                Assert.AreEqual(expected[i].ToString("X4"), actualWords[i].ToString("X4"));
            }
        }

        [Test]
        public void Pdp11Tasm_MovSp()
        {
            var ldr = asm.AssembleFragment(Address.Ptr16(0x100), @"
sav: .word
    mov sp,sav");
            AssertWords(ldr.Image.Bytes, 0x0000, 0x119F, 0x0100);
        }

        [Test]
        public void Pdp11Tasm_FwdRef()
        {
            var assemblerFragment = @"
        mov     #power,sav
power:  .word 666
sav:    .word 777";
            Assemble(assemblerFragment);
            AssertWords(ldr.Image.Bytes, 0x15DF, 0x0106, 0x0108, 0x1B6, 0x1FF);
        }

        private void Assemble(string assemblerFragment)
        {
            ldr = asm.AssembleFragment(Address.Ptr16(0x100), assemblerFragment);
        }

        [Test]
        public void Pdp11Tasm_jsr()
        {
            Assemble(@"
        jsr pc,delay
delay:  .word
");
            AssertWords(ldr.Image.Bytes, 0x09DF, 0x0104, 0x0000);
        }

        [Test]
        public void Pdp11Tasm_sub()
        {
            Assemble(@"sub r2,r3");
            AssertWords(ldr.Image.Bytes, 0xE083);
        }


        [Test]
        public void Pdp11Tasm_asr_w()
        {
            Assemble(@"asr @r2");
            AssertWords(ldr.Image.Bytes, 0x0C8A);
        }

        [Test]
        public void Pdp11Tasm_movb()
        {
            Assemble(@"movb @r2,-(r3)");
            AssertWords(ldr.Image.Bytes, 0x92A3);
        }

        [Test]
        public void Pdp11Tasm_clr_post()
        {
            Assemble("clrb (r2)+");
            AssertWords(ldr.Image.Bytes, 0x8A12);
        }

        [Test]
        public void Pdp11Tasm_dec_beq()
        {
            Assemble(@"
lupe: dec r0
     beq lupe
");
            AssertWords(ldr.Image.Bytes, 0x0AC0, 0x03FE);
        }
    }

    public static class MoreLinq
    {
        public static IEnumerable<ushort> AsWords(this IEnumerable<byte> items)
        {
            var e = items.GetEnumerator();
            while (e.MoveNext())
            {
                var b1 = e.Current;
                if (!e.MoveNext())
                    yield break;
                int b2 = e.Current;
                yield return (ushort) ((b2 << 8) | b1);
            }
        }
    }
}
