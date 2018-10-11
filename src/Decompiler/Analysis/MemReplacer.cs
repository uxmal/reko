#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;

namespace Reko.Analysis
{
	/// <summary>
	/// Placeholder that replaces MEM nodes with C-like equivalents. It's
    /// placeholder because it does no real intelligent work trying to figure
    /// out what types the objects have. When type analysis is complete, this
    /// class should be removed.
	/// </summary>
	public class MemReplacer : InstructionTransformer
	{
		private Program program;

		public MemReplacer(Program program) 
		{
			this.program = program; 
		}

		public void Transform(Procedure proc)
		{
            foreach (var stm in proc.Statements)
            {
                stm.Instruction.Accept(this);
            }
		}

		public void RewriteProgram()
		{
			foreach (Procedure proc in program.Procedures.Values)
			{
				Transform(proc);
			}
		}

		public override Expression VisitMemoryAccess(MemoryAccess access)
		{
			Expression ea = access.EffectiveAddress.Accept(this);
			DataType type = access.DataType;
			Constant c = ea as Constant;
			if (c != null)
			{
                var prefix = program.NamingPolicy.Types.ShortPrefix(type);
				return Identifier.Global(string.Format("g_{0}{1:X8}", prefix, c.ToUInt32()), type);
			}
			Identifier id = ea as Identifier;
			if (id != null)
			{
                return new FieldAccess(type, new Dereference(type, id), CreateField(type, 0));
			}
			BinaryExpression b = ea as BinaryExpression;
			if (b != null)
			{
				c = b.Right as Constant;
				if (c != null && b.Operator == Operator.IAdd)
					return new FieldAccess(type, new Dereference(type, b.Left), CreateField(type, c.ToInt32()));
			}
			return new Dereference(null, ea);
		}

        private Field CreateField(DataType type, int offset)
        {
            return new StructureField(offset, type);
        }

        public override Expression VisitSegmentedAccess(SegmentedAccess access)
        {
            Expression basePtr = access.BasePointer.Accept(this);
            Expression ea = access.EffectiveAddress.Accept(this);
            DataType type = access.DataType;

            Constant c = ea as Constant;
            if (c != null)
            {
                return new FieldAccess(type, new Dereference(type, basePtr), CreateField(type, c.ToInt16()));
            }
            BinaryExpression b = ea as BinaryExpression;
            if (b != null && b.Operator == Operator.IAdd)
            {
                c = b.Right as Constant;
                if (c != null)
                {
                    return new FieldAccess(type, new MemberPointerSelector(type, basePtr, b.Left),
                        CreateField(type, c.ToInt16()));
                }
            }
            return new MemberPointerSelector(null, basePtr, ea);
        }
	}
}
