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

namespace Decompiler.Arch.Intel
{
	/// <summary>
	/// Compute dead-conditioncodes. This helps us avoid emitting a lot of 
	///  SCZ = fn(eax);
	/// statements if we know that the condition codes are never used later
	/// in the same basic block.
	/// </summary>
	public class DeadConditionFlagsFinder
	{
		public FlagM [] DeadOutFlags(IntelInstruction [] instrs)
		{
			FlagM [] deadOutflags = new FlagM[instrs.Length];
			uint grfDeadIn = 0;
			for (int i = instrs.Length - 1; i > 0;)
			{
				grfDeadIn |= (uint) instrs[i].DefCc();
				grfDeadIn &= ~(uint) instrs[i].UseCc();

				// Special case: when calling, we don't know what flags
				// may or may not be used by the called function, so we
				// pessimistically lose all deadness information. The author
				// has personally never seen any functions that use
				// condition code flags as in parameters, but such code
				// may exist. 

				if (instrs[i].code == Opcode.call)
					grfDeadIn = 0;
				deadOutflags[--i] = (FlagM) grfDeadIn;
			}
			return deadOutflags;
		}

	}
}
