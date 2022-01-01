#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Types;
using System;

namespace Reko.Evaluation
{
	public class SliceConstant_Rule
	{
		private Constant? c;
		private Slice? slice;
        private PrimitiveType? pt;

		public bool Match(Slice slice)
		{
            var pt = slice.DataType.ResolveAs<PrimitiveType>();
            if (pt is null)
                return false;
			this.slice = slice;
            if (slice.Expression is Constant c && !(c is InvalidConstant))
            {
                var ct = c.DataType.ResolveAs<PrimitiveType>();
                if (ct != null && pt.BitSize <= ct.BitSize)
                {
                    this.slice = slice;
                    this.c = c;
                    this.pt = pt;
                    return true;
                }
            }
            return false;
        }

		public Expression Transform()
		{
            var cSliced = this.c!.Slice(pt!, slice!.Offset);
            cSliced.DataType = slice.DataType;
            return cSliced;
		}
	}
}
