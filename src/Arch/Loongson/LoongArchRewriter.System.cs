#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Core.Intrinsics;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Loongson
{
    public partial class LoongArchRewriter
    {
        private void RewriteBreak()
        {
            m.SideEffect(m.Fn(break_intrinsic, Op(0, false)));
        }

        private void RewriteCacop()
        {
            m.SideEffect(
                m.Fn(cacop.MakeInstance(arch.WordWidth),
                    Op(0, false),
                    Op(1, false),
                    Op(2, false)));
        }

        private void RewriteCsrrd()
        {
            var src = m.Fn(csrrd, Op(0, true));
            Assign(instr.Operands[0], src);
        }

        private void RewriteCsrwr()
        {
            var src = Op(1, false);
            src = m.Fn(csrwr.MakeInstance(src.DataType), src, Op(0, false));
            m.SideEffect(src);
        }

        private void RewriteLdpte()
        {
            Assign(
                instr.Operands[0],
                m.Fn(
                    ldpte.MakeInstance(arch.WordWidth),
                    Op(1, false),
                    Op(2, false)));
        }

        private void RewriteSyscall()
        {
            m.SideEffect(
                m.Fn(
                    CommonOps.Syscall_1.MakeInstance(PrimitiveType.Word32),
                    Op(0, true)));
        }

        private void RewriteTlbwr()
        {
            m.SideEffect(m.Fn(tlbwr));
        }
    }
}
