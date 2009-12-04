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
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Analysis.Simplification
{
    /// <summary>
    /// Converts expressions (slice (shift x c) c) to x
    /// </summary>
    public class SliceShift
    {
        private SsaIdentifierCollection ssaIds;
        private Expression expr;
        private DataType dt;
        private Identifier id;
        private Statement stmShift;

        public SliceShift(SsaIdentifierCollection ssaIds)
        {
            this.ssaIds = ssaIds;
        }

        public bool Match(Slice slice)
        {
            BinaryExpression shift;
            id = slice.Expression as Identifier;
            if (id != null)
            {
                SsaIdentifier sid = ssaIds[id];
                if (sid.DefStatement == null)
                    return false;
                Assignment ass = sid.DefStatement.Instruction as Assignment;
                if (ass == null)
                    return false;
                if (ass.Dst != id)
                    return false;
                shift = ass.Src as BinaryExpression;
                stmShift = sid.DefStatement;
            }
            else
            {
                shift = slice.Expression as BinaryExpression;
            }
            if (shift == null)
                return false;
            if (shift.op != BinaryOperator.shl)
                return false;
            Constant c = shift.Right as Constant;
            if (c == null)
                return false;
            if (c.ToInt32() != slice.Offset)
                return false;

            expr = shift.Left;
            dt = slice.DataType;
            return true;
        }

        public Expression Transform(Statement stm)
        {
            if (id != null)
            {
                ssaIds[id].Uses.Remove(stm);
                var ur = new UsedIdentifierAdjuster(stmShift, ssaIds, stm);
                ur.Transform();
            }
            expr.DataType = dt;
            return expr;
        }
    }
}
