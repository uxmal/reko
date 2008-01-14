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
using System.Diagnostics;

namespace Decompiler.Arch.Intel
{
	/// <summary>
	/// Keeps track of internal state for the IntelRewriter. Useful when forking at the
	/// rewriting of branches and switch statements.
	/// </summary>
	public class RewriterState
	{
		private Frame frame;
		private int cbStackDepth;
		private int fpuStackDepth;
		private IntelRegister frameReg;
		private Address addrCur;

		public RewriterState(Frame frame)
		{
			this.frame = frame;
			cbStackDepth = 0;
			fpuStackDepth = 0;
			frameReg = Registers.None;
		}

		public Address InstructionAddress
		{
			get { return addrCur; }
			set { addrCur = value; }
		}

		public RewriterState Clone()
		{
			RewriterState state = new RewriterState(frame);
			state.cbStackDepth = cbStackDepth;
			state.fpuStackDepth = fpuStackDepth;
			state.frameReg = frameReg;
			if (addrCur != null)
				state.addrCur = new Address(addrCur.seg, addrCur.off);
			return state;
		}

		public ushort CodeSegment
		{
			get { return addrCur.seg; }
		}

		/// <summary>
		/// Implements a frame shift.
		/// </summary>
		/// <param name="regFrame">Register used as a frame register</param>
		public void EnterFrame(IntelRegister regFrame)
		{
			FrameRegister = regFrame;
			frame.FrameOffset = StackBytes;
		}

		public void LeaveFrame()
		{
			Debug.Assert(FrameRegister != Registers.None);
			FrameRegister = Registers.None;
			StackBytes = frame.FrameOffset;
		}

		public int FpuStackItems
		{
			get { return fpuStackDepth; }
			set { fpuStackDepth = value; }
		}

		public IntelRegister FrameRegister
		{
			get { return frameReg; }
			set { frameReg = value; }
		}

		public void GrowFpuStack(Address addrInstr)
		{
			++fpuStackDepth;
			if (fpuStackDepth > 7)
			{
				Debug.WriteLine(string.Format("Possible FPU stack overflow at address {0}", addrInstr));	//$BUGBUG: should be an exception
			}
		}

		public void GrowStack(int cb)
		{
			cbStackDepth += cb;
		}


		public void ShrinkFpuStack(int cItems)
		{
			fpuStackDepth -= cItems;
		}

		public void ShrinkStack(int cb)
		{
			cbStackDepth -= cb;
		}

		/// <summary>
		/// Number of bytes on the processor stack.
		/// </summary>
		public int StackBytes
		{
			get { return cbStackDepth; }
			set { cbStackDepth = value; }
		}
	}
}
