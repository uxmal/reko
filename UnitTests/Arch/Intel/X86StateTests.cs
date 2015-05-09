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

using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Arch.Intel
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
        public void OnBeforeCall_DecrementStackRegister()
        {
            var arch = new IntelArchitecture(ProcessorMode.Protected32);
            var state = new X86State(arch);
            var esp = CreateId(Registers.esp);
            state.SetRegister(Registers.esp, Constant.Word32(-4));
            state.OnProcedureEntered();
            var site = state.OnBeforeCall(esp, 4);
            Assert.AreEqual(4, site.SizeOfReturnAddressOnStack);
            Assert.AreEqual("0xFFFFFFFC", state.GetValue(esp).ToString());
        }

        [Test]
        public void StackUnderflow_ReportError()
        {
            var arch = new IntelArchitecture(ProcessorMode.Protected32);
            string reportedError = null;
            var state = new X86State(arch)
            {
                ErrorListener = (err) => { reportedError = err; }
            };
            state.OnProcedureEntered();
            state.SetRegister(Registers.esp, Constant.Word32(-4)); // Push only 4 bytes
            var esp = CreateId(Registers.esp);
            var site = state.OnBeforeCall(esp, 4);
            state.OnAfterCall(esp, new ProcedureSignature
            {
                StackDelta = 16,                        // ...but pop 16 bytes
            },
            new Decompiler.Evaluation.ExpressionSimplifier(state)); //$TODO: hm. Move simplification out of state.
            Assert.IsNotNull(reportedError);
        }

        [Test]
        public void Simple()
        {
            var arch = new IntelArchitecture(ProcessorMode.Real);

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
        public void AreEqual()
        {
            var arch = new IntelArchitecture(ProcessorMode.Real);
            X86State st1 = new X86State(arch);
            X86State st2 = new X86State(arch);
            Assert.IsTrue(st1.HasSameValues(st2));
        }
    }
}
