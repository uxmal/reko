#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using System.Globalization;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Reko.Gui.TextViewing;

namespace Reko.UserInterfaces.AvaloniaUI.Controls
{
    /// <summary>
    /// A SimpleTextSpan describes a span of text that has the same
    /// formatting attributes and behaviour.
    /// </summary>
    public abstract class TextSpan : ITextSpan
    {
        protected TextSpan()
        {
            this.Style = null!;
        }

        public abstract string GetText();
        public string? Style { get; set; }

        public object? tag;
        public object? Tag {
            get { return tag; }
            set { tag = value; }
        }
        public int ContextMenuID { get; set; }

        public virtual Size GetSize(string text, Typeface font, double emSize)
        {
            var ft = new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                font,
                emSize,
                null);
            return new Size(ft.Width, ft.Height);
        }
    }
}
