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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Rtl
{
    /// <summary>
    /// This instruction represents "micro-jump"s between <see cref="RtlInstruction"/>s
    /// in a <see cref="RtlInstructionCluster"/>. 
    /// </summary>
    public class RtlMicroLabel : RtlInstruction
    {
        public RtlMicroLabel(string name)
        {
            this.Name = name;
        }

        public string Name { get; }

        public override T Accept<T>(RtlInstructionVisitor<T> visitor)
        {
            return visitor.VisitMicroLabel(this);
        }

        protected override void WriteInner(TextWriter writer)
        {
            writer.Write("{0}::", Name);
        }
    }
}
