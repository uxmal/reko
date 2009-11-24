/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Arch.Intel;
using Decompiler.Assemblers.x86;
using Decompiler.Core;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Assemblers.x86
{
    [TestFixture]
    public class IntelCodeAssemblerTests : AssemblerBase
    {
        IntelEmitter emitter;
        IntelAssembler m;

        [SetUp]
        public new void Setup()
        {
            base.Setup();
            emitter = new IntelEmitter();
            m = new IntelAssembler(new IntelArchitecture(ProcessorMode.Real), PrimitiveType.Word16, new Address(0x100, 0x0100), emitter, new List<EntryPoint>());
        }

        [Test]
        public void MovRegReg()
        {
            m.Mov(Reg(Registers.ax), Reg(Registers.bx));
            AssertEqualBytes("8BC3", emitter.Bytes);
        }

        private ParsedOperand Reg(IntelRegister reg)
        {
            return new ParsedOperand(new RegisterOperand(reg));
        }

        [Test]
        public void MovRegConst()
        {
            m.Mov(Reg(Registers.ax), 0x300);
            AssertEqualBytes("B80003", emitter.Bytes);
        }

        [Test]
        public void MovMemReg()
        {
            m.Mov(m.BytePtr(0x0300), 0x12);
            AssertEqualBytes("C606000312", emitter.Bytes);
        }

    }
}
