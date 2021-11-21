#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core;
using Reko.Core.Memory;
using Reko.Arch.X86;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Arch.X86
{
    [TestFixture]
    public class AttSyntaxTests
    {
        private readonly X86ArchitectureFlat32 arch32;
        private readonly X86ArchitectureFlat64 arch64;

        public AttSyntaxTests()
        {
            var sc = new ServiceContainer();
            var options = new Dictionary<string, object>();
            
            this.arch32 = new X86ArchitectureFlat32(sc,"x86-protected-32", options);
            this.arch64 = new X86ArchitectureFlat64(sc,"x86-protected-64", options);
        }

        private static void AssertEqual(IProcessorArchitecture arch, string sExpected, string hexString)
        {
            var bytes = BytePattern.FromHexBytes(hexString);
            var mem = new ByteMemoryArea(Address.Ptr32(0x0010_0000), bytes);
            var rdr = mem.CreateLeReader(0);
            var dasm = arch.CreateDisassembler(rdr);
            var instr = (X86Instruction) dasm.First();
            var sActual = instr.ToString("A");
            Assert.AreEqual(sExpected, sActual);
        }

        private void AssertEqual32(string sExpected, string hexString)
        {
            AssertEqual(this.arch32, sExpected, hexString);
        }

        private void AssertEqual64(string sExpected, string hexString)
        {
            AssertEqual(this.arch64, sExpected, hexString);
        }

        [Test]
        public void X86AttSx_ShortOffset()
        {
            AssertEqual32("and\t0x1(%eax),%al", "224001");
        }

        [Test]
        public void X86AttSx_call_far()
        {
            AssertEqual32("lcall\t$0x7856, $0x3412", "9A12345678");
        }

        [Test]
        public void X86AttSx_jmp_far()
        {
            AssertEqual32("ljmp\t$0x7856, $0x3412", "EA12345678");
        }

        [Test]
        public void X86AttSx_call_far_indirect()
        {
            AssertEqual32("lcall\t*(%eax)", "FF18");
        }

        [Test]
        public void X86AttSx_retf()
        {
            AssertEqual32("lret\t$0x12","CA 12 00");
        }

        [Test]
        public void X86AttSx_bound()
        {
            AssertEqual32("bound\t%esi,(%edx)","62 32");
            AssertEqual32("bound\t%si,(%edx)","66 62 32");
        }

        [Test]
        public void X86AttSx_enter()
        {
            AssertEqual32("enter\t$0x20,$0x52","c8200052");
        }

        [Test]
        public void X86AttSx_movsx()
        {
            AssertEqual64("movsbl\t0x1234(%rdx),%ebp", "0f be aa 34 12 00 00");
            AssertEqual64("movsbw\t0x1234(%rdx),%bp",  "66 0f be aa 34 12 00 00");
            AssertEqual64("movswl\t0x1234(%rdx),%ebp", "0f bf aa 34 12 00 00");
            AssertEqual64("movsww\t0x1234(%rdx),%bp",  "66 0f bf aa 34 12 00 00");
            AssertEqual64("movswq\t0x1234(%rdx),%rbp", "48 0f bf aa 34 12 00 00");
            AssertEqual64("movsww\t0x1234(%rdx),%bp",  "66 0f bf aa 34 12 00 00");
            AssertEqual64("movswl\t0x1234(%rdx),%ebp", "0f bf aa 34 12 00 00");
            AssertEqual64("movswq\t0x1234(%rdx),%rbp", "48 0f bf aa 34 12 00 00");
        }

        [Test]
        public void X86AttSx_cwd_64()
        {
            AssertEqual64("cwtl","98"); // cwde
            AssertEqual64("cbtw","6698"); // cbw
            AssertEqual64("cltq","4898"); // cdqe
            AssertEqual64("cltd","99"); // cdq
            AssertEqual64("cwtd","6699"); // cwd
            AssertEqual64("cqto","4899"); // cqo
        }

        [Test]
        public void X86AttSx_cmps()
        {
            AssertEqual64("cmpsl", "a7");
            AssertEqual64("cmpsw", "66 a7");
            AssertEqual64("rep cmpsq", "f3 48 a7");
        }

        [Test]
        public void X86AttSx_missing_base_register()
        {
            AssertEqual64("mov\t0x12(,%rsi,1),%ebx", "8B 1C 35 12 00 00 00");
        }

        [Test]
        public void X86AttSx_rip_relative()
        {
            AssertEqual64("mov\t0x1234(%rip),%ebx", "8B 1D 34 12 00 00");
        }

        [Test]
        public void X86AttSx_minvalue_offset()
        {
            AssertEqual64("mov\t-0x80000000(%rbx),%eax", "8B 83 00 00 00 80");
        }

        // (re+rz+c): 
    }
}
