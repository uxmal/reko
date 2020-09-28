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

using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Intel
{
    [TestFixture]
    public class X86StateTests
    {
        public X86StateTests()
        {
        }

        private Identifier CreateId(RegisterStorage reg)
        {
            return new Identifier(reg.Name, reg.DataType, reg);
        }

        [Test]
        public void X86St_OnBeforeCall_DecrementStackRegister()
        {
            var arch = new X86ArchitectureFlat32("x86-protected-32");
            var state = new X86State(arch);
            var esp = CreateId(Registers.esp);
            state.SetRegister(Registers.esp, Constant.Word32(-4));
            state.OnProcedureEntered();
            var site = state.OnBeforeCall(esp, 4);
            Assert.AreEqual(4, site.SizeOfReturnAddressOnStack);
            Assert.AreEqual("0xFFFFFFFC", state.GetValue(esp).ToString());
        }

        [Test]
        public void X86St_Simple()
        {
            var arch = new X86ArchitectureReal("x86-real-16");

            X86State st = new X86State(arch);
            st.SetRegister(Registers.cs, Constant.Word16(0xC00));
            st.SetRegister(Registers.ax, Constant.Word16(0x1234));
            Assert.IsTrue(!st.GetRegister(Registers.bx).IsValid);
            Assert.IsTrue(st.GetRegister(Registers.ax).IsValid);
            Assert.IsTrue(st.GetRegister(Registers.al).IsValid);
            Assert.AreEqual(0x34, st.GetRegister(Registers.al).ToUInt32());
            Assert.IsTrue(st.GetRegister(Registers.ah).IsValid);
            Assert.AreEqual(0x12, st.GetRegister(Registers.ah).ToUInt32());
        }

        [Test]
        public void X86St_SetAhRegisterFileValue()
        {
            var state = new X86State(new X86ArchitectureFlat64("x86-protected-64"));
            state.SetRegister(Registers.ah, Constant.Byte(0x3A));
            Assert.IsFalse(state.IsValid(Registers.ax));
            Assert.IsTrue(state.IsValid(Registers.ah));
        }

        [Test]
        public void X86St_SetAhThenAl()
        {
            var state = new X86State(new X86ArchitectureFlat64("x86-protected-64"));
            state.SetRegister(Registers.ah, Constant.Byte(0x12));
            state.SetRegister(Registers.al, Constant.Byte(0x34));
            Assert.IsTrue(state.IsValid(Registers.ax));
            Assert.IsTrue(state.IsValid(Registers.al));
            Assert.IsTrue(state.IsValid(Registers.ah));
        }

        [Test]
        public void X86St_SetBp()
        {
            var state = new X86State(new X86ArchitectureFlat64("x86-protected-64"));
            state.SetRegister(Registers.bp, Constant.Word16(0x1234));
            Assert.IsFalse(state.IsValid(Registers.ebp));
            Assert.IsTrue(state.IsValid(Registers.bp));
        }

        [Test]
        public void X86St_SetCx()
        {
            var state = new X86State(new X86ArchitectureFlat64("x86-protected-64"));
            state.SetRegister(Registers.cx, Constant.Word16(0x1234));
            Assert.AreEqual(0x1234, (int)state.GetRegister(Registers.cx).ToUInt16());
            Assert.AreEqual(0x34, (int)state.GetRegister(Registers.cl).ToByte());
            Assert.AreEqual(0x12, (int)state.GetRegister(Registers.ch).ToByte());
            Assert.IsTrue(state.IsValid(Registers.cx));
            Assert.IsTrue(state.IsValid(Registers.cl));
            Assert.IsTrue(state.IsValid(Registers.ch));
        }

        [Test]
        public void X86St_SetEsi()
        {
            var state = new X86State(new X86ArchitectureFlat64("x86-protected-64"));
            state.SetRegister(Registers.esi, Constant.Word32(0x12345678));
            Assert.AreEqual(0x12345678, (long)state.GetRegister(Registers.esi).ToUInt64());
            Assert.AreEqual(0x5678, (int)state.GetRegister(Registers.si).ToUInt32());
        }

        [Test]
        public void X86St_SetEdx()
        {
            var state = new X86State(new X86ArchitectureFlat64("x86-protected-64"));
            state.SetRegister(Registers.edx, Constant.Word32(0x12345678));
            Assert.AreEqual(0x12345678, (long)state.GetRegister(Registers.edx).ToUInt64());
            Assert.AreEqual(0x5678, (int)state.GetRegister(Registers.dx).ToUInt32());
            Assert.AreEqual(0x78, (int)state.GetRegister(Registers.dl).ToUInt32());
            Assert.AreEqual(0x56, (int)state.GetRegister(Registers.dh).ToUInt32());
        }

        [Test]
        public void X86St_SetCxSymbolic_Invalid()
        {
            var ctx = new Dictionary<Storage, Expression> {
                { Registers.cl, Constant.Byte(0) }
            };
            var state = new X86State(new X86ArchitectureFlat64("x86-protected-64"));
            state.SetRegister(Registers.cx, Constant.Invalid);
            Assert.IsFalse(state.IsValid(Registers.cx));
            Assert.IsFalse(state.IsValid(Registers.cl));
            Assert.IsFalse(state.IsValid(Registers.ch));
            Assert.IsFalse(state.IsValid(Registers.ecx));
        }

        [Test]
        public void X86St_SetDhSymbolic_Invalid()
        {
            var ctx = new Dictionary<Storage, Expression> {
                { Registers.dl, Constant.Byte(3) }
            };
            var state = new X86State(new X86ArchitectureFlat64("x86-protected-64"));
            state.SetRegister(Registers.dh, Constant.Invalid);
            Assert.IsFalse(state.IsValid(Registers.dh));
            Assert.IsFalse(state.IsValid(Registers.dx));
            Assert.IsFalse(state.IsValid(Registers.edx));
            Assert.AreEqual("0x03", ctx[Registers.dl].ToString());
        }

    }
}
