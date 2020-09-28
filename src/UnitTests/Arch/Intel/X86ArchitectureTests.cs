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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.Text;

namespace Reko.UnitTests.Arch.Intel
{
	[TestFixture]
	public class X86ArchitectureTests
	{
		private IntelArchitecture arch;

		public X86ArchitectureTests()
		{
			arch = new X86ArchitectureReal("x86-real-16");
		}

		[Test]
		public void IaCreate()
		{
			arch = new X86ArchitectureReal("x86-real-16");
			Assert.AreEqual(PrimitiveType.Word16, arch.WordWidth);
			arch = new X86ArchitectureFlat32("x86-protected-32");
			Assert.AreEqual(PrimitiveType.Word32, arch.WordWidth);
            arch = new X86ArchitectureProtected16("x86-protected-16");
			Assert.AreEqual(PrimitiveType.Word16, arch.WordWidth);
		}

		[Test]
		public void IaGetRegisterFromString()
		{
			Assert.AreSame(Registers.eax, arch.GetRegister("eax"));
			Assert.AreSame(Registers.ecx, arch.GetRegister("ecx"));
			Assert.AreSame(Registers.edx, arch.GetRegister("edx"));
			Assert.AreSame(Registers.ebx, arch.GetRegister("ebx"));
			Assert.AreSame(Registers.esp, arch.GetRegister("esp"));
			Assert.AreSame(Registers.ebp, arch.GetRegister("ebp"));
			Assert.AreSame(Registers.esi, arch.GetRegister("esi"));
			Assert.AreSame(Registers.edi, arch.GetRegister("edi"));
			Assert.AreSame(Registers.ax, arch.GetRegister("ax"));
			Assert.AreSame(Registers.cx, arch.GetRegister("cx"));
			Assert.AreSame(Registers.dx, arch.GetRegister("dx"));
			Assert.AreSame(Registers.bx, arch.GetRegister("bx"));
			Assert.AreSame(Registers.sp, arch.GetRegister("sp"));
			Assert.AreSame(Registers.bp, arch.GetRegister("bp"));
			Assert.AreSame(Registers.si, arch.GetRegister("si"));
			Assert.AreSame(Registers.di, arch.GetRegister("di"));
		}

        [Test]
        public void IaFailGetRegisterFromString()
        {
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));
        }

		[Test]
		public void IaGetRegisterFromName()
		{
			Assert.AreEqual("eax", arch.GetRegister("eax").Name);
			Assert.AreEqual("ecx", arch.GetRegister("eCx").Name);
			Assert.AreEqual("edx", arch.GetRegister("edx").Name);
			Assert.AreEqual("ebx", arch.GetRegister("Ebx").Name);
			Assert.AreEqual("esp", arch.GetRegister("esp").Name);
			Assert.AreEqual("ebp", arch.GetRegister("ebp").Name);
			Assert.AreEqual("esi", arch.GetRegister("esi").Name);
			Assert.AreEqual("edi", arch.GetRegister("edi").Name);
			Assert.AreEqual("ax", arch.GetRegister("ax").Name);
			Assert.AreEqual("cx", arch.GetRegister("cx").Name);
			Assert.AreEqual("dx", arch.GetRegister("dx").Name);
			Assert.AreEqual("bx", arch.GetRegister("bx").Name);
			Assert.AreEqual("sp", arch.GetRegister("sp").Name);
			Assert.AreEqual("bp", arch.GetRegister("bp").Name);
			Assert.AreEqual("si", arch.GetRegister("si").Name);
			Assert.AreEqual("di", arch.GetRegister("di").Name);
			Assert.AreEqual("al", arch.GetRegister("al").Name);
			Assert.AreEqual("cl", arch.GetRegister("cl").Name);
			Assert.AreEqual("dl", arch.GetRegister("dl").Name);
			Assert.AreEqual("bl", arch.GetRegister("bl").Name);
			Assert.AreEqual("ah", arch.GetRegister("ah").Name);
			Assert.AreEqual("ch", arch.GetRegister("ch").Name);
			Assert.AreEqual("dh", arch.GetRegister("dh").Name);
			Assert.AreEqual("bh", arch.GetRegister("bh").Name);
			Assert.AreEqual("es", arch.GetRegister("es").Name);
			Assert.AreEqual("cs", arch.GetRegister("cs").Name);
			Assert.AreEqual("ss", arch.GetRegister("ss").Name);
			Assert.AreEqual("ds", arch.GetRegister("ds").Name);
			Assert.AreEqual("fs", arch.GetRegister("fs").Name);
			Assert.AreEqual("gs", arch.GetRegister("gs").Name);
		}

		/// <summary>
		/// Gets a flag group from a single bit.
		/// </summary>
		[Test]
		public void IaGetFlagGroup1()
		{
			var s1 = arch.GetFlagGroup("S");
			var s2 = arch.GetFlagGroup("S");
			Assert.AreEqual("S", s1.Name);
			Assert.AreSame(s1, s2);
			Assert.AreEqual(PrimitiveType.Bool, s1.DataType);
		}


		[Test]
		public void IaGetFlagGroup2()
		{
			var sz = arch.GetFlagGroup("ZS");
			Assert.AreEqual("SZ", sz.Name);
			Assert.AreEqual(PrimitiveType.Byte, sz.DataType);
		}

        [Test]
        public void IaGetInvalidRegisterFromName()
        {
            Assert.Throws<ArgumentException>(() => arch.GetRegister("NonExistingRegisterName"));
        }

        private EndianImageReader CreateImageReader(params byte[] bytes)
        {
            return new LeImageReader(bytes, 0);
        }

        private EndianImageReader CreateImageReader(Address address, params byte[] bytes)
        {
            return new LeImageReader(new MemoryArea(address, bytes), 0);
        }

        [Test]
        public void ReadCodeAddress_RealMode_Offset()
        {
            arch = new X86ArchitectureReal("x86-real-16");
            var rdr = CreateImageReader(0x78, 0x56);
            var state = arch.CreateProcessorState();
            state.SetRegister(Registers.cs, Constant.Word16(0x1234));

            var addr = arch.ReadCodeAddress(2, rdr, state);

            Assert.AreEqual("1234:5678", addr.ToString());
        }


        [Test]
        public void ReadCodeAddress_RealMode_SegOffset()
        {
            arch = new X86ArchitectureReal("x86-real-16");
            var rdr = CreateImageReader(0x78, 0x56, 0x34, 0x12);
            var state = arch.CreateProcessorState();
            state.SetRegister(Registers.cs, Constant.Word16(0x1111));

            var addr = arch.ReadCodeAddress(4, rdr, state);

            Assert.AreEqual("1234:5678", addr.ToString());
        }

        [Test]
        public void ReadCodeAddress_ProtectedMode16_Offset()
        {
            arch = new X86ArchitectureProtected16("x86-protected-16");
            var rdr = CreateImageReader(0x78, 0x56);
            var state = arch.CreateProcessorState();
            state.SetRegister(Registers.cs, Constant.Word16(0x1234));

            var addr = arch.ReadCodeAddress(2, rdr, state);

            Assert.AreEqual("1234:5678", addr.ToString());
        }

        [Test]
        public void X86arch_ReadCodeAddress_ProtectedMode16_SegOffset()
        {
            arch = new X86ArchitectureProtected16("x86-protected-16");
            var rdr = CreateImageReader(0x78, 0x56, 0x34, 0x12);
            var state = arch.CreateProcessorState();
            state.SetRegister(Registers.cs, Constant.Word16(0x1111));

            var addr = arch.ReadCodeAddress(4, rdr, state);

            Assert.AreEqual("1234:5678", addr.ToString());
        }

        [Test]
        public void X86arch_ReadCodeAddress_ProtectedFlatMode32()
        {
            arch = new X86ArchitectureFlat32("x86-protected-32");
            var rdr = CreateImageReader(0x78, 0x56, 0x34, 0x12);
            var state = arch.CreateProcessorState();
            state.SetRegister(Registers.cs, Constant.Word16(0x1111));

            var addr = arch.ReadCodeAddress(4, rdr, state);

            Assert.AreEqual("12345678", addr.ToString());
        }

        [Test]
        public void X86arch_SetAxAliasesTrue()
        {
            arch = new X86ArchitectureFlat32("x86-protected-32");
            var aliases = EnumerableEx.ToHashSet(arch.GetAliases(Registers.ax));
            Assert.IsTrue(aliases.Contains(Registers.ax), "Expected ax set");
            Assert.IsTrue(aliases.Contains(Registers.ah), "Expected ah set");
            Assert.IsTrue(aliases.Contains(Registers.al), "Expected al set");
        }

        [Test]
        public void X86arch_GetOpcodeNames()
        {
            arch = new X86ArchitectureFlat32("x86-protected-32");
            Assert.AreEqual(
                "aaa,aad,aam,aas,adc,add",
                string.Join(",", arch.GetOpcodeNames().Keys.Take(6)));
        }

        [Test]
        public void X86arch_GetOpcodeNumber()
        {
            arch = new X86ArchitectureFlat32("x86-protected-32");
            Assert.AreEqual(
                Mnemonic.mov,
                (Mnemonic)arch.GetOpcodeNumber("mov"));
        }

        [Test(Description = "Inline calls to __x86.get_pc_thunk.bx")]
        public void X86Arch_Inline_x86get_pc_thunk_bx()
        {
            arch = new X86ArchitectureFlat32("x86-protected-32");
            var mem = new MemoryArea(Address.Ptr32(0x1000), new byte[]
            {
               0x8B, 0x1C, 0x24,        // mov ebx,[esp]
               0xC3                     // ret
            });
            var cluster = arch.InlineCall(Address.Ptr16(0x1000), Address.Ptr32(0x2305), mem.CreateLeReader(0), arch.CreateFrame());
            Assert.AreEqual(1, cluster.Count);
            Assert.AreEqual("ebx = 00002305", cluster[0].ToString());
        }

        [Test]
        public void X86Arch_GetDomain_Bitrange()
        {
            arch = new X86ArchitectureFlat32("x86-protected-32");
            var reg = arch.GetRegister(Registers.rbx.Domain, new BitRange(8, 16));
            Assert.AreEqual("bh", reg.Name);
        }
    }
}
