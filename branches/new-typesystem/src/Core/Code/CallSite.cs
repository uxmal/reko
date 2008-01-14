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

using System;
using System.Text;

namespace Decompiler.Core.Code
{
	/// <summary>
	/// Interface between a calling procedure and a callee procedure. All registers
	/// used or defined by the called procedure are stored here, as is the stack
	/// depth before the call. The stack depth includes any return address pushed
	/// on the stack before control transfers to the callee. 
	/// </summary>
	public class CallSite
	{
		private int stackDepthBefore;	// Depth of stack before call.
		private int fpuStackDepthBefore;	// Depth of FPU stack before call.

		public CallSite(int stackDepthBefore, int fpuStackDepthBefore)
		{
			this.stackDepthBefore = stackDepthBefore;
			this.fpuStackDepthBefore = fpuStackDepthBefore;
		}

		/// <summary>
		/// Depth of FPU stack before call.
		/// </summary>
		public int FpuStackDepthBefore
		{
			get { return fpuStackDepthBefore; }
		}

		/// <summary>
		/// Depth of stack before call, including possible return address.
		/// </summary>
		public int StackDepthBefore
		{
			get { return stackDepthBefore; }
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("depth: {0};", StackDepthBefore);
			if (FpuStackDepthBefore != 0)
			{
				sb.AppendFormat(" FPU: {0};", FpuStackDepthBefore);
			}
			return sb.ToString();
		}
	}

}