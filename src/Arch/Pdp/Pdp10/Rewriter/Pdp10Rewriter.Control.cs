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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Pdp.Pdp10.Rewriter
{
    public partial class Pdp10Rewriter
    {
        private void RewriteJrst()
        {
            m.Goto(RewriteEa(0));
        }

        private void RewriteJsp()
        {
            var ac = Ac();
            m.Assign(ac, instr.Address + 1);
            m.Goto(RewriteEa(1));
        }

        private void RewriteJsr()
        {
            var targetaddress = AccessEa(0);
            m.Assign(targetaddress, this.instr.Address + 1);
            if (targetaddress is Address a)
            {
                m.Goto(a + 1);
            }
            else
            {
                m.Goto(m.IAdd(RewriteEa(0), 1));
            }
        }

        private void RewriteJump(Func<Expression, Expression> fn)
        {
            Branch(fn(Ac()), RewriteEa(1));
        }

        private void RewritePushj()
        {
            var target = RewriteEa(1);
            m.Call(target, 1);
        }

        private void RewritePopj()
        {
            m.Return(1, 0);
        }
    }
}
