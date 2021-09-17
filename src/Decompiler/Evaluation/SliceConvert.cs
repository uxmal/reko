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
                // Zero extension?
                if (conv.SourceDataType.IsWord || conv.DataType.IsWord)
                {
                    // Is extension useless?
                    if (slice.DataType.BitSize <= conv.SourceDataType.BitSize)
                    {
                        this.result = slice.DataType.BitSize == conv.SourceDataType.BitSize
                            ? conv.Expression
                            : new Slice(slice.DataType, conv.Expression, 0);
                        return true;
                    } 
                }
            }
            return false;
        }

        public Expression Transform()
        {
            return result!;
        }
    }
}