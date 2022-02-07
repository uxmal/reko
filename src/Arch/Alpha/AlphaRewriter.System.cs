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
using Reko.Core.Serialization;
using Reko.Core.Types;

namespace Reko.Arch.Alpha
{
    partial class AlphaRewriter
    {
        private void RewriteHalt()
        {
            var c = new ProcedureCharacteristics
            {
                Terminates = true,
            };
            m.SideEffect(
                host.Intrinsic("__halt", true, c, VoidType.Instance), 
                InstrClass.Terminates);
        }

        private void RewriteImplver()
        {
            var dst = Rewrite(instr.Operands[0]);
            m.Assign(dst, m.Fn(implver_intrinsic.MakeInstance(dst.DataType)));
        }
    }
}
