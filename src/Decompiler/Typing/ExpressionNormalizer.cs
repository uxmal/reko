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

namespace Decompiler.Typing
{
	/// <summary>
	/// Transform certain expressions to equivalents, to simplify type inference.
	/// Locates array expressions and converts them to array access expressions. 
	/// This simplifies work for later stages of the type inference, when we want to identify
	/// array accesses quickly.
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
                Operator.add,
                PrimitiveType.CreateWord(ea.DataType.Size),
                ea,
                new Constant(PrimitiveType.CreateWord(ea.DataType.Size), 0));
            return bin;
        }


		public override Expression TransformMemoryAccess(MemoryAccess access)
		{
            access.EffectiveAddress = access.EffectiveAddress.Accept(this);
            if (aem.Match(access.EffectiveAddress))
            {
                return aem.Transform(null, access.DataType);
            }
            else if (access.EffectiveAddress is Identifier)
            {
                access.EffectiveAddress = AddZeroToEffectiveAddress(access.EffectiveAddress);
                return access;
            }
            else
            {
                return access;
            }
        }


        public override Expression TransformSegmentedAccess(SegmentedAccess access)
        {
            access.EffectiveAddress = access.EffectiveAddress.Accept(this);
            if (aem.Match(access.EffectiveAddress))
            {
                return aem.Transform(access.BasePointer, access.DataType);
            }
            else if (access.EffectiveAddress is Identifier)
            {
                access.EffectiveAddress = AddZeroToEffectiveAddress(access.EffectiveAddress);
                return access;
            }
            else
            {
                return access;
            }
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
