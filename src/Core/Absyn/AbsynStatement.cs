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
using Reko.Core.Output;
using System;
using System.IO;

namespace Reko.Core.Absyn
{
    /// <summary>
    /// Base class for all abstract syntax statements.
    /// </summary>
    public abstract class AbsynStatement
    {
        public abstract void Accept(IAbsynVisitor visitor);

        public abstract T Accept<T>(IAbsynVisitor<T> visitor);

        public bool As<T>(out T t) where T : AbsynStatement
        {
            t = this as T;
            return t != null;
        }

		public override sealed string ToString()
		{
			StringWriter sw = new StringWriter();
            TextFormatter f = new TextFormatter(sw);
			f.Terminator = "";
			f.Indentation = 0;
			CodeFormatter fmt = new CodeFormatter(f);
			Accept(fmt);
			return sw.ToString();
        }
    }
}
