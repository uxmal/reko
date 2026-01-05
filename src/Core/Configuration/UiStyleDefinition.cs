#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

namespace Reko.Core.Configuration
{
    /// <summary>
    /// A style definition.
    /// </summary>
    public class UiStyleDefinition
    {
        /// <summary>
        /// Style class name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Font name.
        /// </summary>
        public string? FontName { get; set; }

        /// <summary>
        /// Font size.
        /// </summary>
        public double? FontSize { get; set; }

        /// <summary>
        /// Forground color.
        /// </summary>
        public string? ForeColor { get; set; }

        /// <summary>
        /// Background color.
        /// </summary>
        public string? BackColor { get; set; }

        /// <summary>
        /// Cursor to use.
        /// </summary>
        public string? Cursor { get; set; }

        /// <summary>
        /// Width of the element.
        /// </summary>
        public string? Width { get; set; }

        /// <summary>
        /// Text alignment.
        /// </summary>
        public string? TextAlign { get; set; }

        /// <summary>
        /// Top padding of the element.
        /// </summary>
        public string? PaddingTop { get; set; }

        /// <summary>
        /// Left padding of the element.
        /// </summary>
        public string? PaddingLeft { get; set; }

        /// <summary>
        /// Bottom padding of the element.
        /// </summary>
        public string? PaddingBottom { get; set; }

        /// <summary>
        /// Right padding of the element.
        /// </summary>
        public string? PaddingRight { get; set; }
    }
}
