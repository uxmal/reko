#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core.Code;
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Core.Absyn
{
    public class AbsynCase : AbsynStatement
    {
        public AbsynCase(Constant c)
        {
            this.Constant = c;
        }

        public Constant Constant { get; }

        public override void Accept(IAbsynVisitor visitor)
        {
            visitor.VisitCase(this);
        }

        public override T Accept<T>(IAbsynVisitor<T> visitor)
        {
            return visitor.VisitCase(this);
        }
    }

    public class AbsynDefault: AbsynStatement
    {
        public AbsynDefault()
        {
        }

        public override void Accept(IAbsynVisitor visitor)
        {
            visitor.VisitDefault(this);
        }

        public override T Accept<T>(IAbsynVisitor<T> visitor)
        {
            return visitor.VisitDefault(this);
        }
    }
}
