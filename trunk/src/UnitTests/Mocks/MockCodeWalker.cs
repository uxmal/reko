/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core;
using System;
using System.Collections;

namespace Decompiler.UnitTests.Mocks
{
	/// <summary>
	/// A mock code walker.
	/// </summary>
	/// <remarks>
	/// The code walker is "primed" by making calls to the various "Add..." methods. When walkinstruction
	/// is then called, it will call the corresponding methods of the ICodeWalkerListener interface.
	/// </remarks>
	public class MockCodeWalker : CodeWalker
	{
		private Address address;
		private SortedList callbacks;


		public MockCodeWalker(Address addr, ICodeWalkerListener listener) : base(listener)
		{
			if (listener == null)
				throw new ArgumentNullException("listener");
			address = addr;
			callbacks = new SortedList();
		}

		public override Address Address
		{
			get { return address; }
		}

		public void AddCall(Address addrInstr, Address addrCallee)
		{
			AddCall(addrInstr, addrCallee, new FakeProcessorState());
		}

		public void AddCall(Address addrInstr, Address addrCallee, ProcessorState st)
		{
			callbacks.Add(addrInstr, new Call(addrCallee, st));
		}

		public void AddJump(Address addrInstr, Address addrJumpTarget)
		{
			AddJump(addrInstr, addrJumpTarget, new FakeProcessorState());
		}

		public void AddJump(Address addrInstr, Address addrJumpTarget, ProcessorState st)
		{
			callbacks.Add(addrInstr, new Jump(addrInstr, addrJumpTarget, st));
		}

		public void AddReturn(Address addr)
		{
			callbacks.Add(addr, new Return(addr));
		}

		public void SetWalkAddress(Address addr)
		{
			address = addr; 
		}
		
		public override void WalkInstruction()
		{
			Console.WriteLine("Walking {0}", address);
			MockInstruction i = (MockInstruction) callbacks[address];
			if (i != null)
			{
				Console.WriteLine(" executing {0}", i.GetType().Name);
				i.Execute(Listener);
			}
			address = address + 1;
		}

		private abstract class MockInstruction
		{
			public abstract void Execute(ICodeWalkerListener listener);
		}

		private class Return : MockInstruction
		{
			private Address retAddr; 

			public Return(Address addr)
			{
				retAddr = addr;
			}

			public override void Execute(ICodeWalkerListener listener)
			{
				listener.OnReturn(retAddr);
			}
		}

		private class Call : MockInstruction
		{
			private Address addrCallee;
			private ProcessorState st;

			public Call(Address addr, ProcessorState st)
			{
				addrCallee = addr;
				this.st = st;
			}

			public override void Execute(ICodeWalkerListener listener)
			{
				listener.OnProcedure(st, addrCallee);
			}
		}

		private class Jump : MockInstruction
		{
			private ProcessorState st;
			private Address addrInstr;
			private Address addrJumpTarget;

			public Jump(Address addrInstr, Address addrJmp, ProcessorState st)
			{
				this.addrInstr = addrInstr;
				this.addrJumpTarget = addrJmp;
				this.st = st;
			}

			public override void Execute(ICodeWalkerListener listener)
			{
				listener.OnJump(st, addrInstr, addrInstr+1, addrJumpTarget);
			}

		}

	}
}
