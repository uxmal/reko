#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.X86
{
    /// <summary>
    /// Renders X86 instructions using NASM syntax.
    /// </summary>
    public class NasmAssemblyRenderer : X86AssemblyRenderer
    {
        protected override string ExplicitOperandPrefix(DataType width)
        {
            string s;
            if (width == PrimitiveType.Byte)
                s = "byte ";
            else if (width == PrimitiveType.Word16)
                s = "word ";
            else if (width.Size == 4)
                s = "dword ";
            else if (width == PrimitiveType.Word64)
                s = "qword ";
            else if (width == PrimitiveType.Real32)
                s = "float ";
            else if (width == PrimitiveType.Real64)
                s = "double ";
            else if (width == PrimitiveType.Real80 || width == PrimitiveType.Bcd80)
                s = "tword ";
            else if (width == PrimitiveType.Word128)
                s = "xmmword ";
            else if (width == PrimitiveType.Word256)
                s = "ymmword ";
            else
                s = "";
            return s;
        }

        protected override string FormatSignedValue(long n, bool forceSign)
        {
            string fmt;
            if (n < 0)
            {
                n = -n;
                fmt = "-0x{0:X}";
            }
            else if (forceSign)
            {
                fmt = "+0x{0:X}";
            }
            else
            {
                fmt = "0x{0:X}";
            }
            return string.Format(fmt, n);
        }

        protected override string FormatUnsignedValue(ulong n, string? format)
        {
            return "0x" + string.Format(format ?? "{0:X}", n);
        }
    }
}
