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

using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Output;
using Reko.Core.Types;

namespace Reko.ImageLoaders.WebAssembly
{
    public class TypeSectionRenderer : ImageSegmentRenderer
    {
        private readonly TypeSection typeSection;

        public TypeSectionRenderer(TypeSection typeSection)
        {
            this.typeSection = typeSection;
        }

        public override void Render(ImageSegment segment, Program program, Formatter formatter)
        {
            int i = -1;
            foreach (var func in typeSection.Types)
            {
                ++i;
                formatter.Write("  (type (; 0x{0:X} ;) (func", i);
                if (func.Parameters is not null && func.Parameters.Length > 0)
                {
                    formatter.Write(" (param");
                    foreach (var param in func.Parameters)
                    {
                        formatter.Write(' ');
                        FormatAsWasmType(param.DataType, formatter);
                    }
                    formatter.Write(")");
                }
                if (!func.HasVoidReturn)
                {
                    formatter.Write(" (result ");
                    FormatAsWasmType(func.Outputs[0].DataType, formatter);
                    formatter.Write(")");
                }
                formatter.WriteLine(")");
            }
        }

        private static void FormatAsWasmType(DataType dt, Formatter formatter)
        {
            var sigil = dt.Domain switch
            {
                Domain.SignedInt => 's',
                Domain.UnsignedInt => 'u',
                Domain.Real => 'f',
                _ => 'i'
            };
            formatter.Write("{0}{1}", sigil, dt.BitSize);
        }
    }
}