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
    /// A visualizer renders a MemoryArea into some graphical 
    /// representation.
    /// </summary>
    public interface Visualizer
    {
        /// <summary>
        /// Preferred number of bytes per line for this visualization.
        /// </summary>
        int DefaultLineLength { get; }

        /// <summary>
        /// Preferred number of pixels per byte
        /// </summary>
        int DefaultZoom { get; }

        /// <summary>
        /// If set, this visualization requires always having a certain width.
        /// </summary>
        bool IsLineLengthFixed { get; }

        /// <summary>
        /// If true, this visualization wants to track the current selection
        /// of bytes.
        /// </summary>
        bool TrackSelection { get; }

        bool ShowScrollbar { get; }

        /// <summary>
        /// Render the <paramref name="length"/> bytes of the memory area <paramref name="mem"/> starting
        /// at address <paramref name="addrStart"/>. If the visualizer supports it, it can
        /// render the current mouse position given by <paramref name="mouse"/>.
        /// </summary>
        /// <param name="mem">A memory area containing the bytes to render</param>
        /// <param name="addrStart">The starting area from which to start rendering</param>
        /// <param name="length">The number of bytes to render</param>
        /// <param name="mouse">The offset from addrStart where the mouse cursor is located</param>
        /// <returns>An array of bytes in ARGB format. Missing bytes are rendered with the 
        /// ARGB value 0; this is in contrast with the ARGB value for 'black' which is
        /// 0xFF000000.</returns>
        int[] RenderBuffer(Program program, MemoryArea mem, Address addrStart, int length, int? mouse);

        VisualAnnotation[] RenderAnnotations(Program program, Address addrStart, int length, int? mouse);
    }

    public class VisualAnnotation
    {
        public Address Address;
        public string Text;
        public int LineColor;
        public int TextColor;
    }

}
