#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

namespace Reko.Core.Code
{
    /// <summary>
    /// Represents a comment introduced by the user.
    /// </summary>
    public class CodeComment : Instruction
    {
        /// <summary>
        /// Constructs a comment "instruction".
        /// </summary>
        /// <param name="comment">The text of the comment.</param>
        public CodeComment(string comment)
        {
            this.Text = comment;
        }

        /// <inheritdoc/>
        public override bool IsControlFlow => false;

        /// <summary>
        /// The text of the comment.
        /// </summary>
        public string Text { get; set; }

        /// <inheritdoc/>
        public override Instruction Accept(InstructionTransformer xform)
        {
            return xform.TransformComment(this);
        }

        /// <inheritdoc/>
        public override void Accept(InstructionVisitor v)
        {
            v.VisitComment(this);
        }

        /// <inheritdoc/>
        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitComment(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(InstructionVisitor<T, C> visitor, C ctx)
        {
            return visitor.VisitComment(this, ctx);
        }
    }
}
