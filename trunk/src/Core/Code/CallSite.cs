/* 
 * Copyright (C) 1999-2007 John Källén.
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
	/// depth before the call.
	/// </summary>
	public class CallSite
	{
		public int StackDepthBefore;	// Depth of stack before call.
		public int FpuStackDepthBefore;	// Depth of FPU stack before call.
		public Identifier [] Usex;		// registers used by procedure.
		public Identifier [] Def;		// registers defined by procedure.

		public CallSite(int stackDepthBefore, int fpuStackDepthBefore)
		{
			this.StackDepthBefore = stackDepthBefore;
			this.FpuStackDepthBefore = fpuStackDepthBefore;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("depth: {0};", StackDepthBefore);
			if (FpuStackDepthBefore != 0)
			{
				sb.AppendFormat(" FPU: {0};", FpuStackDepthBefore);
			}
			if (Usex != null && Usex.Length > 0)
			{
				sb.Append(" (Use");
				foreach (Identifier id in Usex)
				{
					sb.AppendFormat(" {0}", id);
				}
				sb.Append(")");
			}
			if (Def != null && Def.Length > 0)
			{
				sb.Append(" (Def");
				foreach (Identifier id in Def)
				{
					sb.AppendFormat(" {0}", id);
				}
				sb.Append(")");
			}
			return sb.ToString();
		}
	}

}