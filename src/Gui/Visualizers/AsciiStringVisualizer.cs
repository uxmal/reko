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
using System.Threading.Tasks;
using Reko.Core;

namespace Reko.Gui.Visualizers
{
    public class AsciiStringVisualizer : Visualizer
    {
        private int threshold;

        public AsciiStringVisualizer()
        {
            this.threshold = 5;
        }

        public int DefaultLineLength => 64;

        public int DefaultZoom => 2;

        public bool IsLineLengthFixed => false;
        public bool TrackSelection => true;
        public bool ShowScrollbar => true;

        public int[] RenderBuffer(Program program, MemoryArea mem, Address addrStart, int length, int? mouse)
        {
            var iStart = addrStart - mem.BaseAddress;
            var iEnd = Math.Min(iStart + length, mem.Bytes.Length);
            var colors = new int[iEnd - iStart];
            var offsets = new Dictionary<int, int>
            {
            };
            var cur_len = 0;

            int last_offs = -1;
            var buf = mem.Bytes;
            for (int i = 0; i < colors.Length; ++i)
            {
                var c = buf[i + iStart];
                var printable = (c >= 32 && c <= 126);
                if (printable)
                {
                    if (last_offs != -1)
                    {
                        ++cur_len;
                    }
                    else
                    {
                        last_offs = i;
                        cur_len = 1;
                    }
                }
                else
                {
                    if (last_offs != -1 && cur_len >= this.threshold)
                    {
                        offsets[last_offs] = cur_len;
                    }
                    last_offs = -1;
                    cur_len = 0;
                }
                // muted gray color
                if (!printable)
                    c >>= 2;
                colors[i] = (0xFF << 24) | (c << 16) | (c << 8) | c;
                if (printable)
                    colors[i] |= 0xFF;
            }

            foreach (var de in offsets)
            {
                var k = de.Key;
                var v = de.Value;
                for (var i = 0; i < v; ++i)
                {
                    var c = mem.Bytes[k + i];
                    var r = c + (255 - 126);
                    var g = 0;
                    if (c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c >= '0' && c <= '9')
                    {
                        g = r;
                    }
                    colors[k + i] = (0xFF << 24) | (r << 16) | (g << 8);
                }
            }
            return colors;
        }

        public VisualAnnotation[] RenderAnnotations(Program program, Address addrStart, int length, int? mouse)
        {
            return new VisualAnnotation[0];
        }
    }
}