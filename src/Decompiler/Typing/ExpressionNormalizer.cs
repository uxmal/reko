#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

namespace Reko.Typing
{
	/// <summary>
	/// Transform certain expressions to equivalents, to simplify type inference.
	/// Locates array expressions and converts them to array access expressions. 
	/// This simplifies work for later stages of the type inference, when we want
    /// to identify array accesses quickly.
	/// </summary>
	public class ExpressionNormalizer : InstructionTransformer
	{
        private ArrayExpressionMatcher aem;

        public ExpressionNormalizer(PrimitiveType pointerType)
        {
		    this.aem = new ArrayExpressionMatcher(pointerType);
        }

        /// <summary>
        /// Extends an effective address ''id'' to ''id'' + 0. 
        /// </summary>
        /// <remarks>
        /// The purpose here is to extend the effective address to avoid premature typing of id.
        /// If later in the type inference process [[id]] is discovered to be a signed integer, the
        /// decompiler can accomodate that by having the added 0 be [[pointer]] or [[member pointer]].
        /// This is not possible if all we have is the id.
        /// </remarks>
        /// <param name="ea"></param>
        /// <returns></returns>
        private Expression AddZeroToEffectiveAddress(Expression ea)
        {
            BinaryExpression bin = new BinaryExpression(
                Operator.IAdd,
                PrimitiveType.CreateWord(ea.DataType.BitSize),
                ea,
                Constant.Create(PrimitiveType.CreateWord(ea.DataType.BitSize), 0));
            return bin;
        }

		public override Expression VisitMemoryAccess(MemoryAccess access)
		{
            var ea = access.EffectiveAddress.Accept(this);
            if (aem.Match(ea))
            {
                if (aem.ArrayPointer == null)
                {
                    aem.ArrayPointer = Constant.Create(
                        PrimitiveType.Create(
                            Domain.Pointer,
                            ea.DataType.BitSize),
                        0);
                }
                return aem.Transform(null, access.DataType);
            }
            if (!(ea is BinaryExpression bin && bin.Operator == Operator.IAdd) &&
                !(ea is Constant) && 
                !(ea is Address))
            {
                ea = AddZeroToEffectiveAddress(ea);
            }
            return new MemoryAccess(access.MemoryId, ea, access.DataType);
        }

        public override Expression VisitSegmentedAccess(SegmentedAccess access)
        {
            var ea = access.EffectiveAddress.Accept(this);
            if (aem.Match(ea))
            {
                return aem.Transform(access.BasePointer, access.DataType);
            }
            if (!(ea is BinaryExpression bin && bin.Operator == Operator.IAdd) &&
                !(ea is Constant))
            {
                ea = AddZeroToEffectiveAddress(ea);
            }
            var memId = access.MemoryId;
            var basePtr = access.BasePointer;
            return new SegmentedAccess(memId, basePtr, ea, access.DataType);
        }

		public void Transform(Program program)
		{
			foreach (Procedure proc in program.Procedures.Values)
			{
                foreach (var stm in proc.Statements)
                {
                    stm.Instruction = stm.Instruction.Accept(this);
                }
			}
		}
	}
}
