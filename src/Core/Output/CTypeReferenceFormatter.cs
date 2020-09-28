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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Types;

namespace Reko.Core.Output
{
    /// <summary>
    /// Outputs types specifically for the C programming language.
    /// </summary>
    public class CTypeReferenceFormatter : TypeReferenceFormatter
    {
        private IPlatform platform;

        public CTypeReferenceFormatter(IPlatform platform, Formatter writer)
            : base(writer)
        {
            this.platform = platform;
        }

        public override void WritePrimitiveTypeName(PrimitiveType t)
        {
            var keywordName = platform.GetPrimitiveTypeName(t, "C");
            if (keywordName != null)
                this.Formatter.WriteKeyword(keywordName);
            else
                this.Formatter.WriteType(t.Name, t);
        }

        public override void WriteVoidType(VoidType t)
        {
            this.Formatter.WriteKeyword("void");
        }
    }
}
