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

using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Loongson
{
    public partial class LoongArchRewriter
    {
        private void RewriteB()
        {
            m.Goto(((AddressOperand) instr.Operands[0]).Address);
        }

        private void RewriteBl()
        {
            m.Call(((AddressOperand) instr.Operands[0]).Address, 0);
        }

        private void RewriteBranch(Func<Expression, Expression, Expression> fn)
        {
            var src1 = Op(0, false);
            var src2 = Op(1, false);
            m.Branch(fn(src1, src2), ((AddressOperand) instr.Operands[2]).Address);
        }

        private void RewriteBranch0(Func<Expression, Expression> fn)
        {
            var id = Op(0, false);
            m.Branch(fn(id), ((AddressOperand) instr.Operands[1]).Address);
        }

        private void RewriteJirl()
        {
            var src = Op(1, false);
            var dst = Op(0, false);
            if (dst is Identifier idDst)
            {
                m.Assign(idDst, instr.Address + 4);
            }
            if (src == dst)
            {
                if (src is Identifier id && id.Storage.Domain != 0)
                {
                    m.Goto(instr.Address + 4);
                    return;
                }
            }
            m.Goto(m.IAdd(src, ((ImmediateOperand) instr.Operands[2]).Value.ToInt32()));
        }
    }
}
