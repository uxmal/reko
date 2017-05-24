#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Tlcs.Tlcs90
{
    partial class Tlcs90Rewriter
    {
        private void RewriteLd()
        {
            var src = RewriteSrc(instr.op2);
            var dst = RewriteDst(instr.op1, src, (a, b) => b);
        }

        private void RewritePop()
        {
            var sp = frame.EnsureRegister(Registers.sp);
            var src = RewriteSrc(instr.op1);
            m.Assign(src, m.LoadW(sp));
            m.Assign(sp, m.IAdd(sp, m.Int16((short)src.DataType.Size)));
        }

        private void RewritePush()
        {
            var sp = frame.EnsureRegister(Registers.sp);
            var src = RewriteSrc(instr.op1);
            m.Assign(sp, m.ISub(sp, m.Int16((short)src.DataType.Size)));
            m.Assign(m.LoadW(sp), src);
        }
    }
}
