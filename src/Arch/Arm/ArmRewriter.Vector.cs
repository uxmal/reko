#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm
{
    public partial class ArmRewriter
    {
        private void RewriteVldmia()
        {
            ConditionalSkip();
            var rSrc = this.Operand(Dst);
            var offset = 0;
            foreach (var r in instr.ArchitectureDetail.Operands.Skip(1))
            {
                var dst = this.Operand(r);
                Expression ea =
                    offset != 0
                        ? m.IAdd(rSrc, Constant.Int32(offset))
                        : rSrc;
                m.Assign(dst, m.Load(dst.DataType, ea));
                    offset += dst.DataType.Size;
            }
            if (instr.ArchitectureDetail.UpdateFlags)
            {
                m.Assign(rSrc, m.IAdd(rSrc, Constant.Int32(offset)));
            }
        }
    }
}
