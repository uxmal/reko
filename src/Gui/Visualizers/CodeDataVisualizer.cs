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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.Visualizers
{
    public class CodeDataVisualizer : Visualizer
    {
        public CodeDataVisualizer()
        {
        }

        public int DefaultLineLength => 64;

        public int DefaultZoom => 2;

        public bool IsLineLengthFixed => false;
        public bool TrackSelection => true;
        public bool ShowScrollbar => true;

        public VisualAnnotation[] RenderAnnotations(Program program, Address addrStart, int length, int? mouse)
        {
            var addrEnd = addrStart + length;
            var procs = program.Procedures
                .Where(p => addrStart < p.Key && p.Key < addrEnd)
                .Select(p => new VisualAnnotation
                {
                    Address = p.Key,
                    Text = p.Value.Name
                })
                .ToArray();
            return procs;
        }

        public int[] RenderBuffer(Program program, MemoryArea mem, Address addrStart, int length, int? mouse)
        {
            var iStart = addrStart - mem.BaseAddress;
            var iEnd = Math.Min(iStart + length, mem.Bytes.Length);
            var colors = new int[iEnd - iStart];
            for (int i = 0; i < colors.Length; ++i)
            {
                int c = mem.Bytes[iStart + i];
                int r = c;
                int g = c;
                int b = c;
                if (program.ImageMap.TryFindItem(addrStart + i, out var item))
                {
                    if (item is ImageMapBlock)
                    {
                        g >>= 1;
                        b >>= 1;
                    }
                    else
                    {
                        r >>= 1;
                        g >>= 1;
                    }
                }
                colors[i] = (0xFF << 24) | (r << 16) | (g << 8) | b;
            }
            return colors;
        }
    }
}