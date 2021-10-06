#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Types;
using System;

namespace Reko.Evaluation
{
    public class SliceConvert
    {
        private Expression? result;

        public SliceConvert()
        {
        }

        public bool Match(Slice slice)
        {
            if (slice.Offset != 0)
                return false;
            result = null;
            if (slice.Expression is Conversion conv)
            {
                if (IsUselessIntegralExtension(slice, conv))
                {
                    this.result = slice.DataType.BitSize == conv.SourceDataType.BitSize
                        ? conv.Expression
                        : new Slice(slice.DataType, conv.Expression, 0);
                    return true;
                }
            }
            return false;
        }

        private static bool IsUselessIntegralExtension(Slice slice, Conversion conv)
        {
            if (IsSliceable(slice.DataType) &&
                IsSliceable(conv.DataType) &&
                IsSliceable(conv.SourceDataType))
            {
                return
                    slice.DataType.BitSize <= conv.SourceDataType.BitSize;
            }
            else return false;
        }

        private static bool IsSliceable(DataType dataType)
        {
            if (dataType.IsWord || dataType.IsIntegral)
                return true;
            return (dataType is PrimitiveType pt && pt.Domain.HasFlag(Domain.Character));
        }

        public Expression Transform()
        {
            return result!;
        }
    }
}