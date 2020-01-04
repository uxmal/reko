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

using Reko.Core;
using Reko.Core.Analysis;
using Reko.Core.Types;
using Reko.Libraries.Libc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Environments.Windows
{
    /// <summary>
    /// Parses Microsoft printf format which differs from C standard
    /// </summary>
    public class MsPrintfFormatParser : PrintfFormatParser
    {
        public MsPrintfFormatParser(
            Program program,
            Address addrInstr,
            string format,
            IServiceProvider services) :
        base(program, addrInstr, format, services)
        {
        }

        protected override DataType MakeDataType(PrintfSize size, char cDomain)
        {
            if (cDomain == 'S')
            {
                return program.TypeFactory.CreatePointer(
                    PrimitiveType.WChar,
                    base.pointerSize);
            }
            return base.MakeDataType(size, cDomain);
        }

        protected override PrintfSize CollectSize()
        {
            if (i < format.Length - 3 && format[i] == 'I')
            {
                if (format[i + 1] == '3' && format[i + 2] == '2')
                {
                    i += 3;
                    return PrintfSize.I32;
                }
                if (format[i + 1] == '6' && format[i + 2] == '4')
                {
                    i += 3;
                    return PrintfSize.I64;
                }
            }
            return base.CollectSize();
        }
    }
}
