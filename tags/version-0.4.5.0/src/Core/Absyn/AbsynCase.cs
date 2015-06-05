#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Core.Absyn
{
    public class AbsynCase : AbsynStatement
    {
        [Obsolete("Use constructor with Constant parameter")]
        public AbsynCase(int i)
        {
            this.Number = i;
        }

        public AbsynCase(Constant c)
        {
            this.Constant = c;
        }

        public override void Accept(IAbsynVisitor visitor)
        {
            visitor.VisitCase(this);
        }

        public int Number { get; private set; }
        public Constant Constant { get; private set; }
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
    }
}
