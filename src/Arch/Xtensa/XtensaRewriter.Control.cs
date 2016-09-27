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

using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Xtensa
{
    public partial class XtensaRewriter
    {
        private void RewriteCall0()
        {
            rtlc.Class = RtlClass.Transfer;
            var cont = dasm.Current.Address + dasm.Current.Length;
            emitter.Assign(frame.EnsureRegister(Registers.a0), cont);
            emitter.Call(RewriteOp(dasm.Current.Operands[0]), 0);
        }

        private void RewriteRet()
        {
            rtlc.Class = RtlClass.Transfer;
            emitter.Return(0, 0);
        }
    }
}