
#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using System.Drawing;
using System.Windows.Forms;

namespace Reko.Gui.Windows.Controls
{
    public class StyleStack : IDisposable
    {
        private IUiPreferencesService uiPrefSvc;
        private List<UiStyle> stack;
        private SolidBrush fg;
        private SolidBrush bg;
        private Font font;

        public StyleStack(IUiPreferencesService uiPrefSvc)
        {
            if (uiPrefSvc == null) throw new ArgumentNullException("uiPrefSvc");
            this.uiPrefSvc = uiPrefSvc;
            this.stack = new List<UiStyle>();
        }

        public void Dispose()
        {
            if (font != null)
                font.Dispose();
            font = null;
            if (bg != null)
                bg.Dispose();
            bg = null;
            if (fg != null)
                fg.Dispose();
            fg = null;
        }

        public void PushStyle(string styleSelector)
        {
            if (styleSelector == UiStyles.DisassemblerOpcode)
                styleSelector.ToCharArray();    //$DEBUG
            UiStyle style = null;
            if (!string.IsNullOrEmpty(styleSelector))
            {
                uiPrefSvc.Styles.TryGetValue(styleSelector, out style);
            }
            stack.Add(style);       // May be null if we can't find the style.
        }

        internal void PopStyle()
        {
            stack.RemoveAt(stack.Count - 1);
        }

        private SolidBrush CacheBrush(ref SolidBrush brInstance, SolidBrush brNew)
        {
            if (brInstance != null)
                brInstance.Dispose();
            brInstance = brNew;
            return brNew;
        }

        public SolidBrush GetForeground(Control ctrl)
        {
            for (int i = stack.Count - 1; i >= 0; --i)
            {
                var style = stack[i];
                if (style != null && style.Foreground != null)
                    return style.Foreground;
            }
            return CacheBrush(ref fg, new SolidBrush(ctrl.ForeColor));
        }

        public Cursor GetCursor(Control ctrl)
        {
            for (int i = stack.Count - 1; i>=0; --i)
            {
                var style = stack[i];
                if (style != null && style.Cursor != null)
                    return style.Cursor;
            }
            return Cursors.Default;
        }

        public Color GetForegroundColor(Color fgColor)
        {
           for(int i = stack.Count - 1; i >= 0; --i)
            {
                var style = stack[i];
                if (style != null && style.Foreground != null)
                    return style.Foreground.Color;
            }
            return fgColor;
        }

        public SolidBrush GetBackground(Color bgColor)
        {
            for (int i = stack.Count - 1; i >= 0; --i)
            {
                var style = stack[i];
                if (style != null && style.Background != null)
                    return style.Background;
            }
            return CacheBrush(ref bg, new SolidBrush(bgColor));
        }

        public Font GetFont(Font defaultFont)
        {
            for (int i = stack.Count - 1; i >= 0; --i)
            {
                var style = stack[i];
                if (style != null && style.Font != null)
                    return style.Font;
            }
            return defaultFont;
        }

        public int? GetWidth()
        {
            for (int i = stack.Count - 1; i >= 0; --i)
            {
                var style = stack[i];
                if (style != null && style.Width.HasValue)
                    return style.Width;
            }
            return null;
        }

        public float GetNumber(Func<UiStyle, float> fn)
        {
            for (int i = stack.Count - 1; i >= 0; --i)
            {
                var style = stack[i];
                if (style != null)
                {
                    float n = fn(style);
                    if (n != 0)
                        return n;
                }
            }
            return 0;
        }

        /// <summary>
        /// Given a rectangle <paramref name="rc"/>, creates a padded rectangle
        /// <paramref name="rcPadded"/> and 
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="rcPadded"></param>
        public void PadRectangle(ref RectangleF rc, ref RectangleF rcPadded)
        {
            float top = GetNumber(s => s.PaddingTop);
            float left = GetNumber(s => s.PaddingLeft);
            float bottom = GetNumber(s => s.PaddingBottom);
            float right = GetNumber(s => s.PaddingRight);
            rcPadded.X = rc.X;
            rcPadded.Width = rc.Width + left + right;
            rcPadded.Y = rc.Y;
            rcPadded.Height = rc.Height + top + bottom;

            rc.Offset(left, top);
        }
    }
}