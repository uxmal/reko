#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Core.Rtl
{
    public class RtlCall : RtlInstruction
    {
        public RtlCall(Expression target, byte stackPushedReturnAddressSize)
        {
            this.Target = target;
            this.ReturnAddressSize = stackPushedReturnAddressSize;
        }

        public int ReturnAddressSize { get; private set; }
        public Expression Target { get; private set; }

        public override T Accept<T>(RtlInstructionVisitor<T> visitor)
        {
            return visitor.VisitCall(this);
        }


        protected override void WriteInner(TextWriter writer)
        {
            writer.Write("call {0} ({1})", Target, ReturnAddressSize);
        }
    }
}