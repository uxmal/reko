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
    /// <summary>
    /// Renders a memory area as a heatmap.
    /// </summary>
    public class HeatmapVisualizer : Visualizer
    {
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
            for (int i = 0; i < colors.Length; ++i)
            {
                if (i + iStart < 0)
                {
                    colors[i] = 0;
                }
                else
                {
                    // Render pixel in a heat map color
                    // Code taken from
                    // http://stackoverflow.com/questions/20792445/calculate-rgb-value-for-a-range-of-values-to-create-heat-map
                    var ratio = 2 * mem.Bytes[i + iStart] / 255;
                    var b = Convert.ToInt32(Math.Max(0, 255 * (1 - ratio)));
                    var r = Convert.ToInt32(Math.Max(0, 255 * (ratio - 1)));
                    var g = 255 - b - r;
                    colors[i] = ~0x00FFFFFF | (r << 16) | (g << 8) | b;
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
