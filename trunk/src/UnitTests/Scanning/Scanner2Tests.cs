#region License
/* 
 * Copyright (C) 1999-2010 John Källén.
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
using Decompiler.Core.Machine;
using Decompiler.Scanning;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Scanning
{
    [TestFixture]   
    public class Scanner2Tests
    {
        ArchitectureMock arch;

        [SetUp]
        public void Setup()
        {
            arch = new ArchitectureMock();
        }

        [Test]
        public void Create()
        {
            var sc = new Scanner2(arch);
        }

        [Test]
        public void AddEntryPoint()
        {

            arch.DisassemblyStream = new MachineInstruction[] {
                new FakeInstruction(Operation.Add,
                    new RegisterOperand(arch.GetRegister(1)), 
                    ImmediateOperand.Word32(1))
            };

            var sc = new Scanner2(arch);
            sc.EnqueueEntryPoint(
                new EntryPoint(
                    new Address(0x12314),
                    arch.CreateProcessorState()));
            sc.ProcessQueue();

            Assert.AreEqual(1, sc.Procedures.Count);
            Assert.AreEqual(0x12314, sc.Procedures.Keys[0].Offset);
        }
    }
}
