#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using System.IO;

namespace Reko.Core.Rtl
{
    /// <summary>
    /// Models a multi-way branch.
    /// </summary>
    public class RtlSwitch : RtlInstruction
    {
        public RtlSwitch(Expression expr, Address [] targets)
        {
            this.Expression = expr;
            this.Targets = targets;
        }

        public Expression Expression { get; }

        public Address[] Targets { get; }

        public override T Accept<T>(RtlInstructionVisitor<T> visitor)
        {
            return visitor.VisitSwitch(this);
        }

        public override T Accept<T, C>(IRtlInstructionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitSwitch(this, context);
        }

        protected override void WriteInner(TextWriter writer)
        {
            writer.Write("switch (");
            writer.Write(Expression);
            writer.Write(") {");
            int count = 0;
            var sep = "";
            foreach (var target in Targets)
            {
                if (count % 8 == 0)
                {
                    writer.WriteLine(sep);
                    sep = ",";
                    writer.Write("        ");
                }
                else
                {
                    writer.Write(", ");
                }
                writer.Write(target);
            }
            writer.WriteLine("");
            writer.Write("    }");
        }
    }
}
