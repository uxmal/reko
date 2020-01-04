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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Intel
{
    [TestFixture]
    public class X86FrameApplicationBuilderTests
    {
        private IntelArchitecture arch;
        private Identifier callee;
        private X86FrameApplicationBuilder fab;
        private Frame frame;

        [SetUp]
        public void Setup()
        {
            this.arch = new X86ArchitectureFlat32("x86-protected-32");
            this.frame = arch.CreateFrame();
            this.callee = frame.EnsureRegister(Registers.eax);
        }

        private void CreateApplicationBuilder(CallSite site)
        {
            this.fab = new X86FrameApplicationBuilder(
                arch,
                frame,
                site,
                callee,
                false);
        }

        [Test(Description = "TOP + 0 is the first FPU argument to a function")]
        [Category(Categories.UnitTests)]
        public void X86fab_FpuArg()
        {
            var site = new CallSite(0, 1);
            CreateApplicationBuilder(site);
            var sigCallee = FunctionType.Action(
                new Identifier(
                    "arg",
                    PrimitiveType.Real64,
                    new FpuStackStorage(0, PrimitiveType.Real64)));
            var instr = fab.CreateInstruction(sigCallee, new ProcedureCharacteristics());
            Assert.AreEqual("eax(ST[Top:real64])", instr.ToString());
        }

        [Test(Description = "FPU arguments will have values 0..7")]
        [Category(Categories.UnitTests)]
        public void X86fab_FpuArgs()
        {
            var site = new CallSite(0, 4);
            CreateApplicationBuilder(site);
            var sigCallee = FunctionType.Action(
                new Identifier(
                    "arg0",
                    PrimitiveType.Real64,
                    new FpuStackStorage(0, PrimitiveType.Real64)),
                new Identifier(
                    "arg1",
                    PrimitiveType.Real64,
                    new FpuStackStorage(1, PrimitiveType.Real64)));
            var instr = fab.CreateInstruction(sigCallee, new ProcedureCharacteristics());
            Assert.AreEqual("eax(ST[Top:real64], ST[Top + 1:real64])", instr.ToString());
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void X86fab_FpuRet()
        {
            var site = new CallSite(0, 0);
            CreateApplicationBuilder(site);
            var sigCallee = FunctionType.Func(
                new Identifier(
                    "",         // return values don't have names!
                    PrimitiveType.Real64,
                    new FpuStackStorage(0, PrimitiveType.Real64)));
            sigCallee.FpuStackDelta = 1;
            var instr = fab.CreateInstruction(sigCallee, new ProcedureCharacteristics());
            // Top below refers to the value of Top _before_ the call.
            // Rewriters must remember to emit an instruction to adjust  Top
            // _after_ the call.
            Assert.AreEqual("ST[Top - 1:real64] = eax()", instr.ToString());
        }
    }
}