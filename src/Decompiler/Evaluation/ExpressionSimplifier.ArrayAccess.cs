#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using Reko.Core.Expressions;
using Reko.Core.Lib;

namespace Reko.Evaluation
{
    public partial class ExpressionSimplifier
    {
        public virtual (Expression, bool) VisitArrayAccess(ArrayAccess acc)
        {
            Expression result = acc;
            var (a, aChanged) = acc.Array.Accept(this);
            var (i, iChanged) = acc.Index.Accept(this);
            bool changed = aChanged | iChanged;
            if (changed)
            {
                acc = m.ARef(acc.DataType, a, i);
                result = acc;
            }

            if (i is Constant cIndex)
            {
                var bitPosition = cIndex.ToInt32() * acc.DataType.BitSize;
                if (IsSequence(ctx, a, out var seq))
                {
                    var eNew = SliceSequence(seq, acc.DataType, bitPosition);
                    if (eNew is not null)
                    {
                        (eNew, _) = eNew.Accept(this);
                        return (eNew, true);
                    }
                }
                if (a is Constant cArray)
                {
                    var cValue = (cArray.ToBigInteger() >> bitPosition) & Bits.Mask(acc.DataType.BitSize);
                    result = Constant.Create(acc.DataType, cValue);
                    changed = true;
                }
            }
            return (result, changed);
        }
    }
}
