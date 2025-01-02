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

using Reko.Gui.TextViewing;
using System.Drawing;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    /// <summary>
    /// A TextSpan describes a span of text that has the same
    /// formatting attributes and behaviour.
    /// </summary>
    public abstract class TextSpan : ITextSpan
    {
        private static StringFormat stringFormat;

        static TextSpan()
        {
            stringFormat = new StringFormat(StringFormat.GenericTypographic);
            stringFormat.FormatFlags |=
                StringFormatFlags.MeasureTrailingSpaces;
        }

        public abstract string GetText();
        public string Style { get; set; }
        public object tag;
        public object Tag { get { return tag; } set { tag = value; if (value is Reko.Core.Address) value.ToString(); } }
        public int ContextMenuID { get; set; }

        public virtual SizeF GetSize(string text, Font font, Graphics g)
        {
            var sz = TextRenderer.MeasureText(
               g, text, font, new Size(0, 0),
               TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
            return sz;
        }
    }
}
