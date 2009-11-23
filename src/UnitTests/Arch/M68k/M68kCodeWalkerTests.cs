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

using Decompiler.Arch.M68k;
using Decompiler.Assemblers.M68k;
using Decompiler.Core;
using Decompiler.Core.Assemblers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Arch.M68k
{
    [TestFixture]
    public class M68kCodeWalkerTests
    {
        [Test]
        public void Branch()
        {
            Emitter emitter = new Emitter();
            Address addr = new Address(0x10000000);
            AssemblerImpl asm = new AssemblerImpl(addr, emitter);
            asm.BraB("foo");
            asm.Move(Registers.d0, Registers.d1);
            asm.Label("foo");
            asm.Move(Registers.d1, Registers.d2);

            M68kCodeWalker walker = new M68kCodeWalker(asm.Image, null, addr, new M68kState());
            TestListener listener = new TestListener();
            walker.WalkInstruction(listener);
            Assert.IsTrue(listener.OnJumpCalled);
            Assert.AreEqual(new Address(0x10000004), listener.JumpTarget);
        }

        private class TestListener : ICodeWalkerListener
        {
            public bool OnJumpCalled;
            public Address JumpTarget;

            #region ICodeWalkerListener Members

            public void OnBranch(ProcessorState st, Address addrInstr, Address addrTerm, Address addrBranch)
            {
                throw new NotImplementedException();
            }

            public void OnGlobalVariable(Address addr, Decompiler.Core.Types.PrimitiveType width, Decompiler.Core.Code.Constant c)
            {
                throw new NotImplementedException();
            }

            public void OnIllegalOpcode(Address addrIllegal)
            {
                throw new NotImplementedException();
            }

            public void OnJump(ProcessorState st, Address addrInstr, Address addrTerm, Address addrJump)
            {
                OnJumpCalled = true;
                JumpTarget = addrJump;
            }

            public void OnJumpPointer(ProcessorState st, Address segBase, Address addrPtr, Decompiler.Core.Types.PrimitiveType stride)
            {
                throw new NotImplementedException();
            }

            public void OnJumpTable(ProcessorState st, Address addrInstr, Address addrTable, ushort segBase, Decompiler.Core.Types.PrimitiveType stride)
            {
                throw new NotImplementedException();
            }

            public void OnProcedure(ProcessorState st, Address addr)
            {
                throw new NotImplementedException();
            }

            public void OnProcedurePointer(ProcessorState st, Address addrBase, Address addrPtr, Decompiler.Core.Types.PrimitiveType stride)
            {
                throw new NotImplementedException();
            }

            public void OnProcedureTable(ProcessorState st, Address addrInstr, Address addrTable, ushort segBase, Decompiler.Core.Types.PrimitiveType stride)
            {
                throw new NotImplementedException();
            }

            public void OnProcessExit(Address addrTerm)
            {
                throw new NotImplementedException();
            }

            public void OnReturn(Address addrTerm)
            {
                throw new NotImplementedException();
            }

            public void OnSystemServiceCall(Address addrInstr, SystemService svc)
            {
                throw new NotImplementedException();
            }

            public void OnTrampoline(ProcessorState st, Address addrInstr, Address addrGlob)
            {
                throw new NotImplementedException();
            }

            public void Warn(Address addr, string format, params object[] args)
            {
                throw new NotImplementedException();
            }

            #endregion
        }


    }
}
