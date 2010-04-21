/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Arch.Intel
{
    public partial class IntelRewriter
    {
        private void RewriteFstsw(IntelInstruction[] instrs, int iStart)
        {
            EmitCopy(
                instrCur.op1,
                new BinaryExpression(Operator.Shl, PrimitiveType.Word16,
                        new Cast(PrimitiveType.Word16, orw.FlagGroup(FlagM.FPUF)),
                        new Constant(PrimitiveType.Int16, 8)),
                false);
        }
    }
}