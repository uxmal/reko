#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Arch.Arm;
using Reko.Arch.Arm.AArch64;
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Arm
{
    [TestFixture]
    public class Arm64StateTests
    {
        private Arm64Architecture arch = new Arm64Architecture(new ServiceContainer(), "aarch64", new());

        [Test]
        public void A64State_GetUninitialized()
        {
            var state = new Arm64State(arch);
            var v = state.GetValue(Registers.GpRegs64[0]);
            Assert.IsTrue(v is InvalidConstant); 
        }

        [Test]
        public void A64State_SetGetSameRegister_64bits()
        {
            var state = new Arm64State(arch);
            var reg = Registers.GpRegs64[0];
            state.SetRegister(reg, Constant.UInt64(0x0123456789ABCDEFu));
            var v = (Constant) state.GetValue(reg);
            Assert.IsFalse(v is InvalidConstant);
            Assert.AreEqual(0x0123456789ABCDEFu, v.ToUInt64());
        }

        [Test]
        public void A64State_SetGetSameRegister_32bits()
        {
            var state = new Arm64State(arch);
            var xreg = Registers.GpRegs64[0];
            var wreg = Registers.GpRegs32[0];
            state.SetRegister(xreg, Constant.UInt64(0x0123456789ABCDEFu));
            var v = (Constant) state.GetValue(wreg);
            Assert.IsFalse(v is InvalidConstant);
            Assert.AreEqual(0x89AB_CDEFu, v.ToUInt64());
        }

        [Test]
        public void A64State_SetLowBits()
        {
            var state = new Arm64State(arch);
            var xreg = Registers.GpRegs64[0];
            var wreg = Registers.GpRegs32[0];
            state.SetRegister(xreg, Constant.UInt64(0x42424242_42424242u));
            state.SetRegister(wreg, Constant.UInt32(0x12345678u));
            var v = (Constant) state.GetValue(xreg);
            Assert.IsFalse(v is InvalidConstant);
            Assert.AreEqual(0x42424242_12345678u, v.ToUInt64());

        }
    }
}
