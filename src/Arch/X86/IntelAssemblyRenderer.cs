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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.X86
{
    /// <summary>
    /// Renders X86 instructions using the syntax used by Intel.
    /// </summary>
    public class IntelAssemblyRenderer : X86AssemblyRenderer
    {
        protected override string ExplicitOperandPrefix(DataType width)
        {
            string s;
            if (width == PrimitiveType.Byte)
                s = "byte ptr ";
            else if (width == PrimitiveType.Word16)
                s = "word ptr ";
            else if (width.Size == 4)
                s = "dword ptr ";
            else if (width == PrimitiveType.Word64)
                s = "qword ptr ";
            else if (width == PrimitiveType.Real32)
                s = "float ptr ";
            else if (width == PrimitiveType.Real64)
                s = "double ptr ";
            else if (width == PrimitiveType.Real80 || width == PrimitiveType.Bcd80)
                s = "tword ptr ";
            else if (width == PrimitiveType.Word128)
                s = "xmmword ptr ";
            else if (width == PrimitiveType.Word256)
                s = "ymmword ptr ";
            else
                s = "";
            return s;
        }

        protected override string FormatSignedValue(long n, bool forceSign)
        {
            string sign = "";
            int iExtraZero = 0;
            if (n < 0)
            {
                sign = "-";
                n = -n;
                iExtraZero = 1;
            }
            else if (forceSign)
            {
                sign = "+";
                iExtraZero = 1;
            }
            var sb = new StringBuilder();
            sb.AppendFormat("{0}{1:X}h", sign, n);
            if (!char.IsDigit(sb[1]))
            {
                sb.Insert(iExtraZero, '0');
            }
            return sb.ToString();
        }

        protected override string FormatUnsignedValue(ulong n, string? format)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(format ?? "{0:X}", n);
            if (!char.IsDigit(sb[0]))
            {
                sb.Insert(0, '0');
            }
            sb.Append('h');
            return sb.ToString();
        }
    }
}
