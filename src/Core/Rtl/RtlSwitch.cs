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

using Reko.Core.Expressions;
using System.IO;

namespace Reko.Core.Rtl
{
    /// <summary>
    /// Models a multi-way branch.
    /// </summary>
    public sealed class RtlSwitch : RtlInstruction
    {
        /// <summary>
        /// Constructs a <see cref="RtlSwitch"/> instruction.
        /// </summary>
        /// <param name="expr">Condition expression.</param>
        /// <param name="targets">The addresses of the cases of the RTL switch.</param>
        public RtlSwitch(Expression expr, Address [] targets)
        {
            this.Expression = expr;
            this.Targets = targets;
        }

        /// <summary>
        /// Condition expression of the switch instruction.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// The addresses of the cases of the RTL switch.
        /// </summary>
        public Address[] Targets { get; }

        /// <inheritdoc/>
        public override T Accept<T>(RtlInstructionVisitor<T> visitor)
        {
            return visitor.VisitSwitch(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(IRtlInstructionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitSwitch(this, context);
        }

        /// <inheritdoc/>
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
