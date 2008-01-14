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
using Decompiler.Core.Code;
using System;

namespace Decompiler.Typing
{
	/// <summary>
	/// Locates array expressions and converts them to array access expressions. 
	/// This simplifies work for later stages of the type inference, when we want to identify
	/// array accesses quickly.
	/// </summary>
	public class ArrayExpressionNormalizer : InstructionTransformer
	{
		private ArrayExpressionMatcher aem = new ArrayExpressionMatcher();

		public override Expression TransformMemoryAccess(MemoryAccess access)
		{
			access.EffectiveAddress = access.EffectiveAddress.Accept(this);
			if (aem.Match(access.EffectiveAddress))
			{
				return aem.Transform(access.DataType);
			}
			else
				return access;
		}
	
		public void Transform(Program prog)
		{
			foreach (Procedure proc in prog.Procedures.Values)
			{
				foreach (Block b in proc.RpoBlocks)
				{
					foreach (Statement stm in b.Statements)
					{
						stm.Instruction = stm.Instruction.Accept(this);
					}
				}
			}
		}
	}
}
