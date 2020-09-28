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

using Reko.Core.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Types;
using Reko.Core;
using Reko.Libraries.Libc;

namespace Reko.Environments.Windows
{
    /// <summary>
    /// Microsoft's implementation of wsprintf differs in its interpretation
    /// of the format string.
    /// </summary>
    public class MsWprintfFormatParser : PrintfFormatParser
    {
        public MsWprintfFormatParser(
            Program program,
            Address addrInstr,
            string format,
            IServiceProvider services)
        :
        base(program, addrInstr, format, services)
        {
        }

        protected override DataType MakeDataType(PrintfSize size, char cDomain)
        {
            switch (cDomain)
            {
            case 'c': return PrimitiveType.WChar;
            case 'C': return PrimitiveType.Char;
            case 's': return program.TypeFactory.CreatePointer(
                PrimitiveType.WChar,
                base.pointerSize);
            case 'S': return program.TypeFactory.CreatePointer(
                PrimitiveType.Char,
                base.pointerSize);
            }
            return base.MakeDataType(size, cDomain);
        }
    }
}
