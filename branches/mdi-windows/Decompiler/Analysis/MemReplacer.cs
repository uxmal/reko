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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Placeholder that replaces MEM nodes with C-like equivalents. It's placeholder because 
	/// it does no real intelligent work trying to figure out what types the objects have. 
	/// When type analysis is complete, this class should be removed.
	/// </summary>
	public class MemReplacer : InstructionTransformer
	{
		private Program prog;

		public MemReplacer(Program prog) 
		{
			this.prog = prog; 
		}

		public void Transform(Procedure proc)
		{
			foreach (Block block in proc.RpoBlocks)
			{
				foreach (Statement stm in block.Statements)
				{
					stm.Instruction.Accept(this);
				}
			}
		}

		public void RewriteProgram()
		{
			foreach (Procedure proc in prog.Procedures.Values)
			{
				Transform(proc);
			}
		}

		public override Expression TransformMemoryAccess(MemoryAccess access)
		{
			Expression ea = access.EffectiveAddress.Accept(this);
			DataType type = access.DataType;
			Constant c = ea as Constant;
			if (c != null)
			{
				return new Identifier(string.Format("g_{0}{1:X8}", 0, type.Prefix, c.ToUInt32()), 0, type, new MemoryStorage());
			}
			Identifier id = ea as Identifier;
			if (id != null)
			{
				return new FieldAccess(type, new Dereference(type, id), string.Format("{0}00000000", type.Prefix));
			}
			BinaryExpression b = ea as BinaryExpression;
			if (b != null)
			{
				c = b.Right as Constant;
				if (c != null && b.op == Operator.add)
					return new FieldAccess(type, new Dereference(type, b.Left), string.Format("{0}{1:X8}", type.Prefix, c.ToUInt32()));
			}
			return new Dereference(null, ea);
		}
	}
}
