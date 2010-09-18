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
using System;
using System.Diagnostics;

namespace Decompiler.Arch.Intel
{
	/// <summary>
	/// Keeps track of internal state for the IntelRewriter. Useful when forking at the
	/// rewriting of branches and switch statements.
	/// </summary>
	public class IntelRewriterState : RewriterState
	{
		private Frame frame;
		private int fpuStackDepth;
		private Address addrCur;

		public IntelRewriterState(Frame frame)
		{
			this.frame = frame;
			fpuStackDepth = 0;
			FrameRegister = MachineRegister.None;
		}

		public Address InstructionAddress
		{
			get { return addrCur; } 
			set { addrCur = value; }
		}

		public IntelRewriterState Clone()
		{
			IntelRewriterState state = new IntelRewriterState(frame);
			state.StackBytes = StackBytes;
			state.fpuStackDepth = fpuStackDepth;
			state.FrameRegister = FrameRegister;
            state.FrameOffset = FrameOffset;
			if (addrCur != null)
				state.addrCur = new Address(addrCur.Selector, addrCur.Offset);
			return state;
		}

		public ushort CodeSegment
		{
			get { return addrCur.Selector; }
		}

		/// <summary>
		/// Implements a frame shift.
		/// </summary>
		/// <param name="regFrame">Register used as a frame register</param>
		public void EnterFrame(MachineRegister regFrame)
		{
            if (FrameRegister == MachineRegister.None)
            {
                frame.FrameOffset = StackBytes;
            }
            FrameOffset = StackBytes;
			FrameRegister = regFrame;
		}

		public void LeaveFrame()
		{
			FrameRegister = MachineRegister.None;
			StackBytes = frame.FrameOffset;
            FrameOffset = 0;
		}

		public int FpuStackItems
		{
			get { return fpuStackDepth; }
			set { fpuStackDepth = value; }
		}

        /// <summary>
        /// The difference between the frame pointer and the stack pointer at the time the procedure was entered.
        /// </summary>
        public int FrameOffset { get; set; }

        public MachineRegister FrameRegister { get; set; }

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
			StackBytes += cb;
		}


		public void ShrinkFpuStack(int cItems)
		{
			fpuStackDepth -= cItems;
		}

		public void ShrinkStack(int cb)
		{
			StackBytes -= cb;
		}

		/// <summary>
		/// Number of bytes on the processor stack.
		/// </summary>
		public int StackBytes { get; set; }
	}
}
