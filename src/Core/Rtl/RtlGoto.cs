#region License
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
#endregion

using Decompiler.Core.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Core.Rtl
{
    public class RtlGoto : RtlInstruction
    {
        public RtlGoto(Address addr, uint length, Expression target)
            : base(addr, length)
        {
            this.Target = target;
        }

        public override T Accept<T>(RtlInstructionVisitor<T> visitor)
        {
            return visitor.VisitGoto(this);
        }

        public Expression Target { get; private set; }

        public override void Write(TextWriter writer)
        {
            base.Write(writer);
            writer.Write("goto {0}", Target);
        }
    }
}
