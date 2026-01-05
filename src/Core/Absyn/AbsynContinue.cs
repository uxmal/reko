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

namespace Reko.Core.Absyn
{
    /// <summary>
    /// Abstract syntax for a "continue" statement used in 
    /// C-like languages.
    /// </summary>
    public class AbsynContinue : AbsynStatement 
	{
        /// <inheritdoc/>
		public override void Accept(IAbsynVisitor visitor)
		{
			visitor.VisitContinue(this);
		}

        /// <inheritdoc/>
        public override T Accept<T>(IAbsynVisitor<T> visitor)
        {
            return visitor.VisitContinue(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(IAbsynVisitor<T, C> visitor, C context)
        {
            return visitor.VisitContinue(this, context);
        }
    }
}
