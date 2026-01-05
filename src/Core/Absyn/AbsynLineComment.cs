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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Absyn
{
    /// <summary>
    /// Represents a line comment in the abstract syntax tree.
    /// </summary>
    public class AbsynLineComment : AbsynStatement
    {
        /// <summary>
        /// Constructs a line comment.
        /// </summary>
        /// <param name="comment">The comment text.</param>
        public AbsynLineComment(string comment)
        {
            this.Comment = comment;
        }

        /// <summary>
        /// The comment text.
        /// </summary>
        public string Comment { get; set; }

        /// <inheritdoc/>
        public override void Accept(IAbsynVisitor visitor)
        {
            visitor.VisitLineComment(this);
        }

        /// <inheritdoc/>
        public override T Accept<T>(IAbsynVisitor<T> visitor)
        {
            return visitor.VisitLineComment(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(IAbsynVisitor<T, C> visitor, C context)
        {
            return visitor.VisitLineComment(this, context);
        }
    }
}
