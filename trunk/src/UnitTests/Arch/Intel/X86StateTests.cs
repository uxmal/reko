#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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
using Decompiler.Core.Expressions;
using Decompiler.Core;
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
        [Test]
        public void State()
        {
            var state = new X86State();
            state.SetRegister(Registers.esp, Constant.Word32(-4));
            state.OnProcedureEntered();
            var site = state.OnBeforeCall(4);
            Assert.AreEqual(4, site.SizeOfReturnAddressOnStack);
        }

        [Test]
        public void StackUnderflow_ReportError()
        {
            string reportedError = null;
            var state = new X86State
            {
                ErrorListener = (err) => { reportedError = err; }
            };
            state.OnProcedureEntered();
            state.SetRegister(Registers.esp, Constant.Word32(-4));
            var site = state.OnBeforeCall(4);
            state.OnAfterCall(new ProcedureSignature
            {
                StackDelta = 16,
            });
            Assert.IsNotNull(reportedError);
        }
    }
}
